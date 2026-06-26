using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using VpMobilPlusPlus.UserControls.Pages;
using VpMobilPlusPlus.Util;
using VpMobilPlusPlus.VpAPI;

namespace VpMobilPlusPlus.UserControls.Controls;

public partial class DayPlan : UserControl
{
    public DayPlan()
    {
        InitializeComponent();
    }

    public DayPlan(Task<VPlan?> vplan, int classIndex)
    {
        InitializeComponent();

        if (vplan == null) return;
        
        vplan.ContinueWith((Task<VPlan> t) =>
        {
            Dispatcher.UIThread.Post(() =>
            {
                if (t.Result == null) return;
                DateBlock.Text = t.Result.planDate.ToString("dddd, dd. MM.");

                if (t.Result.plan.ZusatzInfo != null &&
                    t.Result.plan.ZusatzInfo.ZiZeile != null &&
                    t.Result.plan.ZusatzInfo.ZiZeile.Count > 0)
                {
                    InfoButton.IsVisible = true;
                    InfoButton.Click += (sender, args) =>
                    {
                        string text = "";

                        foreach (var info in t.Result.plan.ZusatzInfo.ZiZeile)
                        {
                            text += info + "\n";
                        }
                        
                        PlanPage.ShowPopUp(text);
                    };
                }

                int i = 0;
                foreach (var std in t.Result.plan.Klassen.Kl[classIndex].Pl.Std)
                {
                    if (CourseFilterPage.CourseFilters.ContainsKey(classIndex))
                    {
                        FilterElement? filter = CourseFilterPage.CourseFilters[classIndex].FirstOrDefault(
                            x => 
                                (x.CourseName == std.Fa.Text && x.IsFiltered) ||
                                (x.CourseName == std.Ku2 && x.IsFiltered) ||
                                (x.id == std.Nr && x.IsFiltered));
                        if (filter != null)
                        {
                            continue;
                        }
                    }
            
                    Course course = new Course(
                        std.Fa.Text, 
                        std.Le.Text, 
                        std.Ra.Text, 
                        std.Fa.FaAe, 
                        std.Le.LeAe, 
                        std.Ra.RaAe, 
                        std.If, std.Ku2);
                    
                    Grid.SetRow(course, i);
                    CourseList.Children.Add(course);
                    i++;
                }
                
                CourseList.RowDefinitions = new RowDefinitions(AvaloniaUtil.GridDefinitionStringMaxSize(CourseList.Children.Count));
            });
        });
    }

}