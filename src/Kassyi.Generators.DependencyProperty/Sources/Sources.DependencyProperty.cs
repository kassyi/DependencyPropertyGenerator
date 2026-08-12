using System.Globalization;
using Kassyi.Generators.DependencyProperty.Models;
using Kassyi.Generators.Extensions;

namespace Kassyi.Generators.DependencyProperty.Sources;

internal static partial class SourceGenerationHelper
{
    public static void GenerateDependencyProperty(ref SourceWriter writer, ClassData @class, DependencyPropertyData property)
    {
        writer.AppendLine();
        writer.AppendLine("#nullable enable");
        writer.AppendLine();
        writer.AppendLine($"namespace {@class.Namespace}");
        writer.AppendLine("{");
        writer.AppendLine($"    {@class.Modifiers}partial class {@class.Name}");
        writer.AppendLine("    {");

        GenerateXmlDocumentationFrom(ref writer, property.XmlDocumentation, property, isProperty: false);
        GenerateGeneratedCodeAttribute(ref writer, @class.Version);

        var propertyModifier = GeneratePropertyModifier(property);
        var propertyType = GeneratePropertyType(@class, property);
        var dependencyPropertyName = GenerateDependencyPropertyName(property);

        writer.Append($"        {propertyModifier} static readonly {propertyType} {dependencyPropertyName} =");
        GenerateDependencyPropertyCreateCall(ref writer, @class, property);
        writer.AppendLine();

        GenerateAdditionalFieldForDirectProperties(ref writer, property);
        GenerateAdditionalPropertyForReadOnlyProperties(ref writer, property);
        GenerateXmlDocumentationFrom(ref writer, property.GetterXmlDocumentation, property, isProperty: true);
        GenerateCategoryAttribute(ref writer, property.Category);
        GenerateDescriptionAttribute(ref writer, property.Description);
        GenerateTypeConverterAttribute(ref writer, property.TypeConverter);
        GenerateBindableAttribute(ref writer, property.Bindable);
        GenerateBrowsableAttribute(ref writer, property.Browsable);
        GenerateDesignerSerializationVisibilityAttribute(ref writer, property.DesignerSerializationVisibility);
        GenerateClsCompliantAttribute(ref writer, property.ClsCompliant);
        GenerateLocalizabilityAttribute(ref writer, property.Localizability, @class.Framework);
        GenerateGeneratedCodeAttribute(ref writer, @class.Version);
        GenerateExcludeFromCodeCoverageAttribute(ref writer);

        writer.AppendLine($"        public {GenerateType(property)} {property.Name}");
        writer.AppendLine("        {");
        writer.Append("            ");
        GenerateGetter(ref writer, property);
        writer.AppendLine();
        if (!property.IsReadOnly)
        {
            writer.Append("            ");
            GenerateSetter(ref writer, property);
            writer.AppendLine();
        }
        writer.AppendLine("        }");

        GenerateOnChangedMethods(ref writer, @class, property);
        GenerateOnChangingMethods(ref writer, property);
        GenerateCoercePartialMethod(ref writer, property);
        GenerateValidatePartialMethod(ref writer, @class, property);
        GenerateCreateDefaultValueCallbackPartialMethod(ref writer, property);
        GenerateBindEventMethod(ref writer, property);

        writer.AppendLine("    }");
        writer.AppendLine("}");
    }
    private static void GenerateGetter(ref SourceWriter writer, DependencyPropertyData property)
    {
        if (property is { IsDirect: true, Framework: Framework.Avalonia })
        {
            writer.Append($"get => _{property.Name.ToParameterName()};");
        }
        else
        {
            writer.Append($"get => ({GenerateType(property)})GetValue({property.Name}Property);");
        }
    }

    private static void GenerateSetter(ref SourceWriter writer, DependencyPropertyData property)
    {
        if (property is { IsDirect: true, Framework: Framework.Avalonia })
        {
            writer.Append($$"""
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
            """);
        }
        else
        {
            writer.Append($"{GenerateAdditionalSetterModifier(property)}set => SetValue({GenerateDependencyPropertyName(property)}, value);");
        }
    }

    private static void GenerateDependencyPropertyCreateCall(ref SourceWriter writer, ClassData @class, DependencyPropertyData property)
    {
        if (property.IsAddOwner)
        {
            GenerateAddOwnerCreateCall(ref writer, @class, property);
        }
        else
        {
            writer.AppendLine();
            writer.Append($"""
                              {GenerateManagerType(@class)}.{GenerateRegisterMethod(@class, property)}(
                                  {GenerateRegisterMethodArguments(@class, property)});
                  """);
        }
    }

    private static string GenerateAvaloniaRegisterMethodArguments(ClassData @class, DependencyPropertyData property)
    {
        var writer = new SourceWriter();
        try
        {
            GenerateAvaloniaRegisterMethodArguments(ref writer, @class, property);
            return writer.ToString();
        }
        finally
        {
            writer.Dispose();
        }
    }

