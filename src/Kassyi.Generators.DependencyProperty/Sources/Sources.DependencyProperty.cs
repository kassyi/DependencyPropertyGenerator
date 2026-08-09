using System.Globalization;
using Kassyi.Generators.DependencyProperty.Models;
using Kassyi.Generators.Extensions;

namespace Kassyi.Generators.DependencyProperty.Sources;

internal static partial class SourceGenerationHelper
{
    public static string GenerateDependencyProperty(ClassData @class, DependencyPropertyData property)
    {
        var xmlDocumentation = GenerateXmlDocumentationFrom(property.XmlDocumentation, property, isProperty: false);
        var generatedCodeAttribute = GenerateGeneratedCodeAttribute(@class.Version);
        var propertyModifier = GeneratePropertyModifier(property);
        var propertyType = GeneratePropertyType(@class, property);
        var dependencyPropertyName = GenerateDependencyPropertyName(property);
        var dependencyPropertyCreateCall = GenerateDependencyPropertyCreateCall(@class, property);

        var additionalFieldForDirectProperties = GenerateAdditionalFieldForDirectProperties(property);
        var additionalPropertyForReadOnlyProperties = GenerateAdditionalPropertyForReadOnlyProperties(property);
        var getterXmlDocumentation = GenerateXmlDocumentationFrom(property.GetterXmlDocumentation, property, isProperty: true);
        var categoryAttribute = GenerateCategoryAttribute(property.Category);
        var descriptionAttribute = GenerateDescriptionAttribute(property.Description);
        var typeConverterAttribute = GenerateTypeConverterAttribute(property.TypeConverter);
        var bindableAttribute = GenerateBindableAttribute(property.Bindable);
        var browsableAttribute = GenerateBrowsableAttribute(property.Browsable);
        var designerSerializationVisibilityAttribute = GenerateDesignerSerializationVisibilityAttribute(property.DesignerSerializationVisibility);
        var clsCompliantAttribute = GenerateClsCompliantAttribute(property.ClsCompliant);
        var localizabilityAttribute = GenerateLocalizabilityAttribute(property.Localizability, @class.Framework);
        var excludeFromCodeCoverageAttribute = GenerateExcludeFromCodeCoverageAttribute();

        var type = GenerateType(property);
        var getter = GenerateGetter(property);
        var setter = GenerateSetter(property);

        var onChangedMethods = GenerateOnChangedMethods(@class, property);
        var onChangingMethods = GenerateOnChangingMethods(property);
        var coercePartialMethod = GenerateCoercePartialMethod(property);
        var validatePartialMethod = GenerateValidatePartialMethod(@class, property);
        var createDefaultValueCallbackPartialMethod = GenerateCreateDefaultValueCallbackPartialMethod(property);
        var bindEventMethod = GenerateBindEventMethod(property);

        return $$"""

            #nullable enable

            namespace {{@class.Namespace}}
            {
                {{@class.Modifiers}}partial class {{@class.Name}}
                {
            {{xmlDocumentation}}
            {{generatedCodeAttribute}}
                    {{propertyModifier}} static readonly {{propertyType}} {{dependencyPropertyName}} =
                        {{dependencyPropertyCreateCall}}

            {{additionalFieldForDirectProperties}}
            {{additionalPropertyForReadOnlyProperties}}
            {{getterXmlDocumentation}}
            {{categoryAttribute}}
            {{descriptionAttribute}}
            {{typeConverterAttribute}}
            {{bindableAttribute}}
            {{browsableAttribute}}
            {{designerSerializationVisibilityAttribute}}
            {{clsCompliantAttribute}}
            {{localizabilityAttribute}}
            {{generatedCodeAttribute}}
            {{excludeFromCodeCoverageAttribute}}
                    public {{type}} {{property.Name}}
                    {
                        {{getter}}
                        {{setter}}
                    }

            {{onChangedMethods}}
            {{onChangingMethods}}
            {{coercePartialMethod}}
            {{validatePartialMethod}}
            {{createDefaultValueCallbackPartialMethod}}
            {{bindEventMethod}}
                }
            }
            """.RemoveBlankLinesWhereOnlyWhitespaces();
    }
    
    private static string GenerateGetter(DependencyPropertyData property)
    {
        return property switch
        {
            { IsDirect: true, Framework: Framework.Avalonia } => @$"get => _{property.Name.ToParameterName()};",
            _ => @$"get => ({GenerateType(property)})GetValue({property.Name}Property);"
        };
    }

