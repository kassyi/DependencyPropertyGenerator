using System.Runtime.CompilerServices;
using Kassyi.Generators.DependencyProperty.Diagnostics;
using Kassyi.Generators.DependencyProperty.Rules;
using Kassyi.Generators.DependencyProperty.Rules.Expressions;
using Kassyi.Generators.Extensions;
using Kassyi.Generators.Extensions.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Kassyi.Generators.DependencyProperty.Models;

/// <summary>Builder class for constructing dependency property metadata across multiple passes to minimize allocations.</summary>
internal sealed class DependencyPropertyDataBuilder
{
    private readonly Dictionary<string, TypedConstant> _namedArguments = new(StringComparer.Ordinal);
    private string _name = string.Empty;
    private string _version = string.Empty;
    private string _type = string.Empty;
    private string _shortType = string.Empty;
    private string? _defaultValue;
    private string? _defaultValueDocumentation;
    private Framework _framework;
    
    private ComponentModelData _componentModel;
    private FrameworkMetadataData _frameworkMetadata;
    private XmlDocumentationData _xmlDocumentation;
    private ValidationAndCallbackData _validationAndCallbacks;
    private PropertyModifiersData _modifiers = new(IsValueType: true);

    private INamedTypeSymbol? _classSymbol;
    private ITypeSymbol? _typeSymbol;

    private TypedConstant GetNamedArgument(string name) 
        => _namedArguments.TryGetValue(name, out var value) ? value : default;

    /// <summary>Initializes core property state, evaluating type symbols and initial modifiers.</summary>
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

        // [WHY] Avoid LINQ Any() to eliminate delegate and enumerator allocations during metadata extraction.
        var hidesBaseProperty = !isAttached && classSymbol != null && CheckHidesBaseProperty(classSymbol, _name);

        _modifiers = new PropertyModifiersData(
            IsValueType: typeSymbol?.IsValueType ?? true,
            IsSpecialType: typeSymbol.IsSpecialType() ?? false,
            IsReadOnly: GetNamedArgument(nameof(DependencyPropertyAttribute.IsReadOnly)).ToBoolean(),
            IsDirect: GetNamedArgument(nameof(DependencyPropertyAttribute.IsDirect)).ToBoolean(),
            IsAttached: isAttached,
            IsAddOwner: isAddOwner,
            HidesBaseProperty: hidesBaseProperty);

        _componentModel = DependencyPropertyMetadataExtractor.ExtractComponentModel(attribute, _namedArguments, _componentModel);

