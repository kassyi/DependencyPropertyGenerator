using Kassyi.Generators.DependencyProperty.Models;
using Kassyi.Generators.Extensions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Kassyi.Generators.DependencyProperty.Sources;

internal static partial class SourceGenerationHelper
{
    public static CompilationUnitSyntax GenerateRoutedEventSyntax(ClassData @class, EventData @event)
    {
        var writer = new SourceWriter();
        try
        {
            GenerateRoutedEvent(ref writer, @class, @event);
            return SyntaxFactory.ParseCompilationUnit(writer.ToString());
        }
        finally
        {
            writer.Dispose();
        }
    }

    public static void GenerateRoutedEvent(ref SourceWriter writer, ClassData @class, EventData @event)
    {
        var generator = Strategies.FrameworkGeneratorFactory.CreateRoutedEventStrategy(@class.Framework);
        
        switch (@event.IsAttached)
        {
            case true:
                generator.GenerateAttachedRoutedEvent(ref writer, @class, @event);
                return;
            default:
                generator.GenerateRoutedEvent(ref writer, @class, @event);
                return;
        }
    }
}

