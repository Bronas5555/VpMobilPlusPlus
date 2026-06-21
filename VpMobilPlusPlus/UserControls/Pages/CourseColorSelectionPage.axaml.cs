using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
using VpMobilPlusPlus.Saving;
using VpMobilPlusPlus.UserControls.Controls;
using VpMobilPlusPlus.Views;
using VpMobilPlusPlus.VpAPI;

namespace VpMobilPlusPlus.UserControls.Pages;

public partial class CourseColorSelectionPage : UserControl
{
    public static Dictionary<int, Dictionary<string, Color>> courseColors = new Dictionary<int, Dictionary<string, Color>>();
    
    private static CourseColorSelectionPage instance;

    private string curEditing = "";
    public CourseColorSelectionPage()
    {
        InitializeComponent();
        instance = this;

        CourseColorPicker.ColorChanged += (sender, args) =>
        {
            if (curEditing == "") return;
            courseColors[PlanPage.Instance._curClassIndex][curEditing] = args.color;
            
            AddNewCourses();
            
            //SaveManager.Save();
            _ = SaveManager.SaveAsync();
        };

        BackButton.Click += (sender, args) =>
        {
            PlanPage.ReloadCurrentWeek();
            MainWindow.DisplayPage(UiPages.PlanPage);
        };
    }

    public static void UpdateLayout()
    {
        bool isSingleColumn = PlanPage.Instance.isSingleColumn;
        if (isSingleColumn)
        {
            Grid.SetRow(instance.ListScrollViewer, 0);
            Grid.SetColumn(instance.ListScrollViewer, 0);

            Grid.SetRow(instance.CourseColorPicker, 0);
            Grid.SetColumn(instance.CourseColorPicker, 0);
            
            instance.ColorGrid.ColumnDefinitions = new ColumnDefinitions("*");
            instance.ColorGrid.RowDefinitions = new RowDefinitions("*, *");
            
            Grid.SetRow(instance.ListScrollViewer, 0);
            Grid.SetRow(instance.CourseColorPicker, 1);
        }
        else
        {
            Grid.SetRow(instance.ListScrollViewer, 0);
            Grid.SetColumn(instance.ListScrollViewer, 0);

            Grid.SetRow(instance.CourseColorPicker, 0);
            Grid.SetColumn(instance.CourseColorPicker, 0);
            
            instance.ColorGrid.ColumnDefinitions = new ColumnDefinitions("*, *");
            instance.ColorGrid.RowDefinitions = new RowDefinitions("*");
            
            Grid.SetColumn(instance.ListScrollViewer, 0);
            Grid.SetColumn(instance.CourseColorPicker, 1);
        }
    }

    public static void AddNewCourses()
    {
        if (courseColors == null || PlanPage.Instance == null) return;
        Task<VPlan?> newestPlan = PlanProvider.GetNewest();
        newestPlan.ContinueWith((Task<VPlan> t) =>
        {
            Dispatcher.UIThread.Post(() =>
            {
                if (t.Result == null) return;

                Dictionary<string, Color> colors;

                if (courseColors.ContainsKey(PlanPage.Instance._curClassIndex))
                {
                    colors = courseColors[PlanPage.Instance._curClassIndex];
                }
                else
                {
                    colors = new Dictionary<string, Color>();
                }

                foreach (var ue in t.Result.plan.Klassen.Kl[PlanPage.Instance._curClassIndex].Unterricht.Ue)
                {
                    if (ue.UeNr.UeGr != null)
                    {
                        if (!colors.ContainsKey(ue.UeNr.UeGr))
                        {
                            colors.Add(ue.UeNr.UeGr, new Color(255, 35, 35, 35));
                        }
                    }
                    else
                    {
                        if (!colors.ContainsKey(ue.UeNr.UeFa))
                        {
                            colors.Add(ue.UeNr.UeFa, new Color(255, 20, 20, 20));
                        }
                
                    }
                }
                courseColors[PlanPage.Instance._curClassIndex] = colors;
        
                instance.GenerateUi(colors);
            });
        });
        
    }

    private void GenerateUi(Dictionary<string, Color> colors)
    {

        instance.ColorStackPanel.Children.Clear();

        foreach (var pair in colors)
        {
            StringAndColor sac = new StringAndColor(pair.Key, pair.Value);
            sac.ColorPreviewButton.Click += (s, e) =>
            {
                curEditing = pair.Key;
                UpdateColorPicker(colors);
            };
            
            instance.ColorStackPanel.Children.Add(sac);

        }
    }

    private void UpdateColorPicker(Dictionary<string, Color> colors)
    {
        if(curEditing == "") return;
        
        CourseColorPicker.SetColor(colors[curEditing]);
    }
}