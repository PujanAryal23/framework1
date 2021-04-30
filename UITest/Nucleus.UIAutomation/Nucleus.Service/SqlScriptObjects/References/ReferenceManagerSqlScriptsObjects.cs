using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nucleus.Service.SqlScriptObjects.References
{
    class ReferenceManagerSqlScriptsObjects
    {
        public const string client = "SMTST";

        public const string GetClientListAssignedToUser =
            @"select hc.clientcode from hciclient hc join central.client_user cc on hc.clientcode = cc.clientcode 
            where hc.active='T' and cc.userseq=(select  userseq from central.users where user_id='{0}') order by hc.clientcode";

        public const string FlagList = "select editflg from REPOSITORY.edit where status='A' order by editflg";
        public const string ActiveReferenceCount = "SELECT COUNT(*) FROM   CENTRAL.CLAIM_REFERENCE WHERE ACTIVE = 'T'";
        public const string GetActiveClientList = @"SELECT clientcode FROM hciclient WHERE ACTIVE = 'T' order by clientcode";

        public const string ProcTrigList = "SELECT DISTINCT(CODE) FROM Medical_Code_Description_MV WHERE code_type IN ('CPT','HCPCS')";

        public const string DeleteClaimReference =
            @"DELETE from CENTRAL.CLAIM_REFERENCE where clientcode = '" + client + @"' AND EDITFLG = '{0}' AND LOW_PROC = '{1}'  AND HIGH_PROC = '{2}'
               AND LOW_TRIG_PROC = '{3}' AND HIGH_TRIG_PROC = '{4}' AND ACTIVE = '{5}'";

        public const string DeleteClaimReferenceWithoutTrigCoge = @"select *  from CENTRAL.CLAIM_REFERENCE where clientcode = '" + client + @"' AND EDITFLG = '{0}' AND LOW_PROC = '{1}'  AND HIGH_PROC = '{2}' AND ACTIVE = 'T'";

    }
}
