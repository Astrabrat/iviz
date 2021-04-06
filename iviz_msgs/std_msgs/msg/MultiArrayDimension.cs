/* This file was created automatically, do not edit! */

using System.Runtime.Serialization;

namespace Iviz.Msgs.StdMsgs
{
    [Preserve, DataContract (Name = "std_msgs/MultiArrayDimension")]
    public sealed class MultiArrayDimension : IDeserializable<MultiArrayDimension>, IMessage
    {
        [DataMember (Name = "label")] public string Label { get; set; } // label of given dimension
        [DataMember (Name = "size")] public uint Size { get; set; } // size of given dimension (in type units)
        [DataMember (Name = "stride")] public uint Stride { get; set; } // stride of given dimension
    
        /// <summary> Constructor for empty message. </summary>
        public MultiArrayDimension()
        {
            Label = string.Empty;
        }
        
        /// <summary> Explicit constructor. </summary>
        public MultiArrayDimension(string Label, uint Size, uint Stride)
        {
            this.Label = Label;
            this.Size = Size;
            this.Stride = Stride;
        }
        
        /// <summary> Constructor with buffer. </summary>
        public MultiArrayDimension(ref Buffer b)
        {
            Label = b.DeserializeString();
            Size = b.Deserialize<uint>();
            Stride = b.Deserialize<uint>();
        }
        
        public ISerializable RosDeserialize(ref Buffer b)
        {
            return new MultiArrayDimension(ref b);
        }
        
        MultiArrayDimension IDeserializable<MultiArrayDimension>.RosDeserialize(ref Buffer b)
        {
            return new MultiArrayDimension(ref b);
        }
    
        public void RosSerialize(ref Buffer b)
        {
            b.Serialize(Label);
            b.Serialize(Size);
            b.Serialize(Stride);
        }
        
        public void Dispose()
        {
        }
        
        public void RosValidate()
        {
            if (Label is null) throw new System.NullReferenceException(nameof(Label));
        }
    
        public int RosMessageLength
        {
            get {
                int size = 12;
                size += BuiltIns.UTF8.GetByteCount(Label);
                return size;
            }
        }
    
        public string RosType => RosMessageType;
    
        /// <summary> Full ROS name of this message. </summary>
        [Preserve] public const string RosMessageType = "std_msgs/MultiArrayDimension";
    
        /// <summary> MD5 hash of a compact representation of the message. </summary>
        [Preserve] public const string RosMd5Sum = "4cd0c83a8683deae40ecdac60e53bfa8";
    
        /// <summary> Base64 of the GZip'd compression of the concatenated dependencies file. </summary>
        [Preserve] public const string RosDependenciesBase64 =
                "H4sIAAAAAAAAE22NMQqAMBAEe1+xYKOtvkjJGRbiRbxE0NcrUWy0m2KGsbRSPcIwSgBQPxQneG6icJxF" +
                "jVGrTE19B+MhKGahr4iGirQvgqxM1r7hdXJSwpt+HicAFGWdjgAAAA==";
                
        public override string ToString() => Extensions.ToString(this);
    }
}
