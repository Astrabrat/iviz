using System.Runtime.Serialization;

namespace Iviz.Msgs.Rosapi
{
    [DataContract (Name = "rosapi/ServiceNode")]
    public sealed class ServiceNode : IService
    {
        /// <summary> Request message. </summary>
        [DataMember] public ServiceNodeRequest Request { get; set; }
        
        /// <summary> Response message. </summary>
        [DataMember] public ServiceNodeResponse Response { get; set; }
        
        /// <summary> Empty constructor. </summary>
        public ServiceNode()
        {
            Request = new ServiceNodeRequest();
            Response = new ServiceNodeResponse();
        }
        
        /// <summary> Setter constructor. </summary>
        public ServiceNode(ServiceNodeRequest request)
        {
            Request = request;
            Response = new ServiceNodeResponse();
        }
        
        IService IService.Create() => new ServiceNode();
        
        IRequest IService.Request
        {
            get => Request;
            set => Request = (ServiceNodeRequest)value;
        }
        
        IResponse IService.Response
        {
            get => Response;
            set => Response = (ServiceNodeResponse)value;
        }
        
        string IService.RosType => RosServiceType;
        
        /// <summary> Full ROS name of this service. </summary>
        [Preserve] public const string RosServiceType = "rosapi/ServiceNode";
        
        /// <summary> MD5 hash of a compact representation of the service. </summary>
        [Preserve] public const string RosMd5Sum = "bd2a0a45fd7a73a86c8d6051d5a6db8a";
        
        public override string ToString() => Extensions.ToString(this);
    }

    [DataContract]
    public sealed class ServiceNodeRequest : IRequest<ServiceNode, ServiceNodeResponse>, IDeserializable<ServiceNodeRequest>
    {
        [DataMember (Name = "service")] public string Service;
    
        /// <summary> Constructor for empty message. </summary>
        public ServiceNodeRequest()
        {
            Service = string.Empty;
        }
        
        /// <summary> Explicit constructor. </summary>
        public ServiceNodeRequest(string Service)
        {
            this.Service = Service;
        }
        
        /// <summary> Constructor with buffer. </summary>
        internal ServiceNodeRequest(ref Buffer b)
        {
            Service = b.DeserializeString();
        }
        
        public ISerializable RosDeserialize(ref Buffer b)
        {
            return new ServiceNodeRequest(ref b);
        }
        
        ServiceNodeRequest IDeserializable<ServiceNodeRequest>.RosDeserialize(ref Buffer b)
        {
            return new ServiceNodeRequest(ref b);
        }
    
        public void RosSerialize(ref Buffer b)
        {
            b.Serialize(Service);
        }
        
        public void RosValidate()
        {
            if (Service is null) throw new System.NullReferenceException(nameof(Service));
        }
    
        public int RosMessageLength => 4 + BuiltIns.GetStringSize(Service);
    
        public override string ToString() => Extensions.ToString(this);
    }

    [DataContract]
    public sealed class ServiceNodeResponse : IResponse, IDeserializable<ServiceNodeResponse>
    {
        [DataMember (Name = "node")] public string Node;
    
        /// <summary> Constructor for empty message. </summary>
        public ServiceNodeResponse()
        {
            Node = string.Empty;
        }
        
        /// <summary> Explicit constructor. </summary>
        public ServiceNodeResponse(string Node)
        {
            this.Node = Node;
        }
        
        /// <summary> Constructor with buffer. </summary>
        internal ServiceNodeResponse(ref Buffer b)
        {
            Node = b.DeserializeString();
        }
        
        public ISerializable RosDeserialize(ref Buffer b)
        {
            return new ServiceNodeResponse(ref b);
        }
        
        ServiceNodeResponse IDeserializable<ServiceNodeResponse>.RosDeserialize(ref Buffer b)
        {
            return new ServiceNodeResponse(ref b);
        }
    
        public void RosSerialize(ref Buffer b)
        {
            b.Serialize(Node);
        }
        
        public void RosValidate()
        {
            if (Node is null) throw new System.NullReferenceException(nameof(Node));
        }
    
        public int RosMessageLength => 4 + BuiltIns.GetStringSize(Node);
    
        public override string ToString() => Extensions.ToString(this);
    }
}