    private static void GenerateAvaloniaRegisterMethodArguments(ref SourceWriter writer, ClassData @class, DependencyPropertyData property)
    {
        var defaultBindingMode = property.DefaultBindingMode is null or "Default"
            ? "OneWay"
            : property.DefaultBindingMode;

        switch (property)
        {
            case { IsDirect: true, IsAddOwner: true }:
                writer.Append($"""

                                                                     getter: static sender => sender.{property.Name},
                                                                     setter: {(property.IsReadOnly ? "null" : $"static (sender, value) => sender.{property.Name} = value")},
                                                                     unsetValue: {GenerateDefaultValue(property)},
                                                                     defaultBindingMode: global::Avalonia.Data.BindingMode.{defaultBindingMode},
                                                                     enableDataValidation: {(property.EnableDataValidation ? "true" : "false")}
                                                     """);
                return;
            default:
                if (property.IsDirect)
                {
                    writer.Append($"""

                                                                         name: "{property.Name}",
                                                                         getter: static sender => sender.{property.Name},
                                                                         setter: {(property.IsReadOnly ? "null" : $"static (sender, value) => sender.{property.Name} = value")},
                                                                         unsetValue: {GenerateDefaultValue(property)},
                                                                         defaultBindingMode: global::Avalonia.Data.BindingMode.{defaultBindingMode},
                                                                         enableDataValidation: {(property.EnableDataValidation ? "true" : "false")}
                                                         """);
                }
                else if (property.IsAttached)
                {
                    writer.Append($"""

                                             name: "{property.Name}",
                                             defaultValue: {GenerateDefaultValue(property)},
                                             inherits: {(property.Inherits ? "true" : "false")},
                                             defaultBindingMode: global::Avalonia.Data.BindingMode.{defaultBindingMode},
                                             validate: {GenerateValidateValueCallback(@class, property)},
                                             coerce: {GenerateCoerceValueCallback(@class, property)}
                             """);
                }
                else
                {
                    writer.Append($"""

                                          name: "{property.Name}",
                                          defaultValue: {GenerateDefaultValue(property)},
                                          inherits: {(property.Inherits ? "true" : "false")},
                                          defaultBindingMode: global::Avalonia.Data.BindingMode.{defaultBindingMode},
                                          validate: {GenerateValidateValueCallback(@class, property)},
                                          coerce: {GenerateCoerceValueCallback(@class, property)}
                          """);
                }
                return;
        }
    }

    private static string GeneratePropertyMetadata(ClassData @class, DependencyPropertyData property)
    {
        var writer = new SourceWriter();
        try
        {
            GeneratePropertyMetadata(ref writer, @class, property);
            return writer.ToString();
        }
        finally
        {
            writer.Dispose();
        }
    }

    private static void GeneratePropertyMetadata(ref SourceWriter writer, ClassData @class, DependencyPropertyData property)
    {
        if (property is { IsAddOwner: true, DefaultValue: null })
        {
            writer.Append("null");
            return;
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
                GenerateWpfPropertyMetadata(ref writer, @class, property, parameterName);
                break;
            case Framework.Uwp:
            case Framework.WinUi:
            case Framework.Uno:
            case Framework.UnoWinUi:
                GenerateUwpPropertyMetadata(ref writer, @class, property, parameterName);
                break;
            case Framework.Avalonia:
                GenerateAvaloniaPropertyMetadata(ref writer, @class, property, parameterName);
                break;
            case Framework.None:
            case Framework.Maui:
                throw new InvalidOperationException("Platform is not supported.");
            default:
                throw new ArgumentOutOfRangeException(nameof(@class));
        }
    }

    private static void GenerateWpfPropertyMetadata(ref SourceWriter writer, ClassData @class, DependencyPropertyData property, string parameterName)
    {
        var defaultValue = GenerateDefaultValue(property);
        var flags = GenerateOptions(property);
        var propertyChanged = GeneratePropertyChangedCallback(@class, property);
        var coerceValue = GenerateCoerceValueCallback(@class, property);
        var isAnimationProhibited = property.IsAnimationProhibited.ToString().ToLower(CultureInfo.InvariantCulture);

        if (property.DefaultUpdateSourceTrigger is null)
        {
            writer.Append($"""
                {parameterName}new global::System.Windows.FrameworkPropertyMetadata(
                                    defaultValue: {defaultValue},
                                    flags: {flags},
                                    propertyChangedCallback: {propertyChanged},
                                    coerceValueCallback: {coerceValue},
                                    isAnimationProhibited: {isAnimationProhibited})
                """);
        }
        else
        {
            writer.Append($"""
                {parameterName}new global::System.Windows.FrameworkPropertyMetadata(
                                    defaultValue: {defaultValue},
                                    flags: {flags},
                                    propertyChangedCallback: {propertyChanged},
                                    coerceValueCallback: {coerceValue},
                                    isAnimationProhibited: {isAnimationProhibited},
                                    defaultUpdateSourceTrigger: global::System.Windows.Data.UpdateSourceTrigger.{property.DefaultUpdateSourceTrigger})
                """);
        }
    }

