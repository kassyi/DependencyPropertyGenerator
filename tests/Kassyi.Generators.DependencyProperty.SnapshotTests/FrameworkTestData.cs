namespace Kassyi.Generators.DependencyProperty.SnapshotTests;

public static class FrameworkTestData
{
    public static string GetUIElement(Framework framework)
    {
        return framework switch
        {
            Framework.Maui => "VisualElement",
            Framework.Avalonia => "global::Avalonia.Input.InputElement",
            _ => "UIElement"
        };
    }
    
    public static string GetUserControl(Framework framework)
    {
        return framework switch
        {
            Framework.Maui => "Grid",
            _ => "UserControl"
        };
    }
    
    public static string GetFrameworkElement(Framework framework)
    {
        return framework switch
        {
            Framework.Maui => "VisualElement",
            Framework.Avalonia => "global::Avalonia.Controls.Control",
            _ => "FrameworkElement"
        };
    }

    public static string GetTextBox(Framework framework)
    {
        return framework switch
        {
            Framework.Maui => "Entry",
            _ => "TextBox"
        };
    }

    public static string GetTreeView(Framework framework)
    {
        return framework switch
        {
            Framework.Maui => "VisualElement",
            _ => "TreeView"
        };
    }

    public static string GetDependencyObject(Framework framework)
    {
        return framework switch
        {
            Framework.Avalonia => "global::Avalonia.AvaloniaObject",
            _ => "DependencyObject"
        };
    }
    
    public static string GetDispatcherObject(Framework framework)
    {
        return framework switch
        {
            Framework.Avalonia => "global::Avalonia.AvaloniaObject",
            _ => "DispatcherObject"
        };
    }
    
    public static string GetVisual(Framework framework)
    {
        return framework switch
        {
            Framework.Avalonia => "global::Avalonia.Interactivity.Interactive",
            _ => "Visual"
        };
    }

    public static string GetBrush(Framework framework)
    {
        return framework switch
        {
            Framework.Avalonia => "IBrush",
            _ => "Brush"
        };
    }

    public static string GetBindEventName(Framework framework)
    {
        return framework switch
        {
            Framework.Maui => "SizeChanged",
            _ => "KeyUp"
        };
    }
    
    public static string GetBindEventPropertyName(Framework framework)
    {
        return framework switch
        {
            Framework.Maui => "SizeChanged",
            _ => "KeyUp"
        };
    }

    public static string GetPointerEnteredEventName(Framework framework)
    {
        return framework switch
        {
            Framework.Wpf => "MouseEnter",
            Framework.Avalonia => "PointerEntered",
            Framework.Maui => "Loaded", // dummy for Maui
            _ => "PointerEntered"
        };
    }
    
    public static string GetPointerExitedEventName(Framework framework)
    {
        return framework switch
        {
            Framework.Wpf => "MouseLeave",
            Framework.Avalonia => "PointerExited",
            Framework.Maui => "Unloaded", // dummy for Maui
            _ => "PointerExited"
        };
    }

    public static string GetPointerEventArgs(Framework framework)
    {
        return framework switch
        {
            Framework.Wpf => "global::System.Windows.Input.MouseEventArgs",
            Framework.Avalonia => "global::Avalonia.Input.PointerEventArgs",
            Framework.Maui => "global::System.EventArgs",
            Framework.UnoWinUi or Framework.WinUi => "global::Microsoft.UI.Xaml.Input.PointerRoutedEventArgs",
            _ => "global::Windows.UI.Xaml.Input.PointerRoutedEventArgs"
        };
    }

    public static string GetKeyEventArgsType(Framework framework)
    {
        return framework switch
        {
            Framework.Wpf => "KeyEventArgs",
            Framework.Uno => "KeyRoutedEventArgs",
            Framework.UnoWinUi => "KeyRoutedEventArgs",
            Framework.WinUi => "KeyRoutedEventArgs",
            Framework.Uwp => "KeyRoutedEventArgs",
            Framework.Avalonia => "KeyEventArgs",
            Framework.Maui => "global::System.EventArgs",
            _ => "KeyRoutedEventArgs"
        };
    }

    public static string GetPointerEventArgsType(Framework framework)
    {
        return framework switch
        {
            Framework.Wpf => "MouseEventArgs",
            Framework.Uno => "PointerRoutedEventArgs",
            Framework.UnoWinUi => "PointerRoutedEventArgs",
            Framework.Avalonia => "PointerEventArgs",
            Framework.Maui => "global::System.EventArgs",
            _ => "PointerRoutedEventArgs"
        };
    }
}
