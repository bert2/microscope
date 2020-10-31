#nullable enable

namespace Microscope.CodeAnalysis {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;

    using Microscope.CodeAnalysis.Model;

    using Mono.Cecil;
    using Mono.Cecil.Rocks;

    public static class CollectGeneratedCodeExt {
        public static IReadOnlyList<GeneratedType> CollectGeneratedCode(this MethodDefinition method)
            => method
                .CollectGeneratedMethods(visited: new HashSet<MethodDefinition>())
                .GroupBy(m => m.DeclaringType)
                .Select(GeneratedType.From)
                .ToArray();

        public static IEnumerable<MethodDefinition> CollectGeneratedMethods(
            this MethodDefinition method,
            ISet<MethodDefinition> visited)
            => method
                .Body?
                .Instructions
                .Select(instr => instr.Operand)
                .OfType<MethodReference>()
                .Select(TryResolve)
                .WhereNotNull()
                .Where(m => !m.IsGetter && !m.IsSetter)
                .Where(IsCompilerGenerated)
                .Where(m => visited.Add(m))
                .Aggregate(
                    Enumerable.Empty<MethodDefinition>(),
                    (ms, m) => ms.Append(m).Concat(m.CollectGeneratedMethods(visited)))
                .Concat(method.CollectAsyncStateMachineMethods(visited))
                .Concat(method.CollectIteratorStateMachineMethods(visited))
                ?? Enumerable.Empty<MethodDefinition>();

        private static IEnumerable<MethodDefinition> CollectAsyncStateMachineMethods(
            this MethodDefinition method,
            ISet<MethodDefinition> visited)
            => method
                .TryGetCustomAttribute<AsyncStateMachineAttribute>()?
                .GetStateMachineImplementation()
                .CollectMethod("MoveNext", visited)
                ?? Enumerable.Empty<MethodDefinition>();

        private static IEnumerable<MethodDefinition> CollectIteratorStateMachineMethods(
            this MethodDefinition method,
            ISet<MethodDefinition> visited)
            => method
                .TryGetCustomAttribute<IteratorStateMachineAttribute>()?
                .GetStateMachineImplementation()
                .CollectMethod("MoveNext", visited)
                ?? Enumerable.Empty<MethodDefinition>();

        private static TypeDefinition GetStateMachineImplementation(this CustomAttribute a)
            => (TypeDefinition)a.ConstructorArguments.Single().Value;

        private static IEnumerable<MethodDefinition> CollectMethod(
            this TypeDefinition type,
            string methodName,
            ISet<MethodDefinition> visited) {
            var method = type.Methods.Single(m => m.Name == methodName);
            return visited.Add(method)
                ? method.CollectGeneratedMethods(visited).Prepend(method)
                : Enumerable.Empty<MethodDefinition>();
        }

        private static MethodDefinition? TryResolve(MethodReference method) {
            try {
                return method.Resolve();
            } catch (AssemblyResolutionException) {
                return null;
            }
        }

        private static bool IsCompilerGenerated(MethodDefinition m)
            => m.HasCustomAttribute<CompilerGeneratedAttribute>()
            || m.DeclaringType.HasCustomAttribute<CompilerGeneratedAttribute>();

        private static bool HasCustomAttribute<T>(this ICustomAttributeProvider cap)
            where T : Attribute
            => cap.HasCustomAttributes
            && cap.CustomAttributes.Any(a => a.AttributeType.FullName == typeof(T).FullName);

        private static CustomAttribute? TryGetCustomAttribute<T>(this ICustomAttributeProvider cap)
            where T : Attribute
            => cap.HasCustomAttributes
                ? cap.CustomAttributes.SingleOrDefault(a => a.AttributeType.FullName == typeof(T).FullName)
                : null;

        /// <summary>Filters `null`s and also turns the NRT elements into non-nullable elements.</summary>
        private static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> source)
            where T : class
            => source.Where(x => x != null)!;
    }
}
