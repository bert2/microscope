#nullable enable

namespace Microscope.Tests {
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class CollectGeneratedCodeExtTestData {
        public bool Lambda() => new int[0].Any(x => x > 0);

        public bool Closure(int x) => new int[0].Any(y => y > x);

        public bool ClosureNestedInLambda() => new int[0].Any(x => new int[0].Any(y => y > x));

        public bool NestedLambda() => new int[0].Any(x => new int[0].Any(y => x > y));

        public bool GenericLambda<T>() => new string[0].Any(s => s == typeof(T).FullName);

        public IEnumerable<int> Iterator() {
            yield return 1;
        }

        public async Task AsyncMethod() => await Task.Delay(0);

        public async Task AsyncMethodWithLambda() {
            await Task.Delay(0);
            _ = new int[0].Any(x => x > 0);
        }

        public void LocalFunction() {
            Foo();

            static void Foo() { }
        }

        public void LocalFunctionCalledTwice() {
            Foo();
            Foo();

            static void Foo() { }
        }

        public int MyProperty { get; set; }
        public int AutoProperty() {
            MyProperty = 1;
            return MyProperty;
        }
    }
}
