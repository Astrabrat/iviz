/* This file was created automatically, do not edit! */

using System.Runtime.Serialization;

namespace Iviz.Msgs.StdMsgs
{
    [DataContract]
    public sealed class Duration : IDeserializable<Duration>, IMessage
    {
        [DataMember (Name = "data")] public duration Data;
    
        /// Constructor for empty message.
        public Duration()
        {
        }
        
        /// Explicit constructor.
        public Duration(duration Data)
        {
            this.Data = Data;
        }
        
        /// Constructor with buffer.
        public Duration(ref ReadBuffer b)
        {
            b.Deserialize(out Data);
        }
        
        ISerializable ISerializable.RosDeserializeBase(ref ReadBuffer b) => new Duration(ref b);
        
        public Duration RosDeserialize(ref ReadBuffer b) => new Duration(ref b);
    
        public void RosSerialize(ref WriteBuffer b)
        {
            b.Serialize(Data);
        }
        
        public void RosValidate()
        {
        }
    
        /// <summary> Constant size of this message. </summary> 
        public const int RosFixedMessageLength = 8;
        
        public int RosMessageLength => RosFixedMessageLength;
    
        /// <summary> Full ROS name of this message. </summary>
        public const string MessageType = "std_msgs/Duration";
    
        public string RosMessageType => MessageType;
    
        /// <summary> MD5 hash of a compact representation of the message. </summary>
        public const string Md5Sum = "3e286caf4241d664e55f3ad380e2ae46";
    
        public string RosMd5Sum => Md5Sum;
    
        /// <summary> Base64 of the GZip'd compression of the concatenated dependencies file. </summary>
        public string RosDependenciesBase64 =>
                "H4sIAAAAAAAAE0spLUosyczPU0hJLEnk4gIAtVhIcg8AAAA=";
                
    
        public override string ToString() => Extensions.ToString(this);
    }
}
