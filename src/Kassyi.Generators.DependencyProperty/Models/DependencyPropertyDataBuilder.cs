using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Kassyi.Generators.Extensions;
using Kassyi.Generators.Extensions.Models;
using Kassyi.Generators.DependencyProperty.Diagnostics;
using Kassyi.Generators.DependencyProperty.Rules.Expressions;

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
    private ExpressionSyntax? _parsedDefaultValueExpressionSyntax;
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

    private INamedTypeSymbol? _classSymbol;
    private ITypeSymbol? _typeSymbol;

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
        _classSymbol = classSymbol;

        _name = attribute.ConstructorArguments is { Length: > 0 } ctorArgs
            ? ctorArgs[0].Value?.ToString()?.TrimStart('@') ?? string.Empty
            : string.Empty;

        var typeSymbol = attribute.GetGenericTypeArgument(0) ??
            (attribute.ConstructorArguments is { Length: > 1 } ctorArgs2 ? ctorArgs2[1].Value as ITypeSymbol : null);
        _typeSymbol = typeSymbol;
        
        if (typeSymbol is { IsRefLikeType: true })
        {
            var location = attribute.ApplicationSyntaxReference?.GetSyntax().GetLocation() ?? Location.None;
            throw new DiagnosticException(
                Diagnostic.Create(DiagnosticDescriptors.RefStructPropertyTypeNotSupported, location, typeSymbol.ToDisplayString()));
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

    public DependencyPropertyDataBuilder WithDefaultValues(
        AttributeData attribute,
        AttributeSyntax? attributeSyntax,
        SemanticModel? semanticModel = null)
    {
        ExtractDefaultValueStrings(attributeSyntax);
        ValidateReferenceTypeDefaultValue(attribute, attributeSyntax, semanticModel);

        return this;
    }

    private void ExtractDefaultValueStrings(AttributeSyntax? attributeSyntax)
    {
        var defaultValueExpression = GetNamedArgument(nameof(DependencyPropertyAttribute.DefaultValueExpression)).Value?.ToString();
        var defaultValue = defaultValueExpression ?? GetNamedArgument(nameof(DependencyPropertyAttribute.DefaultValue)).Value?.ToString();
        var defaultValueDoc = defaultValueExpression ?? attributeSyntax?.GetNamedArgumentExpression(nameof(DependencyPropertyAttribute.DefaultValue));

        if (defaultValueExpression != null && _parsedDefaultValueExpressionSyntax == null)
        {
            try
            {
                _parsedDefaultValueExpressionSyntax = Microsoft.CodeAnalysis.CSharp.SyntaxFactory.ParseExpression(defaultValueExpression);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
            }
        }

        var directExpressionSyntax = attributeSyntax?.GetNamedArgumentExpressionSyntax(nameof(DependencyPropertyAttribute.DefaultValue));
        var expressionSyntax = _parsedDefaultValueExpressionSyntax ?? directExpressionSyntax;

        _defaultValue = PrepareData.ExpandDefaultValueExpression(expressionSyntax, defaultValue, _typeSymbol);
        _defaultValueDocumentation = PrepareData.ExpandDefaultValueExpression(expressionSyntax, defaultValueDoc, _typeSymbol);
    }

    private void ValidateReferenceTypeDefaultValue(
        AttributeData attribute,
        AttributeSyntax? attributeSyntax,
        SemanticModel? semanticModel)
    {
        int? position = attributeSyntax?.GetLocation().SourceSpan.Start;
        var directExpressionSyntax = attributeSyntax?.GetNamedArgumentExpressionSyntax(nameof(DependencyPropertyAttribute.DefaultValue));

        var expressionToAnalyze = _parsedDefaultValueExpressionSyntax ?? directExpressionSyntax;

        var isReferenceType = expressionToAnalyze != null
            ? DefaultValueExpressionAnalyzer.IsReferenceTypeExpression(expressionToAnalyze, _typeSymbol, _classSymbol, semanticModel, position)
            : DefaultValueExpressionAnalyzer.IsConservativeReferenceTypeFallback(_defaultValue, _typeSymbol);

        if (!isReferenceType)
        {
            return;
        }

        var location = attributeSyntax?.GetLocation() ?? attribute.ApplicationSyntaxReference?.GetSyntax().GetLocation() ?? Location.None;
        throw new DiagnosticException(Diagnostic.Create(DiagnosticDescriptors.ReferenceTypeDefaultValueSharing, location, _defaultValue));
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
            throw new DiagnosticException(Diagnostic.Create(DiagnosticDescriptors.UnsupportedCallbackSignature, methodLocation, onChangedName));
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
        throw new DiagnosticException(Diagnostic.Create(DiagnosticDescriptors.OverrideMetadataOldAndNewValueNotSupported, location, _framework.ToString()));
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
