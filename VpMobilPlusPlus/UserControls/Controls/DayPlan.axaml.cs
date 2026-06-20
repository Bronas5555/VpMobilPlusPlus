using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia.Markup.Xaml;
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
        //TODO: Make Loading animation

        if (vplan == null) return;
        
        vplan.ContinueWith((Task<VPlan> t) =>
        {
            Dispatcher.UIThread.Post(() =>
            {
                Console.WriteLine("Async!");
                if (t.Result == null) return;
                DateBlock.Text = t.Result.planDate.ToString("dddd, dd. MM.");

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