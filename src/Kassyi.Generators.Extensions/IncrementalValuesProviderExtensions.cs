using System.Diagnostics.CodeAnalysis;
using Kassyi.Generators.Extensions.Models;
using Microsoft.CodeAnalysis;

namespace Kassyi.Generators.Extensions;

/// <summary>Provides extension methods for incremental source generator value providers.</summary>
[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Library extension methods for Source Generators")]
[SuppressMessage("ReSharper", "UnusedMethod.Global", Justification = "Library extension methods for Source Generators")]
[SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Library extension methods for Source Generators")]
public static class IncrementalValuesProviderExtensions
{
    /// <summary>Registers source file outputs with the generator initialization context.</summary>
    public static void AddSource(
        this IncrementalValuesProvider<FileWithName> source,
        IncrementalGeneratorInitializationContext context)
    {
        context.RegisterSourceOutput(source, static (context, file) =>
        {
            if (file.IsEmpty)
            {
                return;
            }

            context.AddSource(
                hintName: file.Name,
                source: file.Text);
        });
    }

    /// <summary>Registers source file outputs with the generator initialization context.</summary>
    public static void AddSource(
        this IncrementalValueProvider<FileWithName> source,
        IncrementalGeneratorInitializationContext context)
    {
        context.RegisterSourceOutput(source, static (context, file) =>
        {
            if (file.IsEmpty)
            {
                return;
            }

            context.AddSource(
                hintName: file.Name,
                source: file.Text);
        });
    }

    /// <summary>Registers multiple source file outputs with the generator initialization context.</summary>
    public static void AddSource(
        this IncrementalValueProvider<EquatableArray<FileWithName>> source,
        IncrementalGeneratorInitializationContext context)
    {
        source
            .SelectMany(static (x, _) => x)
            .AddSource(context);
    }

    /// <summary>Registers multiple source file outputs with the generator initialization context.</summary>
    public static void AddSource(
        this IncrementalValuesProvider<EquatableArray<FileWithName>> source,
        IncrementalGeneratorInitializationContext context)
    {
        source
            .SelectMany(static (x, _) => x)
            .AddSource(context);
    }

    /// <summary>Collects all provider values into an equatable array to ensure incremental caching stability.</summary>
    public static IncrementalValueProvider<EquatableArray<TSource>> CollectAsEquatableArray<TSource>(
        this IncrementalValuesProvider<TSource> source)
        where TSource : IEquatable<TSource>
    {
        return source
            .Collect()
            .Select(static (x, _) => x.AsEquatableArray());
    }

    /// <summary>Transforms values while capturing and reporting unhandled exceptions as compiler diagnostics.</summary>
    public static IncrementalValueProvider<TResult> SelectAndReportExceptions<TSource, TResult>(
        this IncrementalValueProvider<TSource> source,
        Func<TSource, CancellationToken, TResult> selector,
        IncrementalGeneratorInitializationContext initializationContext,
        string id)
    {
        var outputWithErrors = source
            .Select<TSource, (TResult? Value, Exception? Exception)>((value, cancellationToken) =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                try
                {
                    return (Value: selector(value, cancellationToken), Exception: null);
                }
                catch (Exception exception)
                {
                    return (Value: default, Exception: exception);
                }
            });

        initializationContext.RegisterSourceOutput(outputWithErrors,
            (context, tuple) =>
            {
                if (tuple.Exception == null)
                {
                    return;
                }

                context.ReportException(id: id, exception: tuple.Exception);
            });

        return outputWithErrors
            .Select(static (x, _) => x.Value!);
    }

    /// <summary>Emits contained diagnostics and returns valid non-null values.</summary>
    public static IncrementalValuesProvider<T> SelectAndReportDiagnostics<T>(
        this IncrementalValuesProvider<ResultWithDiagnostics<T?>> source,
        IncrementalGeneratorInitializationContext initializationContext)
    {
        initializationContext.RegisterSourceOutput(
            source.SelectMany(static (x, _) => x.Diagnostics),
            static (context, diagnostic) => context.ReportDiagnostic(diagnostic));

        return source
            .Where(static x => x.Result is not null)
            .Select(static (x, _) => x.Result!);
    }

    /// <summary>Emits contained diagnostics and returns the result value.</summary>
    public static IncrementalValueProvider<T?> SelectAndReportDiagnostics<T>(
        this IncrementalValueProvider<ResultWithDiagnostics<T?>> source,
        IncrementalGeneratorInitializationContext initializationContext)
    {
        initializationContext.RegisterSourceOutput(
            source.SelectMany(static (x, _) => x.Diagnostics),
            static (context, diagnostic) => context.ReportDiagnostic(diagnostic));

        return source
            .Select(static (x, _) => x.Result);
    }

    /// <summary>Transforms values while capturing and reporting unhandled exceptions as compiler diagnostics.</summary>
    public static IncrementalValuesProvider<TResult> SelectAndReportExceptions<TSource, TResult>(
        this IncrementalValuesProvider<TSource> source,
        Func<TSource, CancellationToken, TResult> selector,
        IncrementalGeneratorInitializationContext initializationContext,
        string id)
    {
        var outputWithErrors = source
            .Select<TSource, (TResult? Value, Exception? Exception)>((value, cancellationToken) =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                try
                {
                    return (Value: selector(value, cancellationToken), Exception: null);
                }
                catch (Exception exception)
                {
                    return (Value: default, Exception: exception);
                }
            });

        initializationContext.RegisterSourceOutput(outputWithErrors
                .Where(static x => x.Exception is not null),
            (context, tuple) => { context.ReportException(id: id, exception: tuple.Exception!); });

        return outputWithErrors
            .Where(static x => x.Exception is null)
            .Select(static (x, _) => x.Value!);
    }

    /// <summary>Transforms values while capturing unhandled exceptions without reporting them.</summary>
    public static IncrementalValuesProvider<TResult> SelectAndCatchExceptions<TSource, TResult>(
        this IncrementalValuesProvider<TSource> source,
        Func<TSource, TResult> selector)
    {
        return source
            .Select<TSource, (TResult? Value, Exception? Exception)>((value, cancellationToken) =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                try
                {
                    return (Value: selector(value), Exception: null);
                }
                catch (Exception exception)
                {
                    return (Value: default, Exception: exception);
                }
            })
            .Where(static x => x.Exception is null)
            .Select(static (x, _) => x.Value!);
    }

    /// <summary>Transforms values while capturing and reporting unhandled exceptions as compiler diagnostics.</summary>
    public static IncrementalValuesProvider<TResult> SelectAndReportExceptions<TSource, TResult>(
        this IncrementalValuesProvider<TSource> source,
        Func<TSource, TResult> selector,
        IncrementalGeneratorInitializationContext initializationContext,
        string id)
    {
        return source
            .SelectAndReportExceptions((x, _) => selector(x), initializationContext, id);
    }

    /// <summary>Transforms values while capturing and reporting unhandled exceptions as compiler diagnostics.</summary>
    public static IncrementalValueProvider<TResult> SelectAndReportExceptions<TSource, TResult>(
        this IncrementalValueProvider<TSource> source,
        Func<TSource, TResult> selector,
        IncrementalGeneratorInitializationContext initializationContext,
        string id)
    {
        return source
            .SelectAndReportExceptions((x, _) => selector(x), initializationContext, id);
    }

    /// <summary>Transforms paired framework and left values while capturing and reporting unhandled exceptions.</summary>
    public static IncrementalValuesProvider<TResult> SelectAndReportExceptions<TResult, TLeft>(
        this IncrementalValuesProvider<(TLeft Left, Framework Right)> source,
        Func<Framework, TLeft, TResult> selector,
        IncrementalGeneratorInitializationContext context,
        string id)
    {
        return source
            .SelectAndReportExceptions(x => selector(x.Right, x.Left), context, id);
    }

    /// <summary>Filters out null values from the provider pipeline.</summary>
    public static IncrementalValuesProvider<TSource> WhereNotNull<TSource>(
        this IncrementalValuesProvider<TSource?> source)
        where TSource : struct
    {
        return source
            .Where(static x => x is not null)
            .Select(static (x, _) => x!.Value);
    }

    /// <summary>Detects the target UI framework from compilation symbols and configuration, reporting an error diagnostic if unrecognized.</summary>
    public static IncrementalValueProvider<Framework> DetectFramework(
        this IncrementalGeneratorInitializationContext context,
        DiagnosticDescriptor? frameworkNotRecognizedDescriptor = null)
    {
        var frameworkWithDiagnostic = context.CompilationProvider
            .Combine(context.AnalyzerConfigOptionsProvider)
            .Select((tuple, _) =>
            {
                var (compilation, options) = tuple;
                var framework = compilation.TryRecognizeFramework(options);

                var diagnostic = framework == Framework.None && frameworkNotRecognizedDescriptor != null
                    ? Diagnostic.Create(frameworkNotRecognizedDescriptor, Location.None)
                    : null;

                return (Framework: framework, Diagnostic: diagnostic);
            });

        context.RegisterSourceOutput(
            frameworkWithDiagnostic,
            static (sourceProductionContext, tuple) =>
            {
                if (tuple.Diagnostic == null)
                {
                    return;
                }

                sourceProductionContext.ReportDiagnostic(tuple.Diagnostic);
            });

        return frameworkWithDiagnostic
            .Select(static (x, _) => x.Framework);
    }
}

