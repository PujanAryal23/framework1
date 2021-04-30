using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nucleus.Service.SqlScriptObjects.RoleManager
{
    public class RoleManagerSqlScriptObject
    {
        public const string client = "SMTST";

        public const string GetAvailableAuthoritiesListForInternalUserFromDB =
            @"select description from ref_authorization where user_type in (1,2) and internal_role_id is null order by description";

        public const string GetAvailableAuthoritiesListForClientUserFromDB =
            @"select description from ref_authorization where user_type in (1,8) and client_role_id is null order by description";

        public const string DeleteRoleByRoleNameFromDB =
            @"BEGIN 
            update ref_authorization set {0} = null where {0} in 
            (SELECT id from ref_role where role_name = '{1}');

            delete from ref_role where role_name = '{1}';

            END;";

        public const string GetRoleNameAndUserTypeFromDB =
            @"select role_name , 
            case APPLIES_TO
            WHEN 8 THEN 'Client'
            WHEN 2 THEN 'Internal'
            end APPLIES_TO
            from ref_role order by role_name";

        public const string GetRoleAuditForCreateAndModify =
            @"select modified_date from central.role_audit where audit_type = '{0}' AND modified_by in (select userseq from central.users where user_id = 'uiautomation') order by modified_date desc";
    }
}
