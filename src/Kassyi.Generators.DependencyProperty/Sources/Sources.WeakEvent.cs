using Kassyi.Generators.DependencyProperty.Models;
using Kassyi.Generators.Extensions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Kassyi.Generators.DependencyProperty.Sources;

internal static partial class SourceGenerationHelper
{
    public static CompilationUnitSyntax GenerateWeakEventSyntax(ClassData @class, EventData @event)
    {
        var writer = new SourceWriter();
        try
        {
            GenerateWeakEvent(ref writer, @class, @event);
            return SyntaxFactory.ParseCompilationUnit(writer.ToString());
        }
        finally
        {
            writer.Dispose();
        }
    }

    public static void GenerateWeakEvent(ref SourceWriter writer, ClassData @class, EventData @event)
    {
        var generator = Strategies.FrameworkGeneratorFactory.CreateWeakEventStrategy(@class.Framework);
        generator.GenerateWeakEvent(ref writer, @class, @event);
    }
}

