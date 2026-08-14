using System.Collections.Immutable;
using System.ComponentModel;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Kassyi.Generators.Extensions;
using Kassyi.Generators.Extensions.Models;

namespace Kassyi.Generators.DependencyProperty.Models;

internal sealed class DependencyPropertyDataBuilder
{
    private readonly Dictionary<string, TypedConstant> _namedArguments = new(StringComparer.Ordinal);
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
    private bool _isPartialProperty;
    private bool _hidesBaseProperty;

    private TypedConstant GetNamedArgument(string name) => _namedArguments.TryGetValue(name, out var value) ? value : default;

    public DependencyPropertyDataBuilder WithCoreProperties(AttributeData attribute, Framework framework, string version, bool isAddOwner, bool isAttached, INamedTypeSymbol? classSymbol = null)
    {
        // [WHY] Pre-cache named arguments in a dictionary to achieve O(1) lookups instead of repeated linear O(N) searches across multiple With* methods.
        _namedArguments.Clear();
        foreach (var pair in attribute.NamedArguments)
        {
            _namedArguments[pair.Key] = pair.Value;
        }

        _framework = framework;
        _version = version;
        _isAddOwner = isAddOwner;
        _isAttached = isAttached;

        _name = attribute.ConstructorArguments is { Length: > 0 } ctorArgs
            ? ctorArgs[0].Value?.ToString()?.TrimStart('@') ?? string.Empty
            : string.Empty;

        var typeSymbol = attribute.GetGenericTypeArgument(0) ??
            (attribute.ConstructorArguments is { Length: > 1 } ctorArgs2 ? ctorArgs2[1].Value as ITypeSymbol : null);
        
        if (typeSymbol is { IsRefLikeType: true })
        {
            var location = attribute.ApplicationSyntaxReference?.GetSyntax().GetLocation() ?? Location.None;
            var descriptor = new DiagnosticDescriptor(
                id: "DPG0003",
                title: "Invalid Property Type",
                messageFormat: "The property type '{0}' is a ref struct and cannot be used as a DependencyProperty",
                category: "Usage",
                defaultSeverity: DiagnosticSeverity.Error,
                isEnabledByDefault: true);
            throw new DiagnosticException(
                Diagnostic.Create(descriptor, location, typeSymbol.ToDisplayString()));
        }

        _type = typeSymbol?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) ?? string.Empty;
        _shortType = typeSymbol?.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat) ?? string.Empty;
        _isValueType = typeSymbol?.IsValueType ?? true;
        _isSpecialType = typeSymbol.IsSpecialType() ?? false;
        
        _isReadOnly = GetNamedArgument(nameof(DependencyPropertyAttribute.IsReadOnly)).ToBoolean();
        _isDirect = GetNamedArgument(nameof(DependencyPropertyAttribute.IsDirect)).ToBoolean();

        var browsableForTypeSymbol = attribute.GetGenericTypeArgument(1) ??
            (GetNamedArgument(nameof(AttachedDependencyPropertyAttribute.BrowsableForType)).Value as ITypeSymbol);
        var fromTypeSymbol = attribute.GetGenericTypeArgument(1) ??
            (GetNamedArgument(nameof(AddOwnerAttribute.FromType)).Value as ITypeSymbol);

        _componentModel = _componentModel with
        {
            BrowsableForType = browsableForTypeSymbol?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
            FromType = fromTypeSymbol?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
        };

        if (!_isAttached && classSymbol != null)
        {
            var baseType = classSymbol.BaseType;
            while (baseType != null)
            {
                if (baseType.GetMembers(_name).Any(m => m is IPropertySymbol))
                {
                    _hidesBaseProperty = true;
                    break;
                }
                baseType = baseType.BaseType;
            }
        }

        return this;
    }

    public DependencyPropertyDataBuilder WithMetadata(AttributeData attribute)
    {
        _componentModel = _componentModel with
        {
            Description = GetNamedArgument(nameof(DependencyPropertyAttribute.Description)).Value?.ToString(),
            Category = GetNamedArgument(nameof(DependencyPropertyAttribute.Category)).Value?.ToString(),
            TypeConverter = GetNamedArgument(nameof(DependencyPropertyAttribute.TypeConverter)).Value?.ToString(),
            Bindable = GetNamedArgument(nameof(DependencyPropertyAttribute.Bindable)).ToNullableBoolean(),
            Browsable = GetNamedArgument(nameof(DependencyPropertyAttribute.Browsable)).ToNullableBoolean(),
            DesignerSerializationVisibility = GetNamedArgument(nameof(DependencyPropertyAttribute.DesignerSerializationVisibility)).ToEnum<DesignerSerializationVisibility>()?.ToString("G"),
            ClsCompliant = GetNamedArgument(nameof(DependencyPropertyAttribute.ClsCompliant)).ToNullableBoolean(),
            Localizability = GetNamedArgument(nameof(DependencyPropertyAttribute.Localizability)).ToEnum<Localizability>()?.ToString("G")
        };

        _frameworkMetadata = new FrameworkMetadataData(
            AffectsMeasure: GetNamedArgument(nameof(DependencyPropertyAttribute.AffectsMeasure)).ToBoolean(),
            AffectsArrange: GetNamedArgument(nameof(DependencyPropertyAttribute.AffectsArrange)).ToBoolean(),
            AffectsParentMeasure: GetNamedArgument(nameof(DependencyPropertyAttribute.AffectsParentMeasure)).ToBoolean(),
            AffectsParentArrange: GetNamedArgument(nameof(DependencyPropertyAttribute.AffectsParentArrange)).ToBoolean(),
            AffectsRender: GetNamedArgument(nameof(DependencyPropertyAttribute.AffectsRender)).ToBoolean(),
            Inherits: GetNamedArgument(nameof(DependencyPropertyAttribute.Inherits)).ToBoolean(),
            OverridesInheritanceBehavior: GetNamedArgument(nameof(DependencyPropertyAttribute.OverridesInheritanceBehavior)).ToBoolean(),
            NotDataBindable: GetNamedArgument(nameof(DependencyPropertyAttribute.NotDataBindable)).ToBoolean(),
            Journal: GetNamedArgument(nameof(DependencyPropertyAttribute.Journal)).ToBoolean(),
            SubPropertiesDoNotAffectRender: GetNamedArgument(nameof(DependencyPropertyAttribute.SubPropertiesDoNotAffectRender)).ToBoolean(),
            IsAnimationProhibited: GetNamedArgument(nameof(DependencyPropertyAttribute.IsAnimationProhibited)).ToBoolean(),
            DefaultUpdateSourceTrigger: GetNamedArgument(nameof(DependencyPropertyAttribute.DefaultUpdateSourceTrigger)).ToEnum<SourceTrigger>()?.ToString("G"),
            DefaultBindingMode: GetNamedArgument(nameof(DependencyPropertyAttribute.DefaultBindingMode)).ToEnum<DefaultBindingMode>()?.ToString("G")
        );

        _validationAndCallbacks = _validationAndCallbacks with
        {
            EnableDataValidation = GetNamedArgument(nameof(DependencyPropertyAttribute.EnableDataValidation)).ToBoolean(),
            Coerce = GetNamedArgument(nameof(DependencyPropertyAttribute.Coerce)).ToBoolean(),
            Validate = GetNamedArgument(nameof(DependencyPropertyAttribute.Validate)).ToBoolean(),
            CreateDefaultValueCallback = GetNamedArgument(nameof(DependencyPropertyAttribute.CreateDefaultValueCallback)).ToBoolean()
        };

        return this;
    }

    public DependencyPropertyDataBuilder WithDefaultValues(AttributeData attribute, AttributeSyntax? attributeSyntax)
    {
        var typeSymbol = attribute.GetGenericTypeArgument(0) ??
            (attribute.ConstructorArguments is { Length: > 1 } ctorArgs ? ctorArgs[1].Value as ITypeSymbol : null);
        
        var defaultValue = GetNamedArgument(nameof(DependencyPropertyAttribute.DefaultValueExpression)).Value?.ToString() ??
                           GetNamedArgument(nameof(DependencyPropertyAttribute.DefaultValue)).Value?.ToString();
                           
        var defaultValueDoc = GetNamedArgument(nameof(DependencyPropertyAttribute.DefaultValueExpression)).Value?.ToString() ??
                              attributeSyntax?.GetNamedArgumentExpression(nameof(DependencyPropertyAttribute.DefaultValue));

        _defaultValue = PrepareData.ExpandDefaultValueExpression(defaultValue, typeSymbol);
        _defaultValueDocumentation = PrepareData.ExpandDefaultValueExpression(defaultValueDoc, typeSymbol);

        if (_defaultValue != null && 
            (_defaultValue.Contains("new ") || _defaultValue.Contains("new(")) && 
            !_isValueType && 
            _type != "string" && _type != "global::System.String")
        {
            var location = attributeSyntax?.GetLocation() ?? attribute.ApplicationSyntaxReference?.GetSyntax().GetLocation() ?? Location.None;
            var descriptor = new DiagnosticDescriptor(
                id: "DPG0004",
                title: "Reference Type Default Value Sharing",
                messageFormat: "Default value '{0}' is a reference type and will be shared across all instances. Use CreateDefaultValueCallback = true instead.",
                category: "Usage",
                defaultSeverity: DiagnosticSeverity.Error,
                isEnabledByDefault: true);
            throw new DiagnosticException(
                Diagnostic.Create(descriptor, location, _defaultValue));
        }

        return this;
    }

    public DependencyPropertyDataBuilder WithXmlDocumentation(AttributeData attribute)
    {
        var propertyXmlDoc = GetNamedArgument(nameof(DependencyPropertyAttribute.PropertyXmlDocumentation)).Value?.ToString();
        var getterXmlDoc = GetNamedArgument(nameof(AttachedDependencyPropertyAttribute.GetterXmlDocumentation)).Value?.ToString();
        
        _xmlDocumentation = new XmlDocumentationData(
            XmlDocumentation: GetNamedArgument(nameof(DependencyPropertyAttribute.XmlDocumentation)).Value?.ToString(),
            GetterXmlDocumentation: getterXmlDoc ?? propertyXmlDoc,
            SetterXmlDocumentation: GetNamedArgument(nameof(AttachedDependencyPropertyAttribute.SetterXmlDocumentation)).Value?.ToString()
        );
        
        return this;
    }

    public DependencyPropertyDataBuilder WithCallbacks(AttributeData attribute, INamedTypeSymbol? classSymbol)
    {
        var bindEvent = GetNamedArgument(nameof(DependencyPropertyAttribute.BindEvent)).Value?.ToString();
        var bindEvents = GetNamedArgument(nameof(DependencyPropertyAttribute.BindEvents));
        var onChanged = GetNamedArgument(nameof(DependencyPropertyAttribute.OnChanged)).Value?.ToString() ?? string.Empty;

        var isCustomOnChanged = !string.IsNullOrWhiteSpace(onChanged);
        var onChangedName = isCustomOnChanged ? onChanged : $"On{_name}Changed";
        var onChangingName = $"On{_name}Changing";

        var targetType = _type.Replace("global::", string.Empty).Replace("?", string.Empty);
        var targetSenderType = GetTargetSenderType(classSymbol);

        _isPartialProperty = classSymbol != null && classSymbol.DeclaringSyntaxReferences
            .Select(x => x.GetSyntax())
            .OfType<TypeDeclarationSyntax>()
            .SelectMany(c => c.Members)
            .OfType<PropertyDeclarationSyntax>()
            .Any(p => p.Identifier.Text == _name && p.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword) || m.Text == "partial"));

        var matchChanged = classSymbol != null
            ? PrepareData.CheckMethodsDirectly(classSymbol, onChangedName, targetType, targetSenderType)
            : new MethodSignatureMatch();

        if (!isCustomOnChanged)
        {
            matchChanged.Signatures = _isAttached
                ? CallbackSignature.NoParameters | CallbackSignature.NewValue | CallbackSignature.OldAndNewValue | CallbackSignature.SenderAndOldAndNewValue
                : CallbackSignature.NoParameters | CallbackSignature.NewValue | CallbackSignature.OldAndNewValue;
        }

        var matchChanging = classSymbol != null
            ? PrepareData.CheckMethodsDirectly(classSymbol, onChangingName, targetType, targetSenderType)
            : new MethodSignatureMatch();

        matchChanging.Signatures = _isAttached
            ? CallbackSignature.NoParameters | CallbackSignature.NewValue | CallbackSignature.OldAndNewValue | CallbackSignature.SenderAndOldAndNewValue
            : CallbackSignature.NoParameters | CallbackSignature.NewValue | CallbackSignature.OldAndNewValue;

        var bindEventsArray = GetBindEventsArray(bindEvent, bindEvents);

        _validationAndCallbacks = _validationAndCallbacks with
        {
            BindEvents = bindEventsArray,
            OnChanged = onChanged,
            Callbacks = new EventCallbackData(
                ChangedSignatures: GetChangedSignatureFlags(matchChanged.Signatures, isCustomOnChanged, _isAttached, !bindEventsArray.IsEmpty),
                ChangingSignatures: matchChanging.Signatures
            )
        };
        return this;
    }

    private static CallbackSignature GetChangedSignatureFlags(
        CallbackSignature currentSignatures,
        bool isCustomOnChanged,
        bool isAttached,
        bool hasBindEvents)
    {
        var signatures = currentSignatures;
        if (!isCustomOnChanged && hasBindEvents)
        {
            signatures |= isAttached
                ? CallbackSignature.SenderAndOldAndNewValue
                : CallbackSignature.OldAndNewValue;
        }
        return signatures;
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

        // [WHY] Avoid LINQ Select/Where chains to prevent array and enumerator allocations.
        if (bindEvents is { Kind: TypedConstantKind.Array, Values.IsDefaultOrEmpty: false })
        {
            var builder = ImmutableArray.CreateBuilder<string>(bindEvents.Values.Length);
            foreach (var value in bindEvents.Values)
            {
                var str = value.Value?.ToString();
                if (!string.IsNullOrWhiteSpace(str))
                {
                    builder.Add(str!);
                }
            }
            return builder.ToImmutable().AsEquatableArray();
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
            ValidationAndCallbacks: _validationAndCallbacks,
            IsPartialProperty: _isPartialProperty,
            HidesBaseProperty: _hidesBaseProperty
        );
    }
}

