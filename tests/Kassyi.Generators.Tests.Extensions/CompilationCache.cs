using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Kassyi.Generators.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Testing;

namespace Kassyi.Generators.Tests.Extensions;

public static class CompilationCache
{
    private static readonly ConcurrentDictionary<string, AsyncLazy<Compilation>> _baseCompilations = new();

    public static async Task<Compilation> GetBaseCompilationAsync(Framework framework, CancellationToken cancellationToken = default)
    {
        var netVersion = GetDefaultNetVersion(framework);
        var cacheKey = $"{framework}_{netVersion}";

        var lazyCompilation = _baseCompilations.GetOrAdd(cacheKey, _ => new AsyncLazy<Compilation>(async () =>
        {
            ReferenceAssemblies referenceAssemblies = framework switch
            {
                Framework.None => ReferenceAssemblies.Net.Net80,
                Framework.Wpf => ReferenceAssemblies.NetFramework.Net48.Wpf,
                _ => ReferenceAssembliesFactory.Get(framework, netVersion)
            };
            
            var references = await referenceAssemblies.ResolveAsync(null, cancellationToken).ConfigureAwait(false);

            return CSharpCompilation.Create(
                assemblyName: "Tests",
                syntaxTrees: Array.Empty<SyntaxTree>(),
                references: references,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
        }));

        return await lazyCompilation.GetValueAsync().ConfigureAwait(false);
    }

    private static string GetDefaultNetVersion(Framework framework) => framework switch
    {
        Framework.Avalonia => "net6.0",
        Framework.Maui => "net7.0",
        _ => "net8.0"
    };

    // A simple thread-safe lazy wrapper for async initialization
    private class AsyncLazy<T>
    {
        private readonly Lazy<Task<T>> _lazy;

        public AsyncLazy(Func<Task<T>> valueFactory)
        {
            _lazy = new Lazy<Task<T>>(() => Task.Run(valueFactory));
        }

        public Task<T> GetValueAsync() => _lazy.Value;
    }
}
