﻿using System.Runtime.CompilerServices;
using Iviz.Core;
using JetBrains.Annotations;
using Unity.Mathematics;
using UnityEngine;

namespace Iviz.Displays
{
    /// <summary>
    /// Struct for position and color/intensity.
    /// The same field is read either as a Color32 or a float depending on whether UseColormap is enabled in the display.
    /// Internally a wrapper around <see cref="float4"/>. 
    /// </summary>
    public readonly struct PointWithColor
    {
        /// <summary>
        /// Color representation from the bits of a float.
        /// </summary>
        internal static Color32 ColorFromFloatBits(float f)
        {
            unsafe
            {
                return *(Color32*) &f;
            }
        }

        /// <summary>
        /// Float representation from the bits of a Color32.
        /// </summary>
        public static float FloatFromColorBits(Color32 f)
        {
            unsafe
            {
                return *(float*) &f;
            }
        }

        public readonly float4 f;
        
        public float3 Position => f.xyz;
        Color32 Color => ColorFromFloatBits(f.w);
        float Intensity => f.w;

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public PointWithColor(in Vector3 position, Color32 color) :
            this(position.x, position.y, position.z, FloatFromColorBits(color))
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public PointWithColor(in Vector3 position, float intensity) :
            this(position.x, position.y, position.z, intensity)
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public PointWithColor(float x, float y, float z, float w)
        {
            f.x = x;
            f.y = y;
            f.z = z;
            f.w = w;
        }

        /// <summary>
        /// Do the positions have a Nan? (ignores intensity) 
        /// </summary>        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]        
        public bool HasNaN() => f.HasNaN();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [NotNull]
        public override string ToString()
        {
            return $"[x={f.x} y={f.y} z={f.z} i={Intensity} c={Color}]";
        }
    }
}