using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nucleus.Service.SqlScriptObjects.QA
{
    public class QASqlScriptObjects
    {
      
        public const string client = "SMTST";

        public const string ActiveQAManagerList =
                        /*@"select u.first_name || ' ' || u.last_name fullname, 
            nvl2(uqp.START_DATE,to_char(uqp.START_DATE,'mm/dd/yyyy'),'') ||
            nvl2(uqp.end_date,' - ' || to_char(uqp.end_date,'mm/dd/yyyy'),'') 
            CLAIM_QA_DATE,           
            nvl2(uqp.APPEAL_WEEKLY_MAX,'Ongoing QA','Trainee QA') appeal_qa
            from central.user_qa_parameters uqp
                        join central.users  u on u.userseq=uqp.userseq
                        where 
            (APPEAL_REVIEW_FREQUENCY is not null
            or APPEAL_DAILY_MAX is not null
            or APPEAL_WEEKLY_MAX is not null

            )
            union
            select u.first_name || ' ' || u.last_name fullname, 
            nvl2(uqp.START_DATE,to_char(uqp.START_DATE,'mm/dd/yyyy'),'') ||
            nvl2(uqp.end_date,' - ' || to_char(uqp.end_date,'mm/dd/yyyy'),'') 
            CLAIM_QA_DATE,          
            nvl2(uqp.APPEAL_WEEKLY_MAX,'Ongoing QA',nvl2(uqp.APPEAL_REVIEW_FREQUENCY,'Trainee QA','No Appeal QA')) appeal_qa
                        from central.user_qa_parameters  uqp
                        join central.users  u on u.userseq=uqp.userseq
            where trunc(end_date)>trunc(sysdate)"*/
                        @"select u.first_name || ' ' || u.last_name fullname
                        from central.user_qa_parameters  uqp
                        join central.users  u on u.userseq=uqp.userseq
                        where (trunc(end_date)>trunc(sysdate) or uqp.appeal_parameter_type in ('T','O')) and u.status = 'A'";

        public const string GetInactiveQAManagerList = @"select trim(first_name) || ' ' || trim(last_name)  from
                                                        central.users where userseq in (select userseq from 
                                                            central.users where status='A' and user_type=2 and userseq>0
                                                        MINUS
                                                        select u.userseq
                                                            from central.user_qa_parameters  uqp
                                                            join central.users  u on u.userseq=uqp.userseq
                                                            where (trunc(end_date)>trunc(sysdate) 
                                                            or uqp.appeal_parameter_type in ('T', 'O')))";

        public const string ActiveInactiveUserList = @"select trim(first_name) || ' ' || trim(last_name) fullname from 
central.users where status='A' and user_type=2 and userseq>0";

        public const string ChangeStatusOfClaim = @"update  " + client + @".hcicla set status = 'U' where  claseq = {0}";

        public const string UpdateQAStatusofClaim =
            @"UPDATE qa_review_claim SET status='A' where claseq ={0}";

        public const string RevertClaimApprovalRecordInQa = @"delete from central.qa_review_claim  where  claseq = {0}";
        public const string DeleteAuditRecordForClaimToRevertApproval = @"Delete from     " + client + @".claim_audit where  claseq = {0} and status ='{1}' ";
       
        public const string DeleteQaAuditRecordOfClaimApproveAndChangeClaimStatus = @"
        BEGIN
        UPDATE {3}.hcicla set status = 'U' , reltoclient='F'  where  claseq = {0} and clasub={1};
        DELETE from central.qa_review_claim  where  claseq = {0}  and clasub={1} and clientcode='{3}';
        DELETE from {3}.claim_audit where  claseq = {0}  and clasub={1} and trunc(audit_date) >= to_date('{2}','DD-MON-YYYY');
        END;";

        public const string DeleteQaAuditRecordOfClaimApproveAndChangeClaimStatusForQCReview = @"
        BEGIN
        UPDATE smtst.hcicla set status = 'U' , reltoclient='F'  where  claseq = {0} and clasub={1};
        DELETE from smtst.claim_qa_audit  where  claseq = {0} and clasub={1};
        DELETE from smtst.claim_audit where  claseq = {0} and clasub={1} and trunc(audit_date) >= to_date('{2}','DD-MON-YYYY');
        END;";

        public const string ResetUserQaCounterToDefault = @"
        UPDATE user_qa_counters set daily_flagged_count = 0 where userseq = (select userseq from central.users where user_id='{0}')";
        public const string DeleteAuditRecordOfClaimApproveAndChangeClaimStatus = @"
        BEGIN
        DELETE from " + client + @".claim_audit where  claseq = {0} and trunc(audit_date) >= to_date('{1}','DD-MON-YYYY');
        UPDATE " + client + @".hcicla set status = 'U' where  claseq = {0};
        END;";

        public const string AnalystList =
            @"SELECT  TRIM(u.first_name) || ' ' || TRIM(u.last_name) sassigned_to_name 
            FROM users u where status = 'A' and exists (select 1 from user_role ua where ua.userseq = u.userseq and role_id = 8)  and user_type = 2 ";
       
        public const string QaTargetResultsHistoryforUserSequence =
            @"select to_char(created_date,'MM/DD/YYYY') Cdate from central.qa_review_claim where userseq=(select userseq from central.users where user_id='{0}') and status <> 'A' group by  to_char(created_date,'MM/DD/YYYY') order by Cdate desc";


        public const string ClaimAuditByClaimAndAuditDate = @"select * from {0}.claim_audit where claseq={1} and clasub={2}
        and audit_action='APPROVE' and trunc(audit_date)=to_date('{3}','mm/dd/yyyy')";

        public const string QaReviewClaimByUserAndCreatedDate = @"select claseq || '-' || clasub,clientcode
            from central.qa_review_claim 
            where userseq=(select userseq from central.users where user_id='{0}')
            and trunc(created_date)=to_date('{1}','mm/dd/yyyy')";

        public const string TotalFlaggedClaimsCount = @"select count(claseq) from central.qa_review_claim 
            where userseq=(select userseq from central.users where user_id='{0}')
            and trunc(created_date)=to_date('{1}','mm/dd/yyyy')
            and status<>'A'";

        public const string TotalPassedClaimsCount = @"select count(claseq) from central.qa_review_claim 
            where userseq=(select userseq from central.users where user_id='{0}')
            and trunc(created_date)=to_date('{1}','mm/dd/yyyy')
            and status='P'";


        #region QAClaimSearch

        public const string GetCountofQaPassAudit =
            @"select count(*) from " + client +
            @".claim_audit where claseq={0} and audit_action='QAPASS' order by audit_date";

        public const string GetCountofQaFailAudit =
            @"select count(*) from " + client +
            @".claim_audit where claseq={0} and audit_action='QAFAIL' order by audit_date";

        public const string GetCountofNoQaResultAudit =
            @"select count(*) from " + client +
            @".claim_audit where claseq={0} and (audit_action='QAFAIL' or audit_action='QAPASS') order by audit_date";

        public const string GetAssignedClientList = @"select clientcode from client_user where userseq=(select  userseq from central.users where user_id='{0}') order by clientcode";

        public const string GetTotalQaReviewClaimCount = "select count(*) from central.qa_review_claim where clientcode in (select clientcode from client_user where userseq=(select  userseq from central.users where user_id='{0}') )";


        public const string OutstandingQaClaimsList =
            @"select claseq || '-' || clasub from central.qa_review_claim where  status = 'A' and  clientcode in (select clientcode from client_user where userseq=(select  userseq from central.users where user_id='{0}') )";

        public const string QaClaimsListForAnalystWithStatus = @"
             select claseq || '-' || clasub from central.qa_review_claim where userseq=(select  userseq from central.users where user_id='{0}') and status = '{1}'";
        public const string QaClaimsListForClientWithStatus = @"
             select claseq || '-' || clasub from central.qa_review_claim where clientcode = '{0}' and status = '{1}'";
        public const string QaClaimsListForAnalyst = @"
             select claseq || '-' || clasub from central.qa_review_claim where userseq=(select  userseq from central.users where user_id='{0}') ";
        public const string QaClaimsListForClient = @"
             select claseq || '-' || clasub from central.qa_review_claim where clientcode = '{0}'";
        public const string QaClaimsListForApproveDate = @"
             select claseq || '-' || clasub from central.qa_review_claim where trunc(created_date) = to_date('{0}','MM/DD/YYYY')";
        public const string QaClaimsListForQaReviewer= @"
             select claseq || '-' || clasub from central.qa_review_claim where reviewed_by=(select  userseq from central.users where user_id='{0}')  ";
        public const string QaClaimsListForReviewDate = @"
             select claseq || '-' || clasub from central.qa_review_claim where trunc(reviewed_date) = to_date('{0}','MM/DD/YYYY') ";
        public const string QaClaimsListForStatus = @"
             select claseq || '-' || clasub from central.qa_review_claim where status = '{0}'";

        public const string CheckIfClaimIsSelectedForDailyQcByCount = @"select Count(*) from smtst.claim_qa_audit where claseq = {0} and clasub={1}";

        public const string UpdateDeleteQaClaimReviewStatusForClaim = @"
        BEGIN
        UPDATE qa_review_claim set status = 'A' , reviewed_date=null where claseq = {0} and clientcode = '" + client + @"';
        DELETE from " + client + @".claim_audit where claseq = {0} and (audit_action='QAPASS' or audit_action='QAFAIL' or audit_action='OPEN');
        delete from central.qa_review_line where linseq in (select linseq from smtst.claim_line where claseq={0});        
        END;";

        public const string DeleteClaimLock =
           "delete from SMTST.LOCKED_CLAIMS where claseq={0} and clasub={1}";

        public const string QAClaimSearchAssignedRestrictionSearchResult = @"    SELECT qrc.claseq ||'-'|| qrc.clasub
                FROM central.qa_review_claim qrc
                JOIN users u
                    ON u.userseq = qrc.userseq
                LEFT JOIN central.claim_restriction cr ON qrc.clientcode = cr.clientcode and qrc.claseq = cr.claseq and qrc.clasub = cr.clasub
                LEFT JOIN central.ref_restriction rr ON cr.restriction = rr.restriction
                 WHERE qrc.clientcode IN (SELECT clientcode FROM client_user WHERE USERSEQ = (select userseq from central.users where user_id = '{0}'))
                 and qrc.status in ('A','P','F')
                 and u.user_id = '{0}'
                and rr.restriction = {1}
                ORDER BY created_date asc";

        public const string QAClaimSearchResultForAllRestrctionForOutStandingQA =
            @"SELECT qrc.claseq ||'-'|| qrc.clasub
                FROM central.qa_review_claim qrc
                JOIN users u
                    ON u.userseq = qrc.userseq
                LEFT JOIN central.claim_restriction cr ON qrc.clientcode = cr.clientcode and qrc.claseq = cr.claseq and qrc.clasub = cr.clasub
                LEFT JOIN central.ref_restriction rr ON cr.restriction = rr.restriction
                 WHERE qrc.clientcode IN (SELECT clientcode FROM client_user WHERE userseq = (select userseq from central.users where user_id = '{0}'))
                 and qrc.status ='A'
                 and u.user_id = '{0}'                            
                ORDER BY created_date asc";

        public const string QAClaimsSearchResultForNoRestrctionForOutstandingQA =
            @"SELECT qrc.claseq ||'-'|| qrc.clasub
                FROM central.qa_review_claim qrc
                JOIN users u
                    ON u.userseq = qrc.userseq
                LEFT JOIN central.claim_restriction cr ON qrc.clientcode = cr.clientcode and qrc.claseq = cr.claseq and qrc.clasub = cr.clasub
                LEFT JOIN central.ref_restriction rr ON cr.restriction = rr.restriction
                 WHERE qrc.clientcode IN (SELECT clientcode FROM client_user WHERE userseq = (select userseq from central.users where user_id = '{0}'))
                 and qrc.status ='A'
                 and u.user_id = '{0}'                 
                 and rr.restriction is null
                ORDER BY created_date asc";

        public const string QAClaimsAllClaimsAllRestrcitionSearchResult = @"SELECT qrc.claseq ||'-'|| qrc.clasub
                FROM central.qa_review_claim qrc
                JOIN users u
                    ON u.userseq = qrc.userseq
                LEFT JOIN central.claim_restriction cr ON qrc.clientcode = cr.clientcode and qrc.claseq = cr.claseq and qrc.clasub = cr.clasub
                LEFT JOIN central.ref_restriction rr ON cr.restriction = rr.restriction
                 WHERE qrc.clientcode IN (SELECT clientcode FROM client_user WHERE USERSEQ = (select userseq from central.users where user_id = '{0}'))
                 and qrc.status in ('A','P','F')
                 and u.user_id = '{0}'               
                ORDER BY created_date asc";

        public const string QAAllClaimsNoRestrictionSearchResult = @"SELECT qrc.claseq ||'-'|| qrc.clasub
                FROM central.qa_review_claim qrc
                JOIN users u
                    ON u.userseq = qrc.userseq
                LEFT JOIN central.claim_restriction cr ON qrc.clientcode = cr.clientcode and qrc.claseq = cr.claseq and qrc.clasub = cr.clasub
                LEFT JOIN central.ref_restriction rr ON cr.restriction = rr.restriction
                 WHERE qrc.clientcode IN (SELECT clientcode FROM client_user WHERE USERSEQ = (select userseq from central.users where user_id = '{0}'))
                 and qrc.status in ('A','P','F')
                 and u.user_id = '{0}'                 
                and rr.restriction is null
                ORDER BY created_date asc";

        #endregion

        #region QAAppealSearch

        public const string ChangeAppealStatusToNew = "update ats.appeal set status='N' where appealseq in ({0})";

        public static string DeleteFromQAAppeal = " delete from CENTRAL.QA_REVIEW_APPEAL where appealseq in ({0})";
        
        public static string ResetQAppealCounter = "update user_qa_appeal_counters set interval_completed_count = 0, DAILY_FLAGGED_COUNT = 0 where userseq = (select userseq from central.users where user_id='{0}')";

        public static string UpdateQAReviewToNULL = @"update CENTRAL.QA_REVIEW_APPEAL set REVIEWED_DATE = null, 
                    REVIEWED_BY = null, reviewed = 'F' where APPEALSEQ IN  ({0}) and clientcode = '" + client + @"' and userseq = 
                    (select userseq from central.users where user_id='{1}')";

        public static string UpdateQAReviewToNULLAndUpdateCompletedDateToday = @"update CENTRAL.QA_REVIEW_APPEAL set REVIEWED_DATE = null, 
                    REVIEWED_BY = null, reviewed = 'F' ,completed_date=sysdate where APPEALSEQ IN  ({0}) and clientcode = '" + client +"'";

        public static string UpdateQAReviewToReviewed = @"update CENTRAL.QA_REVIEW_APPEAL set reviewed = 'T' where APPEALSEQ  in ({0}) and clientcode = '" + client + "'";


        public static string UpdateQAReviewToNULLForSingleAppeal = @"update CENTRAL.QA_REVIEW_APPEAL set REVIEWED_DATE = null, 
                    REVIEWED_BY = null, reviewed = 'F' where APPEALSEQ ={0} and clientcode = '" + client + @"' and userseq = 
                    (select userseq from central.users where user_id='{1}')";

        public static string AssignedRestrictionForUser =
            @"select restriction from ref_restriction where restriction in (select 
                restriction from user_restriction_access where userseq=(select userseq from central.users where user_id='{0}')) order by description";

        public static string AllRestrictionSearchResult =
            @"select appealseq from CENTRAL.QA_REVIEW_APPEAL  where APPEALSEQ IN (SELECT APPEALSEQ FROM ATS.APPEAL) and userseq = (select userseq from central.users where user_id='{0}') order by appealseq desc";

        public static string AllRestrictionSearchResultForOutstandingQAAppeals =
            @"select appealseq from CENTRAL.QA_REVIEW_APPEAL  where APPEALSEQ IN (SELECT APPEALSEQ FROM ATS.APPEAL) and clientcode = 'SMTST' and userseq = (select userseq from central.users where user_id='{0}') and reviewed = 'F' order by appealseq desc";

        public static string NoRestrictionSearchResult =
            @"select appealseq from CENTRAL.QA_REVIEW_APPEAL  where APPEALSEQ IN (SELECT APPEALSEQ FROM ATS.APPEAL where patseq  not in
(select patseq from smtst.patient_restriction )) AND userseq = (select userseq from central.users where user_id='{0}') order by appealseq desc";

        public static string NoRestrictionSearchResultForOutstandingQAAppeals =
            @"select appealseq from CENTRAL.QA_REVIEW_APPEAL  where APPEALSEQ IN (SELECT APPEALSEQ FROM ATS.APPEAL where patseq  not in
