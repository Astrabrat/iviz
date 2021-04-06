﻿using System;
using System.IO;
using System.Threading.Tasks;
using BitMiracle.LibJpeg;
using Iviz.Core;
using Iviz.Msgs;
using Iviz.Resources;
using JetBrains.Annotations;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using Buffer = System.Buffer;
using Logger = Iviz.Core.Logger;

namespace Iviz.Displays
{
    public sealed class ImageTexture
    {
        static readonly int IntensityCoeffID = Shader.PropertyToID("_IntensityCoeff");
        static readonly int IntensityAddID = Shader.PropertyToID("_IntensityAdd");
        static readonly int IntensityID = Shader.PropertyToID("_IntensityTexture");
        static readonly int MainTexID = Shader.PropertyToID("_MainTex");

        byte[] bitmapBuffer;

        public event Action<Texture2D> TextureChanged;
        public event Action<Texture2D> ColormapChanged;

        Vector2 intensityBounds;

        public Vector2 IntensityBounds
        {
            get => intensityBounds;
            set
            {
                intensityBounds = value;
                float intensitySpan = intensityBounds.y - intensityBounds.x;

                if (intensitySpan == 0)
                {
                    Material.SetFloat(IntensityCoeffID, 1);
                    Material.SetFloat(IntensityAddID, 0);
                }
                else
                {
                    if (!FlipMinMax)
                    {
                        Material.SetFloat(IntensityCoeffID, 1 / intensitySpan);
                        Material.SetFloat(IntensityAddID, -intensityBounds.x / intensitySpan);
                    }
                    else
                    {
                        Material.SetFloat(IntensityCoeffID, -1 / intensitySpan);
                        Material.SetFloat(IntensityAddID, intensityBounds.y / intensitySpan);
                    }
                }
            }
        }

        bool flipMinMax;

        public bool FlipMinMax
        {
            get => flipMinMax;
            set
            {
                flipMinMax = value;
                IntensityBounds = IntensityBounds;
            }
        }

        [CanBeNull] public Texture2D Texture { get; private set; }
        [NotNull] public Material Material { get; }
        [NotNull] public string Description { get; private set; } = "";
        public bool IsMono { get; private set; }
        public int Width => Texture != null ? Texture.width : 0;
        public int Height => Texture != null ? Texture.height : 0;

        Resource.ColormapId colormap;

        public Resource.ColormapId Colormap
        {
            get => colormap;
            set
            {
                colormap = value;

                Material.SetTexture(IntensityID, ColormapTexture);
                ColormapChanged?.Invoke(ColormapTexture);
            }
        }

        public Texture2D ColormapTexture => Resource.Colormaps.Textures[Colormap];

        public ImageTexture()
        {
            Material = Resource.Materials.ImagePreview.Instantiate();
        }

        static int FieldSizeFromEncoding(string encoding)
        {
            switch (encoding)
            {
                case "rgba8":
                case "bgra8":
                case "8SC4":
                    return 4;
                case "rgb8":
                case "bgr8":
                case "8SC3":
                    return 3;
                case "mono16":
                case "16UC1":
                    return 2;
                case "mono8":
                case "8UC1":
                    return 1;
                default:
                    return -1;
            }
        }

        [CanBeNull]
        static string EncodingFromPng([NotNull] BigGustave.Png png)
        {
            switch (png.Header.ColorType)
            {
                case BigGustave.ColorType.None:
                    switch (png.Header.BitDepth)
                    {
                        case 8: return "mono8";
                        case 16: return "mono16";
                    }

                    break;
                case BigGustave.ColorType.ColorUsed:
                    switch (png.Header.BitDepth)
                    {
                        case 8: return "rgb8";
                        case 16: return "rgb16";
                    }

                    break;
                case BigGustave.ColorType.AlphaChannelUsed:
                    switch (png.Header.BitDepth)
                    {
                        case 8: return "rgba8";
                        case 16: return "rgba16";
                    }

                    break;
            }

            return null;
        }

