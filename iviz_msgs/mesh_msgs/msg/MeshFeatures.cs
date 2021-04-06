/* This file was created automatically, do not edit! */

using System.Runtime.Serialization;

namespace Iviz.Msgs.MeshMsgs
{
    [Preserve, DataContract (Name = "mesh_msgs/MeshFeatures")]
    public sealed class MeshFeatures : IDeserializable<MeshFeatures>, IMessage
    {
        [DataMember (Name = "map_uuid")] public string MapUuid { get; set; }
        [DataMember (Name = "features")] public MeshMsgs.Feature[] Features { get; set; }
    
        /// <summary> Constructor for empty message. </summary>
        public MeshFeatures()
        {
            MapUuid = string.Empty;
            Features = System.Array.Empty<MeshMsgs.Feature>();
        }
        
        /// <summary> Explicit constructor. </summary>
        public MeshFeatures(string MapUuid, MeshMsgs.Feature[] Features)
        {
            this.MapUuid = MapUuid;
            this.Features = Features;
        }
        
        /// <summary> Constructor with buffer. </summary>
        public MeshFeatures(ref Buffer b)
        {
            MapUuid = b.DeserializeString();
            Features = b.DeserializeArray<MeshMsgs.Feature>();
            for (int i = 0; i < Features.Length; i++)
            {
                Features[i] = new MeshMsgs.Feature(ref b);
            }
        }
        
        public ISerializable RosDeserialize(ref Buffer b)
        {
            return new MeshFeatures(ref b);
        }
        
        MeshFeatures IDeserializable<MeshFeatures>.RosDeserialize(ref Buffer b)
        {
            return new MeshFeatures(ref b);
        }
    
        public void RosSerialize(ref Buffer b)
        {
            b.Serialize(MapUuid);
            b.SerializeArray(Features, 0);
        }
        
        public void Dispose()
        {
        }
        
        public void RosValidate()
        {
            if (MapUuid is null) throw new System.NullReferenceException(nameof(MapUuid));
            if (Features is null) throw new System.NullReferenceException(nameof(Features));
            for (int i = 0; i < Features.Length; i++)
            {
                if (Features[i] is null) throw new System.NullReferenceException($"{nameof(Features)}[{i}]");
                Features[i].RosValidate();
            }
        }
    
        public int RosMessageLength
        {
            get {
                int size = 8;
                size += BuiltIns.UTF8.GetByteCount(MapUuid);
                foreach (var i in Features)
                {
                    size += i.RosMessageLength;
                }
                return size;
            }
        }
    
        public string RosType => RosMessageType;
    
        /// <summary> Full ROS name of this message. </summary>
        [Preserve] public const string RosMessageType = "mesh_msgs/MeshFeatures";
    
        /// <summary> MD5 hash of a compact representation of the message. </summary>
        [Preserve] public const string RosMd5Sum = "ea0bfd1049bc24f2cd76d68461f1f987";
    
        /// <summary> Base64 of the GZip'd compression of the concatenated dependencies file. </summary>
        [Preserve] public const string RosDependenciesBase64 =
                "H4sIAAAAAAAAE7XQwQqCYAwH8PueYtADBBYdgq51CoK6RcjQqYP8Jt8mZE+fYniojrnTf2OwHzOPEkqs" +
                "qUnbVnKo2aq0ttKWeyZvI19vWIzJAHZ/LjieD1v8ugkla80eu3F6UgmOd83IRQOY5+/tu5Kvkl6Ys2VR" +
                "Gtc4l/GHCBZ4qcQw0+AkwdArxkZNBiVqgdR3g1wCFpEZraGMoRjUmzU+ptRN6TkX//Nn48VVgjk5wQu3" +
                "LIB6BQIAAA==";
                
        public override string ToString() => Extensions.ToString(this);
    }
}
