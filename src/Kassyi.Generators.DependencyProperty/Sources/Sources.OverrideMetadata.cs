using Kassyi.Generators.DependencyProperty.Models;
using Kassyi.Generators.Extensions;

namespace Kassyi.Generators.DependencyProperty.Sources;

internal static partial class SourceGenerationHelper
{
    public static string GenerateRegisterPropertyChangedCallbacksMethod(
        ClassData @class,
        IReadOnlyCollection<DependencyPropertyData> overrideMetadata)
    {
        var registerCallbacks = overrideMetadata.Select(property =>
        {
            var senderType = property.IsAttached
                ? GenerateBrowsableForType(property)
                : @class.Type;

            var (name, isChanged0, isChanged1, isChanged2, isChanged3, _, _) = CheckOnChangedMethods(@class, property);
            if (!isChanged0 &&
                !isChanged1 &&
                !isChanged2 &&
                !isChanged3)
            {
                return " ";
            }

            var type = GenerateType(property);
            var changed0 = isChanged0 ? $"(({senderType})sender).{name}();" : string.Empty;
            var changed1 = isChanged1 ? $"""
                                             (({senderType})sender).{name}(
                                                                     ({type})sender.GetValue(dependencyProperty));
                             """ : string.Empty;
            var changed2 = isChanged2 ? $"""
                                             (({senderType})sender).{name}(
                                                                     ({type})sender.GetValue(dependencyProperty),
                                                                     ({type})sender.GetValue(dependencyProperty));
                             """ : string.Empty;

            return $$"""
                                 _ = this.RegisterPropertyChangedCallback(
                                     dp: {{property.Name}}Property,
                                     callback: static (sender, dependencyProperty) =>
                                     {
                                         {{changed0}}
                                         {{changed1}}
                                         {{changed2}}
                                     });

                     """;
        }).Inject();

        var onChangedMethods = overrideMetadata
            .Select(property => GenerateOnChangedMethods(@class, property))
            .Inject();

        return $$"""
                 #nullable enable

                 namespace {{@class.Namespace}}
                 {
                     {{GenerateModifiers(@class)}}partial class {{@class.Name}}
                     {
                         private void RegisterPropertyChangedCallbacks()
                         {
                 {{registerCallbacks}}
                         }

                 {{onChangedMethods}}
                     }
                 }
                 """.RemoveBlankLinesWhereOnlyWhitespaces();
    }
}

