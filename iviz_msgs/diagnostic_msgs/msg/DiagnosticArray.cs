/* This file was created automatically, do not edit! */

using System.Runtime.Serialization;

namespace Iviz.Msgs.DiagnosticMsgs
{
    [Preserve, DataContract (Name = "diagnostic_msgs/DiagnosticArray")]
    public sealed class DiagnosticArray : IDeserializable<DiagnosticArray>, IMessage
    {
        // This message is used to send diagnostic information about the state of the robot
        [DataMember (Name = "header")] public StdMsgs.Header Header { get; set; } //for timestamp
        [DataMember (Name = "status")] public DiagnosticStatus[] Status { get; set; } // an array of components being reported on
    
        /// <summary> Constructor for empty message. </summary>
        public DiagnosticArray()
        {
            Status = System.Array.Empty<DiagnosticStatus>();
        }
        
        /// <summary> Explicit constructor. </summary>
        public DiagnosticArray(in StdMsgs.Header Header, DiagnosticStatus[] Status)
        {
            this.Header = Header;
            this.Status = Status;
        }
        
        /// <summary> Constructor with buffer. </summary>
        public DiagnosticArray(ref Buffer b)
        {
            Header = new StdMsgs.Header(ref b);
            Status = b.DeserializeArray<DiagnosticStatus>();
            for (int i = 0; i < Status.Length; i++)
            {
                Status[i] = new DiagnosticStatus(ref b);
            }
        }
        
        public ISerializable RosDeserialize(ref Buffer b)
        {
            return new DiagnosticArray(ref b);
        }
        
        DiagnosticArray IDeserializable<DiagnosticArray>.RosDeserialize(ref Buffer b)
        {
            return new DiagnosticArray(ref b);
        }
    
        public void RosSerialize(ref Buffer b)
        {
            Header.RosSerialize(ref b);
            b.SerializeArray(Status, 0);
        }
        
        public void Dispose()
        {
        }
        
        public void RosValidate()
        {
            if (Status is null) throw new System.NullReferenceException(nameof(Status));
            for (int i = 0; i < Status.Length; i++)
            {
                if (Status[i] is null) throw new System.NullReferenceException($"{nameof(Status)}[{i}]");
                Status[i].RosValidate();
            }
        }
    
        public int RosMessageLength
        {
            get {
                int size = 4;
                size += Header.RosMessageLength;
                foreach (var i in Status)
                {
                    size += i.RosMessageLength;
                }
                return size;
            }
        }
    
        public string RosType => RosMessageType;
    
        /// <summary> Full ROS name of this message. </summary>
        [Preserve] public const string RosMessageType = "diagnostic_msgs/DiagnosticArray";
    
        /// <summary> MD5 hash of a compact representation of the message. </summary>
        [Preserve] public const string RosMd5Sum = "60810da900de1dd6ddd437c3503511da";
    
        /// <summary> Base64 of the GZip'd compression of the concatenated dependencies file. </summary>
        [Preserve] public const string RosDependenciesBase64 =
                "H4sIAAAAAAAAE71UwW7bMAy96ysI5NB2QNutvRXIoUC7rei2FmmwHYahoG3WFmpLniQn89/vSYqTrMCA" +
                "HbYFDiTZ5CP5+KgZLRvtqRPvuRbCdvBSUbDkxVRUaa6N9UGXpM2TdR0HbQ1xYYdAoRHygYOQfUoHZwsb" +
                "1HvhShw1eZnBi4JGgMBdr662gA/wHPzXbwli8DQjBrBzPEa40na9NWKCp0K0qclJb11Aatao+V/+qY8P" +
                "7y6QR/XY+dqf5gLUjJCiqdhVoCdwxYEpFtPouhF33MpKWkpVIav0NYy9+BM4Jk7x1GLEcduOW1ZRVzcY" +
                "XUbWtqxM/vDU4IB6diBoaNnB3rpKm2j+5LiTiI7Hy/dBTCl0c3UBG+OlHIJGQiMQSifsI2c3V6QGbcL5" +
                "WXRQs+XaHuMotey1BJ3jEJOVH72DDJAM+wvEeJWLOwE2yBFEqTwdpnePOPojQhCkgMaUDR0i8/sxNFBH" +
                "lMKKneaiTYoqwQBQD6LTwdEesknQhiGIDXxG3MX4E1izxY01HTfoWRur90MNAmHYO7vSFUyLMYGUrYaw" +
                "qNWFYzeq6JVDqtnbyDGM4JU6gpW9t6XmKL21Do3ywUX01I1HXal/pMbd5GVRvhycSWXT5Da2BX/TSGKe" +
                "MEQYKG0qjeIHbncj9cu4JkHhf2+915HZpOvkbnuIN867V8UIAd7dzl/n3ZfLxaf5m7y/XizuFvOzfHhY" +
                "Xn64np+rfMojMtus+4gkZujiPqqtsCuhiVcTG4C7gCrxpdN9st4kHKDY010Z+UqA0+Q7cfEb90zMZNxg" +
                "sNfsYg+Tw3QmjCeGi7KZupXxM7eD4KJaxfXlRbV5+UIl+/H+l0KmTKcCn2VErus43bh4Wi7Qg6TslDI+" +
                "iKGVlvUegflLZCPv4Bccl8+EDuUrQ6mfaRVQPzIGAAA=";
                
        public override string ToString() => Extensions.ToString(this);
    }
}
