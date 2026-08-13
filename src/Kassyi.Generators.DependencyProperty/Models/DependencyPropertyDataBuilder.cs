using System.Collections.Immutable;
using System.ComponentModel;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Kassyi.Generators.Extensions;

namespace Kassyi.Generators.DependencyProperty.Models;

internal sealed class DependencyPropertyDataBuilder
{
    private string _name = string.Empty;
    private string _version = string.Empty;
    private string _type = string.Empty;
    private string _shortType = string.Empty;
    private bool _isValueType = true;
    private bool _isSpecialType;
    private string? _defaultValue;
    private string? _defaultValueDocumentation;
    private bool _isReadOnly;
    private bool _isDirect;
    private bool _isAttached;
    private bool _isAddOwner;
    private Framework _framework;
    
    private ComponentModelData _componentModel;
    private FrameworkMetadataData _frameworkMetadata;
    private XmlDocumentationData _xmlDocumentation;
    private ValidationAndCallbackData _validationAndCallbacks;

    public DependencyPropertyDataBuilder WithCoreProperties(AttributeData attribute, Framework framework, string version, bool isAddOwner, bool isAttached)
    {
        _framework = framework;
        _version = version;
        _isAddOwner = isAddOwner;
        _isAttached = isAttached;

        _name = attribute.ConstructorArguments.ElementAtOrDefault(0).Value?.ToString() ?? string.Empty;
        var typeSymbol = attribute.GetGenericTypeArgument(0) ?? attribute.ConstructorArguments.ElementAtOrDefault(1).Value as ITypeSymbol;
        
        _type = typeSymbol?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) ?? string.Empty;
        _shortType = typeSymbol?.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat) ?? string.Empty;
        _isValueType = typeSymbol?.IsValueType ?? true;
        _isSpecialType = typeSymbol.IsSpecialType() ?? false;
        
        _isReadOnly = attribute.GetNamedArgument(nameof(DependencyPropertyAttribute.IsReadOnly)).ToBoolean();
        _isDirect = attribute.GetNamedArgument(nameof(DependencyPropertyAttribute.IsDirect)).ToBoolean();

        _componentModel = _componentModel with
        {
            BrowsableForType = attribute.GetGenericTypeArgumentOrNamed(1, nameof(AttachedDependencyPropertyAttribute.BrowsableForType))?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
            FromType = attribute.GetGenericTypeArgumentOrNamed(1, nameof(AddOwnerAttribute.FromType))?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
        };

        return this;
    }

    public DependencyPropertyDataBuilder WithMetadata(AttributeData attribute)
    {
        _componentModel = _componentModel with
        {
            Description = attribute.GetNamedArgument(nameof(DependencyPropertyAttribute.Description)).Value?.ToString(),
            Category = attribute.GetNamedArgument(nameof(DependencyPropertyAttribute.Category)).Value?.ToString(),
            TypeConverter = attribute.GetNamedArgument(nameof(DependencyPropertyAttribute.TypeConverter)).Value?.ToString(),
            Bindable = attribute.GetNamedArgument(nameof(DependencyPropertyAttribute.Bindable)).ToNullableBoolean(),
            Browsable = attribute.GetNamedArgument(nameof(DependencyPropertyAttribute.Browsable)).ToNullableBoolean(),
            DesignerSerializationVisibility = attribute.GetNamedArgument(nameof(DependencyPropertyAttribute.DesignerSerializationVisibility)).ToEnum<DesignerSerializationVisibility>()?.ToString("G"),
            ClsCompliant = attribute.GetNamedArgument(nameof(DependencyPropertyAttribute.ClsCompliant)).ToNullableBoolean(),
            Localizability = attribute.GetNamedArgument(nameof(DependencyPropertyAttribute.Localizability)).ToEnum<Localizability>()?.ToString("G")
        };

        _frameworkMetadata = new FrameworkMetadataData(
            AffectsMeasure: attribute.GetNamedArgument(nameof(DependencyPropertyAttribute.AffectsMeasure)).ToBoolean(),
            AffectsArrange: attribute.GetNamedArgument(nameof(DependencyPropertyAttribute.AffectsArrange)).ToBoolean(),
            AffectsParentMeasure: attribute.GetNamedArgument(nameof(DependencyPropertyAttribute.AffectsParentMeasure)).ToBoolean(),
            AffectsParentArrange: attribute.GetNamedArgument(nameof(DependencyPropertyAttribute.AffectsParentArrange)).ToBoolean(),
            AffectsRender: attribute.GetNamedArgument(nameof(DependencyPropertyAttribute.AffectsRender)).ToBoolean(),
            Inherits: attribute.GetNamedArgument(nameof(DependencyPropertyAttribute.Inherits)).ToBoolean(),
            OverridesInheritanceBehavior: attribute.GetNamedArgument(nameof(DependencyPropertyAttribute.OverridesInheritanceBehavior)).ToBoolean(),
            NotDataBindable: attribute.GetNamedArgument(nameof(DependencyPropertyAttribute.NotDataBindable)).ToBoolean(),
            Journal: attribute.GetNamedArgument(nameof(DependencyPropertyAttribute.Journal)).ToBoolean(),
            SubPropertiesDoNotAffectRender: attribute.GetNamedArgument(nameof(DependencyPropertyAttribute.SubPropertiesDoNotAffectRender)).ToBoolean(),
            IsAnimationProhibited: attribute.GetNamedArgument(nameof(DependencyPropertyAttribute.IsAnimationProhibited)).ToBoolean(),
            DefaultUpdateSourceTrigger: attribute.GetNamedArgument(nameof(DependencyPropertyAttribute.DefaultUpdateSourceTrigger)).ToEnum<SourceTrigger>()?.ToString("G"),
            DefaultBindingMode: attribute.GetNamedArgument(nameof(DependencyPropertyAttribute.DefaultBindingMode)).ToEnum<DefaultBindingMode>()?.ToString("G")
        );

        _validationAndCallbacks = _validationAndCallbacks with
        {
            EnableDataValidation = attribute.GetNamedArgument(nameof(DependencyPropertyAttribute.EnableDataValidation)).ToBoolean(),
            Coerce = attribute.GetNamedArgument(nameof(DependencyPropertyAttribute.Coerce)).ToBoolean(),
            Validate = attribute.GetNamedArgument(nameof(DependencyPropertyAttribute.Validate)).ToBoolean(),
            CreateDefaultValueCallback = attribute.GetNamedArgument(nameof(DependencyPropertyAttribute.CreateDefaultValueCallback)).ToBoolean()
        };

        return this;
    }

    public DependencyPropertyDataBuilder WithDefaultValues(AttributeData attribute, AttributeSyntax? attributeSyntax)
    {
        var typeSymbol = attribute.GetGenericTypeArgument(0) ?? attribute.ConstructorArguments.ElementAtOrDefault(1).Value as ITypeSymbol;
        
        var defaultValue = attribute.GetNamedArgument(nameof(DependencyPropertyAttribute.DefaultValueExpression)).Value?.ToString() ??
                           attribute.GetNamedArgument(nameof(DependencyPropertyAttribute.DefaultValue)).Value?.ToString();
                           
        var defaultValueDoc = attribute.GetNamedArgument(nameof(DependencyPropertyAttribute.DefaultValueExpression)).Value?.ToString() ??
                              attributeSyntax?.GetNamedArgumentExpression(nameof(DependencyPropertyAttribute.DefaultValue));

        _defaultValue = PrepareData.ExpandDefaultValueExpression(defaultValue, typeSymbol);
        _defaultValueDocumentation = PrepareData.ExpandDefaultValueExpression(defaultValueDoc, typeSymbol);

        return this;
    }

    public DependencyPropertyDataBuilder WithXmlDocumentation(AttributeData attribute)
    {
        var propertyXmlDoc = attribute.GetNamedArgument(nameof(DependencyPropertyAttribute.PropertyXmlDocumentation)).Value?.ToString();
        var getterXmlDoc = attribute.GetNamedArgument(nameof(AttachedDependencyPropertyAttribute.GetterXmlDocumentation)).Value?.ToString();
        
        _xmlDocumentation = new XmlDocumentationData(
            XmlDocumentation: attribute.GetNamedArgument(nameof(DependencyPropertyAttribute.XmlDocumentation)).Value?.ToString(),
            GetterXmlDocumentation: getterXmlDoc ?? propertyXmlDoc,
            SetterXmlDocumentation: attribute.GetNamedArgument(nameof(AttachedDependencyPropertyAttribute.SetterXmlDocumentation)).Value?.ToString()
        );
        
        return this;
    }

    public DependencyPropertyDataBuilder WithCallbacks(AttributeData attribute, INamedTypeSymbol? classSymbol)
    {
        var bindEvent = attribute.GetNamedArgument(nameof(DependencyPropertyAttribute.BindEvent)).Value?.ToString();
        var bindEvents = attribute.GetNamedArgument(nameof(DependencyPropertyAttribute.BindEvents));
        var onChanged = attribute.GetNamedArgument(nameof(DependencyPropertyAttribute.OnChanged)).Value?.ToString() ?? string.Empty;

        var isCustomOnChanged = !string.IsNullOrWhiteSpace(onChanged);
        var onChangedName = isCustomOnChanged ? onChanged : $"On{_name}Changed";
        var onChangingName = $"On{_name}Changing";

        var targetType = _type.Replace("global::", string.Empty).Replace("?", string.Empty);
        var targetSenderType = GetTargetSenderType(classSymbol);

        var (c0, c1, c2, c3, ca1, ca2) = classSymbol != null
            ? PrepareData.CheckMethodsDirectly(classSymbol, onChangedName, targetType, targetSenderType)
            : (false, false, false, false, false, false);

        var (ch0, ch1, ch2, ch3, _, _) = classSymbol != null
            ? PrepareData.CheckMethodsDirectly(classSymbol, onChangingName, targetType, targetSenderType)
            : (false, false, false, false, false, false);

        var bindEventsArray = GetBindEventsArray(bindEvent, bindEvents);

        c2 |= !isCustomOnChanged && !_isAttached && !bindEventsArray.IsEmpty;
        c3 |= !isCustomOnChanged && _isAttached && !bindEventsArray.IsEmpty;

        _validationAndCallbacks = _validationAndCallbacks with
        {
            BindEvents = bindEventsArray,
            OnChanged = onChanged,
            Callbacks = new EventCallbackData(
                IsChanged0: c0, IsChanged1: c1, IsChanged2: c2, IsChanged3: c3,
                IsChangedArgs1: ca1, IsChangedArgs2: ca2,
                IsChanging0: ch0, IsChanging1: ch1, IsChanging2: ch2, IsChanging3: ch3)
        };

        return this;
    }

    private string GetTargetSenderType(INamedTypeSymbol? classSymbol)
    {
        if (classSymbol == null)
        {
            return string.Empty;
        }

        var typeString = _isAttached
            ? (_componentModel.BrowsableForType ?? PrepareData.GenerateDependencyObjectType(_framework))
            : classSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

        return typeString.Replace("global::", string.Empty).Replace("?", string.Empty);
    }

    private static EquatableArray<string> GetBindEventsArray(string? bindEvent, TypedConstant bindEvents)
    {
        if (bindEvent != null)
        {
            return new[] { bindEvent }.ToImmutableArray().AsEquatableArray();
        }

        if (bindEvents.Kind == TypedConstantKind.Array)
        {
            var values = bindEvents.Values
                .Select(static value => value.Value?.ToString() ?? string.Empty)
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .ToArray();
            return values.ToImmutableArray().AsEquatableArray();
        }

        return Array.Empty<string>().ToImmutableArray().AsEquatableArray();
    }

    public DependencyPropertyData Build()
    {
        return new DependencyPropertyData(
            Name: _name,
            Version: _version,
            Type: _type,
            ShortType: _shortType,
            IsValueType: _isValueType,
            IsSpecialType: _isSpecialType,
            DefaultValue: _defaultValue,
            DefaultValueDocumentation: _defaultValueDocumentation,
            IsReadOnly: _isReadOnly,
            IsDirect: _isDirect,
            IsAttached: _isAttached,
            IsAddOwner: _isAddOwner,
            Framework: _framework,
            ComponentModel: _componentModel,
            FrameworkMetadata: _frameworkMetadata,
            XmlDocumentation: _xmlDocumentation,
            ValidationAndCallbacks: _validationAndCallbacks
        );
    }
}

