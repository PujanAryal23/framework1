using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewRelic.WebReporting.Models
{
    public class TransactionTraces
    {
        public int? ReportSeq { get; set; }
        public int? TransactionSeq { get; set; }
        public string IssueType { get; set; }
        public string Page { get; set; }
        public int? Count { get; set; }
        public string Notes { get; set; }
       
    }
}