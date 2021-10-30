using System.Runtime.Serialization;

namespace Iviz.Msgs.IvizMsgs
{
    [DataContract (Name = "iviz_msgs/SetFixedFrame")]
    public sealed class SetFixedFrame : IService
    {
        /// <summary> Request message. </summary>
        [DataMember] public SetFixedFrameRequest Request { get; set; }
        
        /// <summary> Response message. </summary>
        [DataMember] public SetFixedFrameResponse Response { get; set; }
        
        /// <summary> Empty constructor. </summary>
        public SetFixedFrame()
        {
            Request = new SetFixedFrameRequest();
            Response = new SetFixedFrameResponse();
        }
        
        /// <summary> Setter constructor. </summary>
        public SetFixedFrame(SetFixedFrameRequest request)
        {
            Request = request;
            Response = new SetFixedFrameResponse();
        }
        
        IService IService.Create() => new SetFixedFrame();
        
        IRequest IService.Request
        {
            get => Request;
            set => Request = (SetFixedFrameRequest)value;
        }
        
        IResponse IService.Response
        {
            get => Response;
            set => Response = (SetFixedFrameResponse)value;
        }
        
        public void Dispose()
        {
            Request.Dispose();
            Response.Dispose();
        }
        
        string IService.RosType => RosServiceType;
        
        /// <summary> Full ROS name of this service. </summary>
        [Preserve] public const string RosServiceType = "iviz_msgs/SetFixedFrame";
        
        /// <summary> MD5 hash of a compact representation of the service. </summary>
        [Preserve] public const string RosMd5Sum = "7b2e77c05fb1342786184d949a9f06ed";
        
        public override string ToString() => Extensions.ToString(this);
    }

    [DataContract]
    public sealed class SetFixedFrameRequest : IRequest<SetFixedFrame, SetFixedFrameResponse>, IDeserializable<SetFixedFrameRequest>
    {
        // Sets the fixed frame
        [DataMember (Name = "id")] public string Id; // Id of the frame
    
        /// <summary> Constructor for empty message. </summary>
        public SetFixedFrameRequest()
        {
            Id = string.Empty;
        }
        
        /// <summary> Explicit constructor. </summary>
        public SetFixedFrameRequest(string Id)
        {
            this.Id = Id;
        }
        
        /// <summary> Constructor with buffer. </summary>
        internal SetFixedFrameRequest(ref Buffer b)
        {
            Id = b.DeserializeString();
        }
        
        public ISerializable RosDeserialize(ref Buffer b)
        {
            return new SetFixedFrameRequest(ref b);
        }
        
        SetFixedFrameRequest IDeserializable<SetFixedFrameRequest>.RosDeserialize(ref Buffer b)
        {
            return new SetFixedFrameRequest(ref b);
        }
    
        public void RosSerialize(ref Buffer b)
        {
            b.Serialize(Id);
        }
        
        public void Dispose()
        {
        }
        
        public void RosValidate()
        {
            if (Id is null) throw new System.NullReferenceException(nameof(Id));
        }
    
        public int RosMessageLength => 4 + BuiltIns.GetStringSize(Id);
    
        public override string ToString() => Extensions.ToString(this);
    }

    [DataContract]
    public sealed class SetFixedFrameResponse : IResponse, IDeserializable<SetFixedFrameResponse>
    {
        [DataMember (Name = "success")] public bool Success; // Whether the operation succeeded
        [DataMember (Name = "message")] public string Message; // An error message if success is false
    
        /// <summary> Constructor for empty message. </summary>
        public SetFixedFrameResponse()
        {
            Message = string.Empty;
        }
        
        /// <summary> Explicit constructor. </summary>
        public SetFixedFrameResponse(bool Success, string Message)
        {
            this.Success = Success;
            this.Message = Message;
        }
        
        /// <summary> Constructor with buffer. </summary>
        internal SetFixedFrameResponse(ref Buffer b)
        {
            Success = b.Deserialize<bool>();
            Message = b.DeserializeString();
        }
        
        public ISerializable RosDeserialize(ref Buffer b)
        {
            return new SetFixedFrameResponse(ref b);
        }
        
        SetFixedFrameResponse IDeserializable<SetFixedFrameResponse>.RosDeserialize(ref Buffer b)
        {
            return new SetFixedFrameResponse(ref b);
        }
    
        public void RosSerialize(ref Buffer b)
        {
            b.Serialize(Success);
            b.Serialize(Message);
        }
        
        public void Dispose()
        {
        }
        
        public void RosValidate()
        {
            if (Message is null) throw new System.NullReferenceException(nameof(Message));
        }
    
        public int RosMessageLength => 5 + BuiltIns.GetStringSize(Message);
    
        public override string ToString() => Extensions.ToString(this);
    }
}