(select patseq from smtst.patient_restriction )) AND clientcode = 'SMTST' and userseq = (select userseq from central.users where user_id='{0}')  and reviewed = 'F' order by appealseq desc";

        public static string AssignedRestrictionSearchResult =
            @"select appealseq from CENTRAL.QA_REVIEW_APPEAL  where APPEALSEQ IN (SELECT APPEALSEQ FROM ATS.APPEAL  where patseq   in
(select patseq from smtst.patient_restriction where restriction = '{1}' ))AND clientcode = 'SMTST' and userseq = (select userseq from central.users where user_id='{0}') order by appealseq desc";

        public static string AssignedRestrictionSearchResultForOutstandingQAAppeals =
            @"select appealseq from CENTRAL.QA_REVIEW_APPEAL  where APPEALSEQ IN (SELECT APPEALSEQ FROM ATS.APPEAL  where patseq   in
(select patseq from smtst.patient_restriction where restriction = '{1}' ))AND clientcode = 'SMTST' and userseq = (select userseq from central.users where user_id='{0}') and reviewed = 'F' order by appealseq desc";

        public static string OutStandingAppealSearchResultByCategory = @"select a.appealseq from central.qa_review_appeal q join ats.appeal a on q.appealseq=a.appealseq
where cateGory_code='{0}' 
and reviewed = 'F'";
        public static string AllAppealSearchResultByCategory = @"select a.appealseq from central.qa_review_appeal q join ats.appeal a on q.appealseq=a.appealseq
