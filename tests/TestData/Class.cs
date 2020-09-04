#nullable enable

#pragma warning disable RCS1164 // Unused type parameter.
#pragma warning disable RCS1163 // Unused parameter.
#pragma warning disable IDE0060 // Remove unused parameter
#pragma warning disable RCS1158 // Static member in generic type should use a type parameter.

namespace Microscope.Tests.TestData {
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
}
