using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using VpMobilPlusPlus.UserControls.Pages;

namespace VpMobilPlusPlus.Views;

public partial class MainView : UserControl
{
    public static MainView Instance;
    public MainView()
    {
        InitializeComponent();
        Instance = this;
        
        AddHandler(
            PointerPressedEvent,
            OnGlobalPointerPressed,
            RoutingStrategies.Tunnel);
    }
    
    private void OnGlobalPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        e.Handled = PlanPage.EvaluatePopupClosingOfClick(e.GetPosition(this));
    }
}