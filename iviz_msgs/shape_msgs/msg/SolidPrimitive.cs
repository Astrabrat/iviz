/* This file was created automatically, do not edit! */

using System.Runtime.Serialization;

namespace Iviz.Msgs.ShapeMsgs
{
    [Preserve, DataContract (Name = "shape_msgs/SolidPrimitive")]
    public sealed class SolidPrimitive : IDeserializable<SolidPrimitive>, IMessage
    {
        // Define box, sphere, cylinder, cone 
        // All shapes are defined to have their bounding boxes centered around 0,0,0.
        public const byte BOX = 1;
        public const byte SPHERE = 2;
        public const byte CYLINDER = 3;
        public const byte CONE = 4;
        // The type of the shape
        [DataMember (Name = "type")] public byte Type;
        // The dimensions of the shape
        [DataMember (Name = "dimensions")] public double[] Dimensions;
        // The meaning of the shape dimensions: each constant defines the index in the 'dimensions' array
        // For the BOX type, the X, Y, and Z dimensions are the length of the corresponding
        // sides of the box.
        public const byte BOX_X = 0;
        public const byte BOX_Y = 1;
        public const byte BOX_Z = 2;
        // For the SPHERE type, only one component is used, and it gives the radius of
        // the sphere.
        public const byte SPHERE_RADIUS = 0;
        // For the CYLINDER and CONE types, the center line is oriented along
        // the Z axis.  Therefore the CYLINDER_HEIGHT (CONE_HEIGHT) component
        // of dimensions gives the height of the cylinder (cone).  The
        // CYLINDER_RADIUS (CONE_RADIUS) component of dimensions gives the
        // radius of the base of the cylinder (cone).  Cone and cylinder
        // primitives are defined to be circular. The tip of the cone is
        // pointing up, along +Z axis.
        public const byte CYLINDER_HEIGHT = 0;
        public const byte CYLINDER_RADIUS = 1;
        public const byte CONE_HEIGHT = 0;
        public const byte CONE_RADIUS = 1;
    
        /// <summary> Constructor for empty message. </summary>
        public SolidPrimitive()
        {
            Dimensions = System.Array.Empty<double>();
        }
        
        /// <summary> Explicit constructor. </summary>
        public SolidPrimitive(byte Type, double[] Dimensions)
        {
            this.Type = Type;
            this.Dimensions = Dimensions;
        }
        
        /// <summary> Constructor with buffer. </summary>
        internal SolidPrimitive(ref Buffer b)
        {
            Type = b.Deserialize<byte>();
            Dimensions = b.DeserializeStructArray<double>();
        }
        
        public ISerializable RosDeserialize(ref Buffer b)
        {
            return new SolidPrimitive(ref b);
        }
        
        SolidPrimitive IDeserializable<SolidPrimitive>.RosDeserialize(ref Buffer b)
        {
            return new SolidPrimitive(ref b);
        }
    
        public void RosSerialize(ref Buffer b)
        {
            b.Serialize(Type);
            b.SerializeStructArray(Dimensions, 0);
        }
        
        public void RosValidate()
        {
            if (Dimensions is null) throw new System.NullReferenceException(nameof(Dimensions));
        }
    
        public int RosMessageLength => 5 + 8 * Dimensions.Length;
    
        public string RosType => RosMessageType;
    
        /// <summary> Full ROS name of this message. </summary>
        [Preserve] public const string RosMessageType = "shape_msgs/SolidPrimitive";
    
        /// <summary> MD5 hash of a compact representation of the message. </summary>
        [Preserve] public const string RosMd5Sum = "d8f8cbc74c5ff283fca29569ccefb45d";
    
        /// <summary> Base64 of the GZip'd compression of the concatenated dependencies file. </summary>
        [Preserve] public const string RosDependenciesBase64 =
                "H4sIAAAAAAAACnVTUWvbMBB+N+w/HOShLTOh28oYAz90jbcERjvSDpKMEVRLiQWOZCS5JP++d5JsaynD" +
                "YHy+u++7++5uAjOxk0rAsz7mYNtaGJFDdWqk4sLgl0ZfNoHbpgFbs1ZYYEYA90kcnIaavQhwtZAGMTrF" +
                "pdoTGAZWQjmE45hBDrjO8ZlmWSeV+wLfHlbFh/j9+GteLsviYzTv1j8X97NyWXzqfzzcl8VNhnU81Uh2" +
                "agXoHZGGmmIU/c/6IC4PQlmplf03dNdo5j7f/PmbRPQ5B8EUlZ8mJGFfQbCqJkmsY8pFEayPJbmO+PbG" +
                "xZhzgc0bdiKG79p4LzbuS829tcphnQNDeTZpzSQyuRuh9q7uK6q0McK22quMkFZy5I9OFH06artdFdeJ" +
                "tR60JmuDUqclBf1jVVo1J3wR2wGpcIggLXRW8FCndLCXL7Fvw7jsqARE85r5DerrCLjb5e1s8fsR60k5" +
                "+yF7TBqwp7dBlbA6gFuI0iK8kfQDN6nRvnGK2QA7SjsFGp0ROx0V63G383LxY/4El4QdjauxJwRB3RLF" +
                "x55wl/e1GzSPtwCXdAtXgQ+zB57QXeQJRsLzP5ZsMmoXxsfssNVvOe9oIKRU78L81siDdB7w7CafEUKa" +
                "qmuYmYaTke24Q15Tytc4JNr3rsXJkrLwPoraH+mZmMNKnTWPy5Vc6pvgURgMfJe9ApRvhQh0BAAA";
                
        public override string ToString() => Extensions.ToString(this);
    }
}