    private static string GenerateSetter(DependencyPropertyData property)
    {
        return property switch
        {
            { IsDirect: true, Framework: Framework.Avalonia } => $$"""
                                                                   private set
                                                                               {
                                                                                   var oldValue = _{{property.Name.ToParameterName()}};
                                                                                   SetAndRaise({{property.Name}}Property, ref _{{property.Name.ToParameterName()}}, value);
                                                                                   On{{property.Name}}Changed();
                                                                                   On{{property.Name}}Changed(
                                                                                       ({{GenerateType(property)}})value);
                                                                                   On{{property.Name}}Changed(
                                                                                       ({{GenerateType(property)}})oldValue,
                                                                                       ({{GenerateType(property)}})value);
                                                                               }
                                                                   """,
            _ =>
                @$"{GenerateAdditionalSetterModifier(property)}set => SetValue({GenerateDependencyPropertyName(property)}, value);"
        };
    }
    
    private static string GenerateDependencyPropertyCreateCall(ClassData @class, DependencyPropertyData property)
    {
        return property.IsAddOwner switch
        {
            true => GenerateAddOwnerCreateCall(@class, property),
            _ => $"""

                              {GenerateManagerType(@class)}.{GenerateRegisterMethod(@class, property)}(
                                  {GenerateRegisterMethodArguments(@class, property)});
                   
                  """.RemoveBlankLinesWhereOnlyWhitespaces()
        };
    }

    // https://docs.avaloniaui.net/docs/authoring-controls/defining-properties
    private static string GenerateAvaloniaRegisterMethodArguments(ClassData @class, DependencyPropertyData property)
    {
        var defaultBindingMode = property.DefaultBindingMode is null or "Default"
            ? "OneWay"
            : property.DefaultBindingMode;

        return property switch
        {
            { IsDirect: true, IsAddOwner: true } => $"""

                                                                     getter: static sender => sender.{property.Name},
                                                                     setter: {(property.IsReadOnly ? "null" : $"static (sender, value) => sender.{property.Name} = value")},
                                                                     unsetValue: {GenerateDefaultValue(property)},
                                                                     defaultBindingMode: global::Avalonia.Data.BindingMode.{defaultBindingMode},
                                                                     enableDataValidation: {(property.EnableDataValidation ? "true" : "false")}
                                                     """,
            _ => property.IsDirect switch
            {
                true => $"""

                                         name: "{property.Name}",
                                         getter: static sender => sender.{property.Name},
                                         setter: {(property.IsReadOnly ? "null" : $"static (sender, value) => sender.{property.Name} = value")},
                                         unsetValue: {GenerateDefaultValue(property)},
                                         defaultBindingMode: global::Avalonia.Data.BindingMode.{defaultBindingMode},
                                         enableDataValidation: {(property.EnableDataValidation ? "true" : "false")}
                         """,
                _ => property.IsAttached switch
                {
                    true => $"""

                                             name: "{property.Name}",
                                             defaultValue: {GenerateDefaultValue(property)},
                                             inherits: {(property.Inherits ? "true" : "false")},
                                             defaultBindingMode: global::Avalonia.Data.BindingMode.{defaultBindingMode},
                                             validate: {GenerateValidateValueCallback(@class, property)},
                                             coerce: {GenerateCoerceValueCallback(@class, property)}
                             """,
                    _ => $"""

                                          name: "{property.Name}",
                                          defaultValue: {GenerateDefaultValue(property)},
                                          inherits: {(property.Inherits ? "true" : "false")},
                                          defaultBindingMode: global::Avalonia.Data.BindingMode.{defaultBindingMode},
                                          validate: {GenerateValidateValueCallback(@class, property)},
                                          coerce: {GenerateCoerceValueCallback(@class, property)}
                          """
                }
            }
        };
    }

