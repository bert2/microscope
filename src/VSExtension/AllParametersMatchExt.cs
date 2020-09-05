#nullable enable

namespace Microscope.VSExtension {
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;

    using Microsoft.CodeAnalysis;

    using Mono.Cecil;

    public static class AllParametersMatchExt {
        public static bool AllParametersMatch(this IMethodSymbol target, MethodDefinition candidate)
            => target.Parameters
                .Zip(candidate.Parameters, (t, c) => t.OriginalDefinition.Matches(c))
                .All(methodParamMatched => methodParamMatched);

        private static bool Matches(this IParameterSymbol target, ParameterDefinition candidate)
            => target.MetadataName == candidate.Name
            && target.Type.Matches(candidate.ParameterType);

        private static bool Matches(this ITypeSymbol target, TypeReference candidate) {
            // Mono.Cecil exposes generic type arguments only on the innermost type in a hierarchy
            // of nested types. We collect them all here and compare each with the type arguments
            // we encounter while traversing the Roslyn symbol hierarchy.
            var candidateTypeArgs = candidate is IGenericInstance _candidate
                ? new Stack<TypeReference>(_candidate.GenericArguments)
                : new Stack<TypeReference>(capacity: 0);

            return target.Matches(candidate, candidateTypeArgs);
        }

        private static bool Matches(this ITypeSymbol target, TypeReference candidate, Stack<TypeReference> candidateTypeArgs) {
            if (target.MetadataName != candidate.Name) return false;

            // We can't compare with `==`, because the arity of a nested `TypeReference` is the sum
            // of the arities of all nesting `TypeReferences`.
            if (target.Arity() > candidate.Arity()) {
                return false;
            }

            if (target is INamedTypeSymbol _target
                && _target.IsGenericType
                && !_target.TypeArguments.AllMatch(candidateTypeArgs)) {
                return false;
            }

            if (candidate.IsNested) {
                return target.IsNested()
                    && target.ContainingType.Matches(candidate.DeclaringType, candidateTypeArgs);
            } else {
                return target.Namespace() == candidate.Namespace;
            }
        }

        private static int Arity(this ITypeSymbol type) => type is INamedTypeSymbol _type ? _type.Arity : 0;

        private static int Arity(this TypeReference type) => type is IGenericInstance _type
            ? _type.GenericArguments.Count
            : type.GenericParameters.Count;

        private static bool IsNested(this ITypeSymbol type) => type.ContainingType != null;

        private static bool AllMatch(this ImmutableArray<ITypeSymbol> targetTypeArgs, Stack<TypeReference> candidateTypeArgs) {
            // Take as many of the candidate type args as the target requires. Using a stack
            // ensures we're taking the innermost first. Taken candidates have to be reversed
            // so they are in the same order as they were declared in the type.
            var _candidateTypeArgs = candidateTypeArgs
                .Pop(count: targetTypeArgs.Length)
                .Reverse();

            return targetTypeArgs
                .Zip(_candidateTypeArgs, Matches)
                .All(typeArgMatched => typeArgMatched);
        }

        private static IEnumerable<T> Pop<T>(this Stack<T> s, int count) {
            if (s.Count < count) throw new ArgumentOutOfRangeException(nameof(count), count, "Not enough elements.");
            return f();
            IEnumerable<T> f() {
                for (; count > 0; count--) yield return s.Pop();
            }
        }
    }
}
