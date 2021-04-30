using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewRelic.WebReporting.Models
{
    public class ErrorsList
    {
        public int? ReportSeq { get; set; }
        public int? ErrorSeq { get; set; }
        public string IsNew { get; set; }
        public string Issue { get; set; }
        public string ErrorSource { get; set; }
        public string Description { get; set; }
        public int? ErrorCount { get; set; }
        public string Status { get; set; }
        public string Notes { get; set; }
        public string BeginDate { get; set; }
        public string EndDate { get; set; }
        public string Permalink { get; set; }
    }
}