        public void ProcessPng(byte[] data, [NotNull] Action onFinished)
        {
            Task.Run(() =>
            {
                try
                {
                    BigGustave.Png png = BigGustave.Png.Open(data);

                    byte[] newData;
                    if (png.RowOffset != 0)
                    {
                        int reqSize = png.Height * png.RowSize;
                        if (bitmapBuffer == null || bitmapBuffer.Length < reqSize)
                        {
                            bitmapBuffer = new byte[reqSize];
                        }

                        int srcOffset = png.RowOffset;
                        int dstOffset = 0;
                        int rowSize = png.RowSize;
                        for (int i = png.Height; i != 0; i--, srcOffset += png.RowStep, dstOffset += rowSize)
                        {
                            Buffer.BlockCopy(png.Data, srcOffset, bitmapBuffer, dstOffset, rowSize);
                        }

                        newData = bitmapBuffer;
                    }
                    else
                    {
                        newData = png.Data;
                    }

                    GameThread.PostInListenerQueue(() =>
                    {
                        Set(png.Width, png.Height, EncodingFromPng(png), newData);
                        onFinished();
                    });
                }
                catch (Exception e)
                {
                    Logger.Error($"{this}: Error processing PNG", e);
                    GameThread.PostInListenerQueue(onFinished);
                }
            });
        }

        public void ProcessJpg(byte[] data, [NotNull] Action onFinished)
        {
            Task.Run(() =>
            {
                try
                {
                    JpegImage image = new JpegImage(new MemoryStream(data));

                    string encoding = null;

                    int reqSize = image.Height * image.Width;
                    switch (image.Colorspace)
                    {
                        case Colorspace.RGB when image.BitsPerComponent == 8:
                        {
                            if (image.Width % 4 != 0)
                            {
                                Logger.Debug($"{this}: Row padding not implemented");
                                return;
                            }

                            encoding = "rgb";
                            reqSize *= 3;
                            break;
                        }
                        case Colorspace.Grayscale when image.BitsPerComponent == 8:
                        {
                            if (image.Width % 4 != 0)
                            {
                                Logger.Debug($"{this}: Row padding not implemented");
                                return;
                            }

                            encoding = "mono8";
                            break;
                        }
                        case Colorspace.Grayscale when image.BitsPerComponent == 16:
                        {
                            if (image.Width % 2 != 0)
                            {
                                Logger.Debug($"{this}: Row padding not implemented");
                                return;
                            }

                            encoding = "mono16";
                            reqSize *= 2;
                            break;
                        }
                    }

                    if (encoding == null)
                    {
                        Logger.Debug(
                            $"{this}: Unsupported encoding '{image.Colorspace}' with size {image.BitsPerComponent}");
                        return;
                    }

                    const int bmpHeaderLength = 54;
                    reqSize += bmpHeaderLength;

                    if (bitmapBuffer == null || bitmapBuffer.Length < reqSize)
                    {
                        bitmapBuffer = new byte[reqSize];
                    }

                    using (var outStream = new MemoryStream(bitmapBuffer))
                    {
                        image.WriteBitmap(outStream);
                    }

                    GameThread.PostInListenerQueue(() =>
                    {
                        Set(image.Width, image.Height, encoding, bitmapBuffer.AsSegment(bmpHeaderLength));
                        onFinished();
                    });
                }
                catch (Exception e)
                {
                    Logger.Error($"{this}: Error processing JPG", e);
                    GameThread.PostInListenerQueue(onFinished);
                }
            });
        }

        public void Set(int width, int height, string encoding, [NotNull] byte[] data)
        {
            Set(width, height, encoding, data.AsSegment());
        }

        public void Set(int width, int height, string encoding, in ArraySegment<byte> data,
            bool generateMipmaps = false)
        {
            int size = width * height;
            int bpp = FieldSizeFromEncoding(encoding);

            if (bpp == -1)
            {
                Logger.Debug($"ImageListener: Unsupported encoding '{encoding}'");
                return;
            }

            if (data.Count < size * bpp)
            {
                Logger.Debug(
                    $"ImageListener: Invalid image! Expected at least {(size * bpp).ToString()} bytes, " +
                    $"received {data.Count.ToString()}");
                return;
            }

            Description = $"<b>{width.ToString()}x{height.ToString()} pixels | {encoding}</b>";

            switch (encoding)
            {
                case "rgba8":
                case "8SC4":
                case "rgb8":
                case "8SC3":
                    IsMono = false;
                    Material.DisableKeyword("USE_INTENSITY");
                    Material.DisableKeyword("FLIP_RB");
                    break;
                case "bgra8":
                case "bgr8":
                    IsMono = false;
                    Material.DisableKeyword("USE_INTENSITY");
                    Material.EnableKeyword("FLIP_RB");
                    break;
                case "mono16":
                case "16UC1":
                    IsMono = true;
                    Material.EnableKeyword("USE_INTENSITY");
                    break;
                case "mono8":
                case "8UC1":
                    IsMono = true;
                    Material.EnableKeyword("USE_INTENSITY");
                    break;
                default:
                    return;
            }

            ApplyTexture(width, height, data, encoding, size * bpp, generateMipmaps);
        }

