/* This file was created automatically, do not edit! */

using System.Runtime.Serialization;

namespace Iviz.Msgs.MeshMsgs
{
    [Preserve, DataContract (Name = "mesh_msgs/MeshMaterialsStamped")]
    public sealed class MeshMaterialsStamped : IDeserializable<MeshMaterialsStamped>, IMessage
    {
        // Mesh Attribute Message
        [DataMember (Name = "header")] public StdMsgs.Header Header { get; set; }
        [DataMember (Name = "uuid")] public string Uuid { get; set; }
        [DataMember (Name = "mesh_materials")] public MeshMsgs.MeshMaterials MeshMaterials { get; set; }
    
        /// <summary> Constructor for empty message. </summary>
        public MeshMaterialsStamped()
        {
            Uuid = string.Empty;
            MeshMaterials = new MeshMsgs.MeshMaterials();
        }
        
        /// <summary> Explicit constructor. </summary>
        public MeshMaterialsStamped(in StdMsgs.Header Header, string Uuid, MeshMsgs.MeshMaterials MeshMaterials)
        {
            this.Header = Header;
            this.Uuid = Uuid;
            this.MeshMaterials = MeshMaterials;
        }
        
        /// <summary> Constructor with buffer. </summary>
        public MeshMaterialsStamped(ref Buffer b)
        {
            Header = new StdMsgs.Header(ref b);
            Uuid = b.DeserializeString();
            MeshMaterials = new MeshMsgs.MeshMaterials(ref b);
        }
        
        public ISerializable RosDeserialize(ref Buffer b)
        {
            return new MeshMaterialsStamped(ref b);
        }
        
        MeshMaterialsStamped IDeserializable<MeshMaterialsStamped>.RosDeserialize(ref Buffer b)
        {
            return new MeshMaterialsStamped(ref b);
        }
    
        public void RosSerialize(ref Buffer b)
        {
            Header.RosSerialize(ref b);
            b.Serialize(Uuid);
            MeshMaterials.RosSerialize(ref b);
        }
        
        public void Dispose()
        {
        }
        
        public void RosValidate()
        {
            if (Uuid is null) throw new System.NullReferenceException(nameof(Uuid));
            if (MeshMaterials is null) throw new System.NullReferenceException(nameof(MeshMaterials));
            MeshMaterials.RosValidate();
        }
    
        public int RosMessageLength
        {
            get {
                int size = 4;
                size += Header.RosMessageLength;
                size += BuiltIns.UTF8.GetByteCount(Uuid);
                size += MeshMaterials.RosMessageLength;
                return size;
            }
        }
    
        public string RosType => RosMessageType;
    
        /// <summary> Full ROS name of this message. </summary>
        [Preserve] public const string RosMessageType = "mesh_msgs/MeshMaterialsStamped";
    
        /// <summary> MD5 hash of a compact representation of the message. </summary>
        [Preserve] public const string RosMd5Sum = "80683ad6336327fea277cb1ed5f58927";
    
        /// <summary> Base64 of the GZip'd compression of the concatenated dependencies file. </summary>
        [Preserve] public const string RosDependenciesBase64 =
                "H4sIAAAAAAAAE71UTWvbQBC9768Y8CFJwSm0N0MPqUPSHAylMb2UIkbSWFpY7ar74dj/vm/lSnZSDD00" +
                "MTKaXc17++ZrZ7SS0NJNjF6XKUpeBm5EhVgXXWjC+y/CtXhqhxe2vbYNpaRr1QF58MkcK47iNZtAh/1x" +
                "qdSn//xTq8f7Bb0QqGb0GNnW7GsIiFxzZNo4CNdNK35uZCsGIO56qWn4Gve9hGsA160OhKcRK56N2VMK" +
                "cIqOKtd1yeoKwVDUCOwUD6S2xNSzj7pKhj38na+1ze4bz51kdjxBfiWxldDD7QI+NkiVooagPRgqLxxy" +
                "Uh9uSSVt48cPGaBm6yc3x1IapH86nGLLMYuVXe9RK4jhsMAZ7w7BXYMbyRGcUge6HPYKLMMV4RBIkN5V" +
                "LV1C+dd9bJ0FodCWUavSSCaukAGwXmTQxdUJsx2oLVs30h8Yj2f8C62deHNM8xY1Mzn6kBokEI69d1td" +
                "w7XcDySV0WIjGV169nuVUYcj1ewu5xhOQA0VwZtDcJVGAWp60rEdG3aoRoGmfaVuPDMLiPLMfD0H3HEl" +
                "S5MCYD9+IuLBCmcmDB7H6To0zBF0MnjP0d/FR9mtZbfMPRqA2A47Rf4Pfftqo3o2VjUbjSmMDb4W2ta6" +
                "EmTP9VE7y2Yso+FSzNvWcBxJpCkmP2iT3fF6XDrj/Lf7zzeYa1iqdM5Qy6H44//q198kQG2M46zUT1Yz" +
                "WeVk8duk70W7/T0Ia9y+k6g0WVulfgPgQWQKlAYAAA==";
                
        public override string ToString() => Extensions.ToString(this);
    }
}
