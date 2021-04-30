using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewRelicTestFramework
{
    public enum ReportType
    {
        [StringValue("E", "Error")]
        Error = 0,

        [StringValue("T", "Transaction")]
        Transaction = 1
    }

    public enum Environment
    {
        [StringValue("PROD", "PRODUCTION")]
        PROD = 0,
        [StringValue("UAT", "USER ACCEPTANCE TESTING")]
        UAT = 1

    }

}

