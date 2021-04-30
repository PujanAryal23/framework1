using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nucleus.Service.Support.Enum;

namespace Nucleus.Service.SqlScriptObjects.Invoice
{
    public static class InvoiceSearchSqlScripts
    {
        public const string client = "SMTST";
        
        public const string InvoiceDateList = @"select to_char(invdate,'Mon YYYY') from " + client + ".hciinvoice " +
                                              "where invdate is not null GROUP BY INVDATE order by invdate desc";

        public const string InvoiceCountByClaimNumber =
            @"select count (distinct invnum) from " + client + ".hciinvoice " +
            "where (claseq || '-' || clasub) in (select claseq || '-' || clasub from "+client+".hcicla where altclaimno='{0}' )";

        public const string InvoiceCountByDate =
            @"select count(distinct invnum) from " + client + ".hciinvoice where invdate like '%{0}%'";

        public const string DebitCreditAmountByInvoiceNumber =
            @"select sum(first_reviewfee + dent_reviewfee + fac_reviewfee + first_fraudfee + DENT_FRAUDFEE + FAC_FRAUDFEE) from " +
            client + ".hciinvoice where invnum={0}";

        public const string TotalSavingsByInvoiceNumber =
            @"select sum(tot_savings) from " + client + ".hciinvoice where invnum={0}";

        public const string TotalInvoiceAmountByInvoiceNumber =
            @"select sum(invamt) from " + client + ".hciinvoice where invnum={0}";

        public const string DebitCountByInvoiceNumber =
            @"select count(*) from " + client + ".HCIINVOICE where invnum={0} and invrectyp='D'";

        public const string CreditCountByInvoiceNumber =
            @"select count(*) from " + client + ".HCIINVOICE where invnum={0} and invrectyp='C'";

        public static string GetInvoiceNumberandDate = "SELECT INVNUM,to_char(invdate,'MM/YYYY') FROM " + client+".HCIINVOICE where claseq={0}";
    }
}
