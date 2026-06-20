using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using VpMobilPlusPlus.UserControls.Pages;
using VpMobilPlusPlus.Views;

namespace VpMobilPlusPlus.UserControls.Controls;

public partial class ToolBar : UserControl
{
    public ToolBar()
    {
        InitializeComponent();
        HomeButton.Click += OpenStartPage;
        ColorPageButton.Click += OpenColorPage;
        FilterPageButton.Click += OpenFilterPage;
    }

    private void OpenStartPage(object? sender, RoutedEventArgs e)
    {
        if (MainWindow.currentUiPage != UiPages.StartPage)
        {
            MainWindow.DisplayPage(UiPages.StartPage);
        }
        else
        {
            MainWindow.DisplayPage(MainWindow.previousUiPage);
        }
    }
    private void OpenColorPage(object? sender, RoutedEventArgs e)
    {
        if (MainWindow.currentUiPage != UiPages.ColorPage)
        {
            MainWindow.DisplayPage(UiPages.ColorPage);
        }
        else
        {
            PlanPage.ReloadCurrentWeek();
            MainWindow.DisplayPage(MainWindow.previousUiPage);
        }
    }

    private void OpenFilterPage(object? sender, RoutedEventArgs e)
    {
        if (MainWindow.currentUiPage != UiPages.FilterPage)
        {
            MainWindow.DisplayPage(UiPages.FilterPage);   
        }
        else
        {
            MainWindow.DisplayPage(MainWindow.previousUiPage);
            PlanPage.ReloadCurrentWeek();
        }
    }
}