/* This file was created automatically, do not edit! */

using System.Runtime.Serialization;

namespace Iviz.Msgs.GeometryMsgs
{
    [Preserve, DataContract (Name = "geometry_msgs/Vector3Stamped")]
    public sealed class Vector3Stamped : IDeserializable<Vector3Stamped>, IMessage
    {
        // This represents a Vector3 with reference coordinate frame and timestamp
        [DataMember (Name = "header")] public StdMsgs.Header Header { get; set; }
        [DataMember (Name = "vector")] public Vector3 Vector { get; set; }
    
        /// <summary> Constructor for empty message. </summary>
        public Vector3Stamped()
        {
        }
        
        /// <summary> Explicit constructor. </summary>
        public Vector3Stamped(in StdMsgs.Header Header, in Vector3 Vector)
        {
            this.Header = Header;
            this.Vector = Vector;
        }
        
        /// <summary> Constructor with buffer. </summary>
        public Vector3Stamped(ref Buffer b)
        {
            Header = new StdMsgs.Header(ref b);
            Vector = new Vector3(ref b);
        }
        
        public ISerializable RosDeserialize(ref Buffer b)
        {
            return new Vector3Stamped(ref b);
        }
        
        Vector3Stamped IDeserializable<Vector3Stamped>.RosDeserialize(ref Buffer b)
        {
            return new Vector3Stamped(ref b);
        }
    
        public void RosSerialize(ref Buffer b)
        {
            Header.RosSerialize(ref b);
            Vector.RosSerialize(ref b);
        }
        
        public void Dispose()
        {
        }
        
        public void RosValidate()
        {
        }
    
        public int RosMessageLength
        {
            get {
                int size = 24;
                size += Header.RosMessageLength;
                return size;
            }
        }
    
        public string RosType => RosMessageType;
    
        /// <summary> Full ROS name of this message. </summary>
        [Preserve] public const string RosMessageType = "geometry_msgs/Vector3Stamped";
    
        /// <summary> MD5 hash of a compact representation of the message. </summary>
        [Preserve] public const string RosMd5Sum = "7b324c7325e683bf02a9b14b01090ec7";
    
        /// <summary> Base64 of the GZip'd compression of the concatenated dependencies file. </summary>
        [Preserve] public const string RosDependenciesBase64 =
                "H4sIAAAAAAAAE7VUwWrcMBC96ysG9pBN2biQlB4WeittcygEEnoNs9bYFrUlVxrvxv36PslZtyGXHtrF" +
                "INnSezPz5s1u6KFziaKMUZJ4TcT0TWoN8YZOTjucNBLF10J1CNE6zyrURB6E2FtSN0hSHkbzRdhKpK4s" +
                "5sxxLKsxH/7xz3y9/7ynpPZxSG16uwQ3G7pXZMXR0iDKlpWpCUjKtZ3Eq16O0lNJVyyVU51HSRWARQY8" +
                "rXiJ3PczTQmXNKDuYZi8q3Pha7lnPJDOQ7ORo7p66jm+0imz40nyYyo63n7c445PUk/qkNAMhjoKJ+db" +
                "HJKZnNeb6wwwm4dTuMKrtJB2DU7aseZk5Sm3LefJaY8Yb5biKnBDHEEUm2hbvj3iNV0SgiAFGUPd0RaZ" +
                "383aBQ9CoSNHx4deMnENBcB6kUEXl38w+0Lt2Ycz/cL4O8bf0PqVN9d01aFnfa4+TS0ExMUxhqOzuHqY" +
                "C0ndO5iTeneIHGeTUUtIs/lUvKi5faUjWDmlUDs0wBYPm6Qxs5duPDr7v9zYSoDr4rxY8nkCztZ6MWHL" +
                "VGTnNFFQyci1VNkkt6WtwcMUgzAqhv9WJIDWRUBd8BVYMZcwt+zIKdkgiXxQcAz8HZQCjTOaxxFkMHpk" +
                "n3rO2PwZkK1UbbWjUyd+uZU1Ko4uM+Bqiq51dkEi0LCC17+IHWlzDY37fsl5CYaGgSQGLYDLim4bmsNE" +
                "p1wQNvF59AIdZM2rWERD2OW5e6Z4KehdwCBAlpS4hZt8Ugx9ZUzTB9b37+hp3c3r7qf5BVYBeVDhBAAA";
                
        public override string ToString() => Extensions.ToString(this);
    }
}