where cateGory_code='{0}' ";

        public static string OutstandingQaAppealsGridResult =
            @"select qra.clientcode, qra.appealseq, a.category_code,  qra.completed_date, 
            (select first_name || ' ' || last_name from central.users where userseq = qra.userseq) as AppealAnalyst ,
            (select first_name || ' ' || last_name from central.users where userseq = qra.reviewed_by) as Qa_Reviewer , 
            qra.score, qra.reviewed_date 
            from CENTRAL.qa_review_appeal qra join ats.appeal a on qra.appealseq = a.appealseq
             {0}";

        public static string GetAuditSeqFromDBAfterExport = @"select max(auditseq) from smtst.DOCUMENT_ACTION_AUDIT 
                                                            where userseq = (select userseq from users where user_id = '{0}')
                                                            and trunc(action_time) = to_date('{1}', 'mm/dd/yyyy')
                                                            and action_source = '/api/clients/SMTST/qaAppeals/DownloadQAAppealSearchXLS/'";

        #endregion

        #region AnalystManager
        public const string AppealCategoryCodes = @"select distinct(category) from CENTRAL.APPEAL_CATEGORY_ASSIGNMENT where deleted = 'F' order by category";
        //      public const string GetCurrentRestrictedAndNonRestrictedClaimsAssignmentsByAnalysts = @"SELECT category,
        //           decode(ap.clientcode,null,'ALL',ap.clientcode),replace(product,'PCI','CV'),
        //                              regexp_replace(lowproc||'-'||highproc,'^-',null),
        //                              regexp_replace(lowtrigproc||'-'||hightrigproc,'^-', null)  
        //FROM appeal_category_Assignment ac left join appeal_category_client ap on ac.catcodeseq = ap.catcodeseq
        //WHERE deleted = 'F' and AC.{0} like ('%{1}')
        //ORDER BY category";

        public const string GetCurrentRestrictedAndNonRestrictedClaimsAssignmentsByAnalysts = @"SELECT distinct category,
             decode((SELECT stragg(clientcode) FROM appeal_category_client WHERE catcodeseq = ac.catcodeseq),null,'ALL',
             (SELECT stragg(clientcode) FROM appeal_category_client WHERE catcodeseq = ac.catcodeseq)) ,replace(product,'PCI','CV'),
                                regexp_replace(lowproc||'-'||highproc,'^-',null),
                                regexp_replace(lowtrigproc||'-'||hightrigproc,'^-', null)
  FROM appeal_category_Assignment ac left join appeal_category_client ap on ac.catcodeseq = ap.catcodeseq
  WHERE deleted = 'F' and AC.{0} like ('%{1}')  
  ORDER BY category";

        public const string AnalystPTOByUserId = @"select to_char(beg_date,'MM/DD/YYYY')||'-'||to_char(end_date,'MM/DD/YYYY')
        from central.user_pto where userseq in (select userseq from central.users where user_id= '{0}')";

        public const string InsertAnalystPTOUser = @"insert into central.user_pto values({0},sysdate-60,sysdate-60)";

        public const string DeleteAnalystPto =
            @"delete from  central.user_pto  where userseq in (select userseq from central.users where user_id='{0}')";

        #endregion

        public const string AppealCategoryDetails = @"select category,
                                decode(ap.clientcode,null,'ALL',ap.clientcode),replace(product,'PCI','CV'),
                                regexp_replace(lowproc||'-'||highproc,'^-',null),
                                regexp_replace(lowtrigproc||'-'||hightrigproc,'^-', null) 
                                from CENTRAL.APPEAL_CATEGORY_ASSIGNMENT ac left join appeal_category_client ap on ac.catcodeseq = ap.catcodeseq
                                where deleted = 'F' and category = '{0}' ORDER BY category,lowproc, highproc";


        public const string GetAllAppealCatefory= @"select category
                                                 from CENTRAL.APPEAL_CATEGORY_ASSIGNMENT ac where deleted = 'F' ORDER BY category ";
        public const string AssignedAnalystForACategory = @"with analysts as (select {0} from appeal_category_Assignment where category = '{1}' and deleted = 'F')
