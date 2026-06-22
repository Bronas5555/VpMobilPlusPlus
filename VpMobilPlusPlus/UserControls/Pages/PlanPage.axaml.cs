using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Input.GestureRecognizers;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.Markup.Xaml;
using VpMobilPlusPlus.UserControls.Controls;
using VpMobilPlusPlus.Util;
using VpMobilPlusPlus.Views;
using VpMobilPlusPlus.VpAPI;

namespace VpMobilPlusPlus.UserControls.Pages;

public partial class PlanPage : UserControl
{
    public static PlanPage Instance;
    private DateOnly _curWeek;
    private int singleColumnDayOffset = 0;
    public int _curClassIndex;
    private List<CourseAndTeacher> _courses = new List<CourseAndTeacher>();
    public bool isSingleColumn = false;

    private double width = -1;
    private double height = -1;
    
    public PlanPage()
    {
        InitializeComponent();
        Instance = this;

        PrevButton.Click += (sender, args) =>
        {
            if(!isSingleColumn) ShowPrevWeek(sender, args);
            else
            {
                if (singleColumnDayOffset > 0)
                {
                    singleColumnDayOffset -= 1;
                    LoadWeekByDateAndClass(_curWeek, _curClassIndex);
                }
                else
                {
                    singleColumnDayOffset = 4;
                    ShowPrevWeek(sender, args);
                }
            }
        };

        NextButton.Click += (sender, args) =>
        {
            if(!isSingleColumn) ShowNextWeek(sender, args);
            else
            {
                if (singleColumnDayOffset < 4)
                {
                    singleColumnDayOffset += 1;
                    LoadWeekByDateAndClass(_curWeek, _curClassIndex);
                }
                else
                {
                    singleColumnDayOffset = 0;
                    ShowNextWeek(sender, args);
                }
            }
        };

        LayoutUpdated += (sender, args) =>
        {
            var topLevel = TopLevel.GetTopLevel(this);

            if (topLevel != null)
            {
                var size = topLevel.Bounds.Size;



                if (width != size.Width || height != size.Height)
                {
                    width = size.Width;
                    height = size.Height;
                    
                    UpdateViewState(new Size(width, height));
                }
            }
        };

        DeviceResolution.Changed += (width, height) =>
        {
            UpdateViewState(new Size(width, height)); 
            Console.WriteLine("Updated View From Orientation Change " + width + " " + height);
            
        };

        DaysGrid.AddHandler(InputElement.SwipeGestureEndedEvent, (object? sender, SwipeGestureEndedEventArgs args) =>
        {
            if (Math.Abs(args.Velocity.Y) > 20) return;
            
            if (args.Velocity.X < 0)
            {
                if(!isSingleColumn) ShowPrevWeek(sender, args);
                else
                {
                    if (singleColumnDayOffset > 0)
                    {
                        singleColumnDayOffset -= 1;
                        LoadWeekByDateAndClass(_curWeek, _curClassIndex);
                    }
                    else
                    {
                        singleColumnDayOffset = 4;
                        ShowPrevWeek(sender, args);
                    }
                }
            }
            else
            {
                if(!isSingleColumn) ShowNextWeek(sender, args);
                else
                {
                    if (singleColumnDayOffset < 4)
                    {
                        singleColumnDayOffset += 1;
                        LoadWeekByDateAndClass(_curWeek, _curClassIndex);
                    }
                    else
                    {
                        singleColumnDayOffset = 0;
                        ShowNextWeek(sender, args);
                    }
                }
            }
        });
    }

