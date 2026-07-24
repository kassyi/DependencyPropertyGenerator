using H.Generators.Extensions;

namespace H.Generators;

internal static partial class Sources
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

    private static bool IsMethodExists(ClassData @class, string signature)
    {
        return @class.Methods.Contains($"{@class.FullName}.{signature}");
    }

    private static bool IsEventArgsType(string typeName)
    {
        return typeName.EndsWith("EventArgs", StringComparison.Ordinal) ||
               typeName.EndsWith("EventArgs>", StringComparison.Ordinal) ||
               typeName.Contains("DependencyPropertyChangedEventArgs") ||
               typeName.Contains("ValueChangedEventArgs");
    }
    
    private static (bool IsChanged0, bool IsChanged1, bool IsChanged2, bool IsChanged3, bool IsChangedArgs1, bool IsChangedArgs2) CheckMethods(
        string name,
        ClassData @class,
        DependencyPropertyData property)
    {
        var type = GenerateType(property)
            .Replace("global::", string.Empty)
            .Replace("?", string.Empty);
        var senderType = (property.IsAttached
                ? GenerateBrowsableForType(property)
                : @class.Type)
            .Replace("global::", string.Empty);

        var isChanged0 =
            IsMethodExists(@class, $"{name}()");
        var isChanged1 =
            IsMethodExists(@class, $"{name}({type})") ||
            IsMethodExists(@class, $"{name}({senderType})");
        var isChanged2 =
            IsMethodExists(@class, $"{name}({type}, {type})") ||
            IsMethodExists(@class, $"{name}({senderType}, {type})");
        var isChanged3 =
            IsMethodExists(@class, $"{name}({senderType}, {type}, {type})");

        var prefix = $"{@class.FullName}.{name}(";
        var isChangedArgs1 = false;
        var isChangedArgs2 = false;

        foreach (var method in @class.Methods)
        {
            if (!method.StartsWith(prefix, StringComparison.Ordinal))
            {
                continue;
            }

            var parametersStr = method.Substring(prefix.Length).TrimEnd(')');
            if (string.IsNullOrWhiteSpace(parametersStr))
            {
                continue;
            }

            var parameters = parametersStr.Split([','], StringSplitOptions.RemoveEmptyEntries)
                .Select(static s => s.Trim())
                .ToArray();

            if (parameters.Length == 1 && IsEventArgsType(parameters[0]))
            {
                isChangedArgs1 = true;
            }
            else if (parameters.Length == 2 && IsEventArgsType(parameters[1]))
            {
                isChangedArgs2 = true;
            }
        }

        return (isChanged0, isChanged1, isChanged2, isChanged3, isChangedArgs1, isChangedArgs2);
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

        var (isChanged0, isChanged1, isChanged2, isChanged3, isChangedArgs1, isChangedArgs2) = CheckMethods(name, @class, property);
        isChanged2 |= !isCustom && property is { IsAttached: false, BindEvents.IsEmpty: false };
        isChanged3 |= !isCustom && property is { IsAttached: true, BindEvents.IsEmpty: false };

        return (name, isChanged0, isChanged1, isChanged2, isChanged3, isChangedArgs1, isChangedArgs2);
    }

    private static string GeneratePropertyChangingCallback(ClassData @class, DependencyPropertyData property)
    {
        var (isChanging0, isChanging1, isChanging2, isChanging3, _, _) =
            CheckMethods($"On{property.Name}Changing", @class, property);
        if (!isChanging0 &&
            !isChanging1 &&
            !isChanging2 &&
            !isChanging3)
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
                                        {{(isChanging0 ? @$"On{property.Name}Changing();" : "")}}
                                        {{(isChanging1 ? $"""
                                                          On{property.Name}Changing(
                                                                                  ({senderType})sender);
                                                          """ : "")}}
                                        {{(isChanging2 ? $"""
                                                          On{property.Name}Changing(
                                                                                  ({senderType})sender,
                                                                                  ({GenerateType(property)})newValue);
                                                          """ : "")}}
                                        {{(isChanging3 ? $"""
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
                                        {{(isChanging0 ? @$"(({senderType})sender).On{property.Name}Changing();" : "")}}
                                        {{(isChanging1 ? $"""
                                                          (({senderType})sender).On{property.Name}Changing(
                                                                                  ({GenerateType(property)})newValue);
                                                          """ : "")}}
                                        {{(isChanging2 ? $"""
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
                                        {{(isChanging0 ? @$"On{property.Name}Changing();" : "")}}
                                        {{(isChanging1 ? $"""
                                                          On{property.Name}Changing(
                                                                                      ({senderType})sender);
                                                          """ : "")}}
                                        {{(isChanging2 ? $"""
                                                          On{property.Name}Changing(
                                                                                      ({senderType})sender,
                                                                                      ({GenerateType(property)})args.NewValue);
                                                          """ : "")}}
                                        {{(isChanging3 ? $"""
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
                                        {{(isChanging0 ? @$"(({senderType})sender).On{property.Name}Changing();" : "")}}
                                        {{(isChanging1 ? $"""
                                                          (({senderType})sender).On{property.Name}Changing(
                                                                                      ({GenerateType(property)})args.NewValue);
                                                          """ : "")}}
                                        {{(isChanging2 ? $"""
                                                          (({senderType})sender).On{property.Name}Changing(
                                                                                      ({GenerateType(property)})args.OldValue,
                                                                                      ({GenerateType(property)})args.NewValue);
                                                          """ : "")}}
                                    }
                """;
    }
}