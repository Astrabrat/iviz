/* This file was created automatically, do not edit! */

using System.Runtime.Serialization;

namespace Iviz.Msgs.MoveitMsgs
{
    [Preserve, DataContract (Name = "moveit_msgs/JointConstraint")]
    public sealed class JointConstraint : IDeserializable<JointConstraint>, IMessage
    {
        // Constrain the position of a joint to be within a certain bound
        [DataMember (Name = "joint_name")] public string JointName { get; set; }
        // the bound to be achieved is [position - tolerance_below, position + tolerance_above]
        [DataMember (Name = "position")] public double Position { get; set; }
        [DataMember (Name = "tolerance_above")] public double ToleranceAbove { get; set; }
        [DataMember (Name = "tolerance_below")] public double ToleranceBelow { get; set; }
        // A weighting factor for this constraint (denotes relative importance to other constraints. Closer to zero means less important)
        [DataMember (Name = "weight")] public double Weight { get; set; }
    
        /// <summary> Constructor for empty message. </summary>
        public JointConstraint()
        {
            JointName = string.Empty;
        }
        
        /// <summary> Explicit constructor. </summary>
        public JointConstraint(string JointName, double Position, double ToleranceAbove, double ToleranceBelow, double Weight)
        {
            this.JointName = JointName;
            this.Position = Position;
            this.ToleranceAbove = ToleranceAbove;
            this.ToleranceBelow = ToleranceBelow;
            this.Weight = Weight;
        }
        
        /// <summary> Constructor with buffer. </summary>
        public JointConstraint(ref Buffer b)
        {
            JointName = b.DeserializeString();
            Position = b.Deserialize<double>();
            ToleranceAbove = b.Deserialize<double>();
            ToleranceBelow = b.Deserialize<double>();
            Weight = b.Deserialize<double>();
        }
        
        public ISerializable RosDeserialize(ref Buffer b)
        {
            return new JointConstraint(ref b);
        }
        
        JointConstraint IDeserializable<JointConstraint>.RosDeserialize(ref Buffer b)
        {
            return new JointConstraint(ref b);
        }
    
        public void RosSerialize(ref Buffer b)
        {
            b.Serialize(JointName);
            b.Serialize(Position);
            b.Serialize(ToleranceAbove);
            b.Serialize(ToleranceBelow);
            b.Serialize(Weight);
        }
        
        public void Dispose()
        {
        }
        
        public void RosValidate()
        {
            if (JointName is null) throw new System.NullReferenceException(nameof(JointName));
        }
    
        public int RosMessageLength
        {
            get {
                int size = 36;
                size += BuiltIns.UTF8.GetByteCount(JointName);
                return size;
            }
        }
    
        public string RosType => RosMessageType;
    
        /// <summary> Full ROS name of this message. </summary>
        [Preserve] public const string RosMessageType = "moveit_msgs/JointConstraint";
    
        /// <summary> MD5 hash of a compact representation of the message. </summary>
        [Preserve] public const string RosMd5Sum = "c02a15146bec0ce13564807805b008f0";
    
        /// <summary> Base64 of the GZip'd compression of the concatenated dependencies file. </summary>
        [Preserve] public const string RosDependenciesBase64 =
                "H4sIAAAAAAAAE21Qy07DMBC8+ytG6qUI6AlxR/0MhKqNs2kWOd7KXhKJr2eTgqkQB1/mtTPe4ai5WiHJ" +
                "sJFx0SommqEDCO8q2WCKjrGIjS4iRC62yjv9yH1wr+TzVXnKNHEIuy1po7+9FEfhmXtIxWs78ehs4kI5" +
                "8qnjpMvD7/n7G446nfktDEnJnp+apgF/lP/gW/pa7AULy3m0tfJA0bRg8OfLKuLPRxj2PWc1riicyGRm" +
                "yHRRn+1h6yT1geXGUA84Jq2OOfnJRTEx5YrEtTar3bVm1xLhC72gSgx9AQAA";
                
        public override string ToString() => Extensions.ToString(this);
    }
}
