namespace DotNetApis.Cecil {
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;

    using Mono.Cecil;

    /// <summary>
    /// Code taken from https://github.com/StephenClearyApps/DotNetApis/blob/master/service/DotNetApis.Cecil/CecilExtensions.Xmldoc.cs
    /// </summary>
    public static class CecilExtensions {
        /// <summary>
        /// Returns the Xmldoc identifier for this member.
        /// </summary>
        public static string MemberXmldocIdentifier(this IMemberDefinition member) {
            switch (member) {
                case TypeReference type:
                    return type.XmldocIdentifier();
                case EventReference evt:
                    return evt.XmldocIdentifier();
                case FieldReference field:
                    return field.XmldocIdentifier();
                case PropertyReference property:
                    return property.XmldocIdentifier();
                case MethodReference method:
                    return method.XmldocIdentifier();
            }
            throw new InvalidOperationException("Unknown member definition type " + member.GetType().Name);
        }

        /// <summary>
        /// Returns the Xmldoc identifier for this type.
        /// </summary>
        public static string XmldocIdentifier(this TypeReference type) => "T:" + type.XmldocIdentifierName();

        /// <summary>
        /// Returns the Xmldoc identifier for this event.
        /// </summary>
        public static string XmldocIdentifier(this EventReference evt) => "E:" + evt.DeclaringType.XmldocIdentifierName() + "." + XmldocEncodeName(evt.Name);

        /// <summary>
        /// Returns the Xmldoc identifier for this field.
        /// </summary>
        public static string XmldocIdentifier(this FieldReference field) => "F:" + field.DeclaringType.XmldocIdentifierName() + "." + XmldocEncodeName(field.Name);

        /// <summary>
        /// Returns the Xmldoc identifier for this property.
        /// </summary>
        public static string XmldocIdentifier(this PropertyReference property) {
            var result = "P:" + property.DeclaringType.XmldocIdentifierName() + "." + XmldocEncodeName(property.Name);
            if (property.Parameters.Count == 0)
                return result;
            return result + "(" + string.Join(",", property.Parameters.Select(XmldocName)) + ")";
        }

        /// <summary>
        /// Returns the Xmldoc identifier for this method.
        /// </summary>
        public static string XmldocIdentifier(this MethodReference method) {
            var result = "M:" + method.DeclaringType.XmldocIdentifierName() + "." + XmldocEncodeName(method.Name);
            if (method.HasGenericParameters)
                result += "``" + method.GenericParameters.Count.ToString(CultureInfo.InvariantCulture);
            if (!method.HasParameters)
                return result;
            result += "(" + string.Join(",", method.Parameters.Select(XmldocName)) + ")";
            if (method.Name == "op_Implicit" || method.Name == "op_Explicit")
                result += "~" + method.ReturnType.XmldocName();
            return result;
        }

        /// <summary>
        /// Returns the Xmldoc identifier for the overload group containing this method.
        /// </summary>
        public static string OverloadXmldocIdentifier(this MethodReference method) => "O:" + method.DeclaringType.XmldocIdentifierName() + "." + XmldocEncodeName(method.Name);

        /// <summary>
        /// Returns the Xmldoc identifier for this parameter.
        /// </summary>
        private static string XmldocName(this ParameterDefinition parameter) => parameter.ParameterType.XmldocName();

        /// <summary>
        /// Returns the Xmldoc identifier for this type reference.
        /// </summary>
        public static string XmldocIdentifierName(this TypeReference type) {
            if (type.DeclaringType != null)
                return type.DeclaringType.XmldocIdentifierName() + "." + XmldocEncodeName(type.Name);
            return type.Namespace.DotAppend(type.Name);
        }

        /// <summary>
        /// Returns the simple Xmldoc name for this type reference.
        /// </summary>
        private static string XmldocName(this TypeReference type, List<TypeReference> genericArguments = null) {
            if (type is GenericParameter genericParameter) {
                if (genericParameter.DeclaringMethod == null)
                    return "`" + genericParameter.Position.ToString(CultureInfo.InvariantCulture);
                return "``" + genericParameter.Position.ToString(CultureInfo.InvariantCulture);
            }

            if (type is PointerType pointerType)
                return pointerType.ElementType.XmldocName() + "*";

            if (type is ArrayType arrayType)
                return arrayType.ElementType.XmldocName() + "[" + string.Join(",", arrayType.Dimensions.Select(XmldocName)) + "]";

            if (type is ByReferenceType byrefType)
                return byrefType.ElementType.XmldocName() + "@";

            if (type is OptionalModifierType optmodType)
                return optmodType.ElementType.XmldocName() + "!" + optmodType.ModifierType.XmldocName();

            if (type is RequiredModifierType reqmodType)
                return reqmodType.ElementType.XmldocName() + "|" + reqmodType.ModifierType.XmldocName();

            if (type is GenericInstanceType genericInstance) {
                var arguments = new List<TypeReference>(genericInstance.GenericArguments);
                var result = type.IsNested ? type.DeclaringType.XmldocName(arguments) : type.Namespace;
                var backtickIndex = type.Name.LastIndexOf('`');
                if (backtickIndex != -1) {
                    var myArgs = arguments.Take(int.Parse(type.Name.Substring(backtickIndex + 1), CultureInfo.InvariantCulture)).ToArray();
                    arguments.RemoveRange(0, myArgs.Length);
                    return result.DotAppend(type.Name.Substring(0, backtickIndex) + "{" + string.Join(",", myArgs.Select(x => XmldocName(x, arguments))) + "}");
                }
                return result.DotAppend(type.Name);
            }

            // It's a fully-qualified reference to a type.
            {
                var result = type.IsNested ? type.DeclaringType.XmldocName() : type.Namespace;
                var backtickIndex = type.Name.LastIndexOf('`');
                if (backtickIndex != -1 && genericArguments != null) {
                    var myArgs = genericArguments.Take(int.Parse(type.Name.Substring(backtickIndex + 1), CultureInfo.InvariantCulture)).ToArray();
                    genericArguments.RemoveRange(0, myArgs.Length);
                    return result.DotAppend(type.Name.Substring(0, backtickIndex) + "{" + string.Join(",", myArgs.Select(x => XmldocName(x, genericArguments))) + "}");
                }
                return result.DotAppend(type.Name);
            }
        }

        /// <summary>
        /// Returns the simple Xmldoc name for this array dimension.
        /// </summary>
        private static string XmldocName(this ArrayDimension arrayDimension) {
            if (!arrayDimension.LowerBound.HasValue && !arrayDimension.UpperBound.HasValue)
                return string.Empty;
            var result = new StringBuilder();
            if (arrayDimension.LowerBound.HasValue)
                result.Append(arrayDimension.LowerBound.Value.ToString(CultureInfo.InvariantCulture));
            result.Append(":");
            if (arrayDimension.UpperBound.HasValue)
                result.Append(arrayDimension.UpperBound.Value.ToString(CultureInfo.InvariantCulture));
            return result.ToString();
        }

        /// <summary>
        /// Encodes a string as an Xmldoc name.
        /// </summary>
        private static string XmldocEncodeName(string name) => name.Replace('.', '#').Replace('<', '{').Replace('>', '}');

        /// <summary>
        /// Returns this string appended with a "." and then the <paramref name="append"/> string. If either this string or the <paramref name="append"/> string is empty, then the other string is returned without a ".".
        /// </summary>
        /// <param name="source">The source string.</param>
        /// <param name="append">The string to append.</param>
        /// <returns></returns>
        public static string DotAppend(this string source, string append) {
            if (source == "")
                return append;
            if (append == "")
                return source;
            return source + "." + append;
        }
    }
}
