#nullable enable

#pragma warning disable RCS1164 // Unused type parameter.
#pragma warning disable RCS1163 // Unused parameter.
#pragma warning disable IDE0060 // Remove unused parameter
#pragma warning disable RCS1158 // Static member in generic type should use a type parameter.

// The comment above each method is what's being reported to the CodeLenseProvider
// as the "fully-qualified" name of the code-lensed method. Problem is they aren't
// fully qualified at all...
namespace Tests.TestData {
    public class Class {
        static Class() { }

        // Tests.TestData.Class+Class
        public Class() { }

        // Tests.TestData.Class+Class
        public Class(int x) { }

        public Class(string x) { }

        // Tests.TestData.Class.Method
        public void Method() { }

        // Tests.TestData.Class.StaticMethod
        public static void StaticMethod() { }

        // Tests.TestData.Class.GenericMethod
        public void GenericMethod<T>() { }

        // Tests.TestData.Class.GenericParamMethod
        public void GenericParamMethod<T>(T x) { }

        // Tests.TestData.Class.GenericReturnMethod
        public T GenericReturnMethod<T>() => default!;

        // Tests.TestData.Class.GenericParamAndReturnMethod
        public T GenericParamAndReturnMethod<T>(T x) => x;

        // Tests.TestData.Class.MethodWithOverload
        public void MethodWithOverload(int x) { }

        // Tests.TestData.Class.MethodWithOverload
        public void MethodWithOverload(string x) { }

        public void MethodWithOverload(String x) { }

        // Tests.TestData.Class.MethodWithGenericOverload
        public void MethodWithGenericOverload(int x) { }

        // Tests.TestData.Class.MethodWithGenericOverload
        public void MethodWithGenericOverload<T>(T x) { }

        public class NestedClass {
            // Tests.TestData.Class+NestedClass+NestedClass
            public NestedClass() { }

            // Tests.TestData.Class+NestedClass+NestedClass
            public NestedClass(int x) { }

            // Tests.TestData.Class+NestedClass.Method
            public void Method() { }

            public class NestedNestedClass {
                // Tests.TestData.Class+NestedClass+NestedNestedClass.Method
                public void Method() { }
            }
        }

        public class AmbiguousNestedClass {
            public void Foo() { }
        }
        public class AmbiguousNestedClass<T> {
            public void Bar() { }
        }
    }

    public class String { }

    public class GenericClass<T> {
        // Tests.TestData.GenericClass.Method
        public void Method() { }

        // Tests.TestData.GenericClass.StaticMethod
        public static void StaticMethod() { }

        // Tests.TestData.GenericClass.GenericMethod
        public TMethod GenericMethod<TMethod>(T x, TMethod y) => default!;
    }

    public class AmbiguousClass {
        public void Foo() { }
    }
    public class AmbiguousClass<T> {
        public void Bar() { }
    }
}

/* This is what Mono.Ceceil reports as "FullName" and "Name" (below):

TestAssembly.MainModule.Types.Where(t => t.Namespace == "Tests.TestData").SelectMany(t => t.NestedTypes.SelectMany(_t => _t.NestedTypes.Prepend(_t)).Prepend(t)).SelectMany(t => t.Methods).Select(m => m.FullName).ToArray()
{string[21]}
    [0]: "System.Void Tests.TestData.Class::.ctor()"
    [1]: "System.Void Tests.TestData.Class::.ctor(System.Int32)"
    [2]: "System.Void Tests.TestData.Class::Method()"
    [3]: "System.Void Tests.TestData.Class::StaticMethod()"
    [4]: "System.Void Tests.TestData.Class::GenericMethod()"
    [5]: "System.Void Tests.TestData.Class::GenericParamMethod(!!0)"
    [6]: "!!0 Tests.TestData.Class::GenericReturnMethod()"
    [7]: "!!0 Tests.TestData.Class::GenericParamAndReturnMethod(!!0)"
    [8]: "System.Void Tests.TestData.Class::MethodWithOverload(System.Int32)"
    [9]: "System.Void Tests.TestData.Class::MethodWithOverload(System.String)"
    [10]: "System.Void Tests.TestData.Class::MethodWithGenericOverload(System.Int32)"
    [11]: "System.Void Tests.TestData.Class::MethodWithGenericOverload(!!0)"
    [12]: "System.Void Tests.TestData.Class/NestedClass::.ctor()"
    [13]: "System.Void Tests.TestData.Class/NestedClass::.ctor(System.Int32)"
    [14]: "System.Void Tests.TestData.Class/NestedClass::Method()"
    [15]: "System.Void Tests.TestData.Class/NestedClass/NestedNestedClass::Method()"
    [16]: "System.Void Tests.TestData.Class/NestedClass/NestedNestedClass::.ctor()"
    [17]: "System.Void Tests.TestData.GenericClass`1::Method()"
    [18]: "System.Void Tests.TestData.GenericClass`1::StaticMethod()"
    [19]: "System.Void Tests.TestData.GenericClass`1::GenericMethod()"
    [20]: "System.Void Tests.TestData.GenericClass`1::.ctor()"

TestAssembly.MainModule.Types.Where(t => t.Namespace == "Tests.TestData").SelectMany(t => t.NestedTypes.SelectMany(_t => _t.NestedTypes.Prepend(_t)).Prepend(t)).SelectMany(t => t.Methods).Select(m => m.Name).ToArray()
{string[21]}
    [0]: ".ctor"
    [1]: ".ctor"
    [2]: "Method"
    [3]: "StaticMethod"
    [4]: "GenericMethod"
    [5]: "GenericParamMethod"
    [6]: "GenericReturnMethod"
    [7]: "GenericParamAndReturnMethod"
    [8]: "MethodWithOverload"
    [9]: "MethodWithOverload"
    [10]: "MethodWithGenericOverload"
    [11]: "MethodWithGenericOverload"
    [12]: ".ctor"
    [13]: ".ctor"
    [14]: "Method"
    [15]: "Method"
    [16]: ".ctor"
    [17]: "Method"
    [18]: "StaticMethod"
    [19]: "GenericMethod"
    [20]: ".ctor"
*/
