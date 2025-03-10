/* This file was created automatically, do not edit! */

using System.Runtime.Serialization;

namespace Iviz.Msgs.MeshMsgs
{
    [DataContract]
    public sealed class MeshFeatures : IDeserializable<MeshFeatures>, IMessage
    {
        [DataMember (Name = "map_uuid")] public string MapUuid;
        [DataMember (Name = "features")] public MeshMsgs.Feature[] Features;
    
        /// Constructor for empty message.
        public MeshFeatures()
        {
            MapUuid = "";
            Features = System.Array.Empty<MeshMsgs.Feature>();
        }
        
        /// Explicit constructor.
        public MeshFeatures(string MapUuid, MeshMsgs.Feature[] Features)
        {
            this.MapUuid = MapUuid;
            this.Features = Features;
        }
        
        /// Constructor with buffer.
        public MeshFeatures(ref ReadBuffer b)
        {
            b.DeserializeString(out MapUuid);
            b.DeserializeArray(out Features);
            for (int i = 0; i < Features.Length; i++)
            {
                Features[i] = new MeshMsgs.Feature(ref b);
            }
        }
        
        ISerializable ISerializable.RosDeserializeBase(ref ReadBuffer b) => new MeshFeatures(ref b);
        
        public MeshFeatures RosDeserialize(ref ReadBuffer b) => new MeshFeatures(ref b);
    
        public void RosSerialize(ref WriteBuffer b)
        {
            b.Serialize(MapUuid);
            b.SerializeArray(Features);
        }
        
        public void RosValidate()
        {
            if (MapUuid is null) BuiltIns.ThrowNullReference();
            if (Features is null) BuiltIns.ThrowNullReference();
            for (int i = 0; i < Features.Length; i++)
            {
                if (Features[i] is null) BuiltIns.ThrowNullReference(nameof(Features), i);
                Features[i].RosValidate();
            }
        }
    
        public int RosMessageLength => 8 + BuiltIns.GetStringSize(MapUuid) + BuiltIns.GetArraySize(Features);
    
        /// <summary> Full ROS name of this message. </summary>
        public const string MessageType = "mesh_msgs/MeshFeatures";
    
        public string RosMessageType => MessageType;
    
        /// <summary> MD5 hash of a compact representation of the message. </summary>
        public const string Md5Sum = "ea0bfd1049bc24f2cd76d68461f1f987";
    
        public string RosMd5Sum => Md5Sum;
    
        /// <summary> Base64 of the GZip'd compression of the concatenated dependencies file. </summary>
        public string RosDependenciesBase64 =>
                "H4sIAAAAAAAAE7XQwQqCYAwH8PueYtADBBYdgq51CoK6RcjQqYP8Jt8mZE+fYniojrnTf2OwHzOPEkqs" +
                "qUnbVnKo2aq0ttKWeyZvI19vWIzJAHZ/LjieD1v8ugkla80eu3F6UgmOd83IRQOY5+/tu5Kvkl6Ys2VR" +
                "Gtc4l/GHCBZ4qcQw0+AkwdArxkZNBiVqgdR3g1wCFpEZraGMoRjUmzU+ptRN6TkX//Nn48VVgjk5wQu3" +
                "LIB6BQIAAA==";
                
    
        public override string ToString() => Extensions.ToString(this);
    }
}
