﻿using System;
using System.Runtime.Serialization;
using Iviz.App;
using Iviz.Core;
using Iviz.Resources;
using Iviz.Roslib;
using Iviz.Roslib.Utils;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;

namespace Iviz.Controllers
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum OcclusionQualityType
    {
        Off,
        Fast,
        Medium,
        Best
    }

    [DataContract]
    public sealed class ARConfiguration : JsonToString, IConfiguration
    {
        /* NonSerializable */
        public float WorldScale { get; set; } = 1.0f;

        /* NonSerializable */
        public SerializableVector3 WorldOffset { get; set; } = ARController.DefaultWorldOffset;

        /* NonSerializable */
        public float WorldAngle { get; set; }
        /*
        [DataMember] public bool SearchMarker { get; set; }
        [DataMember] public bool MarkerHorizontal { get; set; } = true;
        [DataMember] public int MarkerAngle { get; set; }
        [DataMember] public string MarkerFrame { get; set; } = "";
        [DataMember] public SerializableVector3 MarkerOffset { get; set; } = Vector3.zero;
        */
        [DataMember] public OcclusionQualityType OcclusionQuality { get; set; }

        /* NonSerializable */
        public bool ShowARJoystick { get; set; }

        /* NonSerializable */
        public bool PinRootMarker { get; set; }
        [DataMember] public string Id { get; set; } = Guid.NewGuid().ToString();
        [DataMember] public Resource.ModuleType ModuleType => Resource.ModuleType.AugmentedReality;
        [DataMember] public bool Visible { get; set; } = true;
    }

    public abstract class ARController : MonoBehaviour, IController, IHasFrame
    {
        public enum RootMover
        {
            ModuleData,
            Anchor,
            ImageMarker,
            ControlMarker,
            Configuration,
            Setup
        }

        public static readonly Vector3 DefaultWorldOffset = new Vector3(0.5f, 0, -0.2f);

        //static ARSessionInfo savedSessionInfo;
        static AnchorToggleButton PinControlButton => ModuleListPanel.Instance.PinControlButton;
        static AnchorToggleButton ShowARJoystickButton => ModuleListPanel.Instance.ShowARJoystickButton;
        static ARJoystick ARJoystick => ModuleListPanel.Instance.ARJoystick;

        readonly ARConfiguration config = new ARConfiguration();
        IModuleData moduleData;

        float? joyVelocityAngle;
        Vector3? joyVelocityPos;

        protected Canvas canvas;
        protected FrameNode node;

        public static bool HasARController => Instance != null;
        [CanBeNull] public static ARFoundationController Instance { get; protected set; }
        
        public ARConfiguration Config
        {
            get => config;
            set
            {
                Visible = value.Visible;
                WorldScale = value.WorldScale;
                /*
                UseMarker = value.SearchMarker;
                MarkerHorizontal = value.MarkerHorizontal;
                MarkerAngle = value.MarkerAngle;
                MarkerFrame = value.MarkerFrame;
                MarkerOffset = value.MarkerOffset;
                */
                OcclusionQuality = value.OcclusionQuality;
            }
        }
        
        public virtual OcclusionQualityType OcclusionQuality
        {
            get => config.OcclusionQuality;
            set => config.OcclusionQuality = value;
        }

        public Vector3 WorldPosition
        {
            get => config.WorldOffset;
            private set => config.WorldOffset = value;
        }

        public float WorldAngle
        {
            get => config.WorldAngle;
            private set => config.WorldAngle = value;
        }

        protected Pose WorldPose { get; private set; } = Pose.identity;

        public float WorldScale
        {
            get => config.WorldScale;
            set
            {
                config.WorldScale = value;
                TfRootScale = value;
            }
        }

        public virtual bool Visible
        {
            get => config.Visible;
            set
            {
                config.Visible = value;
                GuiInputModule.Instance.DisableCameraLock();
                TfListener.RootFrame.transform.SetPose(value ? WorldPose : Pose.identity);
                ARModeChanged?.Invoke(value);
            }
        }

        /*
        public virtual bool UseMarker
        {
            get => config.SearchMarker;
            set => config.SearchMarker = value;
        }

        public virtual bool MarkerHorizontal
        {
            get => config.MarkerHorizontal;
            set => config.MarkerHorizontal = value;
        }

        public virtual int MarkerAngle
        {
            get => config.MarkerAngle;
            set => config.MarkerAngle = value;
        }

        public virtual string MarkerFrame
        {
            get => config.MarkerFrame;
            set => config.MarkerFrame = value;
        }

        public virtual Vector3 MarkerOffset
        {
            get => config.MarkerOffset;
            set => config.MarkerOffset = value;
        }
        */

        static float TfRootScale
        {
            set => TfListener.RootFrame.transform.localScale = value * Vector3.one;
        }

        protected virtual bool PinRootMarker
        {
            get => config.PinRootMarker;
            set
            {
                config.PinRootMarker = value;
                PinControlButton.State = value;
            }
        }

        bool ShowARJoystick
        {
            get => config.ShowARJoystick;
            set
            {
                config.ShowARJoystick = value;
                ShowARJoystickButton.State = value;
                ARJoystick.Visible = value;

                if (value)
                {
                    ModuleListPanel.Instance.AllGuiVisible = false;
                }
            }
        }

        protected virtual void Awake()
        {
            gameObject.name = "AR";

            if (canvas == null)
            {
                canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
            }

            node = FrameNode.Instantiate("AR Node");

            PinControlButton.Visible = true;
            ShowARJoystickButton.Visible = true;

            PinControlButton.Clicked += OnPinControlButtonClicked;
            ShowARJoystickButton.Clicked += OnShowARJoystickClicked;

            ARJoystick.ChangedPosition += OnARJoystickChangedPosition;
            ARJoystick.ChangedAngle += OnARJoystickChangedAngle;
            ARJoystick.PointerUp += OnARJoystickPointerUp;

            GuiInputModule.Instance.UpdateQualityLevel();
        }

        void OnARJoystickChangedAngle(float dA)
        {
            if (joyVelocityAngle == null)
            {
                joyVelocityAngle = 0;
            }
            else if (Sign(joyVelocityAngle.Value) != 0 && Sign(joyVelocityAngle.Value) != Sign(dA))
            {
                joyVelocityAngle = 0;
            }
            else
            {
                joyVelocityAngle += 0.02f * dA;
            }

            if (ARJoystick.IsGlobal)
            {
                SetWorldAngle(WorldAngle + joyVelocityAngle.Value, RootMover.ControlMarker);
            }
            else
            {
                Vector3 cameraPosition = Settings.MainCameraTransform.position;
                var q1 = Pose.identity.WithPosition(cameraPosition);
                var q2 = Pose.identity.WithRotation(Quaternion.AngleAxis(joyVelocityAngle.Value, Vector3.up));
                var q3 = Pose.identity.WithPosition(-cameraPosition);
                SetWorldPose(q1.Multiply(q2.Multiply(q3.Multiply(WorldPose))), RootMover.ControlMarker);
            }
        }

        static int Sign(float f)
        {
            return f > 0 ? 1 : f < 0 ? -1 : 0;
        }

        static int Sign(Vector3 v)
        {
            return Sign(v.x) + Sign(v.y) + Sign(v.z);  // only one of the components is nonzero
        }


        void OnARJoystickChangedPosition(Vector3 dPos)
        {
            Vector3 deltaPosition = 0.0005f * dPos;
            if (joyVelocityPos == null)
            {
                joyVelocityPos = Vector3.zero;
            }
            else if (Sign(joyVelocityPos.Value) != 0 && Sign(joyVelocityPos.Value) != Sign(dPos))
            {
                joyVelocityPos = Vector3.zero;
            }            
            else
            {
                joyVelocityPos += deltaPosition;
            }

            Vector3 deltaWorldPosition;
            if (ARJoystick.IsGlobal)
            {
                deltaWorldPosition = WorldPose.rotation * joyVelocityPos.Value.Ros2Unity();
            }
            else
            {
                Quaternion cameraRotation = Settings.MainCameraTransform.rotation;
                deltaWorldPosition = cameraRotation * joyVelocityPos.Value;
            }

            SetWorldPosition(WorldPosition + deltaWorldPosition, RootMover.ControlMarker);
        }

        void OnARJoystickPointerUp()
        {
            joyVelocityPos = null;
            joyVelocityAngle = null;
        }

        void OnPinControlButtonClicked()
        {
            PinRootMarker = PinControlButton.State;
        }

        void OnShowARJoystickClicked()
        {
            ShowARJoystick = ShowARJoystickButton.State;
        }

        public IModuleData ModuleData
        {
            get => moduleData ?? throw new InvalidOperationException("Controller has not been started!");
            set => moduleData = value ?? throw new InvalidOperationException("Cannot set null value as module data");
        }

        public virtual void StopController()
        {
            Visible = false;
            WorldScale = 1;

            PinControlButton.Visible = false;
            ShowARJoystickButton.Visible = false;

            WorldPoseChanged = null;
            ARJoystick.ChangedPosition -= OnARJoystickChangedPosition;
            ARJoystick.ChangedAngle -= OnARJoystickChangedAngle;
            ARJoystick.PointerUp -= OnARJoystickPointerUp;
            PinControlButton.Clicked -= OnPinControlButtonClicked;
            ShowARJoystickButton.Clicked -= OnShowARJoystickClicked;
            ShowARJoystick = false;
            Instance = null;
            
            GuiInputModule.Instance.UpdateQualityLevel();
        }

        void IController.ResetController()
        {
        }

        public TfFrame Frame => TfListener.Instance.FixedFrame;

        public static event Action<bool> ARModeChanged;

        public event Action<RootMover> WorldPoseChanged;

        void UpdateWorldPose(in Pose pose, RootMover mover)
        {
            WorldPose = pose;
            if (Visible)
            {
                TfListener.RootFrame.transform.SetPose(pose);
            }

            WorldPoseChanged?.Invoke(mover);
        }

        protected static float AngleFromPose(in Pose unityPose)
        {
            float angle = unityPose.rotation.eulerAngles.y;
            if (angle > 180)
            {
                angle -= 360;
            }

            return angle;
        }

        protected void SetWorldPose(in Pose unityPose, RootMover mover)
        {
            float angle = AngleFromPose(unityPose);
            WorldPosition = unityPose.position;
            WorldAngle = angle;
            UpdateWorldPose(unityPose, mover);
        }

        void SetWorldPosition(in Vector3 unityPosition, RootMover mover)
        {
            WorldPosition = unityPosition;
            UpdateWorldPose(WorldPose.WithPosition(unityPosition), mover);
        }

        void SetWorldAngle(float angle, RootMover mover)
        {
            WorldAngle = angle;
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.up);
            UpdateWorldPose(new Pose(WorldPosition, rotation), mover);
        }

        public static Pose RelativePoseToWorld(in Pose unityPose)
        {
            return Instance == null ? unityPose : Instance.WorldPose.Inverse().Multiply(unityPose);
        }

        /*
        void OnDestroy()
        {
            Debug.Log("AR Controller Destroyed!");
        }
        */
    }
}