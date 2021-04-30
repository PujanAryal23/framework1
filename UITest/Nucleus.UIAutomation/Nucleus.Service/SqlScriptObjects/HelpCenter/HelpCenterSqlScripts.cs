using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nucleus.Service.SqlScriptObjects.HelpCenter
{
    public static class HelpCenterSqlScriptObjects
    {
        public const string GetHolidaysForCurrentYear = @"select holiday_description ||' '||Extract(month from holiday_date)||'/'||to_char(Extract(day from holiday_date),'fm09') from client_holidays where Extract(year from holiday_date) = Extract(YEAR from sysdate) and client_code = 'HCI'
                        and holiday_date > trunc(sysdate) order by holiday_date";
    }
}
