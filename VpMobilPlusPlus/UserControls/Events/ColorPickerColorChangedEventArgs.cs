using System;
using Avalonia.Media;

namespace VpMobilPlusPlus.UserControls.Events;

public class ColorPickerColorChangedEventArgs : EventArgs
{
    public Color color { get; }
    
    public ColorPickerColorChangedEventArgs(Color color)
    {
        this.color = color;
    }
}