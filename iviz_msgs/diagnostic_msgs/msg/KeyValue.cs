/* This file was created automatically, do not edit! */

using System.Runtime.Serialization;

namespace Iviz.Msgs.DiagnosticMsgs
{
    [Preserve, DataContract (Name = "diagnostic_msgs/KeyValue")]
    public sealed class KeyValue : IDeserializable<KeyValue>, IMessage
    {
        [DataMember (Name = "key")] public string Key { get; set; } // what to label this value when viewing
        [DataMember (Name = "value")] public string Value { get; set; } // a value to track over time
    
        /// <summary> Constructor for empty message. </summary>
        public KeyValue()
        {
            Key = string.Empty;
            Value = string.Empty;
        }
        
        /// <summary> Explicit constructor. </summary>
        public KeyValue(string Key, string Value)
        {
            this.Key = Key;
            this.Value = Value;
        }
        
        /// <summary> Constructor with buffer. </summary>
        public KeyValue(ref Buffer b)
        {
            Key = b.DeserializeString();
            Value = b.DeserializeString();
        }
        
        public ISerializable RosDeserialize(ref Buffer b)
        {
            return new KeyValue(ref b);
        }
        
        KeyValue IDeserializable<KeyValue>.RosDeserialize(ref Buffer b)
        {
            return new KeyValue(ref b);
        }
    
        public void RosSerialize(ref Buffer b)
        {
            b.Serialize(Key);
            b.Serialize(Value);
        }
        
        public void Dispose()
        {
        }
        
        public void RosValidate()
        {
            if (Key is null) throw new System.NullReferenceException(nameof(Key));
            if (Value is null) throw new System.NullReferenceException(nameof(Value));
        }
    
        public int RosMessageLength
        {
            get {
                int size = 8;
                size += BuiltIns.UTF8.GetByteCount(Key);
                size += BuiltIns.UTF8.GetByteCount(Value);
                return size;
            }
        }
    
        public string RosType => RosMessageType;
    
        /// <summary> Full ROS name of this message. </summary>
        [Preserve] public const string RosMessageType = "diagnostic_msgs/KeyValue";
    
        /// <summary> MD5 hash of a compact representation of the message. </summary>
        [Preserve] public const string RosMd5Sum = "cf57fdc6617a881a88c16e768132149c";
    
        /// <summary> Base64 of the GZip'd compression of the concatenated dependencies file. </summary>
        [Preserve] public const string RosDependenciesBase64 =
                "H4sIAAAAAAAAEy2LQQrAIBDE7r5iwJdty1AXrYJulf6+C/YWSDKsa72Q+SJiJTFYQ5GDBZZ0YEp56IIV" +
                "U7m8DWMv20TIT/5ZlzOjTXaY3gzhA0OUpa5eAAAA";
                
        public override string ToString() => Extensions.ToString(this);
    }
}
