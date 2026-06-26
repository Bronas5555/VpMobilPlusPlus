using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using VpMobilPlusPlus.UserControls.Pages;

namespace VpMobilPlusPlus.UserControls.Controls;

public partial class Course : UserControl
{
    private Color _fontColor = Colors.Black;
    public Course()
    {
        InitializeComponent();
    }

    public Course(string courseName, string teacherName, string roomname, string FaChanged, string LeChanged, string RaChanged, string info, string ku2)
    {
        InitializeComponent();
        if (FaChanged == "FaGeaendert")
        {
            CourseNameBlock.Foreground = Brushes.Red;
            CourseNameBlock.FontWeight = FontWeight.Bold;
            DisplayInfo(info);
        }
        if (LeChanged == "LeGeaendert")
        {
            CourseTeacherBlock.Foreground = Brushes.Red;
            CourseTeacherBlock.FontWeight = FontWeight.Bold;
            DisplayInfo(info);
        }

        if (RaChanged == "RaGeaendert")
        {
            CourseRoomBlock.Foreground = Brushes.Red;
            CourseRoomBlock.FontWeight = FontWeight.Bold;
            DisplayInfo(info);
        }

        if (ku2 != null && FaChanged != "FaGeaendert")
        {
            CourseNameBlock.Text = ku2;
        }
        else
        {
            CourseNameBlock.Text = courseName;
        }
        CourseTeacherBlock.Text = teacherName;
        CourseRoomBlock.Text = roomname;

        if (CourseColorSelectionPage.courseColors.ContainsKey(PlanPage.Instance._curClassIndex) &&
            CourseNameBlock.Text != null &&
           CourseColorSelectionPage.courseColors[PlanPage.Instance._curClassIndex].ContainsKey(CourseNameBlock.Text))
        {
            Color color = CourseColorSelectionPage.courseColors[PlanPage.Instance._curClassIndex][CourseNameBlock.Text];
            Background.Background = new SolidColorBrush(color);
            
            if(FaChanged != "FaGeaendert" && LeChanged != "LeGeaendert" && RaChanged != "RaGeaendert")
                SetFontColors(GetReadableTextColor(color));
        }
        
    }

    private void DisplayInfo(string info)
    {
        InfoBlock.Text = info;
    }

    private void SetFontColors(Color color)
    {
        CourseNameBlock.Foreground = new SolidColorBrush(color);
        CourseTeacherBlock.Foreground = new SolidColorBrush(color);
        CourseRoomBlock.Foreground = new SolidColorBrush(color);
        InfoBlock.Foreground = new SolidColorBrush(color);
    }
    
    public  Color GetReadableTextColor(Color bg)
    {
        // Normalize RGB to 0–1
        double r = bg.R / 255f;
        double g = bg.G / 255f;
        double b = bg.B / 255f;

        // Perceived luminance (sRGB, simple version)
        double luminance = 0.2126 * r + 0.7152 * g + 0.0722 * b;

        // Threshold around mid-point
        return luminance > 0.5 ? Colors.Black : Colors.White;
    }
}