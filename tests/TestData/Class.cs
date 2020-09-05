﻿#nullable enable

#pragma warning disable RCS1164 // Unused type parameter.
#pragma warning disable RCS1163 // Unused parameter.
#pragma warning disable IDE0060 // Remove unused parameter
#pragma warning disable RCS1158 // Static member in generic type should use a type parameter.
#pragma warning disable RCS1102 // Make class static.

namespace Microscope.Tests.TestData {
    using System.Collections.Generic;

    public class Class {
        public void InstanceMethod() { }
        public static void StaticMethod() { }
        public void GenericInstanceMethod<T>() { }
        public static void GenericStaticMethod<T>() { }

        public void InstanceMethodWithParam(int x) { }
        public static void StaticMethodWithParam(bool x) { }
        public void GenericInstanceMethodWithParam<T>(T x) { }
        public static void GenericStaticMethodWithParam<T>(T x) { }

        public class NestedClass {
            public void Method() { }

            public class NestedNestedClass {
                public void Method() { }
            }
        }
    }

    public class GenericClass<T> {
        public void Method(T x) { }
        public void GenericMethod<U>(T x, U y) { }
    }

    public class NonPrimitiveParams {
        public void Method(List<int> x) { }
        public void Method(Dictionary<string, List<int>> x) { }
        public void Method(Name1.Space1.Foo x) { }
        public void Method(Name2.Space2.Foo x) { }
        public void Method(Foo<bool, char>.Bar<float, int> x) { }

        public class Foo<B, C> {
            public class Bar<F, I> { }
        }
    }

    public class Overloads {
        public void Method() { }
        public void Method(int x) { }
        public void Method(string x) { }
        public void Method<T>(T x) { }
        public void Method(List<int> x) { }
        public void Method<T>(List<T> x) { }
    }
}

namespace Name1.Space1 {
    public class Foo { }
}

namespace Name2.Space2 {
    public class Foo { }
}