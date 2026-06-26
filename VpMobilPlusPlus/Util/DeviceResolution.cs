using System;
using Avalonia;

namespace VpMobilPlusPlus.Util;

public class DeviceResolution
{
    public static event Action<int, int>? Changed;
    public static Size CurrentResolution = new Size(1, 1);

    public static void Raise(int width, int height)
    {
        CurrentResolution = new Size(width, height);
        Changed?.Invoke(width, height);
    }
}