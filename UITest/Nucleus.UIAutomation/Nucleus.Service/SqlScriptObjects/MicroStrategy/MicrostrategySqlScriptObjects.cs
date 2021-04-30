using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Nucleus.Service.SqlScriptObjects.MicroStrategy
{
    public class MicrostrategySqlScriptObjects

    {

        public const string ReportNamesForUser =
            "SELECT  MR.REPORT_NAME FROM CENTRAL.USERS U INNER JOIN CENTRAL.USER_ROLE UR ON U.USERSEQ=UR.USERSEQ INNER JOIN CENTRAL.MSTR_REPORTS MR ON UR.ROLE_ID = MR.ROLE_ID WHERE MR.ISACTIVE= 'T' AND UR.USERSEQ IN (SELECT USERSEQ FROM CENTRAL.USERS WHERE USER_ID= '{0}')";
           
      
            
        public const string NumberOfReportsAssigned =
                "SELECT count(*) FROM CENTRAL.USERS U INNER JOIN CENTRAL.USER_ROLE UR ON U.USERSEQ=UR.USERSEQ INNER JOIN CENTRAL.MSTR_REPORTS MR ON UR.ROLE_ID = MR.ROLE_ID WHERE MR.ISACTIVE= 'T' AND UR.USERSEQ IN (SELECT USERSEQ FROM CENTRAL.USERS WHERE USER_ID= '{0}')"
            ;

        public const string ReportInfoForUser = "SELECT MR.REPORT_NAME, (SELECT RR.DESCRIPTION FROM CENTRAL.REF_ROLE RR WHERE RR.ID=MR.ROLE_ID)ROLENAME, (SELECT FIRST_NAME||' '||LAST_NAME FROM CENTRAL.USERS A WHERE A.USERSEQ = MR.CREATED_BY) as CREATED_BY, to_number(to_char(CREATED_ON,'mm')) || '/' || to_number(to_char(CREATED_ON,'dd')) || '/' || to_number(to_char(CREATED_ON,'yyyy')) ||' ' || to_number(to_char(CREATED_ON,'HH')) || ':' || (to_char(CREATED_ON,'MI')) || ':' || to_char(CREATED_ON,'SS ')||(to_char(CREATED_ON,'AM')) as CREATEDATE FROM CENTRAL.USERS U INNER JOIN CENTRAL.USER_ROLE UR ON U.USERSEQ=UR.USERSEQ INNER JOIN CENTRAL.MSTR_REPORTS MR ON UR.ROLE_ID = MR.ROLE_ID WHERE U.USER_ID= '{0}'AND MR.ISACTIVE='T' order by MR.CREATED_ON desc";

        public const string RoleMappedForReport = "SELECT REPORT_NAME FROM CENTRAL.MSTR_REPORTS WHERE ROLE_ID IN(select ID from ref_role where description in ('{0}','{1}') ) AND ISACTIVE='T'"
            ;

        public const string CountOfRolesmapped =
            "SELECT count(*) FROM CENTRAL.MSTR_REPORTS WHERE ROLE_ID IN(select ID from CENTRAL.ref_role where description in ('{0}','{1}') ) AND ISACTIVE='T'";

        public const string UpdateToRemoveRoleMappedForReport = "UPDATE CENTRAL.MSTR_REPORTS SET ISACTIVE='{2}' WHERE ROLE_ID IN(select id from central.ref_role where description in ('{0}','{1}'))"
            
                        ;
        #region MicrostrategyMaintainance

        //public const string ExistingGroupNames = "SELECT GROUP_NAME FROM central.MSTR_GROUP ";
       // public const string ExistingUserNames = "SELECT USERNAME FROM central.MSTR_GROUP";
        public const string ExistingUserGroupDetailsFromDatabase = "SELECT GROUP_NAME,USERNAME FROM CENTRAL.MSTR_GROUP WHERE ISACTIVE='T' ORDER BY lower(GROUP_NAME) ASC";

        public const string CreatedOrEditedUsergroup =
            "SELECT COUNT(*) FROM central.MSTR_GROUP WHERE GROUP_NAME='{0}' AND USERNAME='{1}' AND ISACTIVE='T'";
        public const string UserGrouptoDelete =
            "SELECT COUNT(*) FROM central.MSTR_GROUP WHERE GROUP_NAME='{0}' AND USERNAME='{1}'";
        public const string CreatedUsergroup =
            "SELECT COUNT(*) FROM central.MSTR_GROUP WHERE GROUP_NAME='{0}' AND USERNAME='{1}'";

        public const string MstrGroupstatus = "SELECT ISACTIVE FROM CENTRAL.MSTR_GROUP WHERE GROUP_NAME='{0}' AND USERNAME='{1}'";
        public const string DeletmstrUserGroupValue = @"BEGIN 
                                                        DELETE FROM central.mstr_user_group
                                                            WHERE GROUP_ID IN(SELECT ID FROM central.mstr_group WHERE group_name= '{0}' AND username = '{1}');

                                                        DELETE FROM central.mstr_group WHERE group_name='{0}' AND username = '{1}';

                                                        END;";


        public const string MstrReportsCount =
                @"SELECT COUNT(1) FROM CENTRAL.MSTR_REPORTS MR LEFT OUTER JOIN CENTRAL.REF_ROLE RR ON MR.ROLE_ID= RR.ID WHERE MR.IsActive= 'T'"
            ;

        public const string MstrReports =
                @"SELECT * FROM (SELECT MR.REPORT_NAME, MR.PROJECT_ID, MR.REPORT_ID, RR.Description FROM Central.MSTR_REPORTS MR INNER JOIN CENTRAL.REF_ROLE RR ON MR.ROLE_ID=RR.ID AND MR.IsActive= 'T') WHERE ROWNUM = 1"
            ;


        public const string RoleList = @"SELECT ROLE_NAME FROM CENTRAL.REF_ROLE ORDER BY UPPER(DESCRIPTION)";

        public const string MstrUserIDsPerGroupName =
                @"select USER_ID from userS WHERE USERSEQ IN (select userseq from central.mstr_user_group where group_id in
                                                        (select id from central.mstr_group where group_name = '{0}'))ORDER BY USER_ID"
            ;

        public const string GetMstrReportsByReportName =
                @"SELECT MR.REPORT_NAME, MR.PROJECT_ID, MR.REPORT_ID, RR.ROLE_NAME FROM Central.MSTR_REPORTS MR INNER JOIN CENTRAL.REF_ROLE RR ON MR.ROLE_ID=RR.ID AND MR.IsActive= 'T' WHERE REPORT_NAME='{0}'"
            ;

        public const string GetUserNameDropDownList =
        
        @"SELECT REGEXP_REPLACE(trim(lower(u.first_name)), ' {2,}', ' ')||' '||REGEXP_REPLACE(trim(lower(u.last_name)) , ' {2,}', ' ') fullname FROM CENTRAL.USERS U 
        WHERE u.userseq not in (select userseq FROM CENTRAL.MSTR_USER_GROUP ug inner join CENTRAL.MSTR_GROUP mg on ug.group_id= mg.id where mg.isactive= 'T')
        and U.STATUS='A' order by lower(fullname)";


        public const string CountofUserGroups = "SELECT COUNT(1) FROM CENTRAL.MSTR_GROUP WHERE ISACTIVE='T'";

        public const string PasswordForUserGroup =
            "SELECT PASSWORD FROM CENTRAL.MSTR_GROUP WHERE GROUP_NAME='{0}' AND USERNAME='{1}'";



        public const string DeleteMstrReportByReportName =
            "delete from CENTRAL.MSTR_REPORTS where Report_Name='{0}'";


        #endregion
    }
}
