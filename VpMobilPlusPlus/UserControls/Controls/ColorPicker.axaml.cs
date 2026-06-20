using System;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using VpMobilPlusPlus.UserControls.Events;

namespace VpMobilPlusPlus.UserControls.Controls;

public partial class ColorPicker : UserControl
{
    public event EventHandler<ColorPickerColorChangedEventArgs>? ColorChanged;
    
    private CancellationTokenSource? _colorChangedCts;
    
    public ColorPicker()
    {
        InitializeComponent();

        RedSlider.ValueChanged += UpdateValues;
        GreenSlider.ValueChanged += UpdateValues;
        BlueSlider.ValueChanged += UpdateValues;
        AlphaSlider.ValueChanged += UpdateValues;
        
        UpdateValues(null, null);

    }


    private void UpdateValues(object? sender, RoutedEventArgs e)
    {
        RedValueBlock.Text = ((int)RedSlider.Value).ToString();
        GreenValueBlock.Text = ((int)GreenSlider.Value).ToString();
        BlueValueBlock.Text = ((int)BlueSlider.Value).ToString();
        AlphaValueBlock.Text = ((int)AlphaSlider.Value).ToString();
        
        UpdatePreviewColor();
        
        /*ColorChanged?.Invoke(this, new ColorPickerColorChangedEventArgs(
            new Color((byte)AlphaSlider.Value, (byte)RedSlider.Value, (byte)GreenSlider.Value, (byte)BlueSlider.Value)));*/
        
        DebounceColorChanged();
        
    }
    private void DebounceColorChanged()
    {
        _colorChangedCts?.Cancel();
        _colorChangedCts = new CancellationTokenSource();

        _ = RaiseColorChangedAfterDelay(_colorChangedCts.Token);
    }
    
    private async Task RaiseColorChangedAfterDelay(CancellationToken token)
    {
        try
        {
            // Wait until the slider stops moving
            await Task.Delay(500, token);

            ColorChanged?.Invoke(this, new ColorPickerColorChangedEventArgs(
                new Color(
                    (byte)AlphaSlider.Value,
                    (byte)RedSlider.Value,
                    (byte)GreenSlider.Value,
                    (byte)BlueSlider.Value)));
            
        }
        catch (TaskCanceledException)
        {
            // Slider moved again, ignore this execution
        }
    }

    private void UpdatePreviewColor()
    {
        ColorPreviewRect.Fill = new SolidColorBrush(new Color((byte)AlphaSlider.Value, (byte)RedSlider.Value, (byte)GreenSlider.Value, (byte)BlueSlider.Value));
    }

    public void SetColor(Color color)
    {
        RedSlider.Value = color.R;
        GreenSlider.Value = color.G;
        BlueSlider.Value = color.B;
        AlphaSlider.Value = color.A;
        UpdateValues(null, null);
    }
}