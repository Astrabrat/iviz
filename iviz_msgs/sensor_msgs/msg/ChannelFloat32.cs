/* This file was created automatically, do not edit! */

using System.Runtime.Serialization;

namespace Iviz.Msgs.SensorMsgs
{
    [Preserve, DataContract (Name = "sensor_msgs/ChannelFloat32")]
    public sealed class ChannelFloat32 : IDeserializable<ChannelFloat32>, IMessage
    {
        // This message is used by the PointCloud message to hold optional data
        // associated with each point in the cloud. The length of the values
        // array should be the same as the length of the points array in the
        // PointCloud, and each value should be associated with the corresponding
        // point.
        // Channel names in existing practice include:
        //   "u", "v" - row and column (respectively) in the left stereo image.
        //              This is opposite to usual conventions but remains for
        //              historical reasons. The newer PointCloud2 message has no
        //              such problem.
        //   "rgb" - For point clouds produced by color stereo cameras. uint8
        //           (R,G,B) values packed into the least significant 24 bits,
        //           in order.
        //   "intensity" - laser or pixel intensity.
        //   "distance"
        // The channel name should give semantics of the channel (e.g.
        // "intensity" instead of "value").
        [DataMember (Name = "name")] public string Name { get; set; }
        // The values array should be 1-1 with the elements of the associated
        // PointCloud.
        [DataMember (Name = "values")] public float[] Values { get; set; }
    
        /// <summary> Constructor for empty message. </summary>
        public ChannelFloat32()
        {
            Name = string.Empty;
            Values = System.Array.Empty<float>();
        }
        
        /// <summary> Explicit constructor. </summary>
        public ChannelFloat32(string Name, float[] Values)
        {
            this.Name = Name;
            this.Values = Values;
        }
        
        /// <summary> Constructor with buffer. </summary>
        public ChannelFloat32(ref Buffer b)
        {
            Name = b.DeserializeString();
            Values = b.DeserializeStructArray<float>();
        }
        
        public ISerializable RosDeserialize(ref Buffer b)
        {
            return new ChannelFloat32(ref b);
        }
        
        ChannelFloat32 IDeserializable<ChannelFloat32>.RosDeserialize(ref Buffer b)
        {
            return new ChannelFloat32(ref b);
        }
    
        public void RosSerialize(ref Buffer b)
        {
            b.Serialize(Name);
            b.SerializeStructArray(Values, 0);
        }
        
        public void Dispose()
        {
        }
        
        public void RosValidate()
        {
            if (Name is null) throw new System.NullReferenceException(nameof(Name));
            if (Values is null) throw new System.NullReferenceException(nameof(Values));
        }
    
        public int RosMessageLength
        {
            get {
                int size = 8;
                size += BuiltIns.UTF8.GetByteCount(Name);
                size += 4 * Values.Length;
                return size;
            }
        }
    
        public string RosType => RosMessageType;
    
        /// <summary> Full ROS name of this message. </summary>
        [Preserve] public const string RosMessageType = "sensor_msgs/ChannelFloat32";
    
        /// <summary> MD5 hash of a compact representation of the message. </summary>
        [Preserve] public const string RosMd5Sum = "3d40139cdd33dfedcb71ffeeeb42ae7f";
    
        /// <summary> Base64 of the GZip'd compression of the concatenated dependencies file. </summary>
        [Preserve] public const string RosDependenciesBase64 =
                "H4sIAAAAAAAAE11Ty47bMAy8+ysI7yUBsgY27aHosQu016LoreiBlhhbqCwaopRs/r6UYudl+CBA5Mxw" +
                "hnqB36MTmEgEBwI9ZiEL/RnSSPCTXUjvnrO9ViSGkb0FnpPjgB4sJmxeAEXYOEzafHJpBEIzwlz6wYUK" +
                "ZgpOp3wEnsKgNXyoF0f0maRgxIhnkJGzEvRULwUnUvB6fmyr4LI0XTgU4yZ5BxjsRUdluAN+FlvlcYwk" +
                "MwfrwqBAFb5r9PQ+YgjkIagUKUz04SRpFcwRTXJGfQvGZ0tftRqgze0O2mMLrxD5VFUY9nkKsCkMpC1H" +
                "8uftaoynQwJJFInBTWpyV2HuvpqR/jzPLC7VFLJkdd9wOFIoUQj0OUGkCZ2eDxyfQRQjcXRGuyKhaMcl" +
                "jEAnine+7a9Zj+p74GccySXZyL2n6aK0jUNfpv3OcYm8Zi2lymZz2Se1QK+XMY1aGVEFZK3+8sCw+bX7" +
                "sfu2XbYCZjT/FEDLeDELRd1yQ3AHnUW59p+hd0l2DyhqLUdLcRGo7RTUuXOR6VF04KLVfWis17ul1qpP" +
                "GAy1JftikLnLf12iQSMEUbPVeyPrSq6VG+qGgnbPq6kkQltK2zpbu+0aSbHsUUFe2Za5n9/C2+vbbVdJ" +
                "raey/AvvbZ8fXkDXHDxj+rT/83d9ZM1/XIL8vPEDAAA=";
                
        public override string ToString() => Extensions.ToString(this);
    }
}
