using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VpMobilPlusPlus.UserControls.Pages;
using VpMobilPlusPlus.Util;

namespace VpMobilPlusPlus.VpAPI;

public class PlanProvider
{
    public static List<VPlan> plans = new List<VPlan>();

    public static async Task<VPlan?> GetNewest()
    {
        var newest = plans.MaxBy(x => x.planDate);
        if (newest == null)
        {
            if (Gatherer.GatherDate(DateUtil.GetCurrentWeeksMonday()))
            {
                return await GetNewest();
            }
            return null;
        }
        return newest;
    }

    public static async Task<VPlan?> GetDate(DateOnly date)
    {
        var plan = plans.FirstOrDefault(x =>
            DateOnly.FromDateTime(DateUtil.GermanWeekDayStringToDateTime(x.plan.Kopf.DatumPlan)) == date);
        if (plan == null)
        {
            if (Gatherer.GatherDate(date))
            {
                return await GetDate(date);
            }

            return null;
        }
        CourseColorSelectionPage.AddNewCourses();
        return plan;
    }
}