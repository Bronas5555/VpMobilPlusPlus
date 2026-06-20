using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using VpMobilPlusPlus.Saving;
using VpMobilPlusPlus.Views;
using VpMobilPlusPlus.VpAPI;

namespace VpMobilPlusPlus.UserControls.Pages;

public partial class SettingsPage : UserControl
{
    public static SettingsPage Instance;
    public SettingsPage()
    {
        InitializeComponent();

        Instance = this;

        SchoolNumberBox.TextChanged += (sender, args) =>
        {
            int i;
            if (int.TryParse(SchoolNumberBox.Text, out i))
            {
                SaveManager.schoolNumber = i;
                _ = SaveManager.SaveAsync();
            }
        };
        
        UserNameBox.TextChanged += (sender, args) =>
        {
            SaveManager.username = UserNameBox.Text;
            _ = SaveManager.SaveAsync();
        };
        PasswordBox.TextChanged += (sender, args) =>
        {
            SaveManager.password = PasswordBox.Text;
            _ = SaveManager.SaveAsync();
        };

        BackButton.Click += (sender, args) =>
        {
            MainWindow.DisplayPage(UiPages.StartPage);
            _ = SaveManager.SaveAsync();
        };

        /*DeleteAllDataButton.Click += (sender, args) =>
        {
            SaveManager.DeleteAllData();
            
            //Clearing All Save Related Fields
            SaveManager.CourseFilters = new();
            SaveManager.ClassList = new();
            SaveManager.schoolNumber = -1;
            SaveManager.username = "";
            SaveManager.password = "";
            
            CourseColorSelectionPage.courseColors = new();

            SettingsPage.Instance.SchoolNumberBox.Text = "";
            SettingsPage.Instance.UserNameBox.Text = "";
            SettingsPage.Instance.PasswordBox.Text = "";

            CourseFilterPage.CourseFilters = new();
            CourseColorSelectionPage.courseColors = new();
            
            PlanProvider.plans.Clear();
            
            PlanPage.ReloadCurrentWeek();
        };*/
    }
}