select (first_name||' '||last_name||' ('||user_id||')' ) from central.users,analysts a
          where userseq in (select regexp_substr(a.{0}
          ,'[^,]+', 1, level) from dual
    connect by regexp_substr(a.{0}
    , '[^,]+', 1, level) is not null)";

        public const string AssignedRestrictionDescriptionForAUser = @"select description from ref_restriction where restriction in (select 
                restriction from user_restriction_access where userseq=(select userseq from central.users where user_id='{0}')) order by restriction";

        public const string GetLatestAuditDataInAppealCategoryManager = @"select * from
    (select auditdate,product,catorder,
     regexp_replace(lowproc||'-'||highproc,'^-',null),
    regexp_replace(lowtrigproc||'-'||hightrigproc,'^-', null),
    (select user_id from central.users where userseq = aca.moduserseq) as lastmodifieduser,
    notes
    from central.appealcategoryassignment_audit aca
    where category = '{0}' order by auditdate desc) where rownum = 1";


        public const string GetLatestNonRestricetedAnalystAuditData = @"with analysts as (select * from
    (select 
    assigned_analysts
    from central.appealcategoryassignment_audit aca
    where category = '{0}' order by auditdate desc) where rownum = 1)
select LISTAGG(user_id,',') within group (order by user_id desc) from (select user_id from central.users u,analysts a
          where userseq in (select regexp_substr(a.assigned_analysts
          ,'[^,]+', 1, level) from dual
    connect by regexp_substr(a.assigned_analysts
    , '[^,]+', 1, level) is not null))";

        public const string GetLatestRestricetedAnalystAuditData = @"with analysts as (select * from
    (select 
    unrestricted_assigned_analysts
    from central.appealcategoryassignment_audit aca
    where category = '{0}' order by auditdate desc) where rownum = 1)
