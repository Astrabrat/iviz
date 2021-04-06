﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Iviz.Core;
using Iviz.Displays;
using Iviz.Msgs;
using Iviz.Msgs.NavMsgs;
using Iviz.Msgs.StdMsgs;
using Iviz.Resources;
using Iviz.Ros;
using Iviz.Roslib;
using Iviz.Roslib.Utils;
using Iviz.XmlRpc;
using JetBrains.Annotations;
using Nito.AsyncEx;
using UnityEngine;
using Logger = Iviz.Core.Logger;

namespace Iviz.Controllers
{
    [DataContract]
    public sealed class OccupancyGridConfiguration : JsonToString, IConfiguration
    {
        [DataMember] public string Id { get; set; } = Guid.NewGuid().ToString();
        [DataMember] public Resource.ModuleType ModuleType => Resource.ModuleType.OccupancyGrid;
        [DataMember] public bool Visible { get; set; } = true;
        [DataMember] public string Topic { get; set; } = "";
        [DataMember] public Resource.ColormapId Colormap { get; set; } = Resource.ColormapId.gray;
        [DataMember] public bool FlipMinMax { get; set; } = true;
        [DataMember] public bool CubesVisible { get; set; } = false;
        [DataMember] public bool TextureVisible { get; set; } = true;
        [DataMember] public float ScaleZ { get; set; } = 0.5f;
        [DataMember] public bool RenderAsOcclusionOnly { get; set; } = false;
        [DataMember] public SerializableColor Tint { get; set; } = Color.white;
    }

    public sealed class OccupancyGridListener : ListenerController
    {
        const int MaxTileSize = 256;

        readonly FrameNode cubeNode;
        readonly FrameNode textureNode;

        [NotNull] OccupancyGridResource[] gridTiles = Array.Empty<OccupancyGridResource>();
        readonly List<OccupancyGridTextureResource> textureTiles = new List<OccupancyGridTextureResource>();

        int numCellsX;
        int numCellsY;
        float cellSize;

        public override IModuleData ModuleData { get; }

        public override TfFrame Frame => cubeNode.Parent;

        bool isProcessing;

        bool IsProcessing
        {
            get => isProcessing;
            set
            {
                isProcessing = value;
                Listener.SetPause(value);
            }
        }

        [NotNull]
        public string Description
        {
            get
            {
                int numValid;
                if (TextureVisible)
                {
                    numValid = textureTiles.Sum(texture => texture.NumValidValues);
                }
                else if (CubesVisible)
                {
                    numValid = Enumerable.Sum(gridTiles, grid => grid.NumValidValues);
                }
                else
                {
                    numValid = 0;
                }

                return $"<b>{numCellsX.ToString("N0")}x{numCellsY.ToString("N0")} cells | {cellSize.ToString("#,0.###")} m/cell</b>\n" +
                       $"{numValid.ToString("N0")} valid";
            }
        }

        readonly OccupancyGridConfiguration config = new OccupancyGridConfiguration();

        public OccupancyGridConfiguration Config
        {
            get => config;
            set
            {
                config.Topic = value.Topic;
                Visible = value.Visible;
                Colormap = value.Colormap;
                FlipMinMax = value.FlipMinMax;
                ScaleZ = config.ScaleZ;
                RenderAsOcclusionOnly = value.RenderAsOcclusionOnly;
                Tint = value.Tint;
                CubesVisible = value.CubesVisible;
                TextureVisible = value.TextureVisible;
            }
        }

        public bool Visible
        {
            get => config.Visible;
            set
            {
                config.Visible = value;
                foreach (var grid in gridTiles)
                {
                    grid.Visible = value;
                }

                foreach (var texture in textureTiles)
                {
                    texture.Visible = value;
                }
            }
        }

        public Resource.ColormapId Colormap
        {
            get => config.Colormap;
            set
            {
                config.Colormap = value;
                foreach (var grid in gridTiles)
                {
                    grid.Colormap = value;
                }

                foreach (var texture in textureTiles)
                {
                    texture.Colormap = value;
                }
            }
        }