    private static void GenerateUwpPropertyMetadata(ref SourceWriter writer, ClassData @class, DependencyPropertyData property, string parameterName)
    {
        var type = GenerateTypeByPlatform(@class.Framework, "PropertyMetadata");
        var propertyChanged = GeneratePropertyChangedCallback(@class, property);

        if (property.CreateDefaultValueCallback)
        {
            var createDefaultValue = GenerateCreateDefaultValueCallbackValueCallback(property);
            writer.Append($"""
                {parameterName}{type}.Create(
                                    createDefaultValueCallback: {createDefaultValue},
                                    propertyChangedCallback: {propertyChanged})
                """);
        }
        else
        {
            // fix for NotImplementedException: The member PropertyMetadata PropertyMetadata.Create(object defaultValue, PropertyChangedCallback propertyChangedCallback) is not implemented in Uno.
            var create = @class.Framework switch
            {
                Framework.Uno or Framework.UnoWinUi => $"new {type}",
                _ => $"{type}.Create",
            };
            var defaultValue = GenerateDefaultValue(property);
            writer.Append($"""
                {parameterName}{create}(
                                    defaultValue: {defaultValue},
                                    propertyChangedCallback: {propertyChanged})
                """);
        }
    }

    private static void GenerateAvaloniaPropertyMetadata(ref SourceWriter writer, ClassData @class, DependencyPropertyData property, string parameterName)
    {
        var metadataType = GenerateTypeByPlatform(@class.Framework, $"StyledPropertyMetadata<{property.Type}>");
        var defaultValue = GenerateDefaultValue(property);
        var coerce = GenerateCoerceValueCallback(@class, property);
        var enableValidation = property.EnableDataValidation.ToBooleanKeyword();

        writer.Append($"""
            {parameterName}new {metadataType}(
                                defaultValue: {defaultValue},
                                defaultBindingMode: global::Avalonia.Data.BindingMode.Default,
                                coerce: {coerce},
                                enableDataValidation: {enableValidation})
            """);
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
            _ => property is { IsReadOnly: true, Framework: Framework.Wpf }
                ? property.IsAttached
                    ? "RegisterAttachedReadOnly"
                    : "RegisterReadOnly"
                : property.IsAttached ? "RegisterAttached" : "Register"
        };
    }
    private static void GenerateCoercePartialMethod(ref SourceWriter writer, DependencyPropertyData property)
    {
        if (!property.Coerce)
        {
            writer.Append(" ");
            return;
        }

        if (property.IsAttached)
        {
            writer.Append($"        private static partial {GenerateType(property)} Coerce{property.Name}({GenerateBrowsableForType(property)} {GenerateBrowsableForTypeParameterName(property)}, {GenerateType(property, canBeNull: true)} value);");
        }
        else
        {
            writer.Append($"        private partial {GenerateType(property)} Coerce{property.Name}({GenerateType(property, canBeNull: true)} value);");
        }
    }

    private static void GenerateAdditionalFieldForDirectProperties(ref SourceWriter writer, DependencyPropertyData property)
    {
        if (!property.IsDirect)
        {
            writer.Append(" ");
            return;
        }

        if (property.Framework == Framework.Avalonia)
        {
            writer.Append($"        private {GenerateType(property)} _{property.Name.ToParameterName()} = {GenerateDefaultValue(property)};");
            writer.AppendLine();
        }
        else
        {
            writer.Append(" ");
        }
    }



    private static void GenerateAdditionalPropertyForReadOnlyProperties(ref SourceWriter writer, DependencyPropertyData property)
    {
        if (!property.IsReadOnly)
        {
            writer.Append(" ");
            return;
        }

        if (property.Framework == Framework.Maui)
        {
            GenerateXmlDocumentationFrom(ref writer, property.XmlDocumentation, property, isProperty: false);
            writer.AppendLine($"        public static readonly {GenerateTypeByPlatform(property.Framework, "BindableProperty")} {property.Name}Property");
            writer.AppendLine($"            = {GenerateDependencyPropertyName(property)}.BindableProperty;");
        }
        else if (property.Framework == Framework.Wpf)
        {
            GenerateXmlDocumentationFrom(ref writer, property.XmlDocumentation, property, isProperty: false);
            writer.AppendLine($"        public static readonly {GenerateTypeByPlatform(property.Framework, "DependencyProperty")} {property.Name}Property");
            writer.AppendLine($"            = {GenerateDependencyPropertyName(property)}.DependencyProperty;");
        }
        else
        {
            writer.Append(" ");
        }
    }
}

