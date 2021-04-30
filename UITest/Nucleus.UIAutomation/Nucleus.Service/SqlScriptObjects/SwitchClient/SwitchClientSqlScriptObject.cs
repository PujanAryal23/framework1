using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nucleus.Service.SqlScriptObjects.SwitchClient
{
    public class SwitchClientSqlScriptObject
    {
        public const string client = "SMTST";

        public const string GetMostRecentClients =
            "select * from (select c.clientcode,c.clientname from central.client_user cu join client c on cu.clientcode = c.clientcode " +
            "where userseq = (select userseq from central.users where user_id = '{0}') " +
            "and last_time_used is not null" +
            " order by last_time_used desc) where rownum<=5";

        public const string GetAllClients = "select c.clientcode,c.clientname from central.client_user cu join client c on cu.clientcode = c.clientcode" +
                                            " where userseq = (select userseq from central.users where user_id = '{0}') " +
                                            "order by upper(c.clientname), upper(c.clientcode)";

        public const string GetLastTimeUsed =
            "select last_time_used from client_user where userseq=(select userseq from central.users where user_id='{0}') and clientcode='{1}'";


    }
}