        public bool FlipMinMax
        {
            get => config.FlipMinMax;
            set
            {
                config.FlipMinMax = value;
                foreach (var grid in gridTiles)
                {
                    grid.FlipMinMax = value;
                }

                foreach (var texture in textureTiles)
                {
                    texture.FlipMinMax = value;
                }
            }
        }

        public float ScaleZ
        {
            get => config.ScaleZ;
            set
            {
                config.ScaleZ = value;

                float yScale = Mathf.Approximately(cellSize, 0) ? 1 : value / cellSize;
                cubeNode.transform.localScale = new Vector3(1, yScale, 1);
            }
        }

        public bool RenderAsOcclusionOnly
        {
            get => config.RenderAsOcclusionOnly;
            set
            {
                config.RenderAsOcclusionOnly = value;
                foreach (var grid in gridTiles)
                {
                    grid.OcclusionOnly = value;
                }
            }
        }

        public Color Tint
        {
            get => config.Tint;
            set
            {
                config.Tint = value;
                foreach (var grid in gridTiles)
                {
                    grid.Tint = value;
                }

                foreach (var texture in textureTiles)
                {
                    texture.Tint = value;
                }
            }
        }

        public bool CubesVisible
        {
            get => config.CubesVisible;
            set
            {
                config.CubesVisible = value;
                cubeNode.gameObject.SetActive(value);
            }
        }

        public bool TextureVisible
        {
            get => config.TextureVisible;
            set
            {
                config.TextureVisible = value;
                textureNode.gameObject.SetActive(value);
            }
        }

        public OccupancyGridListener([NotNull] IModuleData moduleData)
        {
            ModuleData = moduleData ?? throw new ArgumentNullException(nameof(moduleData));

            cubeNode = FrameNode.Instantiate("OccupancyGrid Cube Node");
            textureNode = FrameNode.Instantiate("OccupancyGrid Texture Node");

            Config = new OccupancyGridConfiguration();
        }

        public override void StartListening()
        {
            Listener = new Listener<OccupancyGrid>(config.Topic, Handler);
        }

        void Handler(OccupancyGrid msg)
        {
            if (IsProcessing)
            {
                return;
            }

            if (msg.Data.Length != msg.Info.Width * msg.Info.Height)
            {
                Logger.Debug(
                    $"{this}: Size {msg.Info.Width}x{msg.Info.Height} but data length {msg.Data.Length}");
                return;
            }

            if (float.IsNaN(msg.Info.Resolution))
            {
                Logger.Debug($"{this}: NaN in header!");
                return;
            }

            cubeNode.AttachTo(msg.Header);
            textureNode.AttachTo(msg.Header);

            if (msg.Info.Origin.HasNaN())
            {
                Logger.Debug($"{this}: NaN in origin!");
                return;
            }

            Pose origin = msg.Info.Origin.Ros2Unity();
            if (!origin.IsUsable())
            {
                Logger.Error(
                    $"{this}: Cannot use ({origin.position.x}, {origin.position.y}, {origin.position.z}) as position. Values too large");
                origin = Pose.identity;
            }

            numCellsX = (int) msg.Info.Width;
            numCellsY = (int) msg.Info.Height;
            cellSize = msg.Info.Resolution;

            List<Task> tasks = new List<Task>();

            try
            {
                IsProcessing = true;
                if (CubesVisible)
                {
                    SetCubes(msg.Data, origin, tasks);
                }

                if (TextureVisible)
                {
                    SetTextures(msg.Data, origin, tasks);
                }
            }
            finally
            {
                // next frame
                AwaitAndReset(tasks);
            }
        }

        async void AwaitAndReset(IEnumerable<Task> tasks)
        {
            await tasks.WhenAll().AwaitNoThrow(this);
            IsProcessing = false;
        }

