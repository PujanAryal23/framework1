using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIAutomation.Framework.Utils;

namespace Nucleus.Service.Support.Enum
{
    /// <summary>
    /// Dashboard component titles
    /// </summary>
    public enum DashboardOverviewTitlesEnum
    {
        [StringValue("Claims Overview")]
        ClaimsOverview = 0,


        [StringValue("Appeals Overview")]
        AppealsOverview = 1

    }

    public enum COBAppealsDetailEnum
    {
        [StringValue("Outstanding Appeals")]
        OutstandingAppeals = 2,

        [StringValue("Client Appeals")]
        ClientAppeals = 3
    }

    public enum COBClaimsDetailEnum
    {
        [StringValue("Totals by Flag")]
        TotalsByFlag = 1,

        [StringValue("Totals by Batch")]
        TotalsByBatch = 2,

        [StringValue("Totals by Client")]
        TotalsByClient = 3
    }
}
