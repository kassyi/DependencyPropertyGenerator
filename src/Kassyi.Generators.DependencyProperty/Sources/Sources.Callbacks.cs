using Kassyi.Generators.DependencyProperty.Models;
using Kassyi.Generators.Extensions;

namespace Kassyi.Generators.DependencyProperty.Sources;

internal static partial class SourceGenerationHelper
{

    private static string GenerateOnChangedMethods(ClassData @class, DependencyPropertyData property)
    {
        if (!string.IsNullOrWhiteSpace(property.OnChanged))
        {
            var (_, isChanged0, isChanged1, isChanged2, isChanged3, isChangedArgs1, isChangedArgs2) = CheckOnChangedMethods(@class, property);
            if (!isChanged0 && !isChanged1 && !isChanged2 && !isChanged3 && !isChangedArgs1 && !isChangedArgs2)
            {
                return $"""
                    #error DPG0001: The specified OnChanged method '{property.OnChanged}' was not found or has an unsupported signature on '{@class.FullName}'.
                    """;
            }

            return " ";
        }

        var attr = GenerateGeneratedCodeAttribute(property.Version);
        var type = GenerateType(property);
        var browsable = GenerateBrowsableForType(property);
        var browsableName = GenerateBrowsableForTypeParameterName(property);
        var name = property.Name;

        return property.IsAttached
            ? $$"""
                
               {{attr}}
                       static partial void On{{name}}Changed();
               {{attr}}
                       static partial void On{{name}}Changed({{browsable}} {{browsableName}});
               {{attr}}
                       static partial void On{{name}}Changed({{browsable}} {{browsableName}}, {{type}} newValue);
               {{attr}}
                       static partial void On{{name}}Changed({{browsable}} {{browsableName}}, {{type}} oldValue, {{type}} newValue);
               """
            : $$"""
                
               {{attr}}
                       partial void On{{name}}Changed();
               {{attr}}
                       partial void On{{name}}Changed({{type}} newValue);
               {{attr}}
                       partial void On{{name}}Changed({{type}} oldValue, {{type}} newValue);
               """;
    }

    private static string GenerateOnChangingMethods(DependencyPropertyData property)
    {
        if (property.Framework != Framework.Maui)
        {
            return " ";
        }

        var attr = GenerateGeneratedCodeAttribute(property.Version);
        var type = GenerateType(property);
        var browsable = GenerateBrowsableForType(property);
        var browsableName = GenerateBrowsableForTypeParameterName(property);
        var name = property.Name;

        return property.IsAttached
            ? $$"""
                
               {{attr}}
                       static partial void On{{name}}Changing();
               {{attr}}
                       static partial void On{{name}}Changing({{browsable}} {{browsableName}});
               {{attr}}
                       static partial void On{{name}}Changing({{browsable}} {{browsableName}}, {{type}} newValue);
               {{attr}}
                       static partial void On{{name}}Changing({{browsable}} {{browsableName}}, {{type}} oldValue, {{type}} newValue);
               """
            : $$"""
                
               {{attr}}
                       partial void On{{name}}Changing();
               {{attr}}
                       partial void On{{name}}Changing({{type}} newValue);
               {{attr}}
                       partial void On{{name}}Changing({{type}} oldValue, {{type}} newValue);
               """;
    }
    
    private static string GenerateValidateValueCallback(ClassData @class, DependencyPropertyData property)
    {
        if (!property.Validate)
        {
            return "null";
        }

        if (property.Framework == Framework.Maui)
        {
            var senderType = property.IsAttached
                ? GenerateBrowsableForType(property)
                : @class.Type;
            
            return $"""
                    static (sender, value) =>
                                        Is{property.Name}Valid(
                                            ({senderType})sender,
                                            ({GenerateType(property, canBeNull: true)})value)
                    """;
        }
        
        return $"""
                static value =>
                                    Is{property.Name}Valid(
                                        ({GenerateType(property, canBeNull: true)})value)
                """;
    }

    private static string GenerateCreateDefaultValueCallbackValueCallback(DependencyPropertyData property)
    {
        if (!property.CreateDefaultValueCallback)
        {
            return "null";
        }

        if (property.Framework == Framework.Maui)
        {
            return "static _ => Get" + property.Name + "DefaultValue()";
        }

        return "static () => Get" + property.Name + "DefaultValue()";
    }
    
