﻿using System;

namespace Iviz.Msgs
{
    /// <summary>
    /// Interface for all ROS messages.
    /// All classes or structs representing ROS messages derive from this.
    /// </summary>
    public interface IMessage : ISerializable, IDisposable
    {
        /// <summary>
        /// Full ROS name of the message.
        /// </summary>
        string RosMessageType { get; }
        
        /// <summary> MD5 hash of a compact representation of the message. </summary>
        public string RosMd5Sum { get; }

        /// <summary> Base64 of the GZip'd compression of the concatenated dependencies file. </summary>
        public string RosDependenciesBase64 { get; }

        void IDisposable.Dispose()
        {
        }
    }
}