    public static void LoadWeekByDateAndClass(DateOnly date, int classIndex)
    {
        Instance._curWeek = DateUtil.GetDatesMonday(date);
        Instance._curClassIndex = classIndex;
        
        Instance.DaysGrid.Children.Clear();
        
        if (Instance.isSingleColumn)
        {
            DateOnly d = Instance._curWeek.AddDays(Instance.singleColumnDayOffset); 
            Task<VPlan?> vplan = PlanProvider.GetDate(d);
            if (vplan != null)
            {
                DayPlan day = new DayPlan(vplan, classIndex);
            
                Grid.SetColumn(day, 0);
                Instance.DaysGrid.Children.Add(day);

                vplan.ContinueWith((Task<VPlan> t) =>
                {
                    Dispatcher.UIThread.Post(() =>
                    {
                        if (t.Result == null) return;
                        UpdateCourses(t.Result, classIndex);
                        Instance.UpdateWeekAndClassInfo(t.Result, classIndex);
                    });
                });
            }
        }
        else
        {
            for (int i = 0; i < 5; i++)
            {
                DateOnly d = Instance._curWeek.AddDays(i);
                Task<VPlan?> vplan = PlanProvider.GetDate(d);
                if (vplan != null)
                {
                    DayPlan day = new DayPlan(vplan, classIndex);
            
                    Grid.SetColumn(day, i);
                    Instance.DaysGrid.Children.Add(day);

                    vplan.ContinueWith((Task<VPlan> t) =>
                    {
                        Dispatcher.UIThread.Post(() =>
                        {
                            if (t.Result == null) return;
                            UpdateCourses(t.Result, classIndex);
                            Instance.UpdateWeekAndClassInfo(t.Result, classIndex);
                        });
                    });
                }
            
            }
        }
    }

    private static void UpdateCourses(VPlan vplan, int classIndex)
    {
        Instance._courses.Clear();

        foreach (var ue in vplan.plan.Klassen.Kl[classIndex].Unterricht.Ue)
        {
            if (ue.UeNr.UeGr != null)
            {
                Instance._courses.Add(new CourseAndTeacher(ue.UeNr.UeGr, ue.UeNr.UeLe, ue.UeNr.Text));
            }
            else
            {
                Instance._courses.Add(new CourseAndTeacher(ue.UeNr.UeFa, ue.UeNr.UeLe, ue.UeNr.Text));
            }

            Instance._courses = Instance._courses.OrderBy(x => x.CourseName).ToList();
        }
        CourseFilterPage.UpdateCourseList(Instance._courses, Instance._curClassIndex);
    }

    public static void ReloadCurrentWeek()
    {
        LoadWeekByDateAndClass(Instance._curWeek, Instance._curClassIndex);
    }

    private void ShowPrevWeek(object? sender, RoutedEventArgs e)
    {
        LoadWeekByDateAndClass(_curWeek.AddDays(-7), _curClassIndex);
    }
    private void ShowNextWeek(object? sender, RoutedEventArgs e)
    {
        LoadWeekByDateAndClass(_curWeek.AddDays(7), _curClassIndex);
    }

    private void UpdateWeekAndClassInfo(VPlan plan, int classIndex)
    {
        if (isSingleColumn)
        {
            DateBlock.IsVisible = false;
            ClassNameBlock.FontSize = 18;
            ClassNameBlock.VerticalAlignment =  Avalonia.Layout.VerticalAlignment.Center;
        }
        else
        {
            DateBlock.IsVisible = true;
            ClassNameBlock.FontSize = 15;
            ClassNameBlock.VerticalAlignment =  Avalonia.Layout.VerticalAlignment.Bottom;
            
            DateBlock.Text = _curWeek.ToString("dd.MM") + " - " + _curWeek.AddDays(7).ToString("dd.MM");
        }
        ClassNameBlock.Text = plan.plan.Klassen.Kl[classIndex].Kurz;
    }

    public static void UpdateViewState(Size size)
    {
        bool oldState = Instance.isSingleColumn;
        float ratio = (float)(size.Width / size.Height);
        
        if (ratio < 1f)
        {
            //Single column
            Instance.isSingleColumn = true;
            Instance.DaysGrid.ColumnDefinitions = new ColumnDefinitions(AvaloniaUtil.GridDefinitionStringMaxSize(1));
        }
        else
        {
            //Multi column
            Instance.isSingleColumn = false;
            Instance.DaysGrid.ColumnDefinitions = new ColumnDefinitions(AvaloniaUtil.GridDefinitionStringMaxSize(5));
            
        }

        if (oldState != Instance.isSingleColumn)
        {
            ReloadCurrentWeek();
            CourseColorSelectionPage.UpdateLayout();
        }
    }
    
}

public class CourseAndTeacher
{
    public string CourseName;
    public string CourseTeacher;
    public int id;
    public CourseAndTeacher(string courseName, string courseTeacher, int id)
    {
        this.CourseName = courseName;
        this.CourseTeacher = courseTeacher;
        this.id = id;
    }
}