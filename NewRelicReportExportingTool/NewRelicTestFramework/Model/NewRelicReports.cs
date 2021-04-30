using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewRelicTestFramework
{
    public class NewRelicReports
    {
        public DateTime ReportDate { get; set; }
        public string ReportType { get; set; }
        public string Environments { get; set; }
    }
}