        return this;
    }

    /// <summary>Extracts framework metadata and validation flags from the provided attribute.</summary>
    public DependencyPropertyDataBuilder WithMetadata(AttributeData attribute)
    {
        _componentModel = DependencyPropertyMetadataExtractor.ExtractComponentModel(attribute, _namedArguments, _componentModel);
        _frameworkMetadata = DependencyPropertyMetadataExtractor.ExtractFrameworkMetadata(_namedArguments);
        _validationAndCallbacks = DependencyPropertyMetadataExtractor.ExtractValidationFlags(_namedArguments, _validationAndCallbacks);

        return this;
    }

    /// <summary>Parses default value expressions, resolving AST strings safely without runtime evaluation overhead.</summary>
    public DependencyPropertyDataBuilder WithDefaultValues(
        AttributeData attribute,
        AttributeSyntax? attributeSyntax,
        SemanticModel? semanticModel = null)
    {
        var defaultValueExpression = GetNamedArgument(nameof(DependencyPropertyAttribute.DefaultValueExpression)).Value?.ToString();
        var defaultValue = defaultValueExpression ?? GetNamedArgument(nameof(DependencyPropertyAttribute.DefaultValue)).Value?.ToString();
        var defaultValueDoc = defaultValueExpression ?? attributeSyntax?.GetNamedArgumentExpression(nameof(DependencyPropertyAttribute.DefaultValue));

        var (directExpressionSyntax, defaultValueDocSyntax, parseFailed) = ParseDefaultValueExpressions(defaultValueExpression, attributeSyntax);

        _defaultValue = PrepareData.ExpandDefaultValueExpression(defaultValue, directExpressionSyntax, _typeSymbol);
        _defaultValueDocumentation = PrepareData.ExpandDefaultValueExpression(defaultValueDoc, defaultValueDocSyntax, _typeSymbol);

        if (parseFailed)
        {
            var parseErrorLocation = attributeSyntax?.GetLocation() ?? attribute.ApplicationSyntaxReference?.GetSyntax().GetLocation() ?? Location.None;
            throw new DiagnosticException(Diagnostic.Create(
                DiagnosticDescriptors.InvalidDefaultValueExpression,
                parseErrorLocation,
                defaultValueExpression));
        }

        var isReferenceType = false;
        if (directExpressionSyntax != null)
        {
            var position = attributeSyntax?.GetLocation().SourceSpan.Start;
            isReferenceType = DefaultValueExpressionAnalyzer.IsReferenceTypeExpression(directExpressionSyntax, _typeSymbol, _classSymbol, semanticModel, position);
        }

        if (!isReferenceType)
        {
            return this;
        }

        var refTypeErrorLocation = attributeSyntax?.GetLocation() ?? attribute.ApplicationSyntaxReference?.GetSyntax().GetLocation() ?? Location.None;
        throw new DiagnosticException(Diagnostic.Create(DiagnosticDescriptors.ReferenceTypeDefaultValueSharing, refTypeErrorLocation, _defaultValue));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static (ExpressionSyntax? DirectExpr, ExpressionSyntax? DocExpr, bool ParseFailed) ParseDefaultValueExpressions(
        string? defaultValueExpression,
        AttributeSyntax? attributeSyntax)
    {
        ExpressionSyntax? directExpressionSyntax = null;
        var parseFailed = false;

        if (defaultValueExpression != null)
        {
            try 
            { 
                directExpressionSyntax = SyntaxFactory.ParseExpression(defaultValueExpression); 
                if (directExpressionSyntax.ContainsDiagnostics)
                {
                    parseFailed = true;
                }
            } 
            catch 
            { 
                // [WHY] If SyntaxFactory throws during parsing, we assume it's a parse failure and fall back to attribute syntax.
                parseFailed = true;
            }
        }
        else
        {
            directExpressionSyntax = attributeSyntax?.GetNamedArgumentExpressionSyntax(nameof(DependencyPropertyAttribute.DefaultValue));
        }

        var defaultValueDocSyntax = defaultValueExpression != null 
            ? directExpressionSyntax 
            : attributeSyntax?.GetNamedArgumentExpressionSyntax(nameof(DependencyPropertyAttribute.DefaultValue));

        return (directExpressionSyntax, defaultValueDocSyntax, parseFailed);
    }

    /// <summary>Extracts XML documentation overrides from attribute arguments.</summary>
    public DependencyPropertyDataBuilder WithXmlDocumentation(AttributeData attribute)
    {
        _xmlDocumentation = DependencyPropertyMetadataExtractor.ExtractXmlDocumentation(_namedArguments);
        return this;
    }

    /// <summary>Analyzes the declaring class to validate and extract callback method signatures.</summary>
    public DependencyPropertyDataBuilder WithCallbacks(AttributeData attribute, INamedTypeSymbol? classSymbol)
    {
        _validationAndCallbacks = DependencyPropertyMetadataExtractor.ExtractCallbacks(
            _namedArguments,
            _name,
            _type,
            _modifiers.IsAttached,
            _framework,
            classSymbol,
            _componentModel.BrowsableForType,
            _validationAndCallbacks,
            out var isPartialProperty,
            out var isRequired,
            out var isInitOnly);

        _modifiers = _modifiers with
        {
            IsPartialProperty = isPartialProperty,
            IsRequired = isRequired,
            IsInitOnly = isInitOnly
        };

        if (classSymbol == null)
        {
            return this;
        }

        var isCustomOnChanged = !string.IsNullOrWhiteSpace(_validationAndCallbacks.OnChanged);
        var onChangedName = isCustomOnChanged ? _validationAndCallbacks.OnChanged : $"On{_name}Changed";
        var targetType = _typeSymbol != null
            ? SignatureRuleHelper.GetNormalizedTypeName(_typeSymbol)
            : SignatureRuleHelper.NormalizeTypeName(_type);
        var targetSenderType = DependencyPropertyMetadataExtractor.GetTargetSenderType(classSymbol, _modifiers.IsAttached, _componentModel.BrowsableForType, _framework);

        var matchChanged = PrepareData.CheckMethodsDirectly(classSymbol, onChangedName, targetType, targetSenderType);
        
        if (matchChanged is { HasMethod: true, Signatures: CallbackSignature.None })
        {
            var methodLocation = classSymbol.Locations.FirstOrDefault() ?? Location.None;
            if (attribute.ApplicationSyntaxReference?.GetSyntax() is { } syntaxForLocation)
            {
                methodLocation = syntaxForLocation.GetLocation();
            }
            throw new DiagnosticException(Diagnostic.Create(DiagnosticDescriptors.UnsupportedCallbackSignature, methodLocation, onChangedName));
        }

        ValidateOverrideMetadataCallback(attribute, classSymbol, matchChanged);

        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ValidateOverrideMetadataCallback(
        AttributeData attribute,
        INamedTypeSymbol classSymbol,
        MethodSignatureMatch matchChanged)
    {
        var isOverrideMetadata = attribute.AttributeClass?.Name is nameof(OverrideMetadataAttribute) or "OverrideMetadata";

        if (_framework is not (Framework.Uwp or Framework.WinUi or Framework.Uno or Framework.UnoWinUi or Framework.Maui) ||
            !isOverrideMetadata)
        {
            return;
        }

        if (!matchChanged.Signatures.HasFlag(CallbackSignature.OldAndNewValue) &&
            !(_modifiers.IsAttached && matchChanged.Signatures.HasFlag(CallbackSignature.SenderAndOldAndNewValue)))
        {
            return;
        }

        var location = classSymbol.Locations.FirstOrDefault() ?? Location.None;
        if (attribute.ApplicationSyntaxReference?.GetSyntax() is { } syntax)
        {
            location = syntax.GetLocation();
        }
        throw new DiagnosticException(Diagnostic.Create(DiagnosticDescriptors.OverrideMetadataOldAndNewValueNotSupported, location, _framework.ToString()));
    }

    /// <summary>Constructs the immutable dependency property data record.</summary>
    public DependencyPropertyData Build()
    {
        return new DependencyPropertyData(
            Name: _name,
            Version: _version,
            Type: _type,
            ShortType: _shortType,
            DefaultValue: _defaultValue,
            DefaultValueDocumentation: _defaultValueDocumentation,
            Framework: _framework,
            ComponentModel: _componentModel,
            FrameworkMetadata: _frameworkMetadata,
            XmlDocumentation: _xmlDocumentation,
            ValidationAndCallbacks: _validationAndCallbacks,
            Modifiers: _modifiers
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
