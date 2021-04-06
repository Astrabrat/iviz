using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using Iviz.Core;
using Iviz.Msgs;
using Iviz.Msgs.GeometryMsgs;
using Iviz.Msgs.IvizMsgs;
using Iviz.Roslib.Utils;
using Iviz.XmlRpc;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEditorInternal;

namespace Iviz.Opencv
{
    public class Context : IDisposable
    {
        static bool loggerSet;
        readonly IntPtr mContextPtr;
        readonly int imageSize;
        readonly IntPtr imagePtr;
        bool disposed;

        ArucoDictionaryName dictionaryName;

        IntPtr ContextPtr => !disposed
            ? mContextPtr
            : throw new ObjectDisposedException(nameof(mContextPtr), "Context already disposed");

        public int Width { get; }
        public int Height { get; }

        public ArucoDictionaryName DictionaryName
        {
            get => dictionaryName;
            set
            {
                dictionaryName = value;
                if (!Native.SetDictionary(ContextPtr, (int) value))
                {
                    throw new InvalidOperationException("An error happened in the native call");
                }
            }
        }

        public unsafe Context(int width, int height)
        {
            if (!loggerSet)
            {
                Native.SetupDebug(Core.Logger.Debug);
                Native.SetupInfo(Core.Logger.Debug);
                Native.SetupError(Core.Logger.Error);
                loggerSet = true;
            }

            if (width <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(width));
            }

            if (height <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(width));
            }

