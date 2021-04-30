using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nucleus.Service.Support.Enum;

namespace Nucleus.Service.SqlScriptObjects.Appeal
{
    public static class AppealSqlScriptObjects
    {
        public const string client = "SMTST";

        public const string GetUploadedDocInfoInAppealRationale = @"select document_file_name filename, to_char(document_last_updated, 'MM/DD/YYYY') uploaded_date
                                                                    from REPOSITORY.APPEAL_RATIONALE where clientcode = '{0}' and editflg = '{1}' and editsrc = '{2}' 
                                                                    and low_proc = {3} and high_proc = {4} and status = 'A' and rownum = 1";

        public const string GetAppealLevel = @"select count(linseq),linseq from ats.appeal_line al
join ats.appeal a on a.appealseq=al.appealseq where claseq={0} and clasub={1} 
and  al.clientcode='{2}' and a.status<> 'R' group by linseq order by  count(linseq) desc";

        public const string UpdateAppealConsultantAndStatus = @"update ats.appeal
         set consultant=null
         , status='W'
         where appealseq={0}";


        public static string GetAppealsUsingAnalysts =
            @"select appealseq from ats.appeal a where primary_reviewer in (select userseq from users where user_id in ('{0}','{1}'))
            and assigned_to in (select userseq from users where user_id in ('{0}','{1}')) and status='N' and appeal_type='A' 
            AND a.appealseq  NOT IN (SELECT appealseq FROM appeal_restriction ar 
                                              JOIN CENTRAL.REF_RESTRICTION rr ON ar.restriction = rr.restriction 
                                             WHERE  rr.USER_TYPE in (1, 2)  AND ar.appealseq = a.appealseq AND 
                                             rr.RESTRICTION NOT IN (SELECT restriction FROM user_restriction_access WHERE userseq in (select userseq from users where user_id  ='{2}')))";



        public const string ConsultantRationales =
            @"select editflg || ' - '  || description consultant,PROREVIEWLETTEREOB 
from hciuser.HCIEDITDESC where edittype='D' and PROREVIEWLETTEREOB is not null
        order by editflg";

        public const string DeleteAppealRationale =
            @"delete from REPOSITORY.APPEAL_RATIONALE where rationaleseq in (select rationaleseq from 
            REPOSITORY.APPEAL_RATIONALE where clientcode = '{0}' and low_proc = '{1}' and high_proc = '{2}' and 
            rationale like '%{3}%')";

        public const string UpdateAppealStatusToIncomplete = @"update ats.appeal set status = 'N' where appealseq in ({0})";
        public const string UpdateAppealStatusToComplete = @"update ats.appeal set status = 'T' where appealseq in ({0})";

        public const string AppealDocumentUploadedAuditRecord =
            @" SELECT COUNT(1) count from ATS.appeal_document ad
       JOIN ATS.appeal_line al on al.appealseq = ad.appealseq
       JOIN ATS.appeal ap on al.appealseq = ap.appealseq
       where al.claseq = '{0}' and ap.status <>'R' and al.clientcode = '" + client + @"'";

        public const string ClientWisePlanList = "Select plan_name from {0}.plans order by plan_name";


