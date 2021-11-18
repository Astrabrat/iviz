using System.Runtime.Serialization;

namespace Iviz.Msgs.Rosapi
{
    [DataContract (Name = "rosapi/ServicesForType")]
    public sealed class ServicesForType : IService
    {
        /// <summary> Request message. </summary>
        [DataMember] public ServicesForTypeRequest Request { get; set; }
        
        /// <summary> Response message. </summary>
        [DataMember] public ServicesForTypeResponse Response { get; set; }
        
        /// <summary> Empty constructor. </summary>
        public ServicesForType()
        {
            Request = new ServicesForTypeRequest();
            Response = new ServicesForTypeResponse();
        }
        
        /// <summary> Setter constructor. </summary>
        public ServicesForType(ServicesForTypeRequest request)
        {
            Request = request;
            Response = new ServicesForTypeResponse();
        }
        
        IService IService.Create() => new ServicesForType();
        
        IRequest IService.Request
        {
            get => Request;
            set => Request = (ServicesForTypeRequest)value;
        }
        
        IResponse IService.Response
        {
            get => Response;
            set => Response = (ServicesForTypeResponse)value;
        }
        
        string IService.RosType => RosServiceType;
        
        /// <summary> Full ROS name of this service. </summary>
        [Preserve] public const string RosServiceType = "rosapi/ServicesForType";
        
        /// <summary> MD5 hash of a compact representation of the service. </summary>
        [Preserve] public const string RosMd5Sum = "93e9fe8ae5a9136008e260fe510bd2b0";
        
        public override string ToString() => Extensions.ToString(this);
    }

    [DataContract]
    public sealed class ServicesForTypeRequest : IRequest<ServicesForType, ServicesForTypeResponse>, IDeserializable<ServicesForTypeRequest>
    {
        [DataMember (Name = "type")] public string Type;
    
        /// <summary> Constructor for empty message. </summary>
        public ServicesForTypeRequest()
        {
            Type = string.Empty;
        }
        
        /// <summary> Explicit constructor. </summary>
        public ServicesForTypeRequest(string Type)
        {
            this.Type = Type;
        }
        
        /// <summary> Constructor with buffer. </summary>
        internal ServicesForTypeRequest(ref Buffer b)
        {
            Type = b.DeserializeString();
        }
        
        public ISerializable RosDeserialize(ref Buffer b)
        {
            return new ServicesForTypeRequest(ref b);
        }
        
        ServicesForTypeRequest IDeserializable<ServicesForTypeRequest>.RosDeserialize(ref Buffer b)
        {
            return new ServicesForTypeRequest(ref b);
        }
    
        public void RosSerialize(ref Buffer b)
        {
            b.Serialize(Type);
        }
        
        public void RosValidate()
        {
            if (Type is null) throw new System.NullReferenceException(nameof(Type));
        }
    
        public int RosMessageLength => 4 + BuiltIns.GetStringSize(Type);
    
        public override string ToString() => Extensions.ToString(this);
    }

    [DataContract]
    public sealed class ServicesForTypeResponse : IResponse, IDeserializable<ServicesForTypeResponse>
    {
        [DataMember (Name = "services")] public string[] Services;
    
        /// <summary> Constructor for empty message. </summary>
        public ServicesForTypeResponse()
        {
            Services = System.Array.Empty<string>();
        }
        
        /// <summary> Explicit constructor. </summary>
        public ServicesForTypeResponse(string[] Services)
        {
            this.Services = Services;
        }
        
        /// <summary> Constructor with buffer. </summary>
        internal ServicesForTypeResponse(ref Buffer b)
        {
            Services = b.DeserializeStringArray();
        }
        
        public ISerializable RosDeserialize(ref Buffer b)
        {
            return new ServicesForTypeResponse(ref b);
        }
        
        ServicesForTypeResponse IDeserializable<ServicesForTypeResponse>.RosDeserialize(ref Buffer b)
        {
            return new ServicesForTypeResponse(ref b);
        }
    
        public void RosSerialize(ref Buffer b)
        {
            b.SerializeArray(Services, 0);
        }
        
        public void RosValidate()
        {
            if (Services is null) throw new System.NullReferenceException(nameof(Services));
            for (int i = 0; i < Services.Length; i++)
            {
                if (Services[i] is null) throw new System.NullReferenceException($"{nameof(Services)}[{i}]");
            }
        }
    
        public int RosMessageLength => 4 + BuiltIns.GetArraySize(Services);
    
        public override string ToString() => Extensions.ToString(this);
    }
}
