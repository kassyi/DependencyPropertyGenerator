using System.Collections.Immutable;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Kassyi.Generators.Extensions;

namespace Kassyi.Generators.DependencyProperty.Models;

internal static class DependencyPropertyMetadataExtractor
{
    public static ComponentModelData ExtractComponentModel(
        AttributeData attribute,
        Dictionary<string, TypedConstant> namedArgs,
        ComponentModelData initial = default)
    {
        var browsableForTypeSymbol = attribute.GetGenericTypeArgument(1) ??
            (GetNamedArgument(namedArgs, nameof(AttachedDependencyPropertyAttribute.BrowsableForType)).Value as ITypeSymbol);
        var fromTypeSymbol = attribute.GetGenericTypeArgument(1) ??
            (GetNamedArgument(namedArgs, nameof(AddOwnerAttribute.FromType)).Value as ITypeSymbol);

        return new ComponentModelData(BrowsableForType: browsableForTypeSymbol?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat), FromType: fromTypeSymbol?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat), Description: GetNamedArgument(namedArgs, nameof(DependencyPropertyAttribute.Description)).Value?.ToString(), Category: GetNamedArgument(namedArgs, nameof(DependencyPropertyAttribute.Category)).Value?.ToString(), TypeConverter: GetNamedArgument(namedArgs, nameof(DependencyPropertyAttribute.TypeConverter)).Value?.ToString(), Bindable: GetNamedArgument(namedArgs, nameof(DependencyPropertyAttribute.Bindable)).ToNullableBoolean(), Browsable: GetNamedArgument(namedArgs, nameof(DependencyPropertyAttribute.Browsable)).ToNullableBoolean(), DesignerSerializationVisibility: GetNamedArgument(namedArgs, nameof(DependencyPropertyAttribute.DesignerSerializationVisibility)).ToEnum<DesignerSerializationVisibility>()?.ToString("G"), ClsCompliant: GetNamedArgument(namedArgs, nameof(DependencyPropertyAttribute.ClsCompliant)).ToNullableBoolean(), Localizability: GetNamedArgument(namedArgs, nameof(DependencyPropertyAttribute.Localizability)).ToEnum<Localizability>()?.ToString("G"));
    }

    public static FrameworkMetadataData ExtractFrameworkMetadata(Dictionary<string, TypedConstant> namedArgs)
    {
        return new FrameworkMetadataData(
            AffectsMeasure: GetNamedArgument(namedArgs, nameof(DependencyPropertyAttribute.AffectsMeasure)).ToBoolean(),
            AffectsArrange: GetNamedArgument(namedArgs, nameof(DependencyPropertyAttribute.AffectsArrange)).ToBoolean(),
            AffectsParentMeasure: GetNamedArgument(namedArgs, nameof(DependencyPropertyAttribute.AffectsParentMeasure)).ToBoolean(),
            AffectsParentArrange: GetNamedArgument(namedArgs, nameof(DependencyPropertyAttribute.AffectsParentArrange)).ToBoolean(),
            AffectsRender: GetNamedArgument(namedArgs, nameof(DependencyPropertyAttribute.AffectsRender)).ToBoolean(),
            Inherits: GetNamedArgument(namedArgs, nameof(DependencyPropertyAttribute.Inherits)).ToBoolean(),
            OverridesInheritanceBehavior: GetNamedArgument(namedArgs, nameof(DependencyPropertyAttribute.OverridesInheritanceBehavior)).ToBoolean(),
            NotDataBindable: GetNamedArgument(namedArgs, nameof(DependencyPropertyAttribute.NotDataBindable)).ToBoolean(),
            Journal: GetNamedArgument(namedArgs, nameof(DependencyPropertyAttribute.Journal)).ToBoolean(),
            SubPropertiesDoNotAffectRender: GetNamedArgument(namedArgs, nameof(DependencyPropertyAttribute.SubPropertiesDoNotAffectRender)).ToBoolean(),
            IsAnimationProhibited: GetNamedArgument(namedArgs, nameof(DependencyPropertyAttribute.IsAnimationProhibited)).ToBoolean(),
            DefaultUpdateSourceTrigger: GetNamedArgument(namedArgs, nameof(DependencyPropertyAttribute.DefaultUpdateSourceTrigger)).ToEnum<SourceTrigger>()?.ToString("G"),
            DefaultBindingMode: GetNamedArgument(namedArgs, nameof(DependencyPropertyAttribute.DefaultBindingMode)).ToEnum<DefaultBindingMode>()?.ToString("G")
        );
    }

    public static XmlDocumentationData ExtractXmlDocumentation(Dictionary<string, TypedConstant> namedArgs)
    {
        var propertyXmlDoc = GetNamedArgument(namedArgs, nameof(DependencyPropertyAttribute.PropertyXmlDocumentation)).Value?.ToString();
        var getterXmlDoc = GetNamedArgument(namedArgs, nameof(AttachedDependencyPropertyAttribute.GetterXmlDocumentation)).Value?.ToString();

        return new XmlDocumentationData(
            XmlDocumentation: GetNamedArgument(namedArgs, nameof(DependencyPropertyAttribute.XmlDocumentation)).Value?.ToString(),
            GetterXmlDocumentation: getterXmlDoc ?? propertyXmlDoc,
            SetterXmlDocumentation: GetNamedArgument(namedArgs, nameof(AttachedDependencyPropertyAttribute.SetterXmlDocumentation)).Value?.ToString()
        );
    }

    public static ValidationAndCallbackData ExtractValidationFlags(
        Dictionary<string, TypedConstant> namedArgs,
        ValidationAndCallbackData initial = default)
    {
        return initial with
        {
            EnableDataValidation = GetNamedArgument(namedArgs, nameof(DependencyPropertyAttribute.EnableDataValidation)).ToBoolean(),
            Coerce = GetNamedArgument(namedArgs, nameof(DependencyPropertyAttribute.Coerce)).ToBoolean(),
            Validate = GetNamedArgument(namedArgs, nameof(DependencyPropertyAttribute.Validate)).ToBoolean(),
            CreateDefaultValueCallback = GetNamedArgument(namedArgs, nameof(DependencyPropertyAttribute.CreateDefaultValueCallback)).ToBoolean()
        };
    }

    public static ValidationAndCallbackData ExtractCallbacks(
        Dictionary<string, TypedConstant> namedArgs,
        string propertyName,
        string propertyType,
        bool isAttached,
        Framework framework,
        INamedTypeSymbol? classSymbol,
        string? browsableForType,
        ValidationAndCallbackData initial,
        out bool isPartialProperty,
        out bool isRequired,
        out bool isInitOnly)
    {
        var bindEvent = GetNamedArgument(namedArgs, nameof(DependencyPropertyAttribute.BindEvent)).Value?.ToString();
        var bindEvents = GetNamedArgument(namedArgs, nameof(DependencyPropertyAttribute.BindEvents));
        var onChanged = GetNamedArgument(namedArgs, nameof(DependencyPropertyAttribute.OnChanged)).Value?.ToString() ?? string.Empty;

        var isCustomOnChanged = !string.IsNullOrWhiteSpace(onChanged);
        var onChangedName = isCustomOnChanged ? onChanged : $"On{propertyName}Changed";
        var onChangingName = $"On{propertyName}Changing";

        var targetType = propertyType.Replace("global::", string.Empty).Replace("?", string.Empty);
        var targetSenderType = GetTargetSenderType(classSymbol, isAttached, browsableForType, framework);

        isPartialProperty = false;
        isRequired = false;
        isInitOnly = false;

        if (classSymbol != null)
        {
            TryGetPropertyModifiers(classSymbol, propertyName, ref isPartialProperty, ref isRequired, ref isInitOnly);
        }

        var matchChanged = classSymbol != null
            ? PrepareData.CheckMethodsDirectly(classSymbol, onChangedName, targetType, targetSenderType)
            : new MethodSignatureMatch();

        if (!isCustomOnChanged)
        {
            matchChanged.Signatures = isAttached
                ? CallbackSignature.NoParameters | CallbackSignature.NewValue | CallbackSignature.OldAndNewValue | CallbackSignature.SenderAndOldAndNewValue
                : CallbackSignature.NoParameters | CallbackSignature.NewValue | CallbackSignature.OldAndNewValue;
        }

        var matchChanging = classSymbol != null
            ? PrepareData.CheckMethodsDirectly(classSymbol, onChangingName, targetType, targetSenderType)
            : new MethodSignatureMatch();

        matchChanging.Signatures = isAttached
            ? CallbackSignature.NoParameters | CallbackSignature.NewValue | CallbackSignature.OldAndNewValue | CallbackSignature.SenderAndOldAndNewValue
            : CallbackSignature.NoParameters | CallbackSignature.NewValue | CallbackSignature.OldAndNewValue;

        var bindEventsArray = GetBindEventsArray(bindEvent, bindEvents);

        return initial with
        {
            BindEvents = bindEventsArray,
            OnChanged = onChanged,
            Callbacks = new EventCallbackData(
                ChangedSignatures: GetChangedSignatureFlags(matchChanged.Signatures, isCustomOnChanged, isAttached, !bindEventsArray.IsEmpty),
                ChangingSignatures: matchChanging.Signatures
            )
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void TryGetPropertyModifiers(
        INamedTypeSymbol classSymbol,
        string propertyName,
        ref bool isPartialProperty,
        ref bool isRequired,
        ref bool isInitOnly)
    {
        foreach (var syntaxRef in classSymbol.DeclaringSyntaxReferences)
        {
            if (syntaxRef.GetSyntax() is not TypeDeclarationSyntax typeDecl)
            {
                continue;
            }

            foreach (var member in typeDecl.Members)
            {
                if (member is not PropertyDeclarationSyntax p || p.Identifier.Text != propertyName)
                {
                    continue;
                }

                var hasPartial = false;
                foreach (var modifier in p.Modifiers)
                {
                    if (modifier.IsKind(SyntaxKind.PartialKeyword) || modifier.Text == "partial")
                    {
                        hasPartial = true;
                    }
                    else if (modifier.IsKind(SyntaxKind.RequiredKeyword) || modifier.Text == "required")
                    {
                        isRequired = true;
                    }
                }

                if (!hasPartial)
                {
                    continue;
                }

                isPartialProperty = true;

                if (p.AccessorList == null)
                {
                    return;
                }

                foreach (var accessor in p.AccessorList.Accessors)
                {
                    if (!accessor.IsKind(SyntaxKind.InitAccessorDeclaration) && accessor.Keyword.Text != "init")
                    {
                        continue;
                    }

                    isInitOnly = true;
                    break;
                }

                return;
            }
        }
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

    internal static string GetTargetSenderType(INamedTypeSymbol? classSymbol, bool isAttached, string? browsableForType, Framework framework)
    {
        if (classSymbol == null)
        {
            return string.Empty;
        }

        var typeString = isAttached
            ? (browsableForType ?? PrepareData.GenerateDependencyObjectType(framework))
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

    private static TypedConstant GetNamedArgument(Dictionary<string, TypedConstant> namedArgs, string name) =>
        namedArgs.TryGetValue(name, out var value) ? value : default;
}
