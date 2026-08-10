using Kassyi.Generators.DependencyProperty.Models;
using Kassyi.Generators.Extensions;

namespace Kassyi.Generators.DependencyProperty.Sources;

internal static partial class SourceGenerationHelper
{
    public static string GenerateStaticConstructor(
        ClassData @class,
        IReadOnlyCollection<DependencyPropertyData> properties)
    {
        switch (@class.Framework)
        {
            case Framework.Avalonia:
            {
                var generatedAffects = properties
                    .Select(property => GenerateAvaloniaStaticConstructorAffects(@class, property))
                    .Inject();
                var generatedProperties = properties
                    .Where(static property => !property.IsAttached)
                    .Select(property => GenerateAvaloniaStaticConstructorPropertyChanged(@class, property))
                    .Inject();
                var generatedAttachedProperties = properties
                    .Where(static property => property.IsAttached)
                    .Select(property => GenerateAvaloniaStaticConstructorPropertyChanged(@class, property))
                    .Inject();
                if (string.IsNullOrWhiteSpace(generatedAffects) &&
                    string.IsNullOrWhiteSpace(generatedProperties) &&
                    string.IsNullOrWhiteSpace(generatedAttachedProperties))
                {
                    return string.Empty;
                }

                return $$"""
                         #nullable enable

                         namespace {{@class.Namespace}}
                         {
                             {{GenerateModifiers(@class)}}partial class {{@class.Name}}
                             {
                                 static {{@class.Name}}()
                                 {
                         {{generatedAffects}}
                         {{generatedProperties}}
                         {{generatedAttachedProperties}}
                                 }
                             }
                         }
                         """.RemoveBlankLinesWhereOnlyWhitespaces();
            }
            case Framework.Wpf:
            {
                var readOnlyProperties = properties
                    .Where(static property => property.IsReadOnly)
                    .Select(property => $"""

                                                     {property.Name}Property.OverrideMetadata(
                                                         forType: typeof({@class.Type}),
                                                         {GeneratePropertyMetadata(@class, property)},
                                                         key: {property.Name}PropertyKey);

                                         """).Inject();

                var readWriteProperties = properties
                    .Where(static property => !property.IsReadOnly)
                    .Select(property => $"""

                                                     {property.Name}Property.OverrideMetadata(
                                                         forType: typeof({@class.Type}),
                                                         {GeneratePropertyMetadata(@class, property)});

                                         """).Inject();

                var onChangedMethods = properties
                    .Select(property => GenerateOnChangedMethods(@class, property))
                    .Inject();

                return $$"""
                         #nullable enable

                         namespace {{@class.Namespace}}
                         {
                             {{GenerateModifiers(@class)}}partial class {{@class.Name}}
                             {
                                 static {{@class.Name}}()
                                 {
                         {{readOnlyProperties}}
                         {{readWriteProperties}}
                                 }

                         {{onChangedMethods}}
                             }
                         }
                         """.RemoveBlankLinesWhereOnlyWhitespaces();
            }
            default:
                return string.Empty;
        }
    }

    private static string GenerateAvaloniaStaticConstructorAffects(
        ClassData @class,
        DependencyPropertyData property)
    {
        return $"""

                            {(property.AffectsRender ? $"AffectsRender<{@class.Type}>({property.Name}Property);" : string.Empty)}
                            {(property.AffectsMeasure ? $"AffectsMeasure<{@class.Type}>({property.Name}Property);" : string.Empty)}
                            {(property.AffectsArrange ? $"AffectsArrange<{@class.Type}>({property.Name}Property);" : string.Empty)}

                """.RemoveBlankLinesWhereOnlyWhitespaces();
    }
    
    private static string GenerateAvaloniaStaticConstructorPropertyChanged(
        ClassData @class,
        DependencyPropertyData property)
    {
        var (name, isChanged0, isChanged1, isChanged2, isChanged3, isChangedArgs1, isChangedArgs2) = CheckOnChangedMethods(@class, property);
        return isChanged0 switch
        {
            false when !isChanged1 && !isChanged2 && !isChanged3 && !isChangedArgs1 && !isChangedArgs2 => string.Empty,
            _ => property.IsAttached
                ? $$"""

                                {{property.Name}}Property.Changed.Subscribe(new global::Avalonia.Reactive.AnonymousObserver<global::Avalonia.AvaloniaPropertyChangedEventArgs<{{GenerateType(property)}}>>(static x =>
                                {
                                    {{(isChanged0 ? $"{name}();" : "")}}
                                    {{(isChanged1 ? $"""
                                                     {name}(
                                                                         ({GenerateBrowsableForType(property)})x.Sender);
                                                     """ : "")}}
                                    {{(isChanged2 ? $"""
                                                     {name}(
                                                                         ({GenerateBrowsableForType(property)})x.Sender,
                                                                         ({GenerateType(property)})x.NewValue.GetValueOrDefault());
                                                     """ : "")}}
                                    {{(isChanged3 ? $"""
                                                     {name}(
                                                                         ({GenerateBrowsableForType(property)})x.Sender,
                                                                         ({GenerateType(property)})x.OldValue.GetValueOrDefault(),
                                                                         ({GenerateType(property)})x.NewValue.GetValueOrDefault());
                                                     """ : "")}}
                                    {{(isChangedArgs1 ? $"""
                                                         {name}(
                                                                             x);
                                                         """ : "")}}
                                    {{(isChangedArgs2 ? $"""
                                                         {name}(
                                                                             ({GenerateBrowsableForType(property)})x.Sender,
                                                                             x);
                                                         """ : "")}}
                                }));

                    """.RemoveBlankLinesWhereOnlyWhitespaces()
                : $$"""

                                {{property.Name}}Property.Changed.Subscribe(new global::Avalonia.Reactive.AnonymousObserver<global::Avalonia.AvaloniaPropertyChangedEventArgs<{{GenerateType(property)}}>>(static x =>
                                {
                                    {{(isChanged0 ? $"(({@class.Type})x.Sender).{name}();" : "")}}
                                    {{(isChanged1 ? $"""
                                                     (({@class.Type})x.Sender).{name}(
                                                                         ({GenerateType(property)})x.NewValue.GetValueOrDefault());
                                                     """ : "")}}
                                    {{(isChanged2 ? $"""
                                                     (({@class.Type})x.Sender).{name}(
                                                                         ({GenerateType(property)})x.OldValue.GetValueOrDefault(),
                                                                         ({GenerateType(property)})x.NewValue.GetValueOrDefault());
                                                     """ : "")}}
                                    {{(isChangedArgs1 ? $"""
                                                         (({@class.Type})x.Sender).{name}(
                                                                             x);
                                                         """ : "")}}
                                    {{(isChangedArgs2 ? $"""
                                                         {name}(
                                                                             (({@class.Type})x.Sender),
                                                                             x);
                                                         """ : "")}}
                                }));

                    """.RemoveBlankLinesWhereOnlyWhitespaces()
        };
    }
}

