using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;

namespace VpMobilPlusPlus.UserControls.Controls;

public partial class StringAndColor : UserControl
{
    public StringAndColor()
    {
        InitializeComponent();
    }

    public StringAndColor(string text, Color color)
    {
        InitializeComponent();
        ColorTextBlock.Text = text;
        ColorPreviewButton.Foreground = new SolidColorBrush(color);
        ColorPreviewButton.Background = new SolidColorBrush(color);
    }
}