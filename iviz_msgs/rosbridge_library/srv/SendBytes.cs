using System.Runtime.Serialization;

namespace Iviz.Msgs.RosbridgeLibrary
{
    [DataContract (Name = "rosbridge_library/SendBytes")]
    public sealed class SendBytes : IService
    {
        /// <summary> Request message. </summary>
        [DataMember] public SendBytesRequest Request { get; set; }
        
        /// <summary> Response message. </summary>
        [DataMember] public SendBytesResponse Response { get; set; }
        
        /// <summary> Empty constructor. </summary>
        public SendBytes()
        {
            Request = new SendBytesRequest();
            Response = new SendBytesResponse();
        }
        
        /// <summary> Setter constructor. </summary>
        public SendBytes(SendBytesRequest request)
        {
            Request = request;
            Response = new SendBytesResponse();
        }
        
        IService IService.Create() => new SendBytes();
        
        IRequest IService.Request
        {
            get => Request;
            set => Request = (SendBytesRequest)value;
        }
        
        IResponse IService.Response
        {
            get => Response;
            set => Response = (SendBytesResponse)value;
        }
        
        string IService.RosType => RosServiceType;
        
        /// <summary> Full ROS name of this service. </summary>
        [Preserve] public const string RosServiceType = "rosbridge_library/SendBytes";
        
        /// <summary> MD5 hash of a compact representation of the service. </summary>
        [Preserve] public const string RosMd5Sum = "d875457256decc7436099d9d612ebf8a";
        
        public override string ToString() => Extensions.ToString(this);
    }

    [DataContract]
    public sealed class SendBytesRequest : IRequest<SendBytes, SendBytesResponse>, IDeserializable<SendBytesRequest>
    {
        [DataMember (Name = "count")] public long Count;
    
        /// <summary> Constructor for empty message. </summary>
        public SendBytesRequest()
        {
        }
        
        /// <summary> Explicit constructor. </summary>
        public SendBytesRequest(long Count)
        {
            this.Count = Count;
        }
        
        /// <summary> Constructor with buffer. </summary>
        internal SendBytesRequest(ref Buffer b)
        {
            Count = b.Deserialize<long>();
        }
        
        public ISerializable RosDeserialize(ref Buffer b)
        {
            return new SendBytesRequest(ref b);
        }
        
        SendBytesRequest IDeserializable<SendBytesRequest>.RosDeserialize(ref Buffer b)
        {
            return new SendBytesRequest(ref b);
        }
    
        public void RosSerialize(ref Buffer b)
        {
            b.Serialize(Count);
        }
        
        public void RosValidate()
        {
        }
    
        /// <summary> Constant size of this message. </summary>
        [Preserve] public const int RosFixedMessageLength = 8;
        
        public int RosMessageLength => RosFixedMessageLength;
    
        public override string ToString() => Extensions.ToString(this);
    }

    [DataContract]
    public sealed class SendBytesResponse : IResponse, IDeserializable<SendBytesResponse>
    {
        [DataMember (Name = "data")] public string Data;
    
        /// <summary> Constructor for empty message. </summary>
        public SendBytesResponse()
        {
            Data = string.Empty;
        }
        
        /// <summary> Explicit constructor. </summary>
        public SendBytesResponse(string Data)
        {
            this.Data = Data;
        }
        
        /// <summary> Constructor with buffer. </summary>
        internal SendBytesResponse(ref Buffer b)
        {
            Data = b.DeserializeString();
        }
        
        public ISerializable RosDeserialize(ref Buffer b)
        {
            return new SendBytesResponse(ref b);
        }
        
        SendBytesResponse IDeserializable<SendBytesResponse>.RosDeserialize(ref Buffer b)
        {
            return new SendBytesResponse(ref b);
        }
    
        public void RosSerialize(ref Buffer b)
        {
            b.Serialize(Data);
        }
        
        public void RosValidate()
        {
            if (Data is null) throw new System.NullReferenceException(nameof(Data));
        }
    
        public int RosMessageLength => 4 + BuiltIns.GetStringSize(Data);
    
        public override string ToString() => Extensions.ToString(this);
    }
}
