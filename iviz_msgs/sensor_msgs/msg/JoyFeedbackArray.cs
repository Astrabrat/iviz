/* This file was created automatically, do not edit! */

using System.Runtime.Serialization;

namespace Iviz.Msgs.SensorMsgs
{
    [Preserve, DataContract (Name = "sensor_msgs/JoyFeedbackArray")]
    public sealed class JoyFeedbackArray : IDeserializable<JoyFeedbackArray>, IMessage
    {
        // This message publishes values for multiple feedback at once. 
        [DataMember (Name = "array")] public JoyFeedback[] Array;
    
        /// <summary> Constructor for empty message. </summary>
        public JoyFeedbackArray()
        {
            Array = System.Array.Empty<JoyFeedback>();
        }
        
        /// <summary> Explicit constructor. </summary>
        public JoyFeedbackArray(JoyFeedback[] Array)
        {
            this.Array = Array;
        }
        
        /// <summary> Constructor with buffer. </summary>
        internal JoyFeedbackArray(ref Buffer b)
        {
            Array = b.DeserializeArray<JoyFeedback>();
            for (int i = 0; i < Array.Length; i++)
            {
                Array[i] = new JoyFeedback(ref b);
            }
        }
        
        public ISerializable RosDeserialize(ref Buffer b)
        {
            return new JoyFeedbackArray(ref b);
        }
        
        JoyFeedbackArray IDeserializable<JoyFeedbackArray>.RosDeserialize(ref Buffer b)
        {
            return new JoyFeedbackArray(ref b);
        }
    
        public void RosSerialize(ref Buffer b)
        {
            b.SerializeArray(Array, 0);
        }
        
        public void RosValidate()
        {
            if (Array is null) throw new System.NullReferenceException(nameof(Array));
            for (int i = 0; i < Array.Length; i++)
            {
                if (Array[i] is null) throw new System.NullReferenceException($"{nameof(Array)}[{i}]");
                Array[i].RosValidate();
            }
        }
    
        public int RosMessageLength => 4 + 6 * Array.Length;
    
        public string RosType => RosMessageType;
    
        /// <summary> Full ROS name of this message. </summary>
        [Preserve] public const string RosMessageType = "sensor_msgs/JoyFeedbackArray";
    
        /// <summary> MD5 hash of a compact representation of the message. </summary>
        [Preserve] public const string RosMd5Sum = "cde5730a895b1fc4dee6f91b754b213d";
    
        /// <summary> Base64 of the GZip'd compression of the concatenated dependencies file. </summary>
        [Preserve] public const string RosDependenciesBase64 =
                "H4sIAAAAAAAACq1R0UrDMBR9D+wfDux11G4yELEvsiqTDWTqgxMZaXu7BtNkJOm2/r13dZX57n1Jcm5y" +
                "zsm5Q7xWyqMm7+WWsGsyrXxFHnupG15K61A3OqidJpRERSbzL8gAa3KKIJ5s+3BGPz4hnZPtQCT/XAOx" +
                "fHm8hSfjrdvUfuuvLnQHYogZ5Vo6gi0RKkJod92+NywaZcINXt+f080inYErQXyJrt6W94uU0fElev+2" +
                "XqcrRifiDJ+YhTindlBao7K6gDRQBUxTZ+S6zEjm1a8NOh16LxG/To+y5kBHndlSOR+gqcDBNsyVEXMl" +
                "8U/TU27N307vUBUnI3MTOBYV2v7vvc4IpbM14ihGsBhHTKhMrhuv9jw4zEsUtFc5U3qmkXlopNYtMmWk" +
                "a0coHN9z8FWnHBzxzOO75HgXR1NIz2rliMmnJygZd4iJRKmtDNcTVjq7EmIgvgE0QAWKZAIAAA==";
                
        public override string ToString() => Extensions.ToString(this);
    }
}