select LISTAGG(user_id,',') within group (order by user_id desc) from (select user_id from central.users u,analysts a
          where userseq in (select regexp_substr(a.assigned_analysts
          ,'[^,]+', 1, level) from dual
    connect by regexp_substr(a.assigned_analysts
    , '[^,]+', 1, level) is not null))";

        public const string DeleteAppealCategoryAuditHistory = @"delete from CENTRAL.appealcategoryassignment_audit where category in ('{0}','{1}','{2}')
    and moduserseq in (select userseq from central.users where user_id = '{3}')";

        public const string InsertRoleForUser = @"
        Insert into central.user_role(USERSEQ, ROLE_ID)
            values((select userseq from central.users where user_id= '{0}') ,(select id from central.ref_role where role_name='{1}' and applies_to = 2))";

        public const string DeleteRoleForUser = @"
        delete from central.user_role
            where userseq=(select userseq from central.users where user_id='{0}' )
        and ROLE_ID = (select id from central.ref_role where role_name='{1}' and applies_to = 2)";

        public const string UpdateRoleForUser = @" update central.user_role 
 set role_id=(select id from central.ref_role where role_name='{2}' and applies_to=2)
 where 
 userseq=(select userseq from central.users where user_id='{0}' ) and role_id=(select id from central.ref_role where role_name='{1}' and applies_to=2)";

        public const string GetDayHourApproveDate =
            @"SELECT to_char ((select last_updated from smtst.hcicla where claseq = {0} and clasub = {1}
                ), 'D') day,
                    to_char ((select last_updated from smtst.hcicla where claseq = {0} and clasub = {1}
                ), 'HH24') hour from dual";

        public const string IsApprovedDateHoliday =
            @"SELECT COUNT(*) FROM holidays WHERE holiday_date = 
              TRUNC((select last_updated from smtst.hcicla where claseq = {0} and clasub = {1}))";

        public const string AppealResultSearchDataForOutstandingQAAppeals = @"select appealseq from " +
                                                                            "(select qra.*,(select stragg(distinct result) from ats.appeal_line where appealseq  = qra.appealseq) appeal_result" +
                                                                            " FROM central.qa_review_appeal qra where qra.clientcode = 'SMTST' and qra.reviewed = 'F') " +
                                                                            "WHERE appeal_result = '{0}'";

        public const string AdjustAppealResultSearchDataForOutstandingQAAppeals = @"select appealseq from " +
            "(select qra.*,(select stragg(distinct result) from ats.appeal_line where appealseq  = qra.appealseq) appeal_result" +
            " FROM central.qa_review_appeal qra where qra.clientcode = 'SMTST' and qra.reviewed = 'F') " +
            "WHERE appeal_result like '%{0}%' or appeal_result = 'P,D' OR appeal_result = 'D,P'";

        public const string AppealResultSearchDataForAllAppeals = @"select appealseq from " +
                                                                  "(select qra.*,(select stragg(distinct result) from ats.appeal_line where appealseq  = qra.appealseq) appeal_result" +
                                                                  " FROM central.qa_review_appeal qra where qra.clientcode = 'SMTST') " +
                                                                  "WHERE appeal_result = '{0}'";

        public const string AdjustAppealResultSearchDataForAllQAAppeals = @"select appealseq from " +
                                                                          "(select qra.*,(select stragg(distinct result) from ats.appeal_line where appealseq  = qra.appealseq) appeal_result" +
                                                                          " FROM central.qa_review_appeal qra where qra.clientcode = 'SMTST') " +
                                                                          "WHERE appeal_result like '%{0}%' or appeal_result = 'P,D' OR appeal_result = 'D,P'";

        public const string QaAppealForAllCenteneClientSqlScript = @"SELECT appealseq FROM (SELECT qra.*, u.first_name || ' ' || u.last_name analyst_name, u2.first_name || ' ' || u2.last_name reviewer_name, a.category_code appeal_category, (select stragg(distinct result) from appeal_line where appealseq  = a.appealseq) appeal_result
                                                                       FROM central.qa_review_appeal qra
                                                                        JOIN users u
                                                                            ON u.userseq = qra.userseq
                                                                         LEFT JOIN users u2
                                                                            ON u2.userseq = qra.reviewed_by
                                                                            JOIN ats.appeal a
                                                                            ON qra.appealseq = a.appealseq
                                                                            where qra.clientcode like 'CTN%' {0}
                                                                            and qra.clientcode in (select clientcode from client_user where userseq = (select userseq from users where user_id = '{1}')))";
    }
}