            try
            {
                mContextPtr = Native.CreateContext(width, height);
                Width = width;
                Height = height;
                DictionaryName = ArucoDictionaryName.DictArucoOriginal;

                imageSize = width * height * 3;
                imagePtr = Native.GetImagePtr(ContextPtr);
            }
            catch (EntryPointNotFoundException e)
            {
                UnityEngine.Debug.LogError(e);
            }
            catch (DllNotFoundException e)
            {
                UnityEngine.Debug.LogError(e);
            }
        }

        public unsafe void SetImageData(in NativeArray<byte> image)
        {
            if (imageSize > image.Length)
            {
                throw new ArgumentException("Image size is too small", nameof(image));
            }

            if (disposed)
            {
                throw new ObjectDisposedException(nameof(mContextPtr), "Context already disposed");
            }

            UnsafeUtility.MemCpy(imagePtr.ToPointer(), image.GetUnsafePtr(), imageSize);
        }

        public unsafe void SetImageDataFlipY(in NativeArray<byte> image)
        {
            if (imageSize > image.Length)
            {
                throw new ArgumentException("Image size is too small", nameof(image));
            }

            if (disposed)
            {
                throw new ObjectDisposedException(nameof(mContextPtr), "Context already disposed");
            }

            int stride = Width * 3;
            byte* src = (byte*) image.GetUnsafePtr();
            byte* dst = (byte*) imagePtr.ToPointer();
            for (int v = 0; v < Height; v++)
            {
                byte* srcOff = src + stride * v;
                byte* dstOff = dst + stride * (Height - v - 1);
                UnsafeUtility.MemCpy(dstOff, srcOff, stride);
            }
        }

        public void SetImageData([NotNull] byte[] image)
        {
            if (image == null)
            {
                throw new ArgumentNullException(nameof(image));
            }

            if (Width * Height * 3 > image.Length)
            {
                throw new ArgumentException("Image size is too small", nameof(image));
            }

            Native.CopyFrom(ContextPtr, image, image.Length);
        }

        public void GetImageData([NotNull] byte[] image)
        {
            if (image == null)
            {
                throw new ArgumentNullException(nameof(image));
            }

            if (Width * Height * 3 > image.Length)
            {
                throw new ArgumentException("Image size is too small", nameof(image));
            }

            Native.CopyTo(ContextPtr, image, image.Length);
        }

        public void SetCameraMatrix(float fx, float ox, float fy, float oy)
        {
            using (var array = new Rent<float>(9))
            {
                array.Array[0] = fx;
                array.Array[1] = 0;
                array.Array[2] = ox;

                array.Array[3] = 0;
                array.Array[4] = fy;
                array.Array[5] = oy;

                array.Array[6] = 0;
                array.Array[7] = 0;
                array.Array[8] = 1;
                if (!Native.SetCameraMatrix(ContextPtr, array.Array, array.Length))
                {
                    throw new InvalidOperationException("An error happened in the native call");
                }
            }
        }

        public int DetectArucoMarkers()
        {
            Native.DetectArucoMarkers(ContextPtr);
            return Native.GetNumDetectedMarkers(ContextPtr);
        }

        public int DetectQrMarkers()
        {
            Native.DetectQrMarkers(ContextPtr);
            return Native.GetNumDetectedMarkers(ContextPtr);
        }

        public int[] GetDetectedArucoIds()
        {
            int numDetected = Native.GetNumDetectedMarkers(ContextPtr);
            if (numDetected < 0)
            {
                throw new InvalidOperationException("An error happened in the native call");
            }

            if (numDetected == 0)
            {
                return Array.Empty<int>();
            }


            int[] indices = new int[numDetected];
            if (!Native.GetArucoMarkerIds(ContextPtr, indices, indices.Length))
            {
                throw new InvalidOperationException("An error happened in the native call");
            }

            return indices;
        }

        public string[] GetDetectedQrCodes()
        {
            int numDetected = Native.GetNumDetectedMarkers(ContextPtr);
            if (numDetected < 0)
            {
                throw new InvalidOperationException("An error happened in the native call");
            }

            if (numDetected == 0)
            {
                return Array.Empty<string>();
            }


            using (var pointers = new Rent<IntPtr>(numDetected))
            using (var pointerLengths = new Rent<int>(numDetected))
            {
                if (!Native.GetQrMarkerCodes(ContextPtr, pointers.Array, pointerLengths.Array, numDetected))
                {
                    throw new InvalidOperationException("An error happened in the native call");
                }

                string[] indices = new string[numDetected];
                for (int i = 0; i < numDetected; i++)
                {
                    indices[i] = Marshal.PtrToStringAnsi(pointers[i], pointerLengths[i]);
                }

                return indices;
            }
        }

        public ArucoMarkerPoses[] EstimateArucoMarkerPoses(float markerSizeInM)
        {
            int numDetected = Native.GetNumDetectedMarkers(ContextPtr);
            if (numDetected < 0)
            {
                throw new InvalidOperationException("An error happened in the native call");
            }

            if (numDetected == 0)
            {
                return Array.Empty<ArucoMarkerPoses>();
            }

            var markers = new ArucoMarkerPoses[numDetected];

            using (var indices = new Rent<int>(numDetected))
            using (var rotations = new Rent<float>(3 * numDetected))
            using (var translations = new Rent<float>(3 * numDetected))
            {
                if (!Native.GetArucoMarkerIds(ContextPtr, indices.Array, indices.Length) ||
                    !Native.EstimateMarkerPoses(ContextPtr, markerSizeInM, rotations.Array, rotations.Length,
                        translations.Array, translations.Length))
                {
                    throw new InvalidOperationException("An error happened in the native call");
                }

                for (int i = 0; i < numDetected; i++)
                {
                    Vector3 translation = (
                        translations[3 * i + 0],
                        translations[3 * i + 1],
                        translations[3 * i + 2]);
                    Vector3 angleAxis = (
                        rotations[3 * i + 0],
                        rotations[3 * i + 1],
                        rotations[3 * i + 2]);
                    double angle = angleAxis.Norm;
                    var rotation = angle == 0
                        ? Quaternion.Identity
                        : Quaternion.AngleAxis(angle, angleAxis / angle);
                    markers[i] = new ArucoMarkerPoses(indices[i], new Pose(translation, rotation));
                }
            }

            return markers;
        }

        public QrMarkerPoses[] EstimateQrMarkerPoses(float markerSizeInM)
        {
            int numDetected = Native.GetNumDetectedMarkers(ContextPtr);
            if (numDetected < 0)
            {
                throw new InvalidOperationException("An error happened in the native call");
            }

            if (numDetected == 0)
            {
                return Array.Empty<QrMarkerPoses>();
            }

            var markers = new QrMarkerPoses[numDetected];

            using (var pointers = new Rent<IntPtr>(numDetected))
            using (var pointerLengths = new Rent<int>(numDetected))
            using (var rotations = new Rent<float>(3 * numDetected))
            using (var translations = new Rent<float>(3 * numDetected))
            {
                if (!Native.GetQrMarkerCodes(ContextPtr, pointers.Array, pointerLengths.Array, numDetected) ||
                    !Native.EstimateMarkerPoses(ContextPtr, markerSizeInM, rotations.Array, rotations.Length,
                        translations.Array, translations.Length))
                {
                    throw new InvalidOperationException("An error happened in the native call");
                }

                for (int i = 0; i < numDetected; i++)
                {
                    string code = Marshal.PtrToStringAnsi(pointers[i], pointerLengths[i]);
                    Vector3 translation = (
                        translations[3 * i + 0],
                        translations[3 * i + 1],
                        translations[3 * i + 2]);
                    Vector3 angleAxis = (
                        rotations[3 * i + 0],
                        rotations[3 * i + 1],
                        rotations[3 * i + 2]);
                    double angle = angleAxis.Norm;
                    var rotation = angle == 0
                        ? Quaternion.Identity
                        : Quaternion.AngleAxis(angle, angleAxis / angle);
                    markers[i] = new QrMarkerPoses(code, new Pose(translation, rotation));
                }
            }

            return markers;
        }

        public QrMarkerCorners[] GetDetectedQrCorners()
        {
            int numDetected = Native.GetNumDetectedMarkers(ContextPtr);
            if (numDetected < 0)
            {
                throw new InvalidOperationException("An error happened in the native call");
            }

            if (numDetected == 0)
            {
                return Array.Empty<QrMarkerCorners>();
            }

            var markers = new QrMarkerCorners[numDetected];

            using (var pointers = new Rent<IntPtr>(numDetected))
            using (var pointerLengths = new Rent<int>(numDetected))
            using (var corners = new Rent<float>(8 * numDetected))
            {
                if (!Native.GetQrMarkerCodes(ContextPtr, pointers.Array, pointerLengths.Array, numDetected) ||
                    !Native.GetMarkerCorners(ContextPtr, corners.Array, corners.Length))
                {
                    throw new InvalidOperationException("An error happened in the native call");
                }

                int o = 0;
                for (int i = 0; i < numDetected; i++)
                {
                    string code = Marshal.PtrToStringAnsi(pointers[i], pointerLengths[i]);
                    markers[i] = new QrMarkerCorners(code, new Vector2f[]
                    {
                        (corners[o++], corners[o++]),
                        (corners[o++], corners[o++]),
                        (corners[o++], corners[o++]),
                        (corners[o++], corners[o++]),
                    });
                }
            }

            return markers;
        }

        public ArucoMarkerCorners[] GetDetectedArucoCorners()
        {
            int numDetected = Native.GetNumDetectedMarkers(ContextPtr);
            if (numDetected < 0)
            {
                throw new InvalidOperationException("An error happened in the native call");
            }

            if (numDetected == 0)
            {
                return Array.Empty<ArucoMarkerCorners>();
            }

            var markers = new ArucoMarkerCorners[numDetected];

            using (var indices = new Rent<int>(numDetected))
            using (var corners = new Rent<float>(8 * numDetected))
            {
                if (!Native.GetArucoMarkerIds(ContextPtr, indices.Array, indices.Length) ||
                    !Native.GetMarkerCorners(ContextPtr, corners.Array, corners.Length))
                {
                    throw new InvalidOperationException("An error happened in the native call");
                }

                int o = 0;
                for (int i = 0; i < numDetected; i++)
                {
                    markers[i] = new ArucoMarkerCorners(indices[i], new Vector2f[]
                    {
                        (corners[o++], corners[o++]),
                        (corners[o++], corners[o++]),
                        (corners[o++], corners[o++]),
                        (corners[o++], corners[o++]),
                    });
                }
            }

            return markers;
        }

        static (float, Transform) Umeyama(Vector3f[] input, Vector3f[] output, bool estimateScale)
        {
            if (input.Length != output.Length)
            {
                throw new InvalidOperationException("Input and output lengths do not match");
            }

            using (var inputFloats = new Rent<float>(input.Length * 3))
            using (var outputFloats = new Rent<float>(output.Length * 3))
            using (var resultFloats = new Rent<float>(7))
            {
                int o = 0;
                foreach (var v in input)
                {
                    (inputFloats[o], inputFloats[o + 1], inputFloats[o + 2]) = v;
                    o += 3;
                }

                o = 0;
                foreach (var v in output)
                {
                    (outputFloats[o], outputFloats[o + 1], outputFloats[o + 2]) = v;
                    o += 3;
                }

                Native.EstimateUmeyama(inputFloats.Array, inputFloats.Length, outputFloats.Array, outputFloats.Length,
                    estimateScale, resultFloats.Array, resultFloats.Length);
                
                Vector3 translation = (resultFloats[3], resultFloats[4], resultFloats[5]);
                Vector3 angleAxis = (resultFloats[0], resultFloats[1], resultFloats[2]);
                double angle = angleAxis.Norm;
                var rotation = angle == 0
                    ? Quaternion.Identity
                    : Quaternion.AngleAxis(angle, angleAxis / angle);

                return (resultFloats[6], (translation, rotation));
            }
        }

        void ReleaseUnmanagedResources()
        {
            Native.DisposeContext(mContextPtr);
        }

        public void Dispose()
        {
            if (disposed) return;
            disposed = true;
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }


        ~Context()
        {
            ReleaseUnmanagedResources();
        }

        static class Native
        {
            public delegate void Callback(string s);

            [DllImport("IvizOpencv")]
            public static extern IntPtr CreateContext(int width, int height);

            [DllImport("IvizOpencv")]
            public static extern void DisposeContext(IntPtr ctx);

            [DllImport("IvizOpencv")]
            public static extern void SetupDebug(Callback callback);

            [DllImport("IvizOpencv")]
            public static extern void SetupInfo(Callback callback);

            [DllImport("IvizOpencv")]
            public static extern void SetupError(Callback callback);


            [DllImport("IvizOpencv")]
            public static extern int ImageWidth(IntPtr ctx);

            [DllImport("IvizOpencv")]
            public static extern int ImageHeight(IntPtr ctx);

            [DllImport("IvizOpencv")]
            public static extern int ImageFormat(IntPtr ctx);

            [DllImport("IvizOpencv")]
            public static extern int ImageSize(IntPtr ctx);

            [DllImport("IvizOpencv")]
            public static extern bool CopyFrom(IntPtr ctx, /* const */ byte[] ptr, int size);

            [DllImport("IvizOpencv")]
            public static extern bool CopyTo( /* const */ IntPtr ctx, byte[] ptr, int size);

            [DllImport("IvizOpencv")]
            public static extern IntPtr GetImagePtr(IntPtr ctx);

            [DllImport("IvizOpencv")]
            public static extern bool SetDictionary(IntPtr ctx, int value);

            [DllImport("IvizOpencv")]
            public static extern bool DetectArucoMarkers(IntPtr ctx);

            [DllImport("IvizOpencv")]
            public static extern bool DetectQrMarkers(IntPtr ctx);

            [DllImport("IvizOpencv")]
            public static extern int GetNumDetectedMarkers(IntPtr ctx);

            [DllImport("IvizOpencv")]
            public static extern bool GetArucoMarkerIds(IntPtr ctx, int[] arrayPtr, int arraySize);

            [DllImport("IvizOpencv")]
            public static extern bool GetQrMarkerCodes(IntPtr ctx, IntPtr[] arrayPtr, int[] arrayLengths,
                int arraySize);

            [DllImport("IvizOpencv")]
            public static extern bool GetMarkerCorners(IntPtr ctx, float[] arrayPtr, int arraySize);

            [DllImport("IvizOpencv")]
            public static extern bool SetCameraMatrix(IntPtr ctx, float[] arrayPtr, int arraySize);

            [DllImport("IvizOpencv")]
            public static extern bool EstimateMarkerPoses(IntPtr ctx, float markerSize, float[] rotations,
                int rotationsSize, float[] translations, int translationsSize);

            [DllImport("IvizOpencv")]
            public static extern bool EstimateUmeyama(float[] inputs, int inputSize, float[] outputs, int outputSize,
                bool estimateScale, float[] result, int resultSize);
        }
    }

    public enum ArucoDictionaryName
    {
        Dict4X450 = 0,
        Dict4X4100,
        Dict4X4250,
        Dict4X41000,
        Dict5X550,
        Dict5X5100,
        Dict5X5250,
        Dict5X51000,
        Dict6X650,
        Dict6X6100,
        Dict6X6250,
        Dict6X61000,
        Dict7X750,
        Dict7X7100,
        Dict7X7250,
        Dict7X71000,
        DictArucoOriginal,

        /// 4x4 bits, minimum hamming distance between any two codes = 5, 30 codes
        DictApriltag16H5,

        /// 5x5 bits, minimum hamming distance between any two codes = 9, 35 codes
        DictApriltag25H9,

        /// 6x6 bits, minimum hamming distance between any two codes = 10, 2320 codes
        DictApriltag36H10,

        /// 6x6 bits, minimum hamming distance between any two codes = 11, 587 codes
        DictApriltag36H11
    };

    public readonly struct ArucoMarkerPoses
    {
        public int Id { get; }
        public Pose Pose { get; }

        public ArucoMarkerPoses(int id, in Pose pose) => (Id, Pose) = (id, pose);

        public override string ToString() => "{\"Id\":" + Id + ", \"Pose\":" + Pose + "}";
    }

    [Serializable]
    public readonly struct ArucoMarkerCorners
    {
        public int Id { get; }
        public ReadOnlyCollection<Vector2f> Corners { get; }

        public ArucoMarkerCorners(int id, Vector2f[] corners) => (Id, Corners) = (id, corners.AsReadOnly());

        public override string ToString() => "{\"Id\":" + Id + ", \"Corners\":" +
                                             string.Join(", ", Corners.Select(corner => corner.ToString())) + "}";
    }

    public readonly struct QrMarkerPoses
    {
        public string Code { get; }
        public Pose Pose { get; }

        public QrMarkerPoses(string code, in Pose pose) => (Code, Pose) = (code, pose);

        public override string ToString() => "{\"Code\":" + Code + ", \"Pose\":" + Pose + "}";
    }

    public readonly struct QrMarkerCorners
    {
        public string Code { get; }
        public ReadOnlyCollection<Vector2f> Corners { get; }

        public QrMarkerCorners(string code, Vector2f[] corners) => (Code, Corners) = (code, corners.AsReadOnly());

        public override string ToString() => "{\"Code\":" + Code + ", \"Corners\":" +
                                             string.Join(", ", Corners.Select(corner => corner.ToString())) + "}";
    }
}