        void SetCubes([NotNull] sbyte[] data, Pose pose, List<Task> tasks)
        {
            if (gridTiles.Length != 16)
            {
                gridTiles = new OccupancyGridResource[16];
                for (int j = 0; j < gridTiles.Length; j++)
                {
                    gridTiles[j] = ResourcePool.Rent<OccupancyGridResource>(
                        Resource.Displays.OccupancyGridResource,
                        cubeNode.transform);
                    gridTiles[j].transform.SetLocalPose(Pose.identity);
                    gridTiles[j].Colormap = Colormap;
                    gridTiles[j].FlipMinMax = FlipMinMax;
                }
            }

            int i = 0;
            for (int v = 0; v < 4; v++)
            {
                for (int u = 0; u < 4; u++, i++)
                {
                    OccupancyGridResource grid = gridTiles[i];
                    grid.NumCellsX = numCellsX;
                    grid.NumCellsY = numCellsY;
                    grid.CellSize = cellSize;

                    var rect = new OccupancyGridResource.Rect
                    (
                        xMin: u * numCellsX / 4,
                        xMax: (u + 1) * numCellsX / 4,
                        yMin: v * numCellsY / 4,
                        yMax: (v + 1) * numCellsY / 4
                    );

                    tasks.Add(Task.Run(() =>
                    {
                        try
                        {
                            grid.SetOccupancy(data, rect, pose);
                        }
                        catch (Exception e)
                        {
                            Debug.LogWarning(e);
                        }
                    }));
                }
            }


            ScaleZ = ScaleZ;
        }

        void SetTextures([NotNull] sbyte[] data, Pose pose, List<Task> tasks)
        {
            int tileSizeX = (numCellsX + MaxTileSize - 1) / MaxTileSize;
            int tileSizeY = (numCellsY + MaxTileSize - 1) / MaxTileSize;
            int tileTotalSize = tileSizeY * tileSizeX;

            if (tileTotalSize != textureTiles.Count)
            {
                if (tileTotalSize > textureTiles.Count)
                {
                    for (int j = textureTiles.Count; j < tileTotalSize; j++)
                    {
                        textureTiles.Add(
                            ResourcePool.RentDisplay<OccupancyGridTextureResource>(textureNode.transform));
                        textureTiles[j].Visible = Visible;
                        textureTiles[j].Colormap = Colormap;
                        textureTiles[j].FlipMinMax = FlipMinMax;
                    }
                }
                else
                {
                    for (int j = tileTotalSize; j < textureTiles.Count; j++)
                    {
                        textureTiles[j].ReturnToPool();
                        textureTiles[j] = null;
                    }

                    textureTiles.RemoveRange(tileTotalSize, textureTiles.Count - tileTotalSize);
                }
            }

            int i = 0;
            for (int v = 0; v < tileSizeY; v++)
            {
                for (int u = 0; u < tileSizeX; u++, i++)
                {
                    int xMin = u * MaxTileSize;
                    int xMax = Math.Min(xMin + MaxTileSize, numCellsX);
                    int yMin = v * MaxTileSize;
                    int yMax = Math.Min(yMin + MaxTileSize, numCellsY);
                    OccupancyGridResource.Rect rect = new OccupancyGridResource.Rect(xMin, xMax, yMin, yMax);

                    var texture = textureTiles[i];
                    tasks.Add(Task.Run(() =>
                    {
                        try
                        {
                            texture.Set(data, cellSize, numCellsX, numCellsY, rect, pose);
                        }
                        catch (Exception e)
                        {
                            Logger.Error($"{this}: Error processing occupancy grid", e);
                        }
                    }));
                }
            }
        }


        public override void StopController()
        {
            base.StopController();
            foreach (var grid in gridTiles)
            {
                grid.ReturnToPool();
            }

            foreach (var texture in textureTiles)
            {
                texture.ReturnToPool();
            }

            cubeNode.Stop();
            textureNode.Stop();
            UnityEngine.Object.Destroy(cubeNode.gameObject);
            UnityEngine.Object.Destroy(textureNode.gameObject);
        }
    }
}