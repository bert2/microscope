#nullable enable

namespace Microscope.Tests {
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class CollectCompilerGeneratedInstructionsExtTestData {
        public bool Lambda() => new int[0].Any(x => x > 0);

        public bool NestedLambda() => new int[0].Any(x => new int[0].Any(y => x > y));

        public IEnumerable<int> Iterator() {
            yield return 1;
        }

        public async Task AsyncMethod() => await Task.Delay(0);

        public int AnonymousType() {
            var x = new { Foo = "bar" };
            return x.Foo.Length;
        }

        public void LocalFunction() {
            Foo();

            static void Foo() { }
        }
    }
}