        /*The list of users with 'Appeals can be assigned to user' authority */
        //Appeal Processor role
        public const string UserListHavingAppealCanBeAssignedAuthority =
            @"select ( case when restrictions is not null then assigned_to_name || ' - ' || restrictions else
                assigned_to_name end ) as assigned_to_details
                from
                (
                    select assigned_to_name, listagg (description, ',') within group (order by restriction) restrictions
                    from
                        ( 
                             SELECT  u.user_id, u.first_name || ' ' || TRIM(u.last_name) || ' (' || u.user_id || ')' assigned_to_name ,RR.DESCRIPTION, 
                             RR.RESTRICTION from
                             central.users u left join  user_restriction_access ura on u.userseq = URA.USERSEQ
                             left join  ref_restriction rr on URA.RESTRICTION = RR.RESTRICTION 
                             where u.status = 'A' and exists (select 1 from user_role ua where ua.userseq = u.userseq and role_id = 4) 
                            and u.user_type = 2 
                         )
                     group by assigned_to_name, user_id
                 )";
        //@"SELECT  u.first_name || ' ' || TRIM(u.last_name) || ' (' || u.user_id || ')' assigned_to_name FROM users u where status = 'A' and exists (select 1 from user_authorization where userseq = u.userseq and authority_id = 26) and user_type = 2 ";

        public const string UserListHavingAppealCanBeAssignedAuthorityForRestrictedAppeals =
            @"select ( case when restrictions is not null then assigned_to_name || ' - ' || restrictions else
                assigned_to_name end ) as assigned_to_details
                from
                (
                    select assigned_to_name, listagg (description, ',') within group (order by restriction) restrictions
                    from
                    ( 
                         SELECT  u.user_id, u.first_name || ' ' || TRIM(u.last_name) || ' (' || u.user_id || ')' assigned_to_name ,RR.DESCRIPTION, 
                         RR.RESTRICTION from
                         central.users u join  user_restriction_access ura on u.userseq = URA.USERSEQ
                         join  ref_restriction rr on URA.RESTRICTION = RR.RESTRICTION 
                         where u.status = 'A' and exists (select 1 from user_role ua where ua.userseq = u.userseq and role_id = 4) 
                        and u.user_type = 2 
                     )
                     group by assigned_to_name, user_id
                 )";

        public const string GetCountOfDeletedAppealByAppealSequenceList =
            @"select count(*) from ats.appeal where appealseq in ({0}) and status='R'";

        //        public const string CategoryCodeList = @" SELECT COUNT(distinct category_code) from (
        // select (category_code) from ATS.CATEGORY_ASSIGNMENT 
        // UNION
        //SELECT
        //  (CATEGORY)
        //FROM
        //  CENTRAL.APPEAL_CATEGORY_ASSIGNMENT )";

        public const string GetDeletedAppealCount = @"select count(*) from ats.appeal_line  al
        join ats.appeal apl on apl.appealseq=al.appealseq
        where claseq={0} and clasub={1} and apl.clientcode= '" + client + @"' and status='R'";


        public const string GetAssignedClientList = @"select clientcode from client_user where userseq=(select  userseq from central.users where user_id='{0}') order by clientcode";

        public const string GetAssignedClientListForCentene = @"select clientcode from client_user where userseq=(select  userseq from central.users where user_id='{0}') and clientcode like 'CTN%'";

        public const string GetActiveClientList = @"select clientcode from hciclient where active='T' order by clientcode";



        public const string FlagList = "select editflg from REPOSITORY.edit where status='A' order by editflg";

        public const string SourceList = "SELECT value FROM repository.ref_editsrc ORDER BY value";

        public const string ModifierList = "SELECT DISTINCT(CODE) FROM Medical_Code_Description_MV WHERE code_type='MOD'";

        public const string ProcTrigList = "SELECT DISTINCT(CODE) FROM Medical_Code_Description_MV WHERE code_type IN ('CPT','HCPCS')";

        public const string ActiveRationaleCount = "SELECT COUNT(*) FROM   APPEAL_RATIONALE WHERE status='A'";

        public const string DeleteAppealsOnAClaim =
            "delete from ats.appeal where appealseq in (select appealseq from appeal_line where claseq||clasub in ({0}))";
        public const string DeleteClaimFromReatimeClaimSeq= @"delete from central.realtime_claim_queue_pca where CLASEQ||'-'||clasub in ('{0}');";

        public const string DeleteAppealDocument = "delete from SMTST.EXTERNAL_DOCUMENT_TYPE WHERE APPEALSEQ in (select appealseq from ats.appeal_line where claseq={0})";
        public const string DeleteAppealDocumentByClaseq = "delete from SMTST.EXTERNAL_DOCUMENT_TYPE WHERE APPEALSEQ in (select appealseq from ats.appeal_line where claseq||clasub in ({0}))";

        public const string GetAppealDocumentType = "SELECT EXT_DOCUMENT_TYPE FROM SMTST.EXTERNAL_DOCUMENT_TYPE WHERE APPEALSEQ IN (select appealseq from ats.appeal_line where claseq||clasub={0})";

        public static string DeleteAppealLockByAppealSeq =
            "DELETE ATS.locked_appeals WHERE APPEALSEQ={0}";

        public static string DeleteAppealLockByClaimSeq =
            "DELETE ATS.locked_appeals WHERE APPEALSEQ IN (select  distinct appealseq from ats.appeal_line where claseq={0} and clasub={1})";

        public const string DeleteClaimLock =
            "delete from SMTST.LOCKED_CLAIMS where claseq={0} and clasub={1}";

        public static string UpdateAppealStatusByAppealSeq =
                        @"UPDATE ATS.APPEAL
                        SET
                        STATUS = 'N'
                        WHERE appealseq in (SELECT DISTINCT appealseq FROM ats.appeal_line WHERE claseq = {0} AND clasub = {1})";


        public static string UpdateAppealType = @"update ats.appeal set appeal_type='{0}' where appealseq={1} ";

        public static string AppealCountByUser =
            "SELECT count(*) FROM ATS.LOCKED_APPEALS WHERE USERSEQ IN (SELECT USERSEQ FROM USERS WHERE USER_ID='{0}')";

        public static string AppealLock = "select appealseq from ATS.LOCKED_APPEALS WHERE USERSEQ IN (SELECT USERSEQ FROM USERS WHERE USER_ID='{0}')";

        public static string GetTopFlagAndProductForClaimSeq = "SELECT editflg, product from " + client +
                                                     ".line_flag where claseq = {0} and deleted = 'N' and topflag = 'T'";

        public static string GetAppealsOnClaimSeq = "select appealseq from ats.appeal where appealseq in (select appealseq from appeal_line " +
                                                    "where claseq in ({0})) and clientcode='" + client + "' and status = 'N'";

        public static string DeleteAppealLineByLinNo = @"DELETE FROM " + client + ".HCIFLAG WHERE FLAGSEQ = (SELECT MAX(FLAGSEQ) FROM " + client + ".HCIFLAG WHERE CLASEQ = " +
                                                       "{0} AND CLASUB = {1} AND DELETED <> 'N' AND LINNO = {2})";

        public static string ResetAppealToDocumentWaiting = @"DECLARE
           applseq varchar2(20);
           begin
           select appealseq into applseq
           from ats.appeal
           where appealseq  in 
           (select appealseq from ats.appeal_line where claseq={0} and clasub={1})and clientcode='" + client +
                                                                    @"' and status<>'R';
  
          update ats.appeal
        set primary_reviewer=null
        ,assigned_to=null
        ,status='D'
        ,category_code=null
        ,catcodeseq=null
        ,pay1=null
        ,deny1=null
        ,due_date=sysdate
        where appealseq=applseq;
  
        delete from ATS.APPEAL_DOCUMENT where appealseq=applseq;
           end;";

        public static string GetAnalysListWithoutRestrictionDescription =
            @"SELECT  u.first_name || ' ' || TRIM(u.last_name) || ' (' || u.user_id || ')' assigned_to_name FROM users u where status = 'A' and exists (select 1 from user_role where userseq = u.userseq and role_id = 4) and user_type = 2";

        public static string GetAssignedClientListForDCI =
            @"select distinct(hc.clientcode) from HCIUSER.HCICLIENT hc 
            join central.client_user cu on hc.clientcode = cu.clientcode
            where hc.dental_insight_active = 'T' and hc.active = 'T'
            and cu.userseq=(select userseq from central.users where user_id='{0}')";

        public static string GetAppealDueDate =
            @"select to_char(Central.UTILITY.F_GetBusinessDate(sysdate,{0}),'MM/DD/YYYY') from dual";

        public static string GetAppealDueDateForCalendarType = @"select to_char(SMTST.APPEALMANAGEMENT.F_GetCalendarDate(sysdate,{0}),'MM/DD/YYYY') from dual";

        public static string GetEditSrcForClaSeq =
            @"select distinct(editsrc) from smtst.hciflag where claseq = {0} and clasub = {1} and flagseq = {2}";

        public static string DoesClaimRequireStateReview =
                @"select 1 from dual
                where exists
                (
                select claseq, clasub from smtst.hcilin where prvseq in 
                (select prvseq from SMTST.HCIPROV where state in (select state_abbrev from dental_consultant_state_review)) and claseq = {0} and clasub = {1}
                )";

        public static string DeleteAppealAuditHistory =
            @"delete from ats.appeal_audit where appealseq in ({0}, {1})";

        public static string UpdateAppealLineSaveDraft =
            @"update ats.appeal_line set result=null,reasoncode=null,summary=null,RATIONALE=null where claseq={0} and clasub={1} and clientcode='SMTST'";

        public static string GetOutstandingDCIAppeals =
            @"SELECT DISTINCT a.appealseq
    FROM appeal a
    JOIN appeal_line al
    ON (a.appealseq = al.appealseq)
    WHERE a.clientcode IN (SELECT clientcode from client_user where userseq=(select userseq from central.users where user_id = '{0}'))
         AND a.status in ('N','U','S','P','W','K','V','F','M') and a.product = 'D' 
         and a.appealseq NOT IN (SELECT appealseq FROM appeal_restriction ar
                                              JOIN CENTRAL.REF_RESTRICTION rr ON ar.restriction = rr.restriction
                                             WHERE  rr.USER_TYPE in (1, 2)  AND ar.appealseq = a.appealseq AND
                                             rr.RESTRICTION NOT IN (SELECT restriction FROM user_restriction_access WHERE userseq = (select userseq from central.users where user_id = '{0}')))";

        public static string GetLineOfBusiness = @"SELECT description from central.ref_line_of_business order by description";

        public static string DeleteAppealAuditByClaseq =
            @"delete from ats.appeal_audit where appealseq in (select appealseq from ats.appeal_line where claseq={0} and clasub={1})
            and trunc(audit_date)>=trunc(to_date('{2}','dd-Mon-yyyy'))";

        public static string DeleteAppealForQAReviewByAppealSeq = @"delete from qa_review_appeal where appealseq = {0}";

        public static string GetAppealProductFromClaimSequence = @"select product from ats.appeal where appealseq in (select appealseq from ats.appeal_line where claseq={0} and clasub={1})";

        public const string MyAppealsForInternalUser = @"select  distinct a.appealseq,status,product from
                                                         appeal a
                                                          JOIN appeal_line al
                                                          ON (a.appealseq = al.appealseq)
                                                           LEFT OUTER JOIN locked_appeals la
                                                            ON (la.appealseq = a.appealseq)
    WHERE a.clientcode IN (SELECT clientcode from client_user where userseq IN (SELECT USERSEQ FROM USERS WHERE USER_ID='{0}'))
         AND a.appealseq  NOT IN (SELECT appealseq FROM appeal_restriction ar
                                              JOIN CENTRAL.REF_RESTRICTION rr ON ar.restriction = rr.restriction
                                             WHERE  rr.USER_TYPE in (1, 2)  AND ar.appealseq = a.appealseq AND
                                             rr.RESTRICTION NOT IN (SELECT restriction FROM user_restriction_access WHERE userseq IN (SELECT USERSEQ FROM USERS WHERE USER_ID='{0}')))
                                             and ASSIGNED_TO IN (SELECT USERSEQ FROM USERS WHERE USER_ID='{0}')
                                             and status in ('N','U','S','P','W','K','V','F','M')
                                             order by appealseq asc";


        public const string MyAppealsForClientUser = @"select  distinct a.appealseq,status,product from
                                                         appeal a
                                                          JOIN appeal_line al
                                                          ON (a.appealseq = al.appealseq)
                                                           LEFT OUTER JOIN locked_appeals la
                                                            ON (la.appealseq = a.appealseq)
    WHERE a.clientcode IN (SELECT clientcode from client_user where userseq IN (SELECT USERSEQ FROM USERS WHERE USER_ID='{0}'))
         AND a.appealseq  NOT IN (SELECT appealseq FROM appeal_restriction ar
                                              JOIN CENTRAL.REF_RESTRICTION rr ON ar.restriction = rr.restriction
                                             WHERE  rr.USER_TYPE in (1, 8) AND ar.appealseq = a.appealseq AND
                                             rr.RESTRICTION NOT IN (SELECT restriction FROM user_restriction_access WHERE userseq IN (SELECT USERSEQ FROM USERS WHERE USER_ID='{0}')))
                                             and created_by
                                             IN (SELECT USERSEQ FROM USERS WHERE USER_ID='{0}')
                                             and status not in ('C','R')
                                             and AL.CLIENTCODE='SMTST'                                          
                                             order by appealseq asc";

        public const string GetHolidays = @"select to_char(holiday_date,'mm/dd/yyy') from CLIENT_HOLIDAYS where holiday_date between '{0}' and '{1}'";


        public const string GetAllurgentAppealsForInternalUser = @"select  distinct a.appealseq,status from