    private static string GeneratePropertyMetadata(ClassData @class, DependencyPropertyData property)
    {
        switch (property)
        {
            case { IsAddOwner: true, DefaultValue: null }:
                return "null";
        }

        var parameterName = (@class.Framework, property.IsAttached) switch
        {
            (Framework.Wpf, true) or (Framework.Uwp, true) or (Framework.WinUi, true) => "defaultMetadata: ",
            (Framework.Avalonia, _) => "metadata: ",
            (Framework.Uno, true) or (Framework.UnoWinUi, true) => string.Empty,
            _ => "typeMetadata: ",
        };
        switch (@class.Framework)
        {
            case Framework.Wpf:
                return property.DefaultUpdateSourceTrigger switch
                {
                    null => $"""
                             {parameterName}new global::System.Windows.FrameworkPropertyMetadata(
                                                 defaultValue: {GenerateDefaultValue(property)},
                                                 flags: {GenerateOptions(property)},
                                                 propertyChangedCallback: {GeneratePropertyChangedCallback(@class, property)},
                                                 coerceValueCallback: {GenerateCoerceValueCallback(@class, property)},
                                                 isAnimationProhibited: {property.IsAnimationProhibited.ToString().ToLower(CultureInfo.InvariantCulture)})
                             """,
                    _ => $"""
                          {parameterName}new global::System.Windows.FrameworkPropertyMetadata(
                                              defaultValue: {GenerateDefaultValue(property)},
                                              flags: {GenerateOptions(property)},
                                              propertyChangedCallback: {GeneratePropertyChangedCallback(@class, property)},
                                              coerceValueCallback: {GenerateCoerceValueCallback(@class, property)},
                                              isAnimationProhibited: {property.IsAnimationProhibited.ToString().ToLower(CultureInfo.InvariantCulture)},
                                              defaultUpdateSourceTrigger: global::System.Windows.Data.UpdateSourceTrigger.{property.DefaultUpdateSourceTrigger})
                          """
                };

            case Framework.Uwp:
            case Framework.WinUi:
            case Framework.Uno:
            case Framework.UnoWinUi:
            {
                var type = GenerateTypeByPlatform(@class.Framework, "PropertyMetadata");
                switch (property.CreateDefaultValueCallback)
                {
                    case true:
                        return $"""
                                {parameterName}{type}.Create(
                                                    createDefaultValueCallback: {GenerateCreateDefaultValueCallbackValueCallback(property)},
                                                    propertyChangedCallback: {GeneratePropertyChangedCallback(@class, property)})
                                """;
                    default:
                    {
                        // fix for NotImplementedException: The member PropertyMetadata PropertyMetadata.Create(object defaultValue, PropertyChangedCallback propertyChangedCallback) is not implemented in Uno.
                        var create = property.Framework switch
                        {
                            Framework.Uno or Framework.UnoWinUi => $"new {type}",
                            _ => $"{type}.Create",
                        };
                        return $"""
                                {parameterName}{create}(
                                                    defaultValue: {GenerateDefaultValue(property)},
                                                    propertyChangedCallback: {GeneratePropertyChangedCallback(@class, property)})
                                """;
                    }
                }
            }

            case Framework.Avalonia:
            {
                var metadataType = GenerateTypeByPlatform(@class.Framework, $"StyledPropertyMetadata<{property.Type}>");

                return $"""
                        {parameterName}new {metadataType}(
                                            defaultValue: {GenerateDefaultValue(property)},
                                            defaultBindingMode: global::Avalonia.Data.BindingMode.Default,
                                            coerce: {GenerateCoerceValueCallback(@class, property)},
                                            enableDataValidation: {property.EnableDataValidation.ToBooleanKeyword()})
                        """;
            }
        }

        throw new InvalidOperationException("Platform is not supported.");
    }

    private static string GenerateManagerType(ClassData @class)
    {
        return @class.Framework switch
        {
            Framework.Maui => GenerateTypeByPlatform(@class.Framework, "BindableProperty"),
            Framework.Avalonia => GenerateTypeByPlatform(@class.Framework, "AvaloniaProperty"),
            _ => GenerateTypeByPlatform(@class.Framework, "DependencyProperty")
        };
    }
    
    private static string GenerateMauiRegisterMethodArguments(ClassData @class, DependencyPropertyData property)
    {
        var defaultBindingMode = property.DefaultBindingMode is null or "Default"
            ? property.IsReadOnly
                ? "OneWayToSource"
                : "OneWay"
            : property.DefaultBindingMode;

        return $"""

                                propertyName: "{property.Name}",
                                returnType: typeof({property.Type}),
                                declaringType: typeof({@class.Type}),
                                defaultValue: {GenerateDefaultValue(property)},
                                defaultBindingMode: global::Microsoft.Maui.Controls.BindingMode.{defaultBindingMode},
                                validateValue: {GenerateValidateValueCallback(@class, property)},
                                propertyChanged: {GeneratePropertyChangedCallback(@class, property)},
                                propertyChanging: {GeneratePropertyChangingCallback(@class, property)},
                                coerceValue: {GenerateCoerceValueCallback(@class, property)},
                                defaultValueCreator: {GenerateCreateDefaultValueCallbackValueCallback(property)}
                """;
    }

