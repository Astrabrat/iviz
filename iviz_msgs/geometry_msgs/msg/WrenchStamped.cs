/* This file was created automatically, do not edit! */

using System.Runtime.Serialization;

namespace Iviz.Msgs.GeometryMsgs
{
    [Preserve, DataContract (Name = "geometry_msgs/WrenchStamped")]
    public sealed class WrenchStamped : IDeserializable<WrenchStamped>, IMessage
    {
        // A wrench with reference coordinate frame and timestamp
        [DataMember (Name = "header")] public StdMsgs.Header Header { get; set; }
        [DataMember (Name = "wrench")] public Wrench Wrench { get; set; }
    
        /// <summary> Constructor for empty message. </summary>
        public WrenchStamped()
        {
            Wrench = new Wrench();
        }
        
        /// <summary> Explicit constructor. </summary>
        public WrenchStamped(in StdMsgs.Header Header, Wrench Wrench)
        {
            this.Header = Header;
            this.Wrench = Wrench;
        }
        
        /// <summary> Constructor with buffer. </summary>
        public WrenchStamped(ref Buffer b)
        {
            Header = new StdMsgs.Header(ref b);
            Wrench = new Wrench(ref b);
        }
        
        public ISerializable RosDeserialize(ref Buffer b)
        {
            return new WrenchStamped(ref b);
        }
        
        WrenchStamped IDeserializable<WrenchStamped>.RosDeserialize(ref Buffer b)
        {
            return new WrenchStamped(ref b);
        }
    
        public void RosSerialize(ref Buffer b)
        {
            Header.RosSerialize(ref b);
            Wrench.RosSerialize(ref b);
        }
        
        public void Dispose()
        {
        }
        
        public void RosValidate()
        {
            if (Wrench is null) throw new System.NullReferenceException(nameof(Wrench));
            Wrench.RosValidate();
        }
    
        public int RosMessageLength
        {
            get {
                int size = 48;
                size += Header.RosMessageLength;
                return size;
            }
        }
    
        public string RosType => RosMessageType;
    
        /// <summary> Full ROS name of this message. </summary>
        [Preserve] public const string RosMessageType = "geometry_msgs/WrenchStamped";
    
        /// <summary> MD5 hash of a compact representation of the message. </summary>
        [Preserve] public const string RosMd5Sum = "d78d3cb249ce23087ade7e7d0c40cfa7";
    
        /// <summary> Base64 of the GZip'd compression of the concatenated dependencies file. </summary>
        [Preserve] public const string RosDependenciesBase64 =
                "H4sIAAAAAAAAE71UwW7UMBC9+ytG2kNbtA1SizhU4oCEgB6QKrWCYzUbTxKLxA72ZLfh63l2tikVHDgA" +
                "q2gTJ35v5s288Ybe0iGKrzs6OO0oSiN5KVSHEK3zrEJN5EGIvSV1gyTlYTQfha1E6srNfDlSlJsxb/7y" +
                "z3y6/XBFSe39kNr0coltNnSrSIqjpUGULStTE5CTazuJ573spaeSrVgqX3UeJVUA3nUuEa5WvETu+5mm" +
                "hE0aIHsYJu/qrHtV+4gH0nliGjmqq6ee4y9lyuy4knybShmv311hj09ST+qQ0AyGOgon51t8JDM5r5cX" +
                "GWA2d4dwjqW0qOwanLRjzcnKwxgl5Tw5XSHGi0VcBW4URxDFJjot7+6xTGeEIEhBxoDenCLzm1m74EEo" +
                "tOfoeNdLJq5RAbCeZNDJ2U/MvlB79uGRfmF8ivEntH7lzZrOO/Ssz+rT1KKA2DjGsHcWW3dzIal7J16p" +
                "d7vIcTYZtYQ0m/fFiprbVzqCO6cUaocG2GJhkzRm9tKNe2f/lRtbCXBdnBdLLgPw6KwouVPQkLIhUTAU" +
                "qYkCFSPXskWz4KCSMbodsquwEzUR1COPGfu2mCv7DH79LLWGeEkL2dMS/3DZ/xF4DPobhUz78u25yCpP" +
                "wXXxbfBw/SCMlmLAViSA1kVAXfAVWHHuQB+q45RskEQ+KDgG/gpKgYkymscRZJjkyD71nLH5NSCnUrXV" +
                "lg6d+GVXNkEZ2TLkrqboWmcXJAINK5jpKG5L2lzARH2/5LwEgyNBEoMWwFlF1w3NYaJDFoSHeDxbAu1k" +
                "zavMgIawzQfLkeJ5QW8Ceo+ypMRtNkhSnGqVMU0fWF+/oof1aV6fvpsfHy6CgrAFAAA=";
                
        public override string ToString() => Extensions.ToString(this);
    }
}
