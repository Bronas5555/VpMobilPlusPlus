using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using VpMobilPlusPlus.UserControls.Pages;
using VpMobilPlusPlus.Util;
using VpMobilPlusPlus.Views;

namespace VpMobilPlusPlus.UserControls.Controls;

public partial class ClassSelectionPinnableElement : UserControl
{
    private int classIndex = 0;
    public bool canFire = false;
    public ClassSelectionPinnableElement()
    {
        InitializeComponent();
    }

    public ClassSelectionPinnableElement(string className, int classIndex)
    {
        InitializeComponent();
        ClassNameBlock.Text = className;
        PinnedBox.IsCheckedChanged += (sender, args) =>
        {
            if (!canFire) return;
            if (PinnedBox.IsChecked == true)
            {
                StartPage.PinClassListElement(this);
            }
            else
            {
                StartPage.UnpinClassListElement(this);
            }
        };
        PinnedBox.Click += (sender, args) =>
        {
            args.Handled = true;
        };

        LoadPlanButton.Click += (sender, args) =>
        {
            if (args.Handled) return;

            PlanPage.LoadWeekByDateAndClass(DateUtil.GetCurrentWeeksMonday(), classIndex);
            MainWindow.DisplayPage(UiPages.PlanPage);
        };
    }
}