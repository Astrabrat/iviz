using System.Runtime.Serialization;

namespace Iviz.Msgs.IvizMsgs
{
    [DataContract (Name = "iviz_msgs/GetModules")]
    public sealed class GetModules : IService
    {
        /// <summary> Request message. </summary>
        [DataMember] public GetModulesRequest Request { get; set; }
        
        /// <summary> Response message. </summary>
        [DataMember] public GetModulesResponse Response { get; set; }
        
        /// <summary> Empty constructor. </summary>
        public GetModules()
        {
            Request = GetModulesRequest.Singleton;
            Response = new GetModulesResponse();
        }
        
        /// <summary> Setter constructor. </summary>
        public GetModules(GetModulesRequest request)
        {
            Request = request;
            Response = new GetModulesResponse();
        }
        
        IService IService.Create() => new GetModules();
        
        IRequest IService.Request
        {
            get => Request;
            set => Request = (GetModulesRequest)value;
        }
        
        IResponse IService.Response
        {
            get => Response;
            set => Response = (GetModulesResponse)value;
        }
        
        public void Dispose()
        {
            Request.Dispose();
            Response.Dispose();
        }
        
        string IService.RosType => RosServiceType;
        
        /// <summary> Full ROS name of this service. </summary>
        [Preserve] public const string RosServiceType = "iviz_msgs/GetModules";
        
        /// <summary> MD5 hash of a compact representation of the service. </summary>
        [Preserve] public const string RosMd5Sum = "854d12ba02315a7b73d8ac45d1a68e74";
        
        public override string ToString() => Extensions.ToString(this);
    }

    [DataContract]
    public sealed class GetModulesRequest : IRequest<GetModules, GetModulesResponse>, IDeserializable<GetModulesRequest>
    {
        // Gets a list of modules
    
        /// <summary> Constructor for empty message. </summary>
        public GetModulesRequest()
        {
        }
        
        /// <summary> Constructor with buffer. </summary>
        internal GetModulesRequest(ref Buffer b)
        {
        }
        
        public ISerializable RosDeserialize(ref Buffer b)
        {
            return Singleton;
        }
        
        GetModulesRequest IDeserializable<GetModulesRequest>.RosDeserialize(ref Buffer b)
        {
            return Singleton;
        }
        
        public static readonly GetModulesRequest Singleton = new GetModulesRequest();
    
        public void RosSerialize(ref Buffer b)
        {
        }
        
        public void Dispose()
        {
        }
        
        public void RosValidate()
        {
        }
    
        /// <summary> Constant size of this message. </summary>
        [Preserve] public const int RosFixedMessageLength = 0;
        
        public int RosMessageLength => RosFixedMessageLength;
    
        public override string ToString() => Extensions.ToString(this);
    }

    [DataContract]
    public sealed class GetModulesResponse : IResponse, IDeserializable<GetModulesResponse>
    {
        [DataMember (Name = "configs")] public string[] Configs; // List of module configurations in JSON encoding
    
        /// <summary> Constructor for empty message. </summary>
        public GetModulesResponse()
        {
            Configs = System.Array.Empty<string>();
        }
        
        /// <summary> Explicit constructor. </summary>
        public GetModulesResponse(string[] Configs)
        {
            this.Configs = Configs;
        }
        
        /// <summary> Constructor with buffer. </summary>
        internal GetModulesResponse(ref Buffer b)
        {
            Configs = b.DeserializeStringArray();
        }
        
        public ISerializable RosDeserialize(ref Buffer b)
        {
            return new GetModulesResponse(ref b);
        }
        
        GetModulesResponse IDeserializable<GetModulesResponse>.RosDeserialize(ref Buffer b)
        {
            return new GetModulesResponse(ref b);
        }
    
        public void RosSerialize(ref Buffer b)
        {
            b.SerializeArray(Configs, 0);
        }
        
        public void Dispose()
        {
        }
        
        public void RosValidate()
        {
            if (Configs is null) throw new System.NullReferenceException(nameof(Configs));
            for (int i = 0; i < Configs.Length; i++)
            {
                if (Configs[i] is null) throw new System.NullReferenceException($"{nameof(Configs)}[{i}]");
            }
        }
    
        public int RosMessageLength => 4 + BuiltIns.GetArraySize(Configs);
    
        public override string ToString() => Extensions.ToString(this);
    }
}
