using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace VpMobilPlusPlus.UserControls.Controls;

public partial class StringAndCheckbox : UserControl
{
    public StringAndCheckbox()
    {
        InitializeComponent();
    }

    public StringAndCheckbox(string text)
    {
        InitializeComponent();
        Text.Text = text;
        Box.AddHandler(CheckBox.ClickEvent, (_, e) =>
        {
            e.Handled = true;
        });
    }
}