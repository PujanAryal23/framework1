using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewRelicTestFramework
{
    public class TransactionTraces
    {
        public string IssueType { get; set; }
        public string SlowPage { get; set; }
        public int Count { get; set; }
        public string Notes { get; set; }
             
    }

    public class SlowPage
    {
        public int SNo { get; set; }
        public string Page { get; set; }
    }
}
