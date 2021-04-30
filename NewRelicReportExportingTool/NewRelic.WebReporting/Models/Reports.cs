using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewRelic.WebReporting.Models
{
    public class Reports
    {
        public int? ReportSeq { get; set; }
        public string Permalink { get; set; }
        public string Environment{ get; set; }
        public string ReportType { get; set; }
        public int? TotalCount { get; set; }
        public string CreateDate { get; set; }
    }
}