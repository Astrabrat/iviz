/* This file was created automatically, do not edit! */

using System.Runtime.Serialization;

namespace Iviz.Msgs.IvizMsgs
{
    [Preserve, DataContract (Name = "iviz_msgs/Material")]
    public sealed class Material : IDeserializable<Material>, IMessage
    {
        public const byte BLEND_DEFAULT = 0;
        public const byte BLEND_ADDITIVE = 1;
        [DataMember (Name = "name")] public string Name { get; set; }
        [DataMember (Name = "ambient")] public Color32 Ambient { get; set; }
        [DataMember (Name = "diffuse")] public Color32 Diffuse { get; set; }
        [DataMember (Name = "emissive")] public Color32 Emissive { get; set; }
        [DataMember (Name = "opacity")] public float Opacity { get; set; }
        [DataMember (Name = "bump_scaling")] public float BumpScaling { get; set; }
        [DataMember (Name = "shininess")] public float Shininess { get; set; }
        [DataMember (Name = "shininess_strength")] public float ShininessStrength { get; set; }
        [DataMember (Name = "reflectivity")] public float Reflectivity { get; set; }
        [DataMember (Name = "blend_mode")] public byte BlendMode { get; set; }
        [DataMember (Name = "textures")] public Texture[] Textures { get; set; }
    
        /// <summary> Constructor for empty message. </summary>
        public Material()
        {
            Name = string.Empty;
            Textures = System.Array.Empty<Texture>();
        }
        
        /// <summary> Explicit constructor. </summary>
        public Material(string Name, in Color32 Ambient, in Color32 Diffuse, in Color32 Emissive, float Opacity, float BumpScaling, float Shininess, float ShininessStrength, float Reflectivity, byte BlendMode, Texture[] Textures)
        {
            this.Name = Name;
            this.Ambient = Ambient;
            this.Diffuse = Diffuse;
            this.Emissive = Emissive;
            this.Opacity = Opacity;
            this.BumpScaling = BumpScaling;
            this.Shininess = Shininess;
            this.ShininessStrength = ShininessStrength;
            this.Reflectivity = Reflectivity;
            this.BlendMode = BlendMode;
            this.Textures = Textures;
        }
        
        /// <summary> Constructor with buffer. </summary>
        public Material(ref Buffer b)
        {
            Name = b.DeserializeString();
            Ambient = new Color32(ref b);
            Diffuse = new Color32(ref b);
            Emissive = new Color32(ref b);
            Opacity = b.Deserialize<float>();
            BumpScaling = b.Deserialize<float>();
            Shininess = b.Deserialize<float>();
            ShininessStrength = b.Deserialize<float>();
            Reflectivity = b.Deserialize<float>();
            BlendMode = b.Deserialize<byte>();
            Textures = b.DeserializeArray<Texture>();
            for (int i = 0; i < Textures.Length; i++)
            {
                Textures[i] = new Texture(ref b);
            }
        }
        
        public ISerializable RosDeserialize(ref Buffer b)
        {
            return new Material(ref b);
        }
        
        Material IDeserializable<Material>.RosDeserialize(ref Buffer b)
        {
            return new Material(ref b);
        }
    
        public void RosSerialize(ref Buffer b)
        {
            b.Serialize(Name);
            Ambient.RosSerialize(ref b);
            Diffuse.RosSerialize(ref b);
            Emissive.RosSerialize(ref b);
            b.Serialize(Opacity);
            b.Serialize(BumpScaling);
            b.Serialize(Shininess);
            b.Serialize(ShininessStrength);
            b.Serialize(Reflectivity);
            b.Serialize(BlendMode);
            b.SerializeArray(Textures, 0);
        }
        
        public void Dispose()
        {
        }
        
        public void RosValidate()
        {
            if (Name is null) throw new System.NullReferenceException(nameof(Name));
            if (Textures is null) throw new System.NullReferenceException(nameof(Textures));
            for (int i = 0; i < Textures.Length; i++)
            {
                if (Textures[i] is null) throw new System.NullReferenceException($"{nameof(Textures)}[{i}]");
                Textures[i].RosValidate();
            }
        }
    
        public int RosMessageLength
        {
            get {
                int size = 41;
                size += BuiltIns.UTF8.GetByteCount(Name);
                foreach (var i in Textures)
                {
                    size += i.RosMessageLength;
                }
                return size;
            }
        }
    
        public string RosType => RosMessageType;
    
        /// <summary> Full ROS name of this message. </summary>
        [Preserve] public const string RosMessageType = "iviz_msgs/Material";
    
        /// <summary> MD5 hash of a compact representation of the message. </summary>
        [Preserve] public const string RosMd5Sum = "dea645939e7a51f77d59181b714cdac1";
    
        /// <summary> Base64 of the GZip'd compression of the concatenated dependencies file. </summary>
        [Preserve] public const string RosDependenciesBase64 =
                "H4sIAAAAAAAAE7WU247bIBCG73kKv0HbPbTbSntBbJKgArZ8yDaqKkQSkkWKDzI43e3TF9vYS7S3rS9s" +
                "8/0j+Gdg6FRlHoIFQSziEVrCguTBY/ARdB6HUYRzvEFW+ASANq2qTkElSgnC+ly3tzeBKHdKVmYeH9Tx" +
                "2Ok3XZZKa3WR4HiuhbGgbsRemdd5vOvKhuu9ONupZ6ifVaUqqfV7wq0LWZ3M8yy18niWe6Mu/bSj+91Z" +
                "Vgde1gcJcvliulb+/BWY8U8D8PiPH0Cz1bfAGvjDS33SH1zyzkzrvqfJnPuK/2/EJe8WzLcJ4ixmyNvn" +
                "gUV4uSyycZc9nCUoLAhMLb/xOaQLjFh/Wm59jCjOsvGw3Pl8jfBq3UffX/tIKSSZxZ+v1lxjhhnKeuGL" +
                "L8QJDHG+tfjh2nqWEBgiOhr66mukX5fCpM/rKt8ULQkKcxyzXrrKuWDfWfw08BvgBDtFgtmKL9OY8mLj" +
                "VW9SsmSNUr9+kxBuCWYR8ks4SYv4h1fBidpkmF/Bib/5up9sxQmntmlxQraeJUtt13pWLMiKRZ7CMPdc" +
                "WBrhDY6Q56GPpHGcr90Mdx7HK4Yix2cHTylMeP/y1h9YSCBNPA8DpDhNY78SA41QCMlgYrpfGmGb2wbY" +
                "1lbVQb64aPPaTAe5FE3TXxdjUHfhY9x8pQzdfxR7U0+9VzeyFUbVlRv/bkUz3A+8e0cuAPwFjGxOhxwF" +
                "AAA=";
                
        public override string ToString() => Extensions.ToString(this);
    }
}