    private static string GenerateCoerceValueCallback(ClassData @class, DependencyPropertyData property)
    {
        if (!property.Coerce)
        {
            return "null";
        }

        var senderType = property.IsAttached
            ? GenerateBrowsableForType(property)
            : @class.Type;

        if (property.Framework == Framework.Maui)
        {
            return property.IsAttached
                ? $"""
                   static (sender, value) =>
                                           Coerce{property.Name}(
                                               ({senderType})sender,
                                               ({GenerateType(property, canBeNull: true)})value)
                   """
                : $"""
                   static (sender, value) =>
                                           (({senderType})sender).Coerce{property.Name}(
                                               ({GenerateType(property, canBeNull: true)})value)
                   """;
        }

        return property.IsAttached
            ? $"""
               static (sender, args) =>
                                       Coerce{property.Name}(
                                           ({senderType})sender,
                                           ({GenerateType(property, canBeNull: true)})args.Value)
               """
            : $"""
               static (sender, value) =>
                                       (({senderType})sender).Coerce{property.Name}(
                                           ({GenerateType(property, canBeNull: true)})value)
               """;
    }
    
    private static string GeneratePropertyChangedCallback(ClassData @class, DependencyPropertyData property)
    {
        var (name, isChanged0, isChanged1, isChanged2, isChanged3, isChangedArgs1, isChangedArgs2) = CheckOnChangedMethods(@class, property);
        if (!isChanged0 &&
            !isChanged1 &&
            !isChanged2 &&
            !isChanged3 &&
            !isChangedArgs1 &&
            !isChangedArgs2)
        {
            return "null";
        }

        var senderType = property.IsAttached
            ? GenerateBrowsableForType(property)
            : @class.Type;
        if (property.Framework == Framework.Maui)
        {
            return property.IsAttached
                ? $$"""
                    static (sender, oldValue, newValue) =>
                                    {
                                        {{(isChanged0 ? @$"{name}();" : "")}}
                                        {{(isChanged1 ? $"""
                                                         {name}(
                                                                                 ({senderType})sender);
                                                         """ : "")}}
                                        {{(isChanged2 ? $"""
                                                         {name}(
                                                                                 ({senderType})sender,
                                                                                 ({GenerateType(property)})newValue);
                                                         """ : "")}}
                                        {{(isChanged3 ? $"""
                                                         {name}(
                                                                                 ({senderType})sender,
                                                                                 ({GenerateType(property)})oldValue,
                                                                                 ({GenerateType(property)})newValue);
                                                         """ : "")}}
                                    }
                    """
                : $$"""
                    static (sender, oldValue, newValue) =>
                                    {
                                        {{(isChanged0 ? @$"(({senderType})sender).{name}();" : "")}}
                                        {{(isChanged1 ? $"""
                                                         (({senderType})sender).{name}(
                                                                                 ({GenerateType(property)})newValue);
                                                         """ : "")}}
                                        {{(isChanged2 ? $"""
                                                         (({senderType})sender).{name}(
                                                                                 ({GenerateType(property)})oldValue,
                                                                                 ({GenerateType(property)})newValue);
                                                         """ : "")}}
                                    }
                    """;
        }

        return property.IsAttached
            ? $$"""
                static (sender, args) =>
                                    {
                                        {{(isChanged0 ? @$"{name}();" : "")}}
                                        {{(isChanged1 ? $"""
                                                         {name}(
                                                                                     ({senderType})sender);
                                                         """ : "")}}
                                        {{(isChanged2 ? $"""
                                                         {name}(
                                                                                     ({senderType})sender,
                                                                                     ({GenerateType(property)})args.NewValue);
                                                         """ : "")}}
                                        {{(isChanged3 ? $"""
                                                         {name}(
                                                                                     ({senderType})sender,
                                                                                     ({GenerateType(property)})args.OldValue,
                                                                                     ({GenerateType(property)})args.NewValue);
                                                         """ : "")}}
                                        {{(isChangedArgs1 ? $"""
                                                             {name}(
                                                                                         args);
                                                             """ : "")}}
                                        {{(isChangedArgs2 ? $"""
                                                             {name}(
                                                                                         ({senderType})sender,
                                                                                         args);
                                                             """ : "")}}
                                    }
                """
            : $$"""
                static (sender, args) =>
                                    {
                                        {{(isChanged0 ? @$"(({senderType})sender).{name}();" : "")}}
                                        {{(isChanged1 ? $"""
                                                         (({senderType})sender).{name}(
                                                                                     ({GenerateType(property)})args.NewValue);
                                                         """ : "")}}
                                        {{(isChanged2 ? $"""
                                                         (({senderType})sender).{name}(
                                                                                     ({GenerateType(property)})args.OldValue,
                                                                                     ({GenerateType(property)})args.NewValue);
                                                         """ : "")}}
                                        {{(isChangedArgs1 ? $"""
                                                             (({senderType})sender).{name}(
                                                                                         args);
                                                             """ : "")}}
                                        {{(isChangedArgs2 ? $"""
                                                             {name}(
                                                                                         (({senderType})sender),
                                                                                         args);
                                                             """ : "")}}
                                    }
                """;
    }

