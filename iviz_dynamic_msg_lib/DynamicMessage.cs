using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Iviz.Msgs;
using Iviz.Msgs.GeometryMsgs;
using Iviz.Msgs.StdMsgs;
using Newtonsoft.Json;
using ISerializable = Iviz.Msgs.ISerializable;


namespace Iviz.MsgsGen.Dynamic
{
    public sealed class DynamicMessage : IField, IMessage, IDeserializable<DynamicMessage>
    {
        static readonly Dictionary<string, Type> BaseTypes = new()
        {
            ["bool"] = typeof(bool),
            ["int8"] = typeof(sbyte),
            ["uint8"] = typeof(byte),
            ["int16"] = typeof(short),
            ["uint16"] = typeof(ushort),
            ["int32"] = typeof(int),
            ["uint32"] = typeof(uint),
            ["int64"] = typeof(long),
            ["uint64"] = typeof(ulong),
            ["float32"] = typeof(float),
            ["float64"] = typeof(double),
            ["time"] = typeof(time),
            ["duration"] = typeof(duration),
            ["char"] = typeof(char),
            ["byte"] = typeof(byte),
        };

        public ReadOnlyCollection<Property> Fields { get; }
        public string RosType { get; }
        public string? RosInstanceMd5Sum { get; }
        public string? RosInstanceDependencies { get; }

        public FieldType Type => FieldType.DynamicMessage;

        public bool IsInitialized => !string.IsNullOrEmpty(RosType);

        public DynamicMessage()
        {
            RosType = "";
            var fields = Array.Empty<Property>();
            Fields = new ReadOnlyCollection<Property>(fields);
        }

        DynamicMessage(ClassInfo classInfo, bool allowAssemblyLookup = true) : this(classInfo,
            new Dictionary<string, DynamicMessage>(), allowAssemblyLookup)
        {
        }

        DynamicMessage(ClassInfo classInfo, IDictionary<string, DynamicMessage> registered,
            bool allowAssemblyLookup = true)
        {
            if (classInfo == null)
            {
                throw new ArgumentNullException(nameof(classInfo));
            }

            if (registered == null)
            {
                throw new ArgumentNullException(nameof(registered));
            }

            RosType = classInfo.FullRosName;
            RosInstanceMd5Sum = classInfo.Md5Hash;
            RosInstanceDependencies = classInfo.CreateCatDependencies();

            registered[RosType] = this;

            string FullRosName(string rosType) => rosType.Contains("/") ? rosType : $"{classInfo.RosPackage}/{rosType}";

            List<Property> newFields = new();
            foreach (VariableElement element in classInfo.Elements.OfType<VariableElement>())
            {
                IField field;
                string name = element.RosFieldName;
                string rosType = element.RosClassName;

                if (rosType == "string")
                {
                    field = element.ArraySize switch
                    {
                        VariableElement.NotAnArray => new StringField(),
                        VariableElement.DynamicSizeArray => new StringArrayField(),
                        _ => new StringFixedArrayField(element.ArraySize)
                    };
                }
                else if (BaseTypes.TryGetValue(rosType, out Type? csType))
                {
                    object? tmpField = element.ArraySize switch
                    {
                        VariableElement.NotAnArray =>
                            Activator.CreateInstance(typeof(StructField<>).MakeGenericType(csType)),
                        VariableElement.DynamicSizeArray =>
                            Activator.CreateInstance(typeof(StructArrayField<>).MakeGenericType(csType)),
                        _ =>
                            Activator.CreateInstance(typeof(StructFixedArrayField<>).MakeGenericType(csType),
                                element.ArraySize)
                    };
                    field = (IField) (tmpField ?? throw new NullReferenceException());
                }
                else if (allowAssemblyLookup &&
                         (csType = BuiltIns.TryGetTypeFromMessageName(FullRosName(rosType))) != null)
                {
                    object? tmpField = element.ArraySize switch
                    {
                        VariableElement.NotAnArray =>
                            Activator.CreateInstance(typeof(MessageField<>).MakeGenericType(csType)),
                        VariableElement.DynamicSizeArray =>
                            Activator.CreateInstance(typeof(MessageArrayField<>).MakeGenericType(csType)),
                        _ =>
                            Activator.CreateInstance(typeof(MessageFixedArrayField<>).MakeGenericType(csType),
                                element.ArraySize)
                    };
                    field = (IField) (tmpField ?? throw new NullReferenceException());
                }
                else
                {
                    if (!registered.TryGetValue(name, out DynamicMessage? generator))
                    {
                        if (element.ClassInfo == null)
                        {
                            throw new MessageParseException(
                                $"Missing class info for '{element.RosClassName}' for field '{element.RosFieldName}'" +
                                $" in message '{classInfo.FullRosName}'");
                        }

                        generator = new DynamicMessage(element.ClassInfo, registered);
                    }

                    field = element.ArraySize switch
                    {
                        VariableElement.NotAnArray => new DynamicMessage(generator),
                        VariableElement.DynamicSizeArray => new DynamicMessageArrayField(generator),
                        _ => new DynamicMessageFixedArrayField(element.ArraySize, generator)
                    };
                }

                newFields.Add(new Property(name, field));
            }

            Fields = new ReadOnlyCollection<Property>(newFields);
        }

        internal DynamicMessage(DynamicMessage other)
        {
            RosType = other.RosType;
            RosInstanceMd5Sum = other.RosInstanceMd5Sum;
            RosInstanceDependencies = other.RosInstanceDependencies;
            var fields = new Property[other.Fields.Count];
            for (int i = 0; i < fields.Length; i++)
            {
                fields[i] = new Property(other.Fields[i].Name, other.Fields[i].Value.Generate());
            }

            Fields = new ReadOnlyCollection<Property>(fields);
        }

