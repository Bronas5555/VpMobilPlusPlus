using System;
using System.Collections.Generic;
using System.Linq;
using VpMobilPlusPlus.VpAPI;

namespace VpMobilPlusPlus.Util;

public class ListUtil
{
    public static void AddOrUpdate(List<VPlan> list, VPlan newItem)
    {
        int index = list.FindIndex(x => x.planDate == newItem.planDate);

        if (index != -1)
        {
            list[index] = newItem;
        }
        else
        {
            list.Add(newItem);
        }
    }
}