        void ApplyTexture(int width, int height, in ArraySegment<byte> data, string type, int length,
            bool generateMipmaps)
        {
            bool alreadyCopied = false;
            Texture2D texture;
            switch (type)
            {
                case "rgb8" when !Settings.SupportsRGB24:
                case "bgr8" when !Settings.SupportsRGB24:
                case "8SC3" when !Settings.SupportsRGB24:
                    texture = EnsureSize(width, height, TextureFormat.RGBA32);
                    CopyRgb24ToRgba32(data, texture.GetRawTextureData<byte>(), length);
                    alreadyCopied = true;
                    break;
                case "rgb8":
                case "bgr8":
                case "8SC3":
                    texture = EnsureSize(width, height, TextureFormat.RGB24);
                    break;
                case "rgba8":
                case "bgra8":
                case "8SC4":
                    texture = EnsureSize(width, height, TextureFormat.RGBA32);
                    break;
                case "mono16" when !Settings.SupportsR16:
                case "16UC1" when !Settings.SupportsR16:
                    texture = EnsureSize(width, height, TextureFormat.R8);
                    CopyR16ToR8(data, texture.GetRawTextureData<byte>(), length);
                    alreadyCopied = true;
                    break;
                case "mono16":
                case "16UC1":
                    texture = EnsureSize(width, height, TextureFormat.R16);
                    break;
                case "mono8":
                case "8UC1":
                    texture = EnsureSize(width, height, TextureFormat.R8);
                    break;
                default:
                    return;
            }

            if (!alreadyCopied)
            {
                NativeArray<byte>.Copy(data.Array, data.Offset, texture.GetRawTextureData<byte>(), 0, length);
            }

            texture.Apply(generateMipmaps, false);
        }

        unsafe void CopyR16ToR8(in ArraySegment<byte> src, NativeArray<byte> dst, int lengthInBytes)
        {
            if (src.Array == null)
            {
                throw new NullReferenceException($"{this}: Source array in Copy() was null");
            }

            int numElements = lengthInBytes / 2;
            if (src.Offset + lengthInBytes > src.Count || numElements > dst.Length)
            {
                throw new InvalidOperationException($"{this}: Skipping copy. Possible buffer overflow.");
            }

            byte* dstPtr = (byte*) dst.GetUnsafePtr();
            fixed (byte* srcPtr = &src.Array[src.Offset])
            {
                byte* srcPtrOff = srcPtr + 1;
                
                for (int i = numElements; i >= 0; i--)
                {
                    *dstPtr = *srcPtrOff;
                    dstPtr++;
                    srcPtrOff += 2;
                }
            }
        }
        
        unsafe void CopyRgb24ToRgba32(in ArraySegment<byte> src, NativeArray<byte> dst, int lengthInBytes)
        {
            if (src.Array == null)
            {
                throw new NullReferenceException($"{this}: Source array in Copy() was null");
            }

            int numElements = lengthInBytes / 3;
            if (src.Offset + lengthInBytes > src.Count || numElements * 4 > dst.Length)
            {
                throw new InvalidOperationException($"{this}: Skipping copy. Possible buffer overflow.");
            }

            byte* dstPtr = (byte*) dst.GetUnsafePtr();
            fixed (byte* srcPtr0 = &src.Array[src.Offset])
            {
                byte* srcPtr = srcPtr0;
                
                for (int i = numElements; i >= 0; i--)
                {
                    *dstPtr++ = *srcPtr++;
                    *dstPtr++ = *srcPtr++;
                    *dstPtr++ = *srcPtr++;
                    *dstPtr++ = 255;
                }
            }
        }        

        [NotNull]
        Texture2D EnsureSize(int width, int height, TextureFormat format)
        {
            if (Texture != null &&
                Texture.width == width &&
                Texture.height == height &&
                Texture.format == format)
            {
                return Texture;
            }

            if (Texture != null)
            {
                UnityEngine.Object.Destroy(Texture);
            }

            Texture = new Texture2D(width, height, format, false);
            Material.SetTexture(MainTexID, Texture);
            TextureChanged?.Invoke(Texture);
            return Texture;
        }

        public void Stop()
        {
            TextureChanged?.Invoke(null);
            TextureChanged = null;
            ColormapChanged?.Invoke(null);
            ColormapChanged = null;
        }

        public void Destroy()
        {
            if (Texture != null)
            {
                UnityEngine.Object.Destroy(Texture);
            }

            if (Material != null)
            {
                UnityEngine.Object.Destroy(Material);
            }
        }
    }
}