    private static (string Name, bool IsChanged0, bool IsChanged1, bool IsChanged2, bool IsChanged3, bool IsChangedArgs1, bool IsChangedArgs2)
        CheckOnChangedMethods(
            ClassData @class,
            DependencyPropertyData property)
    {
        var isCustom = !string.IsNullOrWhiteSpace(property.OnChanged);
        var name = isCustom
            ? property.OnChanged
            : $"On{property.Name}Changed";

        return (name, property.IsChanged0, property.IsChanged1, property.IsChanged2, property.IsChanged3, property.IsChangedArgs1, property.IsChangedArgs2);
    }

    private static string GeneratePropertyChangingCallback(ClassData @class, DependencyPropertyData property)
    {
        if (!property.IsChanging0 &&
            !property.IsChanging1 &&
            !property.IsChanging2 &&
            !property.IsChanging3)
        {
            return "null";
        }

        var senderType = property.IsAttached
            ? GenerateBrowsableForType(property)
            : @class.Type;
        if (property.Framework == Framework.Maui)
        {
            return property.IsAttached
                ? $$"""
                    static (sender, oldValue, newValue) =>
                                    {
                                        {{(property.IsChanging0 ? @$"On{property.Name}Changing();" : "")}}
                                        {{(property.IsChanging1 ? $"""
                                                          On{property.Name}Changing(
                                                                                  ({senderType})sender);
                                                          """ : "")}}
                                        {{(property.IsChanging2 ? $"""
                                                          On{property.Name}Changing(
                                                                                  ({senderType})sender,
                                                                                  ({GenerateType(property)})newValue);
                                                          """ : "")}}
                                        {{(property.IsChanging3 ? $"""
                                                          On{property.Name}Changing(
                                                                                  ({senderType})sender,
                                                                                  ({GenerateType(property)})oldValue,
                                                                                  ({GenerateType(property)})newValue);
                                                          """ : "")}}
                                    }
                    """
                : $$"""
                    static (sender, oldValue, newValue) =>
                                    {
                                        {{(property.IsChanging0 ? @$"(({senderType})sender).On{property.Name}Changing();" : "")}}
                                        {{(property.IsChanging1 ? $"""
                                                          (({senderType})sender).On{property.Name}Changing(
                                                                                  ({GenerateType(property)})newValue);
                                                          """ : "")}}
                                        {{(property.IsChanging2 ? $"""
                                                          (({senderType})sender).On{property.Name}Changing(
                                                                                  ({GenerateType(property)})oldValue,
                                                                                  ({GenerateType(property)})newValue);
                                                          """ : "")}}
                                    }
                    """;
        }

        return property.IsAttached
            ? $$"""
                static (sender, args) =>
                                    {
                                        {{(property.IsChanging0 ? @$"On{property.Name}Changing();" : "")}}
                                        {{(property.IsChanging1 ? $"""
                                                          On{property.Name}Changing(
                                                                                      ({senderType})sender);
                                                          """ : "")}}
                                        {{(property.IsChanging2 ? $"""
                                                          On{property.Name}Changing(
                                                                                      ({senderType})sender,
                                                                                      ({GenerateType(property)})args.NewValue);
                                                          """ : "")}}
                                        {{(property.IsChanging3 ? $"""
                                                          On{property.Name}Changing(
                                                                                      ({senderType})sender,
                                                                                      ({GenerateType(property)})args.OldValue,
                                                                                      ({GenerateType(property)})args.NewValue);
                                                          """ : "")}}
                                    }
                """
            : $$"""
                static (sender, args) =>
                                    {
                                        {{(property.IsChanging0 ? @$"(({senderType})sender).On{property.Name}Changing();" : "")}}
                                        {{(property.IsChanging1 ? $"""
                                                          (({senderType})sender).On{property.Name}Changing(
                                                                                      ({GenerateType(property)})args.NewValue);
                                                          """ : "")}}
                                        {{(property.IsChanging2 ? $"""
                                                          (({senderType})sender).On{property.Name}Changing(
                                                                                      ({GenerateType(property)})args.OldValue,
                                                                                      ({GenerateType(property)})args.NewValue);
                                                          """ : "")}}
                                    }
                """;
    }
}

