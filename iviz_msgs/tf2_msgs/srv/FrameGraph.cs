using System.Runtime.Serialization;

namespace Iviz.Msgs.Tf2Msgs
{
    [DataContract]
    public sealed class FrameGraph : IService
    {
        /// Request message.
        [DataMember] public FrameGraphRequest Request { get; set; }
        
        /// Response message.
        [DataMember] public FrameGraphResponse Response { get; set; }
        
        /// Empty constructor.
        public FrameGraph()
        {
            Request = FrameGraphRequest.Singleton;
            Response = new FrameGraphResponse();
        }
        
        /// Setter constructor.
        public FrameGraph(FrameGraphRequest request)
        {
            Request = request;
            Response = new FrameGraphResponse();
        }
        
        IRequest IService.Request
        {
            get => Request;
            set => Request = (FrameGraphRequest)value;
        }
        
        IResponse IService.Response
        {
            get => Response;
            set => Response = (FrameGraphResponse)value;
        }
        
        public const string ServiceType = "tf2_msgs/FrameGraph";
        public string RosServiceType => ServiceType;
        
        public string RosMd5Sum => "437ea58e9463815a0d511c7326b686b0";
        
        public override string ToString() => Extensions.ToString(this);
    }

    [DataContract]
    public sealed class FrameGraphRequest : IRequest<FrameGraph, FrameGraphResponse>, IDeserializable<FrameGraphRequest>
    {
    
        /// Constructor for empty message.
        public FrameGraphRequest()
        {
        }
        
        /// Constructor with buffer.
        public FrameGraphRequest(ref ReadBuffer b)
        {
        }
        
        ISerializable ISerializable.RosDeserializeBase(ref ReadBuffer b) => Singleton;
        
        public FrameGraphRequest RosDeserialize(ref ReadBuffer b) => Singleton;
        
        static FrameGraphRequest? singleton;
        public static FrameGraphRequest Singleton => singleton ??= new FrameGraphRequest();
    
        public void RosSerialize(ref WriteBuffer b)
        {
        }
        
        public void RosValidate()
        {
        }
    
        /// <summary> Constant size of this message. </summary> 
        public const int RosFixedMessageLength = 0;
        
        public int RosMessageLength => RosFixedMessageLength;
    
        public override string ToString() => Extensions.ToString(this);
    }

    [DataContract]
    public sealed class FrameGraphResponse : IResponse, IDeserializable<FrameGraphResponse>
    {
        [DataMember (Name = "frame_yaml")] public string FrameYaml;
    
        /// Constructor for empty message.
        public FrameGraphResponse()
        {
            FrameYaml = "";
        }
        
        /// Explicit constructor.
        public FrameGraphResponse(string FrameYaml)
        {
            this.FrameYaml = FrameYaml;
        }
        
        /// Constructor with buffer.
        public FrameGraphResponse(ref ReadBuffer b)
        {
            b.DeserializeString(out FrameYaml);
        }
        
        ISerializable ISerializable.RosDeserializeBase(ref ReadBuffer b) => new FrameGraphResponse(ref b);
        
        public FrameGraphResponse RosDeserialize(ref ReadBuffer b) => new FrameGraphResponse(ref b);
    
        public void RosSerialize(ref WriteBuffer b)
        {
            b.Serialize(FrameYaml);
        }
        
        public void RosValidate()
        {
            if (FrameYaml is null) BuiltIns.ThrowNullReference();
        }
    
        public int RosMessageLength => 4 + BuiltIns.GetStringSize(FrameYaml);
    
        public override string ToString() => Extensions.ToString(this);
    }
}
