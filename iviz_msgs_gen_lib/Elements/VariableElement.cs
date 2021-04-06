using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Iviz.MsgsGen
{
    public sealed class VariableElement : IElement
    {
        public const bool UseShared = false;

        const string CachedHeaderMd5 = "2176decaecbce78abc3b96ef049fabed";

        public const int NotAnArray = -1;
        public const int DynamicSizeArray = 0;

        readonly string rosClassToken;

        public ElementType Type => ElementType.Variable;
        public string Comment { get; }
        public string RosFieldName { get; }
        public string RosClassName { get; }
        public string CsFieldName { get; }
        public string CsClassName { get; }
        public int ArraySize { get; }
        public bool IsArray => ArraySize != NotAnArray;
        public bool IsDynamicSizeArray => ArraySize == DynamicSizeArray;
        public bool IsFixedSizeArray => ArraySize > 0;
        public int FixedArraySize => IsFixedSizeArray ? ArraySize : -1;
        public ClassInfo? ClassInfo { get; internal set; }
        public bool ClassIsStruct => ClassInfo?.ForceStruct ?? ClassInfo.IsClassForceStruct(RosClassName);
        public bool ClassIsBlittable => ClassInfo?.IsBlittable ?? ClassInfo.IsClassBlittable(RosClassName);
        public bool ClassHasFixedSize => ClassInfo != null && ClassInfo.HasFixedSize;

        static readonly HashSet<string> Keywords = new HashSet<string>
        {
            "default",
            "byte",
            "sbyte",
            "char",
            "short",
            "ushort",
            "int",
            "uint",
            "float",
            "double",
            "long",
            "ulong",
            "bool",
            "string",
            "readonly",
            "override",
        };

        internal VariableElement(string comment, string rosClassToken, string fieldName,
            string? parentClassName = null,
            ClassInfo? classInfo = null
        )
        {
            Comment = comment;
            this.rosClassToken = rosClassToken;

            RosFieldName = fieldName;

            CsFieldName = MsgParser.CsIfiy(fieldName);
            if (CsFieldName == parentClassName)
            {
                CsFieldName += "_"; // C# forbids fields with the same name as the class
            }

            if (Keywords.Contains(fieldName))
            {
                CsFieldName = $"@{fieldName}";
            }

            int bracketLeft = rosClassToken.IndexOf('[');
            int bracketRight = rosClassToken.IndexOf(']');
            if (bracketLeft != -1 && bracketRight != -1)
            {
                RosClassName = rosClassToken.Substring(0, bracketLeft);
                string arrayLengthStr = rosClassToken.Substring(bracketLeft + 1, bracketRight - bracketLeft - 1);

                if (string.IsNullOrWhiteSpace(arrayLengthStr))
                {
                    ArraySize = DynamicSizeArray;
                }
                else if (int.TryParse(arrayLengthStr, out int arraySize))
                {
                    ArraySize = arraySize;
                }
                else
                {
                    //TODO: error!
                    ArraySize = NotAnArray;
                }
            }
            else
            {
                RosClassName = rosClassToken;
                ArraySize = NotAnArray;
            }

            int slashIndex;
            if (RosClassName == "Header")
            {
                RosClassName = "std_msgs/Header";
                CsClassName = "StdMsgs.Header";
            }
            else if (MsgParser.BuiltInsMaps.TryGetValue(RosClassName, out string? className))
            {
                CsClassName = className;
            }
            else if ((slashIndex = RosClassName.IndexOf('/')) != -1)
            {
                string packageName = RosClassName.Substring(0, slashIndex);
                string classProper = RosClassName.Substring(slashIndex + 1);
                CsClassName = $"{MsgParser.CsIfiy(packageName)}.{classProper}";
            }
            else
            {
                CsClassName = RosClassName;
            }

            ClassInfo = classInfo;
        }

        public override string ToString()
        {
            return $"['{rosClassToken}' '{RosFieldName}' // '{Comment}']";
        }

        public IEnumerable<string> ToCsString(bool isInStruct)
        {
            string attrStr = (CsFieldName != RosFieldName)
                ? $"[DataMember (Name = \"{RosFieldName}\")]"
                : "[DataMember]";

            string result;
            switch (ArraySize)
            {
                case NotAnArray when CsClassName == "string":
                    if (UseShared)
                    {
                        result = isInStruct
                            ? $"public StringRef? {CsFieldName};"
                            : $"public StringRef {CsFieldName} {{ get; set; }}";
                    }
                    else
                    {
                        result = isInStruct
                            ? $"public string? {CsFieldName};"
                            : $"public string {CsFieldName} {{ get; set; }}";
                    }
                    break;
                case NotAnArray:
                    result = isInStruct
                        ? $"public {CsClassName} {CsFieldName};"
                        : $"public {CsClassName} {CsFieldName} {{ get; set; }}";
                    break;
                case DynamicSizeArray when UseShared && CsClassName == "string":
                    if (UseShared)
                    {
                        result = isInStruct
                            ? $"public UniqueRef<StringRef>? {CsFieldName};"
                            : $"public UniqueRef<StringRef> {CsFieldName} {{ get; set; }}";
                    }
                    else
                    {
                        result = isInStruct
                            ? $"public {CsClassName}[]? {CsFieldName};"
                            : $"public {CsClassName}[] {CsFieldName} {{ get; set; }}";
                    }

                    break;
                case DynamicSizeArray:
                    if (UseShared && CsClassName == "string")
                    {
                        result = isInStruct
                            ? $"public UniqueRef<StringRef>? {CsFieldName};"
                            : $"public UniqueRef<StringRef> {CsFieldName} {{ get; set; }}";
                    }
                    else if (UseShared)
                    {
                        result = isInStruct
                            ? $"public UniqueRef<{CsClassName}>? {CsFieldName};"
                            : $"public UniqueRef<{CsClassName}> {CsFieldName} {{ get; set; }}";
                    }
                    else
                    {
                        result = isInStruct
                            ? $"public {CsClassName}[]? {CsFieldName};"
                            : $"public {CsClassName}[] {CsFieldName} {{ get; set; }}";
                    }

                    break;
                default:
                {
                    if (UseShared)
                    {
                        result = isInStruct
                            ? $"public UniqueRef<{CsClassName}>? {CsFieldName} {{ get; set; }}"
                            : $"public UniqueRef<{CsClassName}> {CsFieldName} {{ get; set; }}";
                    }
                    else
                    {
                        result = isInStruct
                            ? $"public {CsClassName}[/*{ArraySize}*/]? {CsFieldName} {{ get; set; }}"
                            : $"public {CsClassName}[/*{ArraySize}*/] {CsFieldName} {{ get; set; }}";
                    }
                    break;
                }
            }

            string csString = Comment.Length == 0
                ? $"{attrStr} {result}"
                : $"{attrStr} {result} //{Comment}";

            /*
            if (ArraySize <= 0 || !isInStruct)
            {
                return new[] {csString};
            }

            List<string> list = new List<string> {csString};
            for (int i = 0; i < ArraySize; i++)
            {
                list.Add($"public {CsClassName} {CsFieldName}{i}");
                list.Add("{");
                list.Add($"    readonly get => {CsFieldName}[{i}];");
                list.Add($"    set => {CsFieldName}[{i}] = value;");
                list.Add("}");
            }

            return list;
            */

            return new[] {csString};
        }

        public string ToRosString()
        {
            return $"{rosClassToken} {RosFieldName}";
        }

        public string? GetEntryForMd5Hash(string parentPackageName)
        {
            if (ClassInfo != null)
            {
                return $"{ClassInfo.Md5Hash} {RosFieldName}";
            }

            if (ClassInfo.IsBuiltinType(RosClassName))
            {
                return ArraySize switch
                {
                    NotAnArray => $"{RosClassName} {RosFieldName}",
                    DynamicSizeArray => $"{RosClassName}[] {RosFieldName}",
                    _ => $"{RosClassName}[{ArraySize}] {RosFieldName}"
                };
            }

            // now we start improvising
            if (RosClassName == "std_msgs/Header")
            {
                return $"{CachedHeaderMd5} {RosFieldName}";
            }

            string fullRosClassName =
                RosClassName.Contains("/") ? RosClassName : $"{parentPackageName}/{RosClassName}";

            // is it in the assembly?
            
            Type? guessType = BuiltIns.TryGetTypeFromMessageName(fullRosClassName);
            if (guessType == null)
            {
                // nope? we bail out
                throw new MessageDependencyException($"Cannot find md5 for message '{RosClassName}' or '{fullRosClassName}'.");
            }

            string md5Sum = BuiltIns.GetMd5Sum(guessType);
            return $"{md5Sum} {RosFieldName}";
        }
    }

    // this class copies functions from Iviz.Msgs to avoid depending on that project 
    internal static class BuiltIns
    {
        public static Type? TryGetTypeFromMessageName(string fullRosMessageName, string assemblyName = "Iviz.Msgs")
        {
            string guessName = $"Iviz.Msgs.{RosNameToCs(fullRosMessageName)}, {assemblyName}";
            return System.Type.GetType(guessName);
        }
        
        public static string GetMd5Sum(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return GetClassStringConstant(type, "RosMd5Sum");
        }
        
        static string GetClassStringConstant(Type type, string name)
        {
            string? constant = (string?) type.GetField(name)?.GetRawConstantValue();
            if (constant == null)
            {
                throw new ArgumentException($"Failed to resolve constant '{name}' in class {type.FullName}",
                    nameof(name));
            }

            return constant;
        }
        
        public static string RosNameToCs(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            StringBuilder str = new();
            str.Append(char.ToUpper(name[0], CultureInfo.InvariantCulture));
            for (int i = 1; i < name.Length; i++)
            {
                switch (name[i])
                {
                    case '_' when i != name.Length - 1:
                        str.Append(char.ToUpper(name[i + 1], CultureInfo.InvariantCulture));
                        i++;
                        break;
                    case '/':
                        str.Append('.');
                        break;
                    default:
                        str.Append(name[i]);
                        break;
                }
            }

            return str.ToString();
        }
    }
}