/* This file was created automatically, do not edit! */

using System.Runtime.Serialization;

namespace Iviz.Msgs.IvizMsgs
{
    [Preserve, DataContract (Name = "iviz_msgs/ColorChannel")]
    public sealed class ColorChannel : IDeserializable<ColorChannel>, IMessage
    {
        [DataMember (Name = "colors")] public Color32[] Colors { get; set; }
    
        /// <summary> Constructor for empty message. </summary>
        public ColorChannel()
        {
            Colors = System.Array.Empty<Color32>();
        }
        
        /// <summary> Explicit constructor. </summary>
        public ColorChannel(Color32[] Colors)
        {
            this.Colors = Colors;
        }
        
        /// <summary> Constructor with buffer. </summary>
        public ColorChannel(ref Buffer b)
        {
            Colors = b.DeserializeStructArray<Color32>();
        }
        
        public ISerializable RosDeserialize(ref Buffer b)
        {
            return new ColorChannel(ref b);
        }
        
        ColorChannel IDeserializable<ColorChannel>.RosDeserialize(ref Buffer b)
        {
            return new ColorChannel(ref b);
        }
    
        public void RosSerialize(ref Buffer b)
        {
            b.SerializeStructArray(Colors, 0);
        }
        
        public void Dispose()
        {
        }
        
        public void RosValidate()
        {
            if (Colors is null) throw new System.NullReferenceException(nameof(Colors));
        }
    
        public int RosMessageLength
        {
            get {
                int size = 4;
                size += 4 * Colors.Length;
                return size;
            }
        }
    
        public string RosType => RosMessageType;
    
        /// <summary> Full ROS name of this message. </summary>
        [Preserve] public const string RosMessageType = "iviz_msgs/ColorChannel";
    
        /// <summary> MD5 hash of a compact representation of the message. </summary>
        [Preserve] public const string RosMd5Sum = "04d8fd1feb40362aeedd2ef19014ebfa";
    
        /// <summary> Base64 of the GZip'd compression of the concatenated dependencies file. </summary>
        [Preserve] public const string RosDependenciesBase64 =
                "H4sIAAAAAAAAE3POz8kvMjaKjlVIBrGKubhsqQy4fIPdrRQyyzKr4nOL04v1nSE2cpVm5pVYKBRB6XQo" +
                "nQSlE7m4AEmfKA6bAAAA";
                
        public override string ToString() => Extensions.ToString(this);
    }
}
