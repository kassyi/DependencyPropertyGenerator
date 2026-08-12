using Kassyi.Generators.DependencyProperty.Models;
using Kassyi.Generators.Extensions;

namespace Kassyi.Generators.DependencyProperty.Sources;

internal static partial class SourceGenerationHelper
{
    public static void GenerateStaticConstructor(
        ref SourceWriter writer,
        ClassData @class,
        IReadOnlyCollection<DependencyPropertyData> properties)
    {
        switch (@class.Framework)
        {
            case Framework.Avalonia:
            {
                var tempWriter = new SourceWriter();
                try
                {
                    foreach (var property in properties)
                    {
                        GenerateAvaloniaStaticConstructorAffects(ref tempWriter, @class, property);
                    }
                    foreach (var property in properties.Where(static p => !p.IsAttached))
                    {
                        GenerateAvaloniaStaticConstructorPropertyChanged(ref tempWriter, @class, property);
                    }
                    foreach (var property in properties.Where(static p => p.IsAttached))
                    {
                        GenerateAvaloniaStaticConstructorPropertyChanged(ref tempWriter, @class, property);
                    }

                    if (tempWriter.Length == 0)
                    {
                        return;
                    }

                    writer.AppendLine();
                    writer.AppendLine("#nullable enable");
                    writer.AppendLine();
                    writer.AppendLine($"namespace {@class.Namespace}");
                    writer.AppendLine("{");
                    writer.AppendLine($"    {GenerateModifiers(@class)}partial class {@class.Name}");
                    writer.AppendLine("    {");
                    writer.AppendLine($"        static {@class.Name}()");
                    writer.AppendLine("        {");
                    
                    writer.Append(tempWriter.ToString());

                    writer.AppendLine("        }");
                }
                finally
                {
                    tempWriter.Dispose();
                }
                writer.AppendLine("    }");
                writer.AppendLine("}");
                break;
            }
            case Framework.Wpf:
            {
                writer.AppendLine();
                writer.AppendLine("#nullable enable");
                writer.AppendLine();
                writer.AppendLine($"namespace {@class.Namespace}");
                writer.AppendLine("{");
                writer.AppendLine($"    {GenerateModifiers(@class)}partial class {@class.Name}");
                writer.AppendLine("    {");
                writer.AppendLine($"        static {@class.Name}()");
                writer.AppendLine("        {");

                foreach (var property in properties)
                {
                    if (property.IsReadOnly)
                    {
                        writer.AppendLine($"            {property.Name}Property.OverrideMetadata(");
                        writer.AppendLine($"                forType: typeof({@class.Type}),");
                        writer.AppendLine($"                {GeneratePropertyMetadata(@class, property)},");
                        writer.AppendLine($"                key: {property.Name}PropertyKey);");
                        writer.AppendLine();
                    }
                    else
                    {
                        writer.AppendLine($"            {property.Name}Property.OverrideMetadata(");
                        writer.AppendLine($"                forType: typeof({@class.Type}),");
                        writer.AppendLine($"                {GeneratePropertyMetadata(@class, property)});");
                        writer.AppendLine();
                    }
                }
                
                writer.AppendLine("        }");
                writer.AppendLine();

                foreach (var property in properties)
                {
                    GenerateOnChangedMethods(ref writer, @class, property);
                }

                writer.AppendLine("    }");
                writer.AppendLine("}");
                break;
            }
        }
    }

    private static void GenerateAvaloniaStaticConstructorAffects(
        ref SourceWriter writer,
        ClassData @class,
        DependencyPropertyData property)
    {
        writer.LineIf(property.AffectsRender, $"            AffectsRender<{@class.Type}>({property.Name}Property);");
        writer.LineIf(property.AffectsMeasure, $"            AffectsMeasure<{@class.Type}>({property.Name}Property);");
        writer.LineIf(property.AffectsArrange, $"            AffectsArrange<{@class.Type}>({property.Name}Property);");
    }

    private static void GenerateAvaloniaStaticConstructorPropertyChanged(
        ref SourceWriter writer,
        ClassData @class,
        DependencyPropertyData property)
    {
        var (name, isChanged0, isChanged1, isChanged2, isChanged3, isChangedArgs1, isChangedArgs2) = CheckOnChangedMethods(@class, property);
        
        if (!isChanged0 && !isChanged1 && !isChanged2 && !isChanged3 && !isChangedArgs1 && !isChangedArgs2)
        {
            return;
        }
        
        writer.AppendLine($"            {property.Name}Property.Changed.Subscribe(new global::Avalonia.Reactive.AnonymousObserver<global::Avalonia.AvaloniaPropertyChangedEventArgs<{GenerateType(property)}>>(static x =>");
        writer.AppendLine("            {");
        
        if (property.IsAttached)
        {
            writer.LineIf(isChanged0, $"                {name}();");
            writer.LineIf(isChanged1, $"""
                                {name}(
                                    ({GenerateBrowsableForType(property)})x.Sender);
                """);
            writer.LineIf(isChanged2, $"""
                                {name}(
                                    ({GenerateBrowsableForType(property)})x.Sender,
                                    ({GenerateType(property)})x.NewValue.GetValueOrDefault());
                """);
            writer.LineIf(isChanged3, $"""
                                {name}(
                                    ({GenerateBrowsableForType(property)})x.Sender,
                                    ({GenerateType(property)})x.OldValue.GetValueOrDefault(),
                                    ({GenerateType(property)})x.NewValue.GetValueOrDefault());
                """);
            writer.LineIf(isChangedArgs1, $"""
                                {name}(
                                    x);
                """);
            writer.LineIf(isChangedArgs2, $"""
                                {name}(
                                    ({GenerateBrowsableForType(property)})x.Sender,
                                    x);
                """);
        }
        else
        {
            writer.LineIf(isChanged0, $"                (({@class.Type})x.Sender).{name}();");
            writer.LineIf(isChanged1, $"""
                                (({@class.Type})x.Sender).{name}(
                                    ({GenerateType(property)})x.NewValue.GetValueOrDefault());
                """);
            writer.LineIf(isChanged2, $"""
                                (({@class.Type})x.Sender).{name}(
                                    ({GenerateType(property)})x.OldValue.GetValueOrDefault(),
                                    ({GenerateType(property)})x.NewValue.GetValueOrDefault());
                """);
            writer.LineIf(isChangedArgs1, $"""
                                (({@class.Type})x.Sender).{name}(
                                    x);
                """);
            writer.LineIf(isChangedArgs2, $"""
                                {name}(
                                    (({@class.Type})x.Sender),
                                    x);
                """);
        }

        writer.AppendLine("            }));");
    }
}

