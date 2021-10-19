/* This file was created automatically, do not edit! */

using System.Runtime.Serialization;

namespace Iviz.Msgs.Actionlib
{
    [Preserve, DataContract (Name = "actionlib/TestActionFeedback")]
    public sealed class TestActionFeedback : IDeserializable<TestActionFeedback>, IActionFeedback<TestFeedback>
    {
        [DataMember (Name = "header")] public StdMsgs.Header Header { get; set; }
        [DataMember (Name = "status")] public ActionlibMsgs.GoalStatus Status { get; set; }
        [DataMember (Name = "feedback")] public TestFeedback Feedback { get; set; }
    
        /// <summary> Constructor for empty message. </summary>
        public TestActionFeedback()
        {
            Status = new ActionlibMsgs.GoalStatus();
            Feedback = new TestFeedback();
        }
        
        /// <summary> Explicit constructor. </summary>
        public TestActionFeedback(in StdMsgs.Header Header, ActionlibMsgs.GoalStatus Status, TestFeedback Feedback)
        {
            this.Header = Header;
            this.Status = Status;
            this.Feedback = Feedback;
        }
        
        /// <summary> Constructor with buffer. </summary>
        public TestActionFeedback(ref Buffer b)
        {
            Header = new StdMsgs.Header(ref b);
            Status = new ActionlibMsgs.GoalStatus(ref b);
            Feedback = new TestFeedback(ref b);
        }
        
        public ISerializable RosDeserialize(ref Buffer b)
        {
            return new TestActionFeedback(ref b);
        }
        
        TestActionFeedback IDeserializable<TestActionFeedback>.RosDeserialize(ref Buffer b)
        {
            return new TestActionFeedback(ref b);
        }
    
        public void RosSerialize(ref Buffer b)
        {
            Header.RosSerialize(ref b);
            Status.RosSerialize(ref b);
            Feedback.RosSerialize(ref b);
        }
        
        public void Dispose()
        {
        }
        
        public void RosValidate()
        {
            if (Status is null) throw new System.NullReferenceException(nameof(Status));
            Status.RosValidate();
            if (Feedback is null) throw new System.NullReferenceException(nameof(Feedback));
            Feedback.RosValidate();
        }
    
        public int RosMessageLength
        {
            get {
                int size = 4;
                size += Header.RosMessageLength;
                size += Status.RosMessageLength;
                return size;
            }
        }
    
        public string RosType => RosMessageType;
    
        /// <summary> Full ROS name of this message. </summary>
        [Preserve] public const string RosMessageType = "actionlib/TestActionFeedback";
    
        /// <summary> MD5 hash of a compact representation of the message. </summary>
        [Preserve] public const string RosMd5Sum = "6d3d0bf7fb3dda24779c010a9f3eb7cb";
    
        /// <summary> Base64 of the GZip'd compression of the concatenated dependencies file. </summary>
        [Preserve] public const string RosDependenciesBase64 =
                "H4sIAAAAAAAAE71W23LbNhB951dgxg+xO7XSJr2kntGDaiuOOk7isdW+ekBiRaIFQRUXyfr7ngUpinKs" +
                "Rg9JNLJ1A84enD272HckFTlRpZdMFkE31uj8ofalf3ndSHMfZIhe+PSSzcmHt0Qql8U/YtG9ycZf+JG9" +
                "v7++QEjV0njXkjsR4GKVdErUFKSSQYpFA+66rMidG1qRYZ71kpRIv4bNkvwIG+eV9gLPkiw5acxGRI9F" +
                "oRFFU9fR6kIGEkHXtLcfO7UVUiylC7qIRjqsb5zSlpcvnKyJ0fH09G8kW5CYXV1gjfVUxKBBaAOEwpH0" +
                "2pb4UWRR2/D6FW/ITubr5hwfqUQG+uAiVDIwWXpcOvLMU/oLxPiuPdwI2BCHEEV5cZq+e8BHfyYQBBRo" +
                "2RSVOAXz202oGgtAEivptMwNMXABBYD6gje9OBsg2wRtpW228C3iLsYxsLbH5TOdV8iZ4dP7WEJALFy6" +
                "ZqUVluabBFIYTTYI2M5Jt8l4VxsyO3nLGmMRdqWM4FV63xQaCVBirUOV+eAYPWXjQavsK7nxYGlk/BaZ" +
                "LfHC8TnBb7b10n64nX64mn24FtvHWPyA/2xLSttEJb3YUGBD5sT6FG3iO4Ha2Mi5W6EOWszJ5Xz211QM" +
                "MH/cx+SMROegLEyYE2t0FPDt3XT6/nY+veqBX+0DOyoI1oYtkXLYg7+B+30QchHgZB349I4TRI+pDmyZ" +
                "if95nOAPJkkqtIZDVS4NMYIOfosCoqdzcjWqz3ArCHTWUb7/8/JyOr0aUH69T3kNZFlUmpi2jwWrsIjc" +
                "B54T4lCYye8f73a6cJifngmTN+noKiZb7rg/G0lF+qw07ArfoAwWUpvo6BC9u+kf08sBv7H4+VN6jv6m" +
                "IhxwQCqoJoandvn+8xxzKiR6asLsg0X0ySDBlDsEOrW2K2m0OnSAznl9pYzFL9/Aeb31bBNSEe7M1yev" +
                "V/hycnOzq+Sx+PVYgjnhqqJnGR6jLnLyabb2SduFdjVfanx9hGEXSExI7R1iaJM3X+AQx8nMptgrvzYA" +
                "XxsHPHHz8X4+hBqL3xLgxG7F6G4PIAmFrDEItSLIXgJGGbVTgIfBjUq65UfUnmfshtVmSdcax0flSPuk" +
                "dWYnE2OadZpHeCFKwXHd9pcVyHQXFdeYGExWvEVRHsuSZewWBXoM2Te8ymZXWeuAdgTpRPKB083nSXcy" +
                "JF1XGrNFuo8HLSW5gxTPQrM0usTujnmqE/aTZf/glORZIIw4VC+RK2OwmzF9m7w1IXQPvbUeLEmOW0pi" +
                "NBwVOv7oLt14gVYMepv9LGxHVnYjdmC+iiZgnPReltSmxi+p0AtdbIshMfCjDp1nvXYBSNUxFQX6nMaq" +
                "0TZ5PIR87dS9HE7iWTtT9vN49h81MDSS1gsAAA==";
                
        public override string ToString() => Extensions.ToString(this);
    }
}
