#nullable enable

namespace Microscope.Tests.Util {
    using System;

    using FluentAssertions;
    using FluentAssertions.Primitives;

    using Mono.Cecil;

    public static class FluentAssertionsExt {
        public static AndConstraint<ObjectAssertions> Satisfy(
            this ObjectAssertions parent,
            Action<MethodDefinition> inspector) {
            inspector((MethodDefinition)parent.Subject);
            return new AndConstraint<ObjectAssertions>(parent);
        }
    }
}
