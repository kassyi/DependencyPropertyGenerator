using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace DpgShowcaseCart.Wpf;

public partial class MainWindow
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void OnWikiButtonClicked(object sender, RoutedEventArgs e)
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = "https://github.com/kassyi/DependencyPropertyGenerator/wiki",
            UseShellExecute = true
        });
    }
}