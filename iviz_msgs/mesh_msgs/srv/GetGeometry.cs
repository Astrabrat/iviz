using System.Runtime.Serialization;

namespace Iviz.Msgs.MeshMsgs
{
    [DataContract (Name = "mesh_msgs/GetGeometry")]
    public sealed class GetGeometry : IService
    {
        /// <summary> Request message. </summary>
        [DataMember] public GetGeometryRequest Request { get; set; }
        
        /// <summary> Response message. </summary>
        [DataMember] public GetGeometryResponse Response { get; set; }
        
        /// <summary> Empty constructor. </summary>
        public GetGeometry()
        {
            Request = new GetGeometryRequest();
            Response = new GetGeometryResponse();
        }
        
        /// <summary> Setter constructor. </summary>
        public GetGeometry(GetGeometryRequest request)
        {
            Request = request;
            Response = new GetGeometryResponse();
        }
        
        IService IService.Create() => new GetGeometry();
        
        IRequest IService.Request
        {
            get => Request;
            set => Request = (GetGeometryRequest)value;
        }
        
        IResponse IService.Response
        {
            get => Response;
            set => Response = (GetGeometryResponse)value;
        }
        
        public void Dispose()
        {
            Request.Dispose();
            Response.Dispose();
        }
        
        string IService.RosType => RosServiceType;
        
        /// <summary> Full ROS name of this service. </summary>
        [Preserve] public const string RosServiceType = "mesh_msgs/GetGeometry";
        
        /// <summary> MD5 hash of a compact representation of the service. </summary>
        [Preserve] public const string RosMd5Sum = "e21c42f8a3978429fcbcd1c03ddeb4e3";
        
        public override string ToString() => Extensions.ToString(this);
    }

    [DataContract]
    public sealed class GetGeometryRequest : IRequest<GetGeometry, GetGeometryResponse>, IDeserializable<GetGeometryRequest>
    {
        [DataMember (Name = "uuid")] public string Uuid;
    
        /// <summary> Constructor for empty message. </summary>
        public GetGeometryRequest()
        {
            Uuid = string.Empty;
        }
        
        /// <summary> Explicit constructor. </summary>
        public GetGeometryRequest(string Uuid)
        {
            this.Uuid = Uuid;
        }
        
        /// <summary> Constructor with buffer. </summary>
        internal GetGeometryRequest(ref Buffer b)
        {
            Uuid = b.DeserializeString();
        }
        
        public ISerializable RosDeserialize(ref Buffer b)
        {
            return new GetGeometryRequest(ref b);
        }
        
        GetGeometryRequest IDeserializable<GetGeometryRequest>.RosDeserialize(ref Buffer b)
        {
            return new GetGeometryRequest(ref b);
        }
    
        public void RosSerialize(ref Buffer b)
        {
            b.Serialize(Uuid);
        }
        
        public void Dispose()
        {
        }
        
        public void RosValidate()
        {
            if (Uuid is null) throw new System.NullReferenceException(nameof(Uuid));
        }
    
        public int RosMessageLength => 4 + BuiltIns.GetStringSize(Uuid);
    
        public override string ToString() => Extensions.ToString(this);
    }

    [DataContract]
    public sealed class GetGeometryResponse : IResponse, IDeserializable<GetGeometryResponse>
    {
        [DataMember (Name = "mesh_geometry_stamped")] public MeshMsgs.MeshGeometryStamped MeshGeometryStamped;
    
        /// <summary> Constructor for empty message. </summary>
        public GetGeometryResponse()
        {
            MeshGeometryStamped = new MeshMsgs.MeshGeometryStamped();
        }
        
        /// <summary> Explicit constructor. </summary>
        public GetGeometryResponse(MeshMsgs.MeshGeometryStamped MeshGeometryStamped)
        {
            this.MeshGeometryStamped = MeshGeometryStamped;
        }
        
        /// <summary> Constructor with buffer. </summary>
        internal GetGeometryResponse(ref Buffer b)
        {
            MeshGeometryStamped = new MeshMsgs.MeshGeometryStamped(ref b);
        }
        
        public ISerializable RosDeserialize(ref Buffer b)
        {
            return new GetGeometryResponse(ref b);
        }
        
        GetGeometryResponse IDeserializable<GetGeometryResponse>.RosDeserialize(ref Buffer b)
        {
            return new GetGeometryResponse(ref b);
        }
    
        public void RosSerialize(ref Buffer b)
        {
            MeshGeometryStamped.RosSerialize(ref b);
        }
        
        public void Dispose()
        {
        }
        
        public void RosValidate()
        {
            if (MeshGeometryStamped is null) throw new System.NullReferenceException(nameof(MeshGeometryStamped));
            MeshGeometryStamped.RosValidate();
        }
    
        public int RosMessageLength => 0 + MeshGeometryStamped.RosMessageLength;
    
        public override string ToString() => Extensions.ToString(this);
    }
}
