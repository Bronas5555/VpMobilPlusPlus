using System;
using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Controls;
using VpMobilPlusPlus.Saving;
using VpMobilPlusPlus.UserControls.Pages;

namespace VpMobilPlusPlus.Views;

public partial class MainWindow : Window
{
    private static MainWindow Instance;
    
    public static UiPages currentUiPage = UiPages.StartPage;
    public static UiPages previousUiPage = UiPages.StartPage;
    
    public MainWindow()
    {
        InitializeComponent();
        Instance = this;
        //SaveManager.Load();
        _ = SaveManager.LoadAsync();
        PlanPage.LoadWeekByDateAndClass(new DateOnly(2026,6, 7), 0);
        this.PropertyChanged += Window_PropertyChanged;
    }

    private void Window_PropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == Window.WidthProperty ||
            e.Property == Window.HeightProperty)
        {
            Size size = this.ClientSize;
            PlanPage.UpdateViewState(size);
        }
    }

    public static Size GetMainWindowSize()
    {
        return Instance.ClientSize;
    }

    public static void DisplayPage(UiPages page)
    {
        switch (page)
        {
            case UiPages.PlanPage:
            {
                MainView.Instance.StartPage.IsVisible = false;
                MainView.Instance.PlanPage.IsVisible = true;
                MainView.Instance.ColorPage.IsVisible = false;
                MainView.Instance.FilterPage.IsVisible = false;
                MainView.Instance.SettingsPage.IsVisible = false;

                previousUiPage = currentUiPage;
                currentUiPage = UiPages.PlanPage;
            }
            break;
            case UiPages.StartPage:
            {
                MainView.Instance.StartPage.IsVisible = true;
                MainView.Instance.PlanPage.IsVisible = false;
                MainView.Instance.ColorPage.IsVisible = false;
                MainView.Instance.FilterPage.IsVisible = false;
                MainView.Instance.SettingsPage.IsVisible = false;
                
                previousUiPage = currentUiPage;
                currentUiPage = UiPages.StartPage;
                
                _ = StartPage.Instance.LoadAsync();
            }
            break;
            case UiPages.ColorPage:
            {
                MainView.Instance.StartPage.IsVisible = false;
                MainView.Instance.PlanPage.IsVisible = false;
                MainView.Instance.ColorPage.IsVisible = true;
                MainView.Instance.FilterPage.IsVisible = false;
                MainView.Instance.SettingsPage.IsVisible = false;
                
                previousUiPage = currentUiPage;
                currentUiPage = UiPages.ColorPage;
            }
            break;
            case UiPages.FilterPage:
            {
                MainView.Instance.StartPage.IsVisible = false;
                MainView.Instance.PlanPage.IsVisible = false;
                MainView.Instance.ColorPage.IsVisible = false;
                MainView.Instance.FilterPage.IsVisible = true;
                MainView.Instance.SettingsPage.IsVisible = false;
                
                previousUiPage = currentUiPage;
                currentUiPage = UiPages.FilterPage;
            }
            break;
            case UiPages.SettingsPage:
            {
                MainView.Instance.StartPage.IsVisible = false;
                MainView.Instance.PlanPage.IsVisible = false;
                MainView.Instance.ColorPage.IsVisible = false;
                MainView.Instance.FilterPage.IsVisible = false;
                MainView.Instance.SettingsPage.IsVisible = true;
                
                previousUiPage = currentUiPage;
                currentUiPage = UiPages.SettingsPage;
            }
            break;
        }
    }
}

public enum UiPages
{
    StartPage,
    PlanPage,
    ColorPage,
    FilterPage,
    SettingsPage
}