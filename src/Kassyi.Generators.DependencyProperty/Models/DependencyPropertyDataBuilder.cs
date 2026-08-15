using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;
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
    private bool _isRequired;
    private bool _isInitOnly;

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

        _componentModel = DependencyPropertyMetadataExtractor.ExtractComponentModel(attribute, _namedArguments, _componentModel);

        if (_isAttached || classSymbol == null)
        {
            return this;
        }

        // [WHY] Avoid LINQ Any() to eliminate delegate and enumerator allocations during metadata extraction.
        _hidesBaseProperty = CheckHidesBaseProperty(classSymbol, _name);

        return this;
    }

    public DependencyPropertyDataBuilder WithMetadata(AttributeData attribute)
    {
        _componentModel = DependencyPropertyMetadataExtractor.ExtractComponentModel(attribute, _namedArguments, _componentModel);
        _frameworkMetadata = DependencyPropertyMetadataExtractor.ExtractFrameworkMetadata(_namedArguments);
        _validationAndCallbacks = DependencyPropertyMetadataExtractor.ExtractValidationFlags(_namedArguments, _validationAndCallbacks);

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

        var trimmedDefaultValue = _defaultValue?.Trim();
        var isCollectionExpression = trimmedDefaultValue != null && trimmedDefaultValue.StartsWith("[", StringComparison.Ordinal) && trimmedDefaultValue.EndsWith("]", StringComparison.Ordinal);

        if (_defaultValue == null ||
            (!_defaultValue.Contains("new ") && !_defaultValue.Contains("new(") && !isCollectionExpression) ||
            _isValueType ||
            _type == "string" || _type == "global::System.String")
        {
            return this;
        }

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

    public DependencyPropertyDataBuilder WithXmlDocumentation(AttributeData attribute)
    {
        _xmlDocumentation = DependencyPropertyMetadataExtractor.ExtractXmlDocumentation(_namedArguments);
        return this;
    }

    public DependencyPropertyDataBuilder WithCallbacks(AttributeData attribute, INamedTypeSymbol? classSymbol)
    {
        _validationAndCallbacks = DependencyPropertyMetadataExtractor.ExtractCallbacks(
            _namedArguments,
            _name,
            _type,
            _isAttached,
            _framework,
            classSymbol,
            _componentModel.BrowsableForType,
            _validationAndCallbacks,
            out _isPartialProperty,
            out _isRequired,
            out _isInitOnly);

        if (classSymbol == null)
        {
            return this;
        }

        var isCustomOnChanged = !string.IsNullOrWhiteSpace(_validationAndCallbacks.OnChanged);
        var onChangedName = isCustomOnChanged ? _validationAndCallbacks.OnChanged : $"On{_name}Changed";
        var targetType = _type.Replace("global::", string.Empty).Replace("?", string.Empty);
        var targetSenderType = DependencyPropertyMetadataExtractor.GetTargetSenderType(classSymbol, _isAttached, _componentModel.BrowsableForType, _framework);

        var matchChanged = PrepareData.CheckMethodsDirectly(classSymbol, onChangedName, targetType, targetSenderType);
        
        if (matchChanged.HasMethod && matchChanged.Signatures == CallbackSignature.None)
        {
            var methodLocation = classSymbol.Locations.FirstOrDefault() ?? Location.None;
            if (attribute.ApplicationSyntaxReference?.GetSyntax() is { } syntaxForLocation)
            {
                methodLocation = syntaxForLocation.GetLocation();
            }
            var unsupportedDescriptor = new DiagnosticDescriptor(
                id: "DPG0007",
                title: "Unsupported Callback Signature",
                messageFormat: "Method '{0}' matches the naming convention but has an unsupported signature.",
                category: "Usage",
                defaultSeverity: DiagnosticSeverity.Error,
                isEnabledByDefault: true);
            throw new DiagnosticException(Diagnostic.Create(unsupportedDescriptor, methodLocation, onChangedName));
        }

        var isOverrideMetadata = attribute.AttributeClass?.Name.Contains("OverrideMetadata") == true;

        if (_framework is not (Framework.Uwp or Framework.WinUi or Framework.Uno or Framework.UnoWinUi or Framework.Maui) ||
            !isOverrideMetadata)
        {
            return this;
        }

        if (!matchChanged.Signatures.HasFlag(CallbackSignature.OldAndNewValue) &&
            !(_isAttached && matchChanged.Signatures.HasFlag(CallbackSignature.SenderAndOldAndNewValue)))
        {
            return this;
        }

        var location = classSymbol.Locations.FirstOrDefault() ?? Location.None;
        if (attribute.ApplicationSyntaxReference?.GetSyntax() is { } syntax)
        {
            location = syntax.GetLocation();
        }
        var descriptor = new DiagnosticDescriptor(
            id: "DPG0005",
            title: "Invalid Callback Signature",
            messageFormat: "The OldAndNewValue signature is not supported for OverrideMetadata in {0} because RegisterPropertyChangedCallback does not provide the old value",
            category: "Usage",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true);
        throw new DiagnosticException(Diagnostic.Create(descriptor, location, _framework.ToString()));
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
            HidesBaseProperty: _hidesBaseProperty,
            IsRequired: _isRequired,
            IsInitOnly: _isInitOnly
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool CheckHidesBaseProperty(INamedTypeSymbol classSymbol, string name)
    {
        var baseType = classSymbol.BaseType;
        while (baseType != null)
        {
            foreach (var member in baseType.GetMembers(name))
            {
                if (member is IPropertySymbol)
                {
                    return true;
                }
            }
            baseType = baseType.BaseType;
        }
        return false;
    }
}
