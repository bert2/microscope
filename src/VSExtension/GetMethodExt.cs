#nullable enable

namespace Microscope.VSExtension {
    using System;
    using System.Linq;

    using Mono.Cecil;

    public static class GetMethodExt {
        public static MethodDefinition GetMethod(this AssemblyDefinition assembly, string methodFullName) {
            var lastDot = methodFullName.LastIndexOf('.');
            var typeName = methodFullName.Substring(0, lastDot);
            var memberName = methodFullName.Substring(lastDot + 1, methodFullName.Length - lastDot - 1);

            var type = assembly.MainModule.Types.SingleOrDefault(type => type.FullName == typeName)
                ?? throw new InvalidOperationException($"Type {typeName} could not be found in assembly {assembly.FullName}.");
            return type.Methods.SingleOrDefault(m => m.Name == memberName)
                ?? throw new InvalidOperationException($"Method {memberName} could not be found in type {typeName}.");
        }
    }
}