    private static string GenerateRegisterMethodArguments(ClassData @class, DependencyPropertyData property)
    {
        return @class.Framework switch
        {
            Framework.Avalonia => GenerateAvaloniaRegisterMethodArguments(@class, property),
            Framework.Maui => GenerateMauiRegisterMethodArguments(@class, property),
            Framework.Wpf => $"""

                                              name: "{property.Name}",
                                              propertyType: typeof({property.Type}),
                                              ownerType: typeof({@class.Type}),
                                              {GeneratePropertyMetadata(@class, property)},
                                              validateValueCallback: {GenerateValidateValueCallback(@class, property)}
                              """,
            _ => $"""

                                  name: "{property.Name}",
                                  propertyType: typeof({property.Type}),
                                  ownerType: typeof({@class.Type}),
                                  {GeneratePropertyMetadata(@class, property)}
                  """
        };
    }

    private static string GenerateRegisterMethod(ClassData @class, DependencyPropertyData property)
    {
        return property.Framework switch
        {
            Framework.Maui => property.IsAttached ? property.IsReadOnly ? "CreateAttachedReadOnly" : "CreateAttached" :
                property.IsReadOnly ? "CreateReadOnly" : "Create",
            Framework.Avalonia => property.IsDirect
                ? $"RegisterDirect<{@class.Type}, {GenerateType(property)}>"
                :
                property.IsAttached
                    ?
                    $"RegisterAttached<{@class.Type}, {GenerateBrowsableForType(property)}, {GenerateType(property)}>"
                    : $"Register<{@class.Type}, {GenerateType(property)}>",
            _ => property switch
            {
                { IsReadOnly: true, Framework: Framework.Wpf } => property.IsAttached
                    ? "RegisterAttachedReadOnly"
                    : "RegisterReadOnly",
                _ => property.IsAttached ? "RegisterAttached" : "Register"
            }
        };
    }

    private static string GenerateCoercePartialMethod(DependencyPropertyData property)
    {
        return property.Coerce switch
        {
            false => " ",
            _ => property.IsAttached
                ? $"        private static partial {GenerateType(property)} Coerce{property.Name}({GenerateBrowsableForType(property)} {GenerateBrowsableForTypeParameterName(property)}, {GenerateType(property, canBeNull: true)} value);"
                : $"        private partial {GenerateType(property)} Coerce{property.Name}({GenerateType(property, canBeNull: true)} value);"
        };
    }

    private static string GenerateAdditionalFieldForDirectProperties(DependencyPropertyData property)
    {
        return property.IsDirect switch
        {
            false => " ",
            _ => property.Framework switch
            {
                Framework.Avalonia => $"""
                                        
                                               private {GenerateType(property)} _{property.Name.ToParameterName()} = {GenerateDefaultValue(property)};

                                       """,
                _ => " ",
            }
        };
    }

    private static string GenerateAdditionalPropertyForReadOnlyProperties(DependencyPropertyData property)
    {
        return property.IsReadOnly switch
        {
            false => " ",
            _ => property.Framework switch
            {
                Framework.Maui => $"""
                                    
                                   {GenerateXmlDocumentationFrom(property.XmlDocumentation, property, isProperty: false)}
                                           public static readonly {GenerateTypeByPlatform(property.Framework, "BindableProperty")} {property.Name}Property
                                               = {GenerateDependencyPropertyName(property)}.BindableProperty;

                                   """,
                // https://docs.microsoft.com/en-us/dotnet/api/system.windows.dependencypropertykey?view=windowsdesktop-6.0#examples
                Framework.Wpf => $"""
                                   
                                  {GenerateXmlDocumentationFrom(property.XmlDocumentation, property, isProperty: false)}
                                          public static readonly {GenerateTypeByPlatform(property.Framework, "DependencyProperty")} {property.Name}Property
                                              = {GenerateDependencyPropertyName(property)}.DependencyProperty;

                                  """,
                _ => " ",
            }
        };
    }
}

