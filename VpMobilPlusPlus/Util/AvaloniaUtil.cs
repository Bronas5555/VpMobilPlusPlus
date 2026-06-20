namespace VpMobilPlusPlus.Util;

public class AvaloniaUtil
{
    public static string GridDefinitionStringMaxSize(int count)
    {
        string result = "";
        for (int i = 0; i < count; i++)
        {
            if (i == count - 1)
            {
                result += "*";
                continue;
            }
            result += "*, ";
        }

        return result;
    }
}