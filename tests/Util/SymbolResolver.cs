#nullable enable

namespace Microscope.Tests.Util {
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;

    using Microscope.Shared;

    using Microsoft.Build.Locator;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.MSBuild;

    /// <summary>
    /// In order to test that `GetMethodDefinitionExt.GetMethodDefinition(this AssemblyDefinition, IMethodSymbol)`
    /// can resolve methods with ambiguous names in an `AssemblyDefinition` we need a reliable way to get an
    /// `IMethodSymbol` for a given method from the `TestData` namespace.
    /// One way would be to compile the test project in memory and then look up the method symbol by name. This
    /// has the major disadvantage that we'd have to implement the logic to resolve ambiguous method names again.
    /// Instead we are using a special method `Get()` that can locate its own call in the syntax tree and in turn
    /// analyze the syntax of the arguments passed to it. Hence the only purpose of the lambda passed to `Get()`
    /// is to reference the method which we want the `IMethodSymbol` for.
    /// </summary>
    public class SymbolResolver : IDisposable {
        private const string get = nameof(SymbolResolver) + "." + nameof(Get) + "()";
        private readonly MSBuildWorkspace workspace;
        private readonly SyntaxTree syntaxTree;
        private readonly SemanticModel semanticModel;

        static SymbolResolver() {
            var latest = MSBuildLocator
                .QueryVisualStudioInstances()
                .OrderByDescending(vs => vs.Version)
                .First();
            MSBuildLocator.RegisterInstance(latest);
        }

        public SymbolResolver(MSBuildWorkspace workspace, Compilation compilation, SyntaxTree syntaxTree) {
            this.workspace = workspace;
            this.syntaxTree = syntaxTree;
            semanticModel = compilation.GetSemanticModel(syntaxTree);
        }

        public void Dispose() => workspace.Dispose();

        public static async Task<SymbolResolver> ForMyTests(CancellationToken ct, [CallerFilePath] string? testClassFile = null) {
            var workspace = MSBuildWorkspace.Create();
            workspace.WorkspaceFailed += (_, e) => {
                if (e.Diagnostic.Kind == WorkspaceDiagnosticKind.Failure)
                    throw new InvalidOperationException(e.Diagnostic.ToString());
            };

            var testProjPath = Path.Combine(Path.GetDirectoryName(testClassFile), "Tests.csproj");
            var testProj = await workspace.OpenProjectAsync(testProjPath, progress: null, ct).Caf();

            var testClassDocId = testProj.Solution.GetDocumentIdsWithFilePath(testClassFile).SingleOrDefault()
                ?? throw new InvalidOperationException($"Could not find file {testClassFile} in project {testProj.Name}.");
            var testClassDoc = testProj.GetDocument(testClassDocId)
                ?? throw new InvalidOperationException($"Could not find document with id {testClassDocId} in project {testProj.Name}.");
            var syntaxTree = await testClassDoc.GetSyntaxTreeAsync(ct).Caf()
                ?? throw new InvalidOperationException($"Document {testClassDoc.Name} does not have a syntax tree.");

            var compilation = await testProj.GetCompilationAsync(ct).Caf()
                ?? throw new InvalidOperationException($"Project {testProj.Name} does not support compilation.");

            return new SymbolResolver(workspace, compilation, syntaxTree);
        }

        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Param `selector` needed indirectly.")]
        public IMethodSymbol Get<T>(Action<T> selector, [CallerLineNumber] int line = 0)
            => ResolveReferencedMethodAt(line);

        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Param `selector` needed indirectly.")]
        public IMethodSymbol Get(Action selector, [CallerLineNumber] int line = 0)
            => ResolveReferencedMethodAt(line);

        private IMethodSymbol ResolveReferencedMethodAt(int line) {
            var span = syntaxTree.GetText().Lines[line - 1].Span;
            var node = syntaxTree.GetRoot().FindNode(span);

            var symbolResolverGetCall = node
                .DescendantNodes()
                .OfType<InvocationExpressionSyntax>()
                .SingleOrDefault(call
                    => call.Expression is MemberAccessExpressionSyntax ma
                    && ma.Name.Identifier.ValueText == nameof(Get)
                    && semanticModel.GetSymbolInfo(ma).Symbol?.ContainingType.Name == nameof(SymbolResolver))
                ?? throw new InvalidOperationException($"Could not find call to {get} at line {line} (node: {node}).");

            var args = symbolResolverGetCall.ArgumentList.Arguments;

            if (args.Count > 1)
                throw new ArgumentException($"Don't pass argument '{nameof(line)}' to {get}.");

            var invocation = args[0].Expression switch {
                MemberAccessExpressionSyntax ma => ma,          // when using method group: `Get(Foo.Bar)`
                LambdaExpressionSyntax l => GetMemberAccess(l), // when using lambda: `Get<Foo>(foo => foo.Bar())`
                _ => throw new InvalidOperationException($"Unexpected argument expression {args}.")
            };

            var symbol = semanticModel.GetSymbolInfo(invocation).Symbol
                ?? throw new InvalidOperationException($"Failed to get symbol for {invocation}.");

            return (IMethodSymbol)symbol;
        }

        private MemberAccessExpressionSyntax GetMemberAccess(LambdaExpressionSyntax l)
            => l.ExpressionBody is null
                ? throw new ArgumentException($"Don't pass a statement block to {get}. Use an expression body instead.")
                : !(l.ExpressionBody is InvocationExpressionSyntax inv)
                ? throw new ArgumentException($"Lambda passed to {get} must be a method call.")
                : !(inv.Expression is MemberAccessExpressionSyntax ma)
                ? throw new ArgumentException($"Lambda passed to {get} must be a call of a class method.")
                : ma;
    }
}
