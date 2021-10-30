using System.Runtime.Serialization;

namespace Iviz.Msgs.StdSrvs
{
    [DataContract (Name = "std_srvs/Trigger")]
    public sealed class Trigger : IService
    {
        /// <summary> Request message. </summary>
        [DataMember] public TriggerRequest Request { get; set; }
        
        /// <summary> Response message. </summary>
        [DataMember] public TriggerResponse Response { get; set; }
        
        /// <summary> Empty constructor. </summary>
        public Trigger()
        {
            Request = TriggerRequest.Singleton;
            Response = new TriggerResponse();
        }
        
        /// <summary> Setter constructor. </summary>
        public Trigger(TriggerRequest request)
        {
            Request = request;
            Response = new TriggerResponse();
        }
        
        IService IService.Create() => new Trigger();
        
        IRequest IService.Request
        {
            get => Request;
            set => Request = (TriggerRequest)value;
        }
        
        IResponse IService.Response
        {
            get => Response;
            set => Response = (TriggerResponse)value;
        }
        
        public void Dispose()
        {
            Request.Dispose();
            Response.Dispose();
        }
        
        string IService.RosType => RosServiceType;
        
        /// <summary> Full ROS name of this service. </summary>
        [Preserve] public const string RosServiceType = "std_srvs/Trigger";
        
        /// <summary> MD5 hash of a compact representation of the service. </summary>
        [Preserve] public const string RosMd5Sum = "937c9679a518e3a18d831e57125ea522";
        
        public override string ToString() => Extensions.ToString(this);
    }

    [DataContract]
    public sealed class TriggerRequest : IRequest<Trigger, TriggerResponse>, IDeserializable<TriggerRequest>
    {
    
        /// <summary> Constructor for empty message. </summary>
        public TriggerRequest()
        {
        }
        
        /// <summary> Constructor with buffer. </summary>
        internal TriggerRequest(ref Buffer b)
        {
        }
        
        public ISerializable RosDeserialize(ref Buffer b)
        {
            return Singleton;
        }
        
        TriggerRequest IDeserializable<TriggerRequest>.RosDeserialize(ref Buffer b)
        {
            return Singleton;
        }
        
        public static readonly TriggerRequest Singleton = new TriggerRequest();
    
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
    public sealed class TriggerResponse : IResponse, IDeserializable<TriggerResponse>
    {
        [DataMember (Name = "success")] public bool Success; // indicate successful run of triggered service
        [DataMember (Name = "message")] public string Message; // informational, e.g. for error messages
    
        /// <summary> Constructor for empty message. </summary>
        public TriggerResponse()
        {
            Message = string.Empty;
        }
        
        /// <summary> Explicit constructor. </summary>
        public TriggerResponse(bool Success, string Message)
        {
            this.Success = Success;
            this.Message = Message;
        }
        
        /// <summary> Constructor with buffer. </summary>
        internal TriggerResponse(ref Buffer b)
        {
            Success = b.Deserialize<bool>();
            Message = b.DeserializeString();
        }
        
        public ISerializable RosDeserialize(ref Buffer b)
        {
            return new TriggerResponse(ref b);
        }
        
        TriggerResponse IDeserializable<TriggerResponse>.RosDeserialize(ref Buffer b)
        {
            return new TriggerResponse(ref b);
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
