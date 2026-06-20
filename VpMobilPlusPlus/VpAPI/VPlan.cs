using System;
using VpMobilPlusPlus.Util;

namespace VpMobilPlusPlus.VpAPI;

public class VPlan
{
    public VpMobil plan;
    public DateTime planDate;
    public DateTime planUpdateDate;

    public VPlan(VpMobil plan)
    {
        this.plan = plan;
        this.planDate = DateUtil.GermanWeekDayStringToDateTime(plan.Kopf.DatumPlan);
        this.planUpdateDate = DateUtil.GermanStringToDateTime(plan.Kopf.Zeitstempel);
    }
}