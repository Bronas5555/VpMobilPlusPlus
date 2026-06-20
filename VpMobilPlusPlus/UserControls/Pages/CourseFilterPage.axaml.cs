using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using VpMobilPlusPlus.Saving;
using VpMobilPlusPlus.UserControls.Controls;
using VpMobilPlusPlus.Util;
using VpMobilPlusPlus.Views;

namespace VpMobilPlusPlus.UserControls.Pages;

public partial class CourseFilterPage : UserControl
{
    private static CourseFilterPage _instance;
    public static Dictionary<int, List<FilterElement>> CourseFilters = new Dictionary<int, List<FilterElement>>();

    public CourseFilterPage()
    {
        InitializeComponent();
        _instance = this;
        BackButton.Click += (sender, args) =>
        {
            MainWindow.DisplayPage(UiPages.PlanPage);
            PlanPage.ReloadCurrentWeek();
        };
    }
    
    public static void UpdateCourseList(List<CourseAndTeacher> courses, int classIndex)
    {
        if (!CourseFilters.ContainsKey(classIndex))
        {
            CourseFilters.Add(classIndex, new List<FilterElement>());
            foreach (var course in courses)
            {
                CourseFilters[classIndex].Add(new FilterElement(course.CourseName, course.CourseTeacher, false, course.id));
            }
        }
        CreateCourseListUiElements(CourseFilters[classIndex], classIndex);
        
        SaveManager.CourseFilters = CourseFilters;
        _ = SaveManager.SaveAsync();
    }

    private static void CreateCourseListUiElements(List<FilterElement> courseFilters, int classIndex)
    {
        _instance.CourseGrid.Children.Clear();

        int columns = (PlanPage.Instance.isSingleColumn) ? 2 : 3;

        _instance.CourseGrid.ColumnDefinitions =
            new ColumnDefinitions(AvaloniaUtil.GridDefinitionStringMaxSize(columns));
        _instance.CourseGrid.RowDefinitions = new RowDefinitions(AvaloniaUtil.GridDefinitionStringMaxSize((int)Math.Ceiling(courseFilters.Count / (float)columns)));
        
        int i = 0;
        foreach (var courseFilter in courseFilters)
        {
            int j = i;
            StringAndCheckbox sac = new StringAndCheckbox(courseFilter.CourseName + " (" + courseFilter.TeacherName + ")");
            sac.Box.IsChecked = courseFilter.IsFiltered;

            sac.Box.IsCheckedChanged += (sender, args) =>
            {
                CourseFilters[classIndex][j].IsFiltered = sac.Box.IsChecked ?? false;
            };
            
            _instance.CourseGrid.Children.Add(sac);
            
            Grid.SetColumn(sac, i % columns);
            Grid.SetRow(sac, (int)(i / (float)columns));
            
            i++;
        }
    }
}

public class FilterElement
{
    public string CourseName;
    public string TeacherName;
    public bool IsFiltered = false;
    public int id;
    
    public  FilterElement(string CourseName,  string TeacherName, bool IsFiltered, int id)
    {
        this.CourseName = CourseName;
        this.TeacherName = TeacherName;
        this.IsFiltered = IsFiltered;
        this.id = id;
    }
}