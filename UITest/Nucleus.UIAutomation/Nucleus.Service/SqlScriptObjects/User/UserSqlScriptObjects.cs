using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nucleus.Service.SqlScriptObjects.User
{
    public class UserSqlScriptObjects
    {
        public const string client = "SMTST";
        public const string RestoreSecurityAnswers = @"update central.users set  answer_1='l',answer_2='l' where user_id='{0}'";
        public const string DeletePasswordHistoryForUser =
            @"Delete from central.user_authentication_history where userseq = (select userseq from central.users where user_id='{0}')";
        public const string GetSecurityQuestionList =
            @"SELECT question from central.REF_SECURITY_QUESTION";

        public const string ResetUserPasswordAndSecurityQuestion = @"UPDATE USERS 
            set password = '{0}',
            salt = '{1}',
            security_question_1 = '{2}',
            security_question_2 = '{3}'
            where userseq = (select userseq from central.users where user_id='{4}')";

        public const string AvailableAccessForAccessForInternalUser = @"SELECT description from REF_RESTRICTION where user_type in (1,2)";
        public const string AvailableAccessForAccessForClientUser = @"SELECT description from REF_RESTRICTION where user_type in (1,8)";

        public const string UpdateSSOSettingforUser = @"update users set is_federated='{0}' where user_id='{1}'";

        #region UserProfileSearch

        public const string PrimaryDataForUserProfileSearch = "select FIRST_NAME||' '||LAST_NAME USERNAME,USER_ID,PHONE,EMAIL_ADDRESS,Decode(USER_TYPE,'2','Internal','8','Client') UserType  from CENTRAL.users where" +
                                                              " FIRST_NAME LIKE '{0}%' AND USER_TYPE='8' AND STATUS='A' ORDER BY upper(FIRST_NAME),upper(LAST_NAME) ASC";

        public const string UsersByActiveStatus = "select user_id from central.users where status='A'   ";
        public const string UsersByInactiveStatus = "select user_id from central.users where status='I' ";
        public const string UpdateUserToFrozenStatus = " update  users  set status='F' where user_id='{0}'";
        public const string RevertUserFrozenStatus = "update  users  set status='A' where user_id='{0}'";

        public const string UpdateUserToLockedStatus =
            " update users set  FAILED_LOGINS ='3' ,LAST_FAILED_LOGIN=SYSTIMESTAMP  WHERE USER_ID='{0}'";

        public const string RevertUserLockedStatus =
            "update users set  FAILED_LOGINS ='0' ,LAST_FAILED_LOGIN=null  WHERE USER_ID='{0}'";

        public const string RevertUserToActiveStatus =
            "update users set  FAILED_LOGINS ='0' ,LAST_FAILED_LOGIN=null,status='A'  WHERE USER_ID='{0}'";

        public const string UserDetailUsingPartialMatch =
            "SELECT USER_ID FROM CENTRAL.USERS WHERE FIRST_NAME LIKE '{0}%' AND LAST_NAME LIKE '{1}%'  ORDER BY FIRST_NAME,LAST_NAME ";

       

        public const string GetFullNameByUserID =
            "select concat(first_name, concat(' ',last_name)) as name from central.users where user_id = '{0}'";

        public const string UserAssignedClientList = "select clientcode from central.client_user where userseq =(select userseq from central.users where user_id = '{0}')";

        public const string DeleteAssignedUserClient =
            "delete from central.client_user where userseq =(select userseq from central.users where user_id ='{0}') and clientcode in ( '{1}', '{2}' )";


        public const string GetDefaultClientOfUser = @"select default_clientcode from central.users where user_id = '{0}'";

        public const string GetLastSavedPasswordAuditByUser = "select count(*) from central.user_audit where user_id = '{0}'";

        public const string DeleteLastSavedPasswordByUser =
            "delete from central.user_audit where user_id = '{0}'";
            //"delete  from central.user_audit where id  in (select id from (select * from central.user_audit where user_id = '{0}' order by password_changed desc) where rownum <=2)";

        public const string DeleteLastSavedPasswordHistory =
            "delete from central.user_authentication_history where userseq = (select userseq from central.users where user_id = '{0}')";
        //"delete from central.user_authentication_history where created in (select created from (select * from central.user_authentication_history where userseq = '{0}' order by created desc) where rownum <=2)";

        #endregion

        #region Role Manager

        public const string GetAllRoleName = @"select role_name from central.ref_role order by lower(role_name) asc";
        public const string GetRoleNameByUserType = @"select role_name from central.ref_role where applies_to = {0} order by lower(role_name) asc";

        public const string GetAssignedRoleIdsToUserByUserId = 
            @"select role_id from user_role where userseq = (select userseq from central.users where user_id = '{0}') order by role_id";

        public const string GetAssignedRolesToUserFromAuditTable =
            @"select role_name from central.ref_role where id in
                (
                select regexp_substr((select * from (select roles from user_role_audit  where userseq = (select userseq from central.users where user_id = 'uiautomation8')  order by modified_date desc) where rownum =1), '[^,]+', 1, level)
                from dual
                connect by regexp_substr((select * from 
                (select roles from user_role_audit  where userseq = (select userseq from central.users where user_id = '{0}')  
                order by modified_date desc) where rownum =1), '[^,]+', 1, level) is not null
                )
                order by role_name";

        public const string GetLatestAuditFromRoleAuditTableByUserId =
            @"select * from
                (
                select users.user_id, ra.roles, ra.modified_by, ra.modified_date  from 
                (select * from user_role_audit  where userseq = (select userseq from central.users where user_id = '{0}'))  ra
                join CENTRAL.users users
                on ra.userseq = users.userseq
                order by modified_date desc
                ) where rownum = 1";

        public const string GetRoleDesriptionByUserTypeFromDb = @"select description from ref_role where applies_to = {0} order by role_name";

        public const string GetAllRoleDesriptionFromDb = @"select description from ref_role order by role_name";

        public const string GetUserDetailsFromDb = @"Select count(*) from central.users where user_id = '{0}'";

        public const string DeleteUserDetailsFromDb =
                @"begin
                delete from central.user_notification_audit where userseq in (select userseq from central.users where user_id = '{0}');
                delete from central.user_notification where userseq in (select userseq from central.users where user_id = '{0}');
                delete from central.user_role_audit where userseq in (select userseq from central.users where user_id = '{0}');
                delete from central.user_role where userseq in (select userseq from central.users where user_id = '{0}');
                delete from central.user_restriction_audit where userseq in (select userseq from central.users where user_id = '{0}');
                delete from central.user_restriction_access where userseq in (select userseq from central.users where user_id = '{0}');
                delete from central.client_user_audit where userseq in (select userseq from central.users where user_id = '{0}');
                delete from central.client_user where userseq in (select userseq from central.users where user_id = '{0}');
                delete from central.user_audit where user_id = '{0}';
                delete from central.users where user_id = '{0}';    
                end;";

        public const string UpdateFlagsValueFromDb = "update central.users set flags = {0} where user_id = '{1}'";

        public const string GetDetailsOfUsersFromDbByUserId = "select {0} from central.users where user_id = '{1}'";

        public const string DeleteLatestPasswordFromDb =
            @"delete from user_authentication_history where userseq = (select userseq from central.users where user_id = '{0}') and created =(select max(created) from user_authentication_history where userseq = (select userseq from central.users where user_id = '{0}'))";

        public const string SelectPasswordDetail =
            @"select * from user_authentication_history where userseq = (select userseq from central.users where user_id = '{0}')";

        public const string UpdateEmailVerificationFromDb =
            @"update central.users set verify_email = '{0}' where user_id = '{1}'";

        public const string UserAuthenticationAudit = @"select EVENT_TYPE from USER_AUTHENTICATION_AUDIT where userseq in (select userseq from users where user_id='{0}')
 order by event_time desc  FETCH FIRST ROW ONLY";

        public const string GetUsesSSOFlagValueFromDb =
            "select uses_sso from central.users where user_id = '{0}'";


        public const string GetIsFederatedFlagValueFromDb =
            "select is_federated from central.users where user_id = '{0}'";

        public const string GetFailedLoginCountsFromDatabase =
            "select failed_logins from central.users where user_id = '{0}'";

        public const string GetReferenceDepartments = @"select description from REF_DEPARTMENT";

        #endregion

        #region My Profile

        public const string RevertUserInfoInProfileAndPreferencesForInternalUser =
                                                    @"update central.users set prefix_name = 'Mr.', first_name = 'My Profile', 
                                                    last_name = 'For Internal',
                                                    maturity_suffix = 'Jr.', job_title = 'claims adjuster', other_suffix = 'M.D.', 
                                                    phone = 2314567890, phone_ext = 111, 
                                                    fax = 2222222222, fax_ext = 222, alternate_phone = 3333333333, alternate_phone_ext = 333,
                                                    email_address = 'uiautomation@cotiviti.com', default_clientcode = 'SMTST', 
                                                    enable_claim_action_tooltips = 'T', auto_claim_history_popup = 'T', 
                                                    landing_page = '/claims?claimSearchType=findClaims' where user_id = '{0}'";

        public const string RevertUserInfoInProfileAndPreferencesForClientUser =
            @"update central.users set prefix_name = 'Mr.', first_name = 'My Profile', 
                                                    last_name = 'For Client',
                                                    maturity_suffix = 'Jr.', job_title = 'claims adjuster', other_suffix = 'M.D.', 
                                                    phone = 2314567890, phone_ext = 111, 
                                                    fax = 2222222222, fax_ext = 222, alternate_phone = 3333333333, alternate_phone_ext = 333,
                                                    email_address = 'uiautomation@cotiviti.com', default_clientcode = 'SMTST', 
                                                    enable_claim_action_tooltips = 'T', auto_claim_history_popup = 'T', subscriber_id = 12345,
                                                    landing_page = '/claims?claimSearchType=findClaims' where user_id = '{0}'";

        #endregion
    }
}
