using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices.JavaScript;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Input.GestureRecognizers;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
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
    private int singleColumnDayOffsetOld = 0;
    
    public int _curClassIndex;
    private List<CourseAndTeacher> _courses = new List<CourseAndTeacher>();
    public bool isSingleColumn = false;
    public bool isPopupOpen = false;

    private double width = -1;
    private double height = -1;
    
    public PlanPage()
    {
        InitializeComponent();
        Instance = this;
        
        int dayOfWeek = (int)(DateTime.Now.DayOfWeek + 6) % 7;
        if (dayOfWeek >= 5) dayOfWeek = 0;
        
        singleColumnDayOffset = dayOfWeek;

        PrevButton.Click += (sender, args) =>
        {
            if(!isSingleColumn) ShowPrevWeek(sender, args);
            else
            {
                if (singleColumnDayOffset > 0)
                {
                    singleColumnDayOffsetOld = singleColumnDayOffset;
                    singleColumnDayOffset -= 1;
                    LoadWeekByDateAndClass(_curWeek, _curClassIndex);
                }
                else
                {
                    singleColumnDayOffsetOld = singleColumnDayOffset;
                    singleColumnDayOffset = 4;
                    ShowPrevWeek(sender, args);
                }
            }
            ClosePopUp();
        };

        NextButton.Click += (sender, args) =>
        {
            if(!isSingleColumn) ShowNextWeek(sender, args);
            else
            {
                if (singleColumnDayOffset < 4)
                {
                    singleColumnDayOffsetOld = singleColumnDayOffset;
                    singleColumnDayOffset += 1;
                    LoadWeekByDateAndClass(_curWeek, _curClassIndex);
                }
                else
                {
                    singleColumnDayOffsetOld = singleColumnDayOffset;
                    singleColumnDayOffset = 0;
                    ShowNextWeek(sender, args);
                }
            }
            ClosePopUp();
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
            RepositionPopUp();
        };

        DaysGrid.AddHandler(InputElement.SwipeGestureEndedEvent, (object? sender, SwipeGestureEndedEventArgs args) =>
        {
            if (Math.Abs(args.Velocity.Y) > Math.Abs(args.Velocity.X)) return;
            
            if (args.Velocity.X < 0)
            {
                if(!isSingleColumn) ShowPrevWeek(sender, args);
                else
                {
                    if (singleColumnDayOffset > 0)
                    {
                        singleColumnDayOffsetOld = singleColumnDayOffset;
                        singleColumnDayOffset -= 1;
                        LoadWeekByDateAndClass(_curWeek, _curClassIndex);
                    }
                    else
                    {
                        singleColumnDayOffsetOld = singleColumnDayOffset;
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
                        singleColumnDayOffsetOld = singleColumnDayOffset;
                        singleColumnDayOffset += 1;
                        LoadWeekByDateAndClass(_curWeek, _curClassIndex);
                    }
                    else
                    {
                        singleColumnDayOffsetOld = singleColumnDayOffset;
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
        
        //Instance.DaysGrid.Children.Clear();
        
        if (Instance.isSingleColumn)
        {
            DateOnly d = Instance._curWeek.AddDays(Instance.singleColumnDayOffset); 
            Task<VPlan?> vplan = PlanProvider.GetDate(d);
            
            if (vplan != null)
            {
                vplan.ContinueWith((Task<VPlan> t) =>
                {
                    Dispatcher.UIThread.Post(() =>
                    {
                        if (t.Result == null)
                        {
                            Instance.singleColumnDayOffset = Instance.singleColumnDayOffsetOld;
                            ShowPopUp("404 Plan not Found.");
                            return;
                        }
                        
                        Instance.DaysGrid.Children.Clear();
                        UpdateCourses(t.Result, classIndex);
                        
                        DayPlan day = new DayPlan(vplan, classIndex);
                        
                        Grid.SetColumn(day, 0);
                        Instance.DaysGrid.Children.Add(day);
                        
                        Instance.UpdateWeekAndClassInfo(t.Result, classIndex);
                    });
                });
                
            }
        }
        else
        {
            Instance.DaysGrid.Children.Clear();
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
        Instance.DaysGrid.Children.Clear();
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

    public static void ShowPopUp(string text)
    {
        if (Instance.PopupCanvas.IsVisible)
        {
            if (Instance.PopupTextBlock.Text == text)
            {
                ClosePopUp();
            }
            else
            {
                Instance.PopupTextBlock.Text = text;
                Instance.isPopupOpen = true;
            }
        }
        else
        {
            Instance.PopupCanvas.IsVisible = true;
            Instance.PopupTextBlock.Text = text;
            Instance.isPopupOpen = true;
        }
        
        RepositionPopUp();
    }

    public static bool EvaluatePopupClosingOfClick(Point point)
    {
        if(!Instance.PopupBorder.Bounds.Contains(point) && Instance.isPopupOpen)
        {
            ClosePopUp();
            return true;
        }

        return false;
    }
    
    public static void ClosePopUp()
    {
        Instance.PopupCanvas.IsVisible = false;
        Instance.isPopupOpen = false;
    }

    private static void RepositionPopUp()
    {
        Instance.PopupBorder.Width = Math.Sqrt(DeviceResolution.CurrentResolution.Width) * 15f;
        Instance.PopupBorder.Height = Math.Sqrt(DeviceResolution.CurrentResolution.Height) * 15f;

        Instance.BackgroundBorder.Width = DeviceResolution.CurrentResolution.Width;
        Instance.BackgroundBorder.Height = DeviceResolution.CurrentResolution.Height;
        
        Canvas.SetLeft(Instance.PopupBorder, 
            (DeviceResolution.CurrentResolution.Width / 2f) - (Instance.PopupBorder.Width / 2f));
        Canvas.SetTop(Instance.PopupBorder,
            (DeviceResolution.CurrentResolution.Height / 2f) - (Instance.PopupBorder.Height / 2f));
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