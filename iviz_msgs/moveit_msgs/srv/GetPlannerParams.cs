using System.Runtime.Serialization;

namespace Iviz.Msgs.MoveitMsgs
{
    [DataContract (Name = "moveit_msgs/GetPlannerParams")]
    public sealed class GetPlannerParams : IService
    {
        /// <summary> Request message. </summary>
        [DataMember] public GetPlannerParamsRequest Request { get; set; }
        
        /// <summary> Response message. </summary>
        [DataMember] public GetPlannerParamsResponse Response { get; set; }
        
        /// <summary> Empty constructor. </summary>
        public GetPlannerParams()
        {
            Request = new GetPlannerParamsRequest();
            Response = new GetPlannerParamsResponse();
        }
        
        /// <summary> Setter constructor. </summary>
        public GetPlannerParams(GetPlannerParamsRequest request)
        {
            Request = request;
            Response = new GetPlannerParamsResponse();
        }
        
        IService IService.Create() => new GetPlannerParams();
        
        IRequest IService.Request
        {
            get => Request;
            set => Request = (GetPlannerParamsRequest)value;
        }
        
        IResponse IService.Response
        {
            get => Response;
            set => Response = (GetPlannerParamsResponse)value;
        }
        
        string IService.RosType => RosServiceType;
        
        /// <summary> Full ROS name of this service. </summary>
        [Preserve] public const string RosServiceType = "moveit_msgs/GetPlannerParams";
        
        /// <summary> MD5 hash of a compact representation of the service. </summary>
        [Preserve] public const string RosMd5Sum = "b3ec1aca2b1471e3eea051c548c69810";
        
        public override string ToString() => Extensions.ToString(this);
    }

    [DataContract]
    public sealed class GetPlannerParamsRequest : IRequest<GetPlannerParams, GetPlannerParamsResponse>, IDeserializable<GetPlannerParamsRequest>
    {
        // Name of planning config
        [DataMember (Name = "planner_config")] public string PlannerConfig;
        // Optional name of planning group (return global defaults if empty)
        [DataMember (Name = "group")] public string Group;
    
        /// <summary> Constructor for empty message. </summary>
        public GetPlannerParamsRequest()
        {
            PlannerConfig = string.Empty;
            Group = string.Empty;
        }
        
        /// <summary> Explicit constructor. </summary>
        public GetPlannerParamsRequest(string PlannerConfig, string Group)
        {
            this.PlannerConfig = PlannerConfig;
            this.Group = Group;
        }
        
        /// <summary> Constructor with buffer. </summary>
        internal GetPlannerParamsRequest(ref Buffer b)
        {
            PlannerConfig = b.DeserializeString();
            Group = b.DeserializeString();
        }
        
        public ISerializable RosDeserialize(ref Buffer b)
        {
            return new GetPlannerParamsRequest(ref b);
        }
        
        GetPlannerParamsRequest IDeserializable<GetPlannerParamsRequest>.RosDeserialize(ref Buffer b)
        {
            return new GetPlannerParamsRequest(ref b);
        }
    
        public void RosSerialize(ref Buffer b)
        {
            b.Serialize(PlannerConfig);
            b.Serialize(Group);
        }
        
        public void RosValidate()
        {
            if (PlannerConfig is null) throw new System.NullReferenceException(nameof(PlannerConfig));
            if (Group is null) throw new System.NullReferenceException(nameof(Group));
        }
    
        public int RosMessageLength => 8 + BuiltIns.GetStringSize(PlannerConfig) + BuiltIns.GetStringSize(Group);
    
        public override string ToString() => Extensions.ToString(this);
    }

    [DataContract]
    public sealed class GetPlannerParamsResponse : IResponse, IDeserializable<GetPlannerParamsResponse>
    {
        // parameters as key-value pairs
        [DataMember (Name = "params")] public PlannerParams Params;
    
        /// <summary> Constructor for empty message. </summary>
        public GetPlannerParamsResponse()
        {
            Params = new PlannerParams();
        }
        
        /// <summary> Explicit constructor. </summary>
        public GetPlannerParamsResponse(PlannerParams Params)
        {
            this.Params = Params;
        }
        
        /// <summary> Constructor with buffer. </summary>
        internal GetPlannerParamsResponse(ref Buffer b)
        {
            Params = new PlannerParams(ref b);
        }
        
        public ISerializable RosDeserialize(ref Buffer b)
        {
            return new GetPlannerParamsResponse(ref b);
        }
        
        GetPlannerParamsResponse IDeserializable<GetPlannerParamsResponse>.RosDeserialize(ref Buffer b)
        {
            return new GetPlannerParamsResponse(ref b);
        }
    
        public void RosSerialize(ref Buffer b)
        {
            Params.RosSerialize(ref b);
        }
        
        public void RosValidate()
        {
            if (Params is null) throw new System.NullReferenceException(nameof(Params));
            Params.RosValidate();
        }
    
        public int RosMessageLength => 0 + Params.RosMessageLength;
    
        public override string ToString() => Extensions.ToString(this);
    }
}
