using System;

namespace VpMobilPlusPlus.Util;

public class DeviceResolution
{
    public static event Action<int, int>? Changed;

    public static void Raise(int width, int height)
    {
        Changed?.Invoke(width, height);
    }
}