        int IField.RosLength => RosMessageLength;

        internal int RosLength => RosMessageLength;

        public int RosMessageLength
        {
            get
            {
                int size = 0;
                foreach (var field in Fields)
                {
                    size += field.Value.RosLength;
                }

                return size;
            }
        }

        public void RosValidate()
        {
            foreach (var field in Fields)
            {
                field.Value.RosValidate();
            }
        }

        public void RosSerialize(ref WriteBuffer b)
        {
            foreach (var field in Fields)
            {
                field.Value.RosSerialize(ref b);
            }
        }

        internal void RosDeserializeInPlace(ref ReadBuffer b)
        {
            foreach (var field in Fields)
            {
                field.Value.RosDeserializeInPlace(ref b);
            }
        }

        void IField.RosDeserializeInPlace(ref ReadBuffer b) => RosDeserializeInPlace(ref b);

        IField IField.Generate()
        {
            return new DynamicMessage(this);
        }

        object IField.Value => Fields;

        public DynamicMessage RosDeserialize(ref ReadBuffer b)
        {
            var t = new DynamicMessage(this);
            t.RosDeserializeInPlace(ref b);
            return t;
        }

        ISerializable ISerializable.RosDeserializeBase(ref ReadBuffer b)
        {
            return RosDeserialize(ref b);
        }

        public void Dispose()
        {
        }

        public static bool IsDynamic<T>() => typeof(DynamicMessage) == typeof(T);

        public static bool IsGenericMessage<T>() => typeof(IMessage) == typeof(T);

        [Preserve] public const string RosMessageType = "*";

        /// <summary> MD5 hash of a compact representation of the message. </summary>
        [Preserve] public const string RosMd5Sum = "*";

        /// <summary> Base64 of the GZip'd compression of the concatenated dependencies file. </summary>
        [Preserve] public const string RosDependenciesBase64 = "";

        public override string ToString() => this.ToJsonString();

        static ClassInfo CreateDefinitionFromDependencyString(string fullRosMsgName, string dependencies)
        {
            if (fullRosMsgName == null)
            {
                throw new ArgumentNullException(nameof(fullRosMsgName));
            }

            if (dependencies == null)
            {
                throw new ArgumentNullException(nameof(dependencies));
            }

            var msgDefinitionsWithHeader = dependencies
                .Split(new[] {ClassInfo.DependencySeparator}, StringSplitOptions.RemoveEmptyEntries)
                .Select(def => def.Trim());
            var msgDefinitions = StripHeader(fullRosMsgName, msgDefinitionsWithHeader);
            var classInfos = msgDefinitions.Select(tuple => new ClassInfo("", tuple.Name, tuple.Definition));

            PackageInfo packageInfo = new(classInfos, Array.Empty<ServiceInfo>());
            packageInfo.ResolveAll();
            return packageInfo.Messages[fullRosMsgName];
        }

        public static DynamicMessage CreateFromDependencyString(string fullRosMsgName, string dependencies,
            bool allowAssemblyLookup = true)
        {
            return new(
                CreateDefinitionFromDependencyString(fullRosMsgName, dependencies),
                allowAssemblyLookup);
        }

        static IEnumerable<(string Name, string Definition)> StripHeader(string msgName,
            IEnumerable<string> msgDefinitions)
        {
            bool isFirst = true;
            foreach (string msgDefinition in msgDefinitions)
            {
                if (!isFirst)
                {
                    int newLineIndex = msgDefinition.IndexOf('\n');
                    if (newLineIndex < 5 || msgDefinition.Substring(0, 5) != "MSG: ")
                    {
                        throw new MessageParseException("Expected message name in the first line");
                    }

                    string name = msgDefinition.Substring(5, newLineIndex - 5).Trim();
                    yield return (name, msgDefinition.Substring(newLineIndex + 1));
                }
                else
                {
                    yield return (msgName, msgDefinition);
                    isFirst = false;
                }
            }
        }
        
        [Preserve] public static IField[][] AotFields => new[]
        {
            CreateAotFields<Vector3>(),
            CreateAotFields<Point>(),
            CreateAotFields<Point32>(),
            CreateAotFields<Quaternion>(),
            CreateAotFields<Pose>(),
            CreateAotFields<Transform>(),
            CreateAotFields<Twist>(),
            CreateAotFields<ColorRGBA>(),
            CreateAotFields<Msgs.IvizMsgs.Color32>(),
            CreateAotFields<Msgs.IvizMsgs.Vector3f>(),
            CreateAotFields<Msgs.IvizMsgs.Vector2f>(),
            CreateAotFields<Msgs.IvizMsgs.Triangle>(),
            CreateAotFields<Header>(),
            CreateAotFields<TransformStamped>(),
            CreateAotFields<Msgs.RosgraphMsgs.Log>(),

            CreateAotStructFields<bool>(),
            CreateAotStructFields<sbyte>(),
            CreateAotStructFields<byte>(),
            CreateAotStructFields<short>(),
            CreateAotStructFields<ushort>(),
            CreateAotStructFields<int>(),
            CreateAotStructFields<uint>(),
            CreateAotStructFields<long>(),
            CreateAotStructFields<ulong>(),
            CreateAotStructFields<float>(),
            CreateAotStructFields<double>(),
            CreateAotStructFields<time>(),
            CreateAotStructFields<duration>(),
        };

        static IField[] CreateAotFields<T>() where T : struct, IMessage, IDeserializable<T> => new IField[]
            {new MessageField<T>(), new MessageArrayField<T>(), new MessageFixedArrayField<T>(0)};

        static IField[] CreateAotStructFields<T>() where T : unmanaged => new IField[]
            {new StructField<T>(), new StructArrayField<T>(), new StructFixedArrayField<T>(0)};
    }
}