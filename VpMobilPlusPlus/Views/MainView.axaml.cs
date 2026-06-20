using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace VpMobilPlusPlus.Views;

public partial class MainView : UserControl
{
    public static MainView Instance;
    public MainView()
    {
        InitializeComponent();
        Instance = this;
    }
}