appeal a
    JOIN appeal_line al
    ON (a.appealseq = al.appealseq)
    LEFT OUTER JOIN locked_appeals la
    ON (la.appealseq = a.appealseq)
    WHERE a.clientcode IN (SELECT clientcode from client_user where userseq in (select userseq from users where user_id='{0}'))
         AND a.appealseq  NOT IN (SELECT appealseq FROM appeal_restriction ar
                                              JOIN CENTRAL.REF_RESTRICTION rr ON ar.restriction = rr.restriction
                                             WHERE  rr.USER_TYPE in (1, 2)  AND ar.appealseq = a.appealseq AND
                                             rr.RESTRICTION NOT IN (SELECT restriction FROM user_restriction_access WHERE userseq in (select userseq from users where user_id='{0}')))
                                             and PRIORITY='U'
                                             and status in ('N','M')
                                             and product not in ('D')
                                             order by status asc";

        public const string GetRecordReviewAppealsForInternalUser = @"select  distinct a.appealseq,status,product from
appeal a
    JOIN appeal_line al
    ON (a.appealseq = al.appealseq)
    LEFT OUTER JOIN locked_appeals la
    ON (la.appealseq = a.appealseq)
    WHERE a.clientcode IN (SELECT clientcode from client_user where userseq in (select userseq from users where user_id='{0}'))
         AND a.appealseq  NOT IN (SELECT appealseq FROM appeal_restriction ar
                                              JOIN CENTRAL.REF_RESTRICTION rr ON ar.restriction = rr.restriction
                                             WHERE  rr.USER_TYPE in (1, 2)  AND ar.appealseq = a.appealseq AND
                                             rr.RESTRICTION NOT IN (SELECT restriction FROM user_restriction_access WHERE userseq in (select userseq from users where user_id='{0}')))
                                             and appeal_type='R'
                                             and status in ('N','M')
                                             order by status asc";

        public const string GetAppealsDueToday = @"select   distinct a.appealseq,status,product from
appeal a
    JOIN appeal_line al
    ON (a.appealseq = al.appealseq)
    LEFT OUTER JOIN locked_appeals la
    ON (la.appealseq = a.appealseq)
    WHERE a.clientcode IN (SELECT clientcode from client_user where userseq in (select userseq from users where user_id='{0}'))
         AND a.appealseq  NOT IN (SELECT appealseq FROM appeal_restriction ar
                                              JOIN CENTRAL.REF_RESTRICTION rr ON ar.restriction = rr.restriction
                                             WHERE  rr.USER_TYPE in (1, 2)  AND ar.appealseq = a.appealseq AND
                                             rr.RESTRICTION NOT IN (SELECT restriction FROM user_restriction_access WHERE userseq in (select userseq from users where user_id='{0}')))
                                            and status in ('N','M')
                                            and due_date <sysdate and a.clientcode='SMTST' AND a.APPEAL_TYPE='R'
                                           order by status asc ";

        public const string GetAppealReasonCodesSqlScript = @"select trim(code || ' - ' || shortdesc) reasoncode 
                                        from hciuser.overreason
                                        where type ='A' and product in ('A','{0}') and appeal_result_type in ('L', '{1}')
                                        order by reasoncode";

        public const string GetLatestAuditDataInAppealCategoryManager = @"select * from
    (select to_char(auditdate,'MM/DD/YYYY'),replace(product,'PCI','CV'),catorder,
     regexp_replace(lowproc||'-'||highproc,'^-',null),
    regexp_replace(lowtrigproc||'-'||hightrigproc,'^-', null),
    (select user_id from central.users where userseq = aca.moduserseq) as lastmodifieduser,
    regexp_replace(notes, '<.*?>')
    from central.appealcategoryassignment_audit aca
    where category = '{0}' order by auditdate desc) where rownum = 1";

        public const string deleteCategorySpecificAppeal = "delete from ats.appeal where clientcode='SMTST' and category_code='{0}'";


        public const string GetLatestNonRestricetedAnalystAuditData = @"with analysts as (select * from
    (select 
    assigned_analysts
    from central.appealcategoryassignment_audit aca
    where category = '{0}' order by auditdate desc) where rownum = 1)
select LISTAGG(user_id,', ') within group (order by user_id) from (select user_id from central.users u,analysts a
          where userseq in (select regexp_substr(a.assigned_analysts
          ,'[^,]+', 1, level) from dual
    connect by regexp_substr(a.assigned_analysts
    , '[^,]+', 1, level) is not null))";

        public const string GetLatestRestrictedAnalystAuditData = @"with analysts as (select * from
    (select 
    unrestricted_assigned_analysts
    from central.appealcategoryassignment_audit aca
    where category = '{0}' order by auditdate desc) where rownum = 1)
select LISTAGG(user_id,', ') within group (order by user_id) from (select user_id from central.users u,analysts a
          where userseq in (select regexp_substr(a.unrestricted_assigned_analysts
          ,'[^,]+', 1, level) from dual
    connect by regexp_substr(a.unrestricted_assigned_analysts
    , '[^,]+', 1, level) is not null))";

        public const string GetAppealCategoryForAppeals = @"select A.CATEGORY from APPEAL_CATEGORY_ASSIGNMENT A 
        join appeal_category_client c
        on a.catcodeseq= c.catcodeseq
        WHERE 
        c.CLIENTCODE='SMTST' AND
        FLAGS IN (SELECT EDITFLG FROM SMTST.HCIFLAG WHERE CLASEQ||CLASUB={0} AND TOPFLAG='T') AND DELETED='F'
        ORDER BY A.CAT_ORDER FETCH FIRST 1 ROW ONLY"; 
        
        public const string GetCVFlagsForAddCategory =
            @"select * from edit where product = 'F' and status = 'A' order by editflg asc";

        public const string EnableDisableDCIForClient =
            @"UPDATE CLIENT SET dci_active='{1}' where clientcode='{0}'";

        public const string GetMedicalRecordReviewAppealsFromDb = @"select appealseq from ats.appeal where appeal_type = 'M' and clientcode = 'SMTST' and create_date between '20-SEP-20' and '26-SEP-20' and status not in ('R')";
        public const string GetAppeaStatusFromRealTimeQueue = @"select review_type,review_status from central.realtime_claim_queue_pca where CLASEQ||'-'||clasub in ('{0}')";
        public const string DeleteFromRealTimeQueue = @"delete from central.realtime_claim_queue_pca where CLASEQ||'-'||clasub in ('{0}')";

        public const string GetAppealHelpfulHintsInfoInAppealRationale = @"select helpful_hints
                                                                    from REPOSITORY.APPEAL_RATIONALE where clientcode = '{0}' and editflg = '{1}' and editsrc = '{2}' 
                                                                    and low_proc = {3} and high_proc = {4} and status = 'A' and rownum = 1";

        public const string GetMaxCategoryOrderFromDb = "select max(cat_order) from CENTRAL.APPEAL_CATEGORY_ASSIGNMENT where deleted = 'F'";

        public const string GetAppealCompletedDateFromDb = "select to_char(completed_date, 'MM/DD/YYYY') from ats.appeal where appealseq = '{0}'";

        public const string GetFormTypeInfoInAppealRationale = @"select form_type
                                                                    from REPOSITORY.APPEAL_RATIONALE where clientcode = '{0}' and editflg = '{1}' and editsrc = '{2}' 
                                                                    and low_proc = {3} and high_proc = {4} and status = 'A' and rownum = 1";

        public const string UpdateFormTypeInfoInAppealRationale = @"update REPOSITORY.APPEAL_RATIONALE set form_type = {5} where clientcode = '{0}' and editflg = '{1}' and editsrc = '{2}' 
                                                                    and low_proc = {3} and high_proc = {4} and status = 'A' and rownum = 1";

        public const string GetClaimFormTypeFromDb = @"select form_type from smtst.hcicla_nucleus where claseq = {0} and clasub = {1}";

        public const string UpdateClaimFormTypeFromDb = @"update smtst.hcicla_nucleus set form_type='{2}' where claseq = {0} and clasub = {1}";

    }
}
