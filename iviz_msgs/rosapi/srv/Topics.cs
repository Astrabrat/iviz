using System.Runtime.Serialization;

namespace Iviz.Msgs.Rosapi
{
    [DataContract (Name = "rosapi/Topics")]
    public sealed class Topics : IService
    {
        /// <summary> Request message. </summary>
        [DataMember] public TopicsRequest Request { get; set; }
        
        /// <summary> Response message. </summary>
        [DataMember] public TopicsResponse Response { get; set; }
        
        /// <summary> Empty constructor. </summary>
        public Topics()
        {
            Request = TopicsRequest.Singleton;
            Response = new TopicsResponse();
        }
        
        /// <summary> Setter constructor. </summary>
        public Topics(TopicsRequest request)
        {
            Request = request;
            Response = new TopicsResponse();
        }
        
        IService IService.Create() => new Topics();
        
        IRequest IService.Request
        {
            get => Request;
            set => Request = (TopicsRequest)value;
        }
        
        IResponse IService.Response
        {
            get => Response;
            set => Response = (TopicsResponse)value;
        }
        
        public void Dispose()
        {
            Request.Dispose();
            Response.Dispose();
        }
        
        string IService.RosType => RosServiceType;
        
        /// <summary> Full ROS name of this service. </summary>
        [Preserve] public const string RosServiceType = "rosapi/Topics";
        
        /// <summary> MD5 hash of a compact representation of the service. </summary>
        [Preserve] public const string RosMd5Sum = "d966d98fc333fa1f3135af765eac1ba8";
        
        public override string ToString() => Extensions.ToString(this);
    }

    [DataContract]
    public sealed class TopicsRequest : IRequest<Topics, TopicsResponse>, IDeserializable<TopicsRequest>
    {
    
        /// <summary> Constructor for empty message. </summary>
        public TopicsRequest()
        {
        }
        
        /// <summary> Constructor with buffer. </summary>
        public TopicsRequest(ref Buffer b)
        {
        }
        
        public ISerializable RosDeserialize(ref Buffer b)
        {
            return Singleton;
        }
        
        TopicsRequest IDeserializable<TopicsRequest>.RosDeserialize(ref Buffer b)
        {
            return Singleton;
        }
        
        public static readonly TopicsRequest Singleton = new TopicsRequest();
    
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
    public sealed class TopicsResponse : IResponse, IDeserializable<TopicsResponse>
    {
        [DataMember (Name = "topics")] public string[] Topics_;
        [DataMember (Name = "types")] public string[] Types;
    
        /// <summary> Constructor for empty message. </summary>
        public TopicsResponse()
        {
            Topics_ = System.Array.Empty<string>();
            Types = System.Array.Empty<string>();
        }
        
        /// <summary> Explicit constructor. </summary>
        public TopicsResponse(string[] Topics_, string[] Types)
        {
            this.Topics_ = Topics_;
            this.Types = Types;
        }
        
        /// <summary> Constructor with buffer. </summary>
        public TopicsResponse(ref Buffer b)
        {
            Topics_ = b.DeserializeStringArray();
            Types = b.DeserializeStringArray();
        }
        
        public ISerializable RosDeserialize(ref Buffer b)
        {
            return new TopicsResponse(ref b);
        }
        
        TopicsResponse IDeserializable<TopicsResponse>.RosDeserialize(ref Buffer b)
        {
            return new TopicsResponse(ref b);
        }
    
        public void RosSerialize(ref Buffer b)
        {
            b.SerializeArray(Topics_, 0);
            b.SerializeArray(Types, 0);
        }
        
        public void Dispose()
        {
        }
        
        public void RosValidate()
        {
            if (Topics_ is null) throw new System.NullReferenceException(nameof(Topics_));
            for (int i = 0; i < Topics_.Length; i++)
            {
                if (Topics_[i] is null) throw new System.NullReferenceException($"{nameof(Topics_)}[{i}]");
            }
            if (Types is null) throw new System.NullReferenceException(nameof(Types));
            for (int i = 0; i < Types.Length; i++)
            {
                if (Types[i] is null) throw new System.NullReferenceException($"{nameof(Types)}[{i}]");
            }
        }
    
        public int RosMessageLength
        {
            get {
                int size = 8;
                size += 4 * Topics_.Length;
                foreach (string s in Topics_)
                {
                    size += BuiltIns.UTF8.GetByteCount(s);
                }
                size += 4 * Types.Length;
                foreach (string s in Types)
                {
                    size += BuiltIns.UTF8.GetByteCount(s);
                }
                return size;
            }
        }
    
        public override string ToString() => Extensions.ToString(this);
    }
}
