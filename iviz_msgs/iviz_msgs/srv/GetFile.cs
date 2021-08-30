using System.Runtime.Serialization;

namespace Iviz.Msgs.IvizMsgs
{
    [DataContract (Name = "iviz_msgs/GetFile")]
    public sealed class GetFile : IService
    {
        /// <summary> Request message. </summary>
        [DataMember] public GetFileRequest Request { get; set; }
        
        /// <summary> Response message. </summary>
        [DataMember] public GetFileResponse Response { get; set; }
        
        /// <summary> Empty constructor. </summary>
        public GetFile()
        {
            Request = new GetFileRequest();
            Response = new GetFileResponse();
        }
        
        /// <summary> Setter constructor. </summary>
        public GetFile(GetFileRequest request)
        {
            Request = request;
            Response = new GetFileResponse();
        }
        
        IService IService.Create() => new GetFile();
        
        IRequest IService.Request
        {
            get => Request;
            set => Request = (GetFileRequest)value;
        }
        
        IResponse IService.Response
        {
            get => Response;
            set => Response = (GetFileResponse)value;
        }
        
        public void Dispose()
        {
            Request.Dispose();
            Response.Dispose();
        }
        
        string IService.RosType => RosServiceType;
        
        /// <summary> Full ROS name of this service. </summary>
        [Preserve] public const string RosServiceType = "iviz_msgs/GetFile";
        
        /// <summary> MD5 hash of a compact representation of the service. </summary>
        [Preserve] public const string RosMd5Sum = "08088c7144705ee9cf37b287c931476d";
        
        public override string ToString() => Extensions.ToString(this);
    }

    [DataContract]
    public sealed class GetFileRequest : IRequest<GetFile, GetFileResponse>, IDeserializable<GetFileRequest>
    {
        // Retrieves a file
        [DataMember (Name = "uri")] public string Uri; // Uri of the file. Example: package://some_package/file.dae
    
        /// <summary> Constructor for empty message. </summary>
        public GetFileRequest()
        {
            Uri = string.Empty;
        }
        
        /// <summary> Explicit constructor. </summary>
        public GetFileRequest(string Uri)
        {
            this.Uri = Uri;
        }
        
        /// <summary> Constructor with buffer. </summary>
        public GetFileRequest(ref Buffer b)
        {
            Uri = b.DeserializeString();
        }
        
        public ISerializable RosDeserialize(ref Buffer b)
        {
            return new GetFileRequest(ref b);
        }
        
        GetFileRequest IDeserializable<GetFileRequest>.RosDeserialize(ref Buffer b)
        {
            return new GetFileRequest(ref b);
        }
    
        public void RosSerialize(ref Buffer b)
        {
            b.Serialize(Uri);
        }
        
        public void Dispose()
        {
        }
        
        public void RosValidate()
        {
            if (Uri is null) throw new System.NullReferenceException(nameof(Uri));
        }
    
        public int RosMessageLength
        {
            get {
                int size = 4;
                size += BuiltIns.UTF8.GetByteCount(Uri);
                return size;
            }
        }
    
        public override string ToString() => Extensions.ToString(this);
    }

    [DataContract]
    public sealed class GetFileResponse : IResponse, IDeserializable<GetFileResponse>
    {
        [DataMember (Name = "success")] public bool Success; // Whether the retrieval succeeded
        [DataMember (Name = "bytes")] public byte[] Bytes; // The content of the file as byte array
        [DataMember (Name = "message")] public string Message; // An error message if success is false
    
        /// <summary> Constructor for empty message. </summary>
        public GetFileResponse()
        {
            Bytes = System.Array.Empty<byte>();
            Message = string.Empty;
        }
        
        /// <summary> Explicit constructor. </summary>
        public GetFileResponse(bool Success, byte[] Bytes, string Message)
        {
            this.Success = Success;
            this.Bytes = Bytes;
            this.Message = Message;
        }
        
        /// <summary> Constructor with buffer. </summary>
        public GetFileResponse(ref Buffer b)
        {
            Success = b.Deserialize<bool>();
            Bytes = b.DeserializeStructArray<byte>();
            Message = b.DeserializeString();
        }
        
        public ISerializable RosDeserialize(ref Buffer b)
        {
            return new GetFileResponse(ref b);
        }
        
        GetFileResponse IDeserializable<GetFileResponse>.RosDeserialize(ref Buffer b)
        {
            return new GetFileResponse(ref b);
        }
    
        public void RosSerialize(ref Buffer b)
        {
            b.Serialize(Success);
            b.SerializeStructArray(Bytes, 0);
            b.Serialize(Message);
        }
        
        public void Dispose()
        {
        }
        
        public void RosValidate()
        {
            if (Bytes is null) throw new System.NullReferenceException(nameof(Bytes));
            if (Message is null) throw new System.NullReferenceException(nameof(Message));
        }
    
        public int RosMessageLength
        {
            get {
                int size = 9;
                size += 1 * Bytes.Length;
                size += BuiltIns.UTF8.GetByteCount(Message);
                return size;
            }
        }
    
        public override string ToString() => Extensions.ToString(this);
    }
}
