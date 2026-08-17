using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Kassyi.Generators.DependencyProperty.SnapshotTests;

internal sealed class FrameworkSyntaxRewriter : CSharpSyntaxRewriter
{
    private readonly Framework _framework;

    public FrameworkSyntaxRewriter(Framework framework)
    {
        _framework = framework;
    }

    public override SyntaxNode? VisitIdentifierName(IdentifierNameSyntax node)
    {
        var name = node.Identifier.ValueText;
        var replacement = GetReplacement(name);
        
        if (replacement != null && replacement != name)
        {
            if (replacement.StartsWith("global::"))
            {
                return SyntaxFactory.ParseName(replacement).WithTriviaFrom(node);
            }
            return SyntaxFactory.IdentifierName(replacement).WithTriviaFrom(node);
        }
        return base.VisitIdentifierName(node);
    }

    private string? GetReplacement(string originalName)
    {
        return _framework switch
        {
            Framework.Wpf => originalName switch
            {
                "PointerEntered" => "MouseEnter",
                "PointerExited" => "MouseLeave",
                "PointerRoutedEventArgs" => "MouseEventArgs",
                _ => originalName
            },
            Framework.Uno or Framework.UnoWinUi or Framework.WinUi or Framework.Uwp => originalName switch
            {
                "KeyEventArgs" => "KeyRoutedEventArgs",
                _ => originalName
            },
            Framework.Avalonia => originalName switch
            {
                "DispatcherObject" or "DependencyObject" => "global::Avalonia.AvaloniaObject",
                "Visual" => "global::Avalonia.Interactivity.Interactive",
                "UIElement" => "global::Avalonia.Input.InputElement",
                "FrameworkElement" => "global::Avalonia.Controls.Control",
                "PointerRoutedEventArgs" => "PointerEventArgs",
                "Brush" => "IBrush",
                _ => originalName
            },
            Framework.Maui => originalName switch
            {
                "UIElement" or "FrameworkElement" => "VisualElement",
                "TreeView" or "UserControl" => "Grid",
                "TextBox" => "Entry",
                "MyControl" => "MyGrid",
                "KeyUp" => "SizeChanged",
                "KeyEventArgs" or "PointerRoutedEventArgs" => "global::System.EventArgs",
                "PointerEntered" => "Loaded",
                "PointerExited" => "Unloaded",
                _ => originalName
            },
            _ => originalName
        };
    }
}
