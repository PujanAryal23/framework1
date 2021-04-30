using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nucleus.Service.Support.Enum;

namespace Nucleus.Service.SqlScriptObjects.Dashboard
{
    public static class DashboardSqlScriptObjects
    {
        public const string client = "SMTST";

        public const string CotivitiUserWithYelloIconList =
            @"select username || ' (' || user_id || ')' assigned_to_name from central.users u where  userseq in (        
     SELECT  distinct a.assigned_to FROM appeal a
                INNER JOIN hciclient c on a.clientcode=c.clientcode WHERE a.product = 'F' and u.status='A' AND c.active = 'T' AND c.FIRST_INSIGHT_ACTIVE = 'T'
                AND c.NUCLEUS_CLIENT = 'T' AND c.demo_client='F' AND a.assigned_to IS NOT NULL AND (a.status in ('N', 'M') AND trunc(due_date) < trunc(sysdate))
                AND a.clientcode in (SELECT clientcode FROM client_user WHERE userseq = (select  userseq from central.users where user_id='{0}')))order by assigned_to_name";

        public const string CotivitiUserWithAssignAppealsAuthorityList = @"     
 SELECT    distinct assigned_to, assigned_to_name  FROM (
        SELECT  assigned_to, u.username || ' (' || u.user_id || ')' assigned_to_name  FROM  appeal a LEFT JOIN users u on a.assigned_to = u.userseq WHERE product = 'F' and u.status='A' AND assigned_to IS NOT NULL AND  (a.status = 'N' AND (trunc(due_date)  <=trunc(sysdate)   or trunc(due_date) in 
        (trunc(sysdate+1), trunc(sysdate+2), trunc(sysdate+3), trunc(sysdate+4), trunc(sysdate+5)))
        or (  trunc(completed_date) = trunc(sysdate)  and a.status in ('T', 'C'))))
       UNION
         SELECT userseq assigned_to, u.username || ' (' || u.user_id || ')' assigned_to_name FROM users u where status = 'A' and exists (select 1 from CENTRAL.user_role where userseq = u.userseq and role_id = 4) and user_type = 2 
         ";

        public const string ClaimByClilentForInternalUser = @"
        SELECT distinct dcc.clientcode FROM central.dashboard_claim dcc join hciuser.hciclient hc
        on dcc.clientcode=hc.primary_schema or dcc.clientcode=hc.clientcode join central.client_user cc on cc.clientcode=dcc.clientcode
        where active = 'T' AND demo_client ='F' AND FIRST_INSIGHT_ACTIVE = 'T' AND NUCLEUS_CLIENT = 'T' and
        userseq=(select  userseq from central.users where user_id='{0}') order by dcc.clientcode asc";

        public const string LogicRequestByClilentForInternalUser = @"
        SELECT  distinct dl.clientcode FROM central.dashboard_logic dl join hciuser.hciclient hc on dl.clientcode=hc.primary_schema 
        or dl.clientcode=hc.clientcode join central.client_user cc  on cc.clientcode=dl.clientcode where 
        active = 'T' AND demo_client ='F' AND FIRST_INSIGHT_ACTIVE = 'T' AND NUCLEUS_CLIENT = 'T' and 
        userseq=(select  userseq from central.users where user_id='{0}') order by dl.clientcode asc";

        public const string AppealsByClientForInternalUser = @"SELECT  distinct clientcode  FROM (
        SELECT  a.clientcode, c.clientname  FROM  appeal a LEFT JOIN client  c on a.clientcode = c.clientcode WHERE product = 'F' AND
		a.clientcode IN (SELECT cc.CLIENTCODE FROM HCICLIENT hc join central.client_user cc on cc.clientcode = hc.clientcode 
        WHERE hc.active = 'T' AND  hc.FIRST_INSIGHT_ACTIVE = 'T' AND hc.NUCLEUS_CLIENT = 'T' AND  hc.demo_client='F' and  
        cc.userseq=(select  userseq from central.users where user_id='{0}'))
		AND a.assigned_to IS NOT NULL AND  (a.status = 'N' AND (trunc(due_date)  < trunc(sysdate+1)   ))) order by clientcode ";

        public const string ClaimByClilentForClientUser = @"
        SELECT distinct dcc.clientcode FROM central.dashboard_claim_client dcc join hciuser.hciclient hc
        on dcc.clientcode=hc.primary_schema or dcc.clientcode=hc.clientcode 
        join central.client_user cc on cc.clientcode=dcc.clientcode
        where 
        active = 'T' AND demo_client ='F' AND FIRST_INSIGHT_ACTIVE = 'T' AND NUCLEUS_CLIENT = 'T' and
        userseq=(select  userseq from central.users where user_id='{0}')
        order by dcc.clientcode asc";

        public const string LogicRequestByClilentForClientUser = @"
         SELECT  distinct dl.clientcode FROM central.dashboard_logic_client dl join hciuser.hciclient hc
        on dl.clientcode=hc.primary_schema or dl.clientcode=hc.clientcode  
        join central.client_user cc on cc.clientcode=dl.clientcode
        where 
        active = 'T' AND demo_client ='F' AND FIRST_INSIGHT_ACTIVE = 'T' AND NUCLEUS_CLIENT = 'T' and
        userseq=(select  userseq from central.users where user_id='{0}')
        order by dl.clientcode asc";

        public const string AppealsByClientForClientUser = @"
                 WITH abc AS
(SELECT  a.clientcode, a.appealseq, a.priority, a.status, a.appeal_Type, aa.audit_date FROM appeal a
 JOIN appeal_audit aa ON aa.appealseq = A.appealseq AND audit_action = 'CLOSE' 
 AND audit_date >= trunc(DASHBOARD.F_GetBusinessDate(sysdate, -1))  and audit_date <  sysdate
 WHERE A.status = 'C' 
 and a.product = 'F' 
 and a.clientcode in (select cu.clientcode from client_user cu join hciclient hc on cu.clientcode = hc.clientcode  
 where hc.active = 'T' and hc.first_insight_active = 'T' and hc.nucleus_client = 'T' and demo_client='F'
 and userseq= (select  userseq from central.users where user_id='{0}')) 

           UNION ALL
        SELECT /*+  ALL_ROWS */ A.clientcode, A.appealseq, A.priority, A.status, A.appeal_Type,  
        SYSDATE audit_date FROM appeal A  WHERE A.status = 'T' AND 
       A.product = 'F' AND   
        clientcode in (select cu.clientcode from client_user cu join hciclient hc on cu.clientcode = hc.clientcode  
        where hc.active = 'T' and hc.first_insight_active = 'T' and hc.nucleus_client = 'T' and demo_client='F'
        and userseq=(select  userseq from central.users where user_id='{0}')) 
     )
        SELECT distinct clientcode FROM abc order by clientcode";

        //change this query
        public const string FFPClaimByClientForInternalUser = @"
        SELECT distinct dcc.clientcode FROM central.dashboard_claim_ffp dcc join  hciuser.hciclient hc
        on dcc.clientcode=hc.primary_schema or dcc.clientcode=hc.clientcode join central.client_user cc on cc.clientcode=dcc.clientcode where
        active = 'T' AND demo_client ='F' AND FRAUDFINDERPRO_ACTIVE = 'T' AND NUCLEUS_CLIENT = 'T' and
         userseq=(select  userseq from central.users where user_id='{0}') order by dcc.clientcode asc";

        public const string TruncateDashboardClaimFFP = @"DELETE FROM CENTRAL.DASHBOARD_CLAIM_FFP";



        public const string FFPUnreviewedClaimForInternalUser = @"SELECT SUM(UNREVIEWED) FROM CENTRAL.DASHBOARD_CLAIM_FFP uc join  hciuser.hciclient hc ON  
        uc.clientcode=hc.clientcode join central.client_user cc on cc.clientcode=uc.clientcode
        where active = 'T' AND demo_client ='F' AND FRAUDFINDERPRO_ACTIVE = 'T' AND NUCLEUS_CLIENT = 'T' and
        userseq=(select  userseq from central.users where user_id='{0}') order by uc.clientcode asc";

        public const string COBWidgetCountsForInternalUser =
            @"select sum({0}) from central.{1} uc join  hciuser.hciclient hc ON  
        uc.clientcode=hc.clientcode join central.client_user cc on cc.clientcode=uc.clientcode
        where active = 'T' AND demo_client ='F' AND COB_ACTIVE = 'T' AND NUCLEUS_CLIENT = 'T' and
        userseq=(select  userseq from central.users where user_id='{2}') order by uc.clientcode asc";

        public const string COBUnreviewedMembersForInternalUser =
            @"select sum(patients) from dashboard_claim_cob uc join  hciuser.hciclient hc ON  
        uc.clientcode=hc.clientcode join central.client_user cc on cc.clientcode=uc.clientcode
        where active = 'T' AND demo_client ='F' AND COB_ACTIVE = 'T' AND NUCLEUS_CLIENT = 'T' and
        userseq=(select  userseq from central.users where user_id='{0}') order by uc.clientcode asc";

        public const string COBUnreviewedClaimsForClientUser = @"select sum(client_unreviewed) from dashboard_claim_cob uc join  hciuser.hciclient hc ON  
        uc.clientcode=hc.clientcode join central.client_user cc on cc.clientcode=uc.clientcode
        where active = 'T' AND demo_client ='F' AND COB_ACTIVE = 'T' AND NUCLEUS_CLIENT = 'T' and
        userseq=(select  userseq from central.users where user_id='{0}') order by uc.clientcode asc";

        public const string COBUnreviewedClaimsCountByClientForInternalUser = "select uc.clientcode,uc.unreviewed from dashboard_claim_cob uc join hciuser.hciclient hc ON " +
                                                                              "uc.clientcode=hc.clientcode join central.client_user cc on cc.clientcode=uc.clientcode where active = 'T' " +
                                                                              "AND demo_client = 'F' AND COB_ACTIVE = 'T' AND NUCLEUS_CLIENT = 'T' and userseq = (select  userseq from central.users " +
                                                                              "where user_id= '{0}') order by uc.clientcode asc";

        public const string COBUnreviewdClaimsCountByClientForClientUser = "select uc.clientcode,uc.client_unreviewed from dashboard_claim_cob uc join hciuser.hciclient hc ON " +
                                                                           "uc.clientcode=hc.clientcode join central.client_user cc on cc.clientcode=uc.clientcode where active = 'T' " +
                                                                           "AND demo_client = 'F' AND COB_ACTIVE = 'T' AND NUCLEUS_CLIENT = 'T' and userseq = (select  userseq from central.users " +
                                                                           "where user_id= '{0}') order by uc.clientcode asc";


        public const string COBClientsForUser = @"select a.clientcode from  central.Dashboard_claim_cob_batch_detail a
                                                    join 
                                                    hciuser.hciclient b
                                                    on a.clientcode = b.clientcode
                                                    join 
                                                    central.client_user c
                                                    on a.clientcode = c.clientcode 
                                                    where active = 'T' and demo_client = 'F' and cob_active = 'T' and nucleus_client = 'T' 
                                                    and userseq = (select userseq from central.users where user_id = '{0}')";



        public const string COBBatchData =
            @"select unreviewed_today,unreviewed_one_day,unreviewed_two_days,unreviewed_three_days,unreviewed_four_days
                from central.Dashboard_claim_cob_batch_detail a
                join 
                hciuser.hciclient b
                on a.clientcode = b.clientcode
                join 
                central.client_user c
                on a.clientcode = c.clientcode 
                where active = 'T' and demo_client = 'F' and cob_active = 'T' and nucleus_client = 'T' 
                and userseq = (select userseq from central.users where user_id = '{0}') order by a.clientcode";




        //@"SELECT COUNT(DISTINCT claseq || '-' || clasub) FROM {0}.hcicla c JOIN {0}.hcibatch b ON c.batchseq = b.batchseq 
        //WHERE active = 'T' AND status IN ( 'U', 'P') and reltoclient = 'F'
        //AND EXISTS (SELECT 1 FROM {0}.hciflag WHERE claseq = c.claseq AND clasub = c.clasub
        //AND hcidone = 'F' AND edittype = 'R' AND deleted = 'N')";



        public const string FFPPendedClaimForInternalUser = @"SELECT SUM(PENDED_UNRELEASED) FROM CENTRAL.DASHBOARD_CLAIM_FFP pc join  hciuser.hciclient hc ON  
        pc.clientcode=hc.clientcode join central.client_user cc on cc.clientcode=pc.clientcode
        where active = 'T' AND demo_client ='F' AND FRAUDFINDERPRO_ACTIVE = 'T' AND NUCLEUS_CLIENT = 'T' and
        userseq=(select  userseq from central.users where user_id='{0}')
        order by pc.clientcode asc";
        //    @"
        //SELECT COUNT(DISTINCT claseq || '-' || clasub) FROM {0}.hcicla c JOIN {0}.hcibatch b ON c.batchseq = b.batchseq 
        //WHERE active = 'T' AND status = 'P' and reltoclient = 'F'
        //AND EXISTS (SELECT 1 FROM {0}.hciflag WHERE claseq = c.claseq AND clasub = c.clasub
        //AND edittype = 'R' AND deleted = 'N')";



        public const string FFPUnreleasedClaimForInternalUser = @"SELECT SUM(UNRELEASED) FROM CENTRAL.DASHBOARD_CLAIM_FFP ur join  hciuser.hciclient hc ON  
        ur.clientcode=hc.clientcode join central.client_user cc on cc.clientcode=ur.clientcode
        where active = 'T' AND demo_client ='F' AND FRAUDFINDERPRO_ACTIVE = 'T' AND NUCLEUS_CLIENT = 'T' and
        userseq=(select  userseq from central.users where user_id='{0}') order by ur.clientcode asc";
        //    @"SELECT COUNT(DISTINCT claseq || '-' || clasub) FROM {0}.hcicla c  JOIN {0}.hcibatch b ON c.batchseq = b.batchseq 
        //WHERE active = 'T' AND  reltoclient = 'F' 
        //AND EXISTS (SELECT 1 FROM {0}.hciflag WHERE claseq = c.claseq AND clasub = c.clasub 
        //AND edittype = 'R' AND deleted = 'N')";



        public const string FFPAllPendedClaimForInternalUser = @"SELECT SUM(PENDED_ALL) FROM CENTRAL.DASHBOARD_CLAIM_FFP dcc join  hciuser.hciclient hc ON  
        dcc.clientcode=hc.clientcode join central.client_user cc on cc.clientcode=dcc.clientcode
        where active = 'T' AND demo_client ='F' AND FRAUDFINDERPRO_ACTIVE = 'T' AND NUCLEUS_CLIENT = 'T' and
        userseq=(select  userseq from central.users where user_id='{0}') order by dcc.clientcode asc";
        //    @"SELECT COUNT(DISTINCT claseq || '-' || clasub) FROM {0}.hcicla c JOIN {0}.hcibatch b ON c.batchseq = b.batchseq 
        //WHERE active = 'T' AND status = 'P' AND EXISTS (SELECT 1 FROM {0}.hciflag WHERE claseq = c.claseq AND clasub = c.clasub 
        //AND edittype = 'R' AND deleted = 'N')";t


        public const string FFPApprovedYesterdayClaimForInternalUser = @"SELECT SUM(APPROVED_PREVIOUS_DAY) FROM CENTRAL.DASHBOARD_CLAIM_FFP dcc 
        join  hciuser.hciclient hc ON  
        dcc.clientcode=hc.clientcode join central.client_user cc on cc.clientcode=dcc.clientcode
        where active = 'T' AND demo_client ='F' AND FRAUDFINDERPRO_ACTIVE = 'T' AND NUCLEUS_CLIENT = 'T' and
        userseq=(select  userseq from central.users where user_id='{0}') order by dcc.clientcode asc";

        public const string FFPApprovedYesterday10DaysOldClaimForClientUser = @"SELECT SUM(Approved_PREVIOUS_DAY) FROM CENTRAL.DASHBOARD_CLAIM_CLIENT_FFP dcc 
        join  hciuser.hciclient hc ON  
        dcc.clientcode=hc.clientcode join central.client_user cc on cc.clientcode=dcc.clientcode
        where active = 'T' AND demo_client ='F' AND FIRST_INSIGHT_ACTIVE = 'T' AND NUCLEUS_CLIENT = 'T' and
        userseq=(select  userseq from central.users where user_id='{0}') order by dcc.clientcode asc";

        public const string FFPReleasedYesterdayClaimForInternalUser = @"SELECT SUM(RELEASED_PREVIOUS_DAY) FROM CENTRAL.DASHBOARD_CLAIM_FFP dcc 
        join  hciuser.hciclient hc ON  
        dcc.clientcode=hc.clientcode join central.client_user cc on cc.clientcode=dcc.clientcode
        where active = 'T' AND demo_client ='F' AND FIRST_INSIGHT_ACTIVE = 'T' AND NUCLEUS_CLIENT = 'T' and
        userseq=(select  userseq from central.users where user_id='{0}') order by dcc.clientcode asc";

        public const string FFPUnreviewedClaimForCotivitiUserByClient = @"SELECT UNREVIEWED FROM CENTRAL.DASHBOARD_CLAIM_FFP WHERE CLIENTCODE='{0}'";
        public const string FFPPendedClaimForInternalUserByClient = @"SELECT PENDED_ALL FROM CENTRAL.DASHBOARD_CLAIM_FFP WHERE CLIENTCODE='{0}'";
        public const string FFPUnreleasedClaimForInternalUserByClient = @"SELECT UNRELEASED FROM CENTRAL.DASHBOARD_CLAIM_FFP WHERE CLIENTCODE='{0}'";

        public const string FFPUnapprovedClaimsForInternalUser = @"SELECT CLASEQ || '-' || CLASUB FROM CENTRAL.DASHBOARD_UNAPPR_CLM_DTL_FFP dtl join 
        central.client_user cc on cc.clientcode = dtl.clientcode WHERE 
        cc.userseq=(select  userseq from central.users  where user_id='{0}')ORDER BY dtl.CLIENTCODE,dtl.CLASEQ,dtl.CLASUB";


        public const string FFPUnapprovedAltClaimNoForClientUser = @"SELECT altclaimno FROM CENTRAL.DASHBRD_UNAPPR_CLM_CLI_DTL_FFP dtl join 
        central.client_user cc on cc.clientcode = dtl.clientcode WHERE 
        cc.userseq=(select  userseq from central.users  where user_id='{0}')ORDER BY dtl.CLIENTCODE";

        public const string PCIUnapprovedClaimsForInternalUser = @"SELECT CLASEQ || '-' || CLASUB FROM CENTRAL.dashboard_unapproved_claim_dtl dtl join 
        central.client_user cc on cc.clientcode = dtl.clientcode WHERE 
        cc.userseq=(select  userseq from central.users  where user_id='{0}')ORDER BY dtl.CLIENTCODE,dtl.CLASEQ,dtl.CLASUB";


        public const string FFPUnapprovedClaimClientCodeForInternalUser = @"SELECT dtl.CLIENTCODE FROM CENTRAL.DASHBOARD_UNAPPR_CLM_DTL_FFP dtl join 
        central.client_user cc on cc.clientcode = dtl.clientcode WHERE   
        cc.userseq=(select  userseq from central.users where user_id='{0}')ORDER BY dtl.CLIENTCODE,dtl.CLASEQ,dtl.CLASUB";

        public const string FFPUnapprovedClaimClientCodeForClientUser = @"SELECT dtl.CLIENTCODE FROM CENTRAL.DASHBRD_UNAPPR_CLM_CLI_DTL_FFP dtl join 
        central.client_user cc on cc.clientcode = dtl.clientcode WHERE   
        cc.userseq=(select  userseq from central.users where user_id='{0}')ORDER BY dtl.CLIENTCODE";

        public const string PCIUnapprovedClaimClientCodeForInternalUser = @"SELECT  dtl.CLIENTCODE FROM CENTRAL.dashboard_unapproved_claim_dtl dtl join 
        central.client_user cc on cc.clientcode = dtl.clientcode WHERE   
        cc.userseq=(select  userseq from central.users where user_id='{0}')ORDER BY dtl.clientcode";

        public const string FFPUnapporvedClaimCountForInternalUser = @"SELECT COUNT(*) FROM CENTRAL.DASHBOARD_UNAPPR_CLM_DTL_FFP dtl   join 
        central.client_user cc on cc.clientcode = dtl.clientcode WHERE cc.userseq=(select  userseq from central.users where user_id='{0}')";

        public const string FFPUnapporvedClaimCountForClientUser = @"SELECT COUNT(*) FROM CENTRAL.DASHBRD_UNAPPR_CLM_CLI_DTL_FFP dtl   join 
        central.client_user cc on cc.clientcode = dtl.clientcode WHERE cc.userseq=(select  userseq from central.users where user_id='{0}')";

        public const string PCIUnapporvedClaimCountForInternalUser = @"SELECT COUNT(*) FROM CENTRAL.dashboard_unapproved_claim_dtl dtl   join 
        central.client_user cc on cc.clientcode = dtl.clientcode WHERE cc.userseq=(select  userseq from central.users where user_id='{0}')";

        public const string GetHolidays = @"select  to_char(holiday_date,'mm/dd/yyyy') 
        from HCIUSER.HOLIDAYS where holiday_date>sysdate and holiday_date<=sysdate+30";

        public const string MyDashboardMvUserKpiDetails = @"SELECT 
        nvl(leg_claims_reviewed, 0) LegacyClaimsReviewed,
        nvl(nuc_claims_reviewed,0) NucleusClaimsReviewed,
        nvl(total_claims_reviewed,0) TotalClaimsReviewed,
        nvl(leg_average_claims_per_hour, 0) LegacyAvgClaimsPerHour,
        nvl(nuc_average_claims_per_hour, 0) NucleusAvgClaimsPerHour,
        nvl(total_average_claims_per_hour, 0) TotalAvgClaimsPerHour,
        nvl(leg_appeals_completed, 0) LegacyAppealsReviewed,
        nvl(nuc_appeals_completed, 0) NucleusAppealsReviewed,
        nvl(average_appeals_per_hour, 0) AvgAppealsPerHour,
        nvl(total_appeals_completed, 0) TotalAppealsCompleted,
        nvl(weighted_appeals_completed, 0) WeightedAppealsCompleted,
        nvl(total_weighted_claims, 0) WeightedClaimsCompleted,
        last_updated LastUpdated
        FROM CENTRAL.MV_USER_KPI WHERE userseq= (SELECT userseq FROM central.users WHERE user_id='{0}')";

        public const string GetLastUpdatedFromMvUserKpiInMyDashboard = @"SELECT Last_updated FROM CENTRAL.MV_USER_KPI WHERE userseq= (SELECT userseq FROM central.users WHERE user_id='{0}')";

        public const string UpdateLastUpdatedInUserKpiAndRefreshMaterizaliedView = @"BEGIN
        UPDATE central.user_kpi
        SET Last_updated = TO_DATE('{0}', 'MM/DD/YYYY HH24:MI:SS')
        WHERE userseq = 4174;

        Shared.RefreshMView('MV_USER_KPI', method => 'C', atomic_refresh  => FALSE);
        END;";


        public const string TotalClaimsCountInClaimsOverviewWidgetForInternalUser = @"SELECT  SUM(uc.PENDED_UNRELEASED) TotalPendedClaims,
            SUM(uc.PENDED_ALL) TotalAllPendedClaims,
            SUM(uc.QA_PENDING) TotalAwaitingQAReviewClaims ,
            SUM(uc.QA_COMPLETED) TotalCompletedQAClaims,
            SUM(uc.UNRELEASED) TotalUnreleasedClaims,
            SUM(uc.RELEASED_PREVIOUS_DAY) TotalReleasedYesterdayClaims
        FROM CENTRAL.dashboard_claim uc join  hciuser.hciclient hc ON  
        uc.clientcode=hc.clientcode join central.client_user cc on cc.clientcode=uc.clientcode
        where active = 'T' AND demo_client ='F' AND FIRST_INSIGHT_ACTIVE = 'T' AND NUCLEUS_CLIENT = 'T' and
        userseq=(select  userseq from central.users where user_id='{0}') ";

        public const string RealTimeClientsForInternalUser = @" select hc.clientcode from hciuser.hciclient hc join hciclient_nucleus hcn on hc.clientcode=hcn.clientcode  
        join central.client_user cc on cc.clientcode=hc.clientcode
         where active = 'T' AND demo_client ='F' AND FIRST_INSIGHT_ACTIVE = 'T' AND NUCLEUS_CLIENT = 'T' and
         hcn.PROCESSING_TYPE in ('R','C','PR','PB') and  userseq=(select  userseq from central.users where user_id='{0}' ) order by hc.clientcode";

        public const string GetTotalUnreviewedDataForRealTimeClients =
            @"SELECT
                OVERDUE,
                LESSTHAN30MINS,
                LESSTHAN1HR,
                LESSTHAN2HRS,
                LESSTHAN4HRS,
                LESSTHAN6HRS,
                TOTAL_UNREVIEWED
            FROM
                CENTRAL.DASHBOARD_RT_UNREVIEWED_CLAIMS
            WHERE
                CLIENT_CODE IN ('{0}')";

        public const string DataByClaimRestrictionForRealTimeClients =
            @"SELECT
                R.DESCRIPTION,
                CR.OVERDUE,
                CR.THIRTY_MINUTES,
                CR.ONE_HOUR,
                CR.TWO_HOURS,
                CR.FOUR_HOURS,
                CR.SIX_HOURS,
                CR.TOTAL_CLAIMS
            FROM
                DASHBOARD_RT_CLAIM_RESTRICTION CR
                JOIN REF_RESTRICTION R ON CR.RESTRICTION = R.RESTRICTION
            WHERE
                CR.CLIENTCODE IN ('{0}')
            ORDER BY
                R.DESCRIPTION";


        public const string TotalUnreviewedClaimsForPCIInternalUser =
            @"SELECT SUM(UNREVIEWED) , 
                SUM(APPROVED_PREVIOUS_DAY)  FROM CENTRAL.dashboard_claim uc join  hciuser.hciclient hc ON  
        uc.clientcode=hc.clientcode join central.client_user cc on cc.clientcode=uc.clientcode
        where active = 'T' AND demo_client ='F' AND FIRST_INSIGHT_ACTIVE = 'T' AND NUCLEUS_CLIENT = 'T' and
        userseq=(select  userseq from central.users where user_id='{0}') ";

        public const string TotalUnreviewedRealTimeClaims = @"SELECT
             SUM(RC.LESSTHAN2HRS) ,
        SUM(RC.TOTAL_UNREVIEWED)  
        from central.DASHBOARD_RT_UNREVIEWED_CLAIMS RC JOIN client_user CC ON RC.CLIENT_CODE= CC.clientcode WHERE
        userseq = (select  userseq from central.users where user_id= '{0}') 
        and RC.CLIENT_CODE in (select clientcode from HCIUSER.hciclient_nucleus where PROCESSING_TYPE IN ('R','C','PR','PB'))";


        public const string OverdueUnreviewedClaimsCountByClient = @"SELECT count(*)
        FROM {0}.hcicla c JOIN {0}.hcibatch b ON c.batchseq = b.batchseq
        JOIN {0}.HCICLA_NUCLEUS hn on hn.claseq=c.claseq and hn.clasub=c.clasub
        WHERE active = 'T' AND  status IN ('U','P') and reltoclient = 'F' AND  (hn.DUE_DATE < SYSDATE )
        AND EXISTS (SELECT 1 FROM {0}.hciflag WHERE claseq = c.claseq AND clasub = c.clasub AND hcidone = 'F' AND edittype = 'F'
        AND deleted = 'N')";

        public const string UnreviewedClaimsWithDuedateLessThan30MinsByClient = @"SELECT count(*)
        FROM {0}.hcicla c JOIN {0}.hcibatch b ON c.batchseq = b.batchseq
        JOIN {0}.HCICLA_NUCLEUS hn on hn.claseq=c.claseq and hn.clasub=c.clasub
        WHERE active = 'T' AND  status IN ('U','P') and reltoclient = 'F' AND (hn.DUE_DATE <= (SYSDATE+interval '30' minute) and hn.DUE_DATE >sysdate)
        AND EXISTS (SELECT 1 FROM {0}.hciflag WHERE claseq = c.claseq AND clasub = c.clasub AND hcidone = 'F' AND edittype = 'F'
        AND deleted = 'N')";

        public const string UnreviewedClaimsWithDuedateLessThan1HrByClient = @"SELECT count(*)
        FROM {0}.hcicla c JOIN {0}.hcibatch b ON c.batchseq = b.batchseq
        JOIN {0}.HCICLA_NUCLEUS hn on hn.claseq=c.claseq and hn.clasub=c.clasub
        WHERE active = 'T' AND  status IN ('U','P') and reltoclient = 'F' AND (hn.DUE_DATE <= (SYSDATE+interval '1' hour) and hn.DUE_DATE >sysdate)
        AND EXISTS (SELECT 1 FROM {0}.hciflag WHERE claseq = c.claseq AND clasub = c.clasub AND hcidone = 'F' AND edittype = 'F'
        AND deleted = 'N')";

        public const string UnreviewedClaimsWithDuedateLessThan2HrByClient = @"SELECT count(*)
        FROM {0}.hcicla c JOIN {0}.hcibatch b ON c.batchseq = b.batchseq
        JOIN {0}.HCICLA_NUCLEUS hn on hn.claseq=c.claseq and hn.clasub=c.clasub
        WHERE active = 'T' AND  status IN ('U','P') and reltoclient = 'F' AND (hn.DUE_DATE <= (SYSDATE+interval '2' hour) and hn.DUE_DATE >sysdate)
        AND EXISTS (SELECT 1 FROM {0}.hciflag WHERE claseq = c.claseq AND clasub = c.clasub AND hcidone = 'F' AND edittype = 'F'
        AND deleted = 'N')";

        public const string UnreviewedClaimsWithDuedateLessThan4HrByClient = @"SELECT count(*)
        FROM {0}.hcicla c JOIN {0}.hcibatch b ON c.batchseq = b.batchseq
        JOIN {0}.HCICLA_NUCLEUS hn on hn.claseq=c.claseq and hn.clasub=c.clasub
        WHERE active = 'T' AND  status IN ('U','P') and reltoclient = 'F' AND (hn.DUE_DATE <= (SYSDATE+interval '4' hour) and hn.DUE_DATE >sysdate)
        AND EXISTS (SELECT 1 FROM {0}.hciflag WHERE claseq = c.claseq AND clasub = c.clasub AND hcidone = 'F' AND edittype = 'F'
        AND deleted = 'N')";

        public const string UnreviewedClaimsWithDuedateLessThan6HrByClient = @"SELECT count(*)
        FROM {0}.hcicla c JOIN {0}.hcibatch b ON c.batchseq = b.batchseq
        JOIN {0}.HCICLA_NUCLEUS hn on hn.claseq=c.claseq and hn.clasub=c.clasub
        WHERE active = 'T' AND  status IN ('U','P') and reltoclient = 'F' AND (hn.DUE_DATE <= (SYSDATE+interval '6' hour) and hn.DUE_DATE >sysdate)
        AND EXISTS (SELECT 1 FROM {0}.hciflag WHERE claseq = c.claseq AND clasub = c.clasub AND hcidone = 'F' AND edittype = 'F'
        AND deleted = 'N')";

        public const string PCIUnreviewedClaimsCountByClient = @"SELECT count(*)
        FROM {0}.hcicla c JOIN {0}.hcibatch b ON c.batchseq = b.batchseq        
        WHERE active = 'T' AND  status IN ('U','P') and reltoclient = 'F'
        AND EXISTS (SELECT 1 FROM {0}.hciflag WHERE claseq = c.claseq AND clasub = c.clasub AND hcidone = 'F' AND edittype = 'F'
        AND deleted = 'N')";

        public const string FFPClaimDataByClient =
            @"SELECT SUM(UNREVIEWED),SUM(PENDED),sum(UNAPPROVED),sum(unreviewed_old),sum(PENDED_OLD),sum(APPROVED_PREVIOUS_DAY) FROM CENTRAL.DASHBOARD_CLAIM_CLIENT_FFP uc join  hciuser.hciclient hc ON  
        uc.clientcode=hc.clientcode join central.client_user cc on cc.clientcode=uc.clientcode
        where active = 'T' AND demo_client ='F' AND FRAUDFINDERPRO_ACTIVE = 'T' AND NUCLEUS_CLIENT = 'T' and
        userseq=(select  userseq from central.users where user_id='{0}') order by uc.clientcode asc";

        public const string GetLastUpdatedCVDashboard =
            @"select to_char(max(last_updated) + interval '10' minute,'HH:MI AM') from central.dashboard_claim where clientcode in (select clientcode from central.client_user where userseq=(select userseq from central.users where user_id='{0}'))";

        public const string GetLastUpdatedFFPDashboard =
            @"select to_char(max(last_updated) + interval '10' minute,'HH:MI AM') from central.dashboard_claim_ffp where clientcode in (select clientcode from central.client_user where userseq=(select userseq from central.users where user_id='{0}'))";

        public const string GetLastUpdatedCVDashboardClient =
            @"select to_char(max(last_updated) + interval '10' minute,'HH:MI AM') from central.dashboard_claim_client where clientcode in (select clientcode from central.client_user where userseq=(select userseq from central.users where user_id='{0}'))";

        public const string GetLastUpdatedFFPDashboardClient =
            @"select to_char(max(last_updated) + interval '10' minute,'HH:MI AM') from central.dashboard_claim_client_ffp where clientcode in (select clientcode from central.client_user where userseq=(select userseq from central.users where user_id='{0}'))";

        public const string GetCOBAppealDetailForInternalUsersFromDb = @"select dac.clientcode, dac.outstanding_appeals, dac.client_appeals from CENTRAL.DASHBOARD_APPEAL_COB dac join 
                                                                         hciuser.hciclient hc ON  
                                                                        dac.clientcode=hc.clientcode join central.client_user cc on cc.clientcode=dac.clientcode
                                                                        where active = 'T' AND demo_client ='F' AND COB_ACTIVE = 'T' AND NUCLEUS_CLIENT = 'T' and
                                                                         userseq=(select  userseq from central.users where user_id='{0}') order by dac.clientcode asc";

        public const string GetCOBAppealsDetailValueForClientUsersFromDb =
            @"select dac.clientcode, dac.client_appeals from CENTRAL.DASHBOARD_APPEAL_COB dac join 
                                                                            hciuser.hciclient hc ON  
                                                                            dac.clientcode=hc.clientcode join central.client_user cc on cc.clientcode=dac.clientcode
                                                                            where active = 'T' AND demo_client ='F' AND COB_ACTIVE = 'T' AND NUCLEUS_CLIENT = 'T' and
                                                                            userseq=(select  userseq from central.users where user_id='{0}') order by dac.clientcode asc";

        public const string GetCVAppealsDueTodayCountByClientFromDb =
            @"select count(distinct a.appealseq) from appeal a WHERE status in ('N','M') and due_date <sysdate and a.clientcode='{0}' and product = 'F' and a.assigned_to IS NOT NULL and a.assigned_to in (select userseq from central.users where status = 'A')";

        public const string GetCVAppealsDueTodayUrgentCountByClientFromDb =
            @"select count(distinct a.appealseq) from appeal a WHERE status in ('N','M') and due_date <sysdate and a.clientcode='{0}' and product = 'F' and priority = 'U' and a.assigned_to IS NOT NULL and a.assigned_to in (select userseq from central.users where status = 'A')";

        public const string GetCVAppealsDueTodayRecordReviewCountByClientFromDb =
            @"select count(distinct a.appealseq) from appeal a WHERE status in ('N','M') and due_date <sysdate and a.clientcode='{0}' and product = 'F' and appeal_type = 'R' and a.assigned_to IS NOT NULL and a.assigned_to in (select userseq from central.users where status = 'A')";

        public const string GetCVAppealsDueTodayMedicalRecordReviewCountByClientFromDb =
            @"select count(distinct a.appealseq) from appeal a WHERE status in ('N','M') and due_date <sysdate and a.clientcode='{0}' and product = 'F' and appeal_type = 'M' and priority = 'N' and a.assigned_to IS NOT NULL and a.assigned_to in (select userseq from central.users where status = 'A')";

        public const string GetCVAppealsDueTodayRestrictedAppealsCountByClientFromDb =
            @"SELECT count(distinct a.appealseq) FROM appeal_restriction ar join ats.appeal a on ar.appealseq = a.appealseq
                WHERE a.status in ('N','M') and 
                a.due_date <sysdate and a.clientcode='{0}' and a.product = 'F' and a.assigned_to
                IS NOT NULL and a.assigned_to in (select userseq from central.users where status = 'A')";

        public const string GetCVAppealsDueTodayTotalCountFromDb =
            @"select count(distinct a.appealseq) from appeal a INNER JOIN hciclient c on a.clientcode=c.clientcode WHERE status in ('N','M') 
                and due_date <sysdate and c.active = 'T' AND c.FIRST_INSIGHT_ACTIVE = 'T'
                AND c.NUCLEUS_CLIENT = 'T' AND c.demo_client='F' and a.clientcode in (select clientcode from central.client_user where userseq = 
                (select userseq from central.users where user_id = '{0}')) and product = 'F' 
                 and a.assigned_to IS NOT NULL and a.assigned_to in (select userseq from central.users where status = 'A')";

        public const string GetCVAppealsDueTodayTotalUrgentCountFromDb =
            @"select count(distinct a.appealseq) from appeal a INNER JOIN hciclient c on a.clientcode=c.clientcode WHERE status in ('N','M') 
                and due_date <sysdate and c.active = 'T' AND c.FIRST_INSIGHT_ACTIVE = 'T'
                AND c.NUCLEUS_CLIENT = 'T' AND c.demo_client='F' and a.clientcode in (select clientcode from central.client_user where userseq = 
                (select userseq from central.users where user_id = '{0}')) and product = 'F' 
                and a.assigned_to IS NOT NULL and a.assigned_to in (select userseq from central.users where status = 'A') and priority = 'U'";

        public const string GetCVAppealsDueTodayTotalRecordReviewCountFromDb =
            @"select count(distinct a.appealseq) from appeal a INNER JOIN hciclient c on a.clientcode=c.clientcode WHERE status in ('N','M') 
                and due_date <sysdate and c.active = 'T' AND c.FIRST_INSIGHT_ACTIVE = 'T'
                AND c.NUCLEUS_CLIENT = 'T' AND c.demo_client='F' and a.clientcode in (select clientcode from central.client_user where userseq = 
                (select userseq from central.users where user_id = '{0}')) and product = 'F' 
                and a.assigned_to IS NOT NULL and a.assigned_to in (select userseq from central.users where status = 'A') and appeal_type = 'R'";

        public const string GetCVAppealsTotalRestrictedAppealsCountFromDb =
            @"SELECT count(distinct a.appealseq) FROM appeal_restriction ar join ats.appeal a on ar.appealseq = a.appealseq
                INNER JOIN hciclient c on a.clientcode=c.clientcode
                WHERE a.status in ('N','M') and c.active = 'T' AND c.FIRST_INSIGHT_ACTIVE = 'T'
                AND c.NUCLEUS_CLIENT = 'T' AND c.demo_client='F' and
                a.due_date <sysdate and a.clientcode in (select clientcode from central.client_user where userseq = 
                (select userseq from central.users where user_id = '{0}')) and a.product = 'F' and a.assigned_to
                IS NOT NULL and a.assigned_to in (select userseq from central.users where status = 'A')";

        public const string GetCVAppealsTotalMedicalRecordReviewsCountFromDb =
            @"select count(distinct a.appealseq) from appeal a INNER JOIN hciclient c on a.clientcode=c.clientcode WHERE status in ('N','M') 
                and due_date <sysdate and c.active = 'T' AND c.FIRST_INSIGHT_ACTIVE = 'T'
                AND c.NUCLEUS_CLIENT = 'T' AND c.demo_client='F' and a.clientcode in (select clientcode from central.client_user where userseq = 
                (select userseq from central.users where user_id = '{0}')) and product = 'F' 
                and a.assigned_to IS NOT NULL and a.assigned_to in (select userseq from central.users where status = 'A') and appeal_type = 'M' and priority='N'";

        public const string GetCVAppealsTotalRestrictedAppealsCountFromDbWithRecordReviewType =
            @"SELECT count(distinct a.appealseq) FROM appeal_restriction ar join ats.appeal a on ar.appealseq = a.appealseq
                INNER JOIN hciclient c on a.clientcode=c.clientcode
                WHERE a.status in ('N','M') and c.active = 'T' AND c.FIRST_INSIGHT_ACTIVE = 'T'
                AND c.NUCLEUS_CLIENT = 'T' AND c.demo_client='F' and
                a.due_date <sysdate and a.clientcode in (select clientcode from central.client_user where userseq = 
                (select userseq from central.users where user_id = '{0}')) and a.assigned_to in (select userseq from central.users where status = 'A') and a.product = 'F' and a.assigned_to
                IS NOT NULL and appeal_type = 'R'";

        public const string GetCVAppealsTotalRestrictedAppealsCountFromDbWithMedicalRecordReviewType =
            @"SELECT count(distinct a.appealseq) FROM appeal_restriction ar join ats.appeal a on ar.appealseq = a.appealseq
                INNER JOIN hciclient c on a.clientcode=c.clientcode
                WHERE a.status in ('N','M') and c.active = 'T' AND c.FIRST_INSIGHT_ACTIVE = 'T'
                AND c.NUCLEUS_CLIENT = 'T' AND c.demo_client='F' and
                a.due_date <sysdate and a.clientcode in (select clientcode from central.client_user where userseq = 
                (select userseq from central.users where user_id = '{0}')) and a.assigned_to in (select userseq from central.users where status = 'A') and a.product = 'F' and a.assigned_to
                IS NOT NULL and appeal_type = 'M' and priority = 'N'";

        public const string GetCVAppealsTotalRestrictedAppealsCountFromDbWithUrgentPriority =
            @"SELECT count(distinct a.appealseq) FROM appeal_restriction ar join ats.appeal a on ar.appealseq = a.appealseq
                INNER JOIN hciclient c on a.clientcode=c.clientcode
                WHERE a.status in ('N','M') and c.active = 'T' AND c.FIRST_INSIGHT_ACTIVE = 'T'
                AND c.NUCLEUS_CLIENT = 'T' AND c.demo_client='F' and
                a.due_date <sysdate and a.clientcode in (select clientcode from central.client_user where userseq = 
                (select userseq from central.users where user_id = '{0}')) and a.product = 'F' and a.assigned_to
                IS NOT NULL and a.assigned_to in (select userseq from central.users where status = 'A') and priority = 'U'";

        public const string GetCVAppealsTotalUnRestrictedAppealsCountFromDb =
        @"select count(distinct a.appealseq) from appeal a INNER JOIN hciclient c on a.clientcode=c.clientcode WHERE status in ('N','M') 
                and due_date <sysdate and c.active = 'T' AND c.FIRST_INSIGHT_ACTIVE = 'T'
                AND c.NUCLEUS_CLIENT = 'T' AND c.demo_client='F' and a.clientcode in (select clientcode from central.client_user where userseq = 
                (select userseq from central.users where user_id = '{0}')) and product = 'F' 
                and a.assigned_to IS NOT NULL and a.assigned_to in (select userseq from central.users where status = 'A') and a.appealseq not in
                (select appealseq from appeal_restriction ar where appealseq = a.appealseq)";

        public const string GetCVAppealsTotalUnRestrictedAppealsCountFromDbWithRecordReviewType =
            @"select count(distinct a.appealseq) from appeal a INNER JOIN hciclient c on a.clientcode=c.clientcode WHERE status in ('N','M') 
                and due_date <sysdate and c.active = 'T' AND c.FIRST_INSIGHT_ACTIVE = 'T'
                AND c.NUCLEUS_CLIENT = 'T' AND c.demo_client='F' and a.clientcode in (select clientcode from central.client_user where userseq = 
                (select userseq from central.users where user_id = '{0}')) and product = 'F' and appeal_type = 'R'
                and a.assigned_to IS NOT NULL and a.assigned_to in (select userseq from central.users where status = 'A') and a.appealseq not in
                (select appealseq from appeal_restriction ar where appealseq = a.appealseq)";

        public const string GetCVAppealsTotalUnRestrictedAppealsCountFromDbWithMedicalRecordReviewType =
            @"select count(distinct a.appealseq) from appeal a INNER JOIN hciclient c on a.clientcode=c.clientcode WHERE status in ('N','M') 
                and due_date <sysdate and c.active = 'T' AND c.FIRST_INSIGHT_ACTIVE = 'T'
                AND c.NUCLEUS_CLIENT = 'T' AND c.demo_client='F' and a.clientcode in (select clientcode from central.client_user where userseq = 
                (select userseq from central.users where user_id = '{0}')) and product = 'F' and appeal_type = 'M' and priority = 'N'
                and a.assigned_to IS NOT NULL and a.assigned_to in (select userseq from central.users where status = 'A') and a.appealseq not in
                (select appealseq from appeal_restriction ar where appealseq = a.appealseq)";

        public const string GetCVAppealsTotalUnRestrictedAppealsCountFromDbWithUrgentPriority =
            @"select count(distinct a.appealseq) from appeal a INNER JOIN hciclient c on a.clientcode=c.clientcode WHERE status in ('N','M') 
                and due_date <sysdate and c.active = 'T' AND c.FIRST_INSIGHT_ACTIVE = 'T'
                AND c.NUCLEUS_CLIENT = 'T' AND c.demo_client='F' and a.clientcode in (select clientcode from central.client_user where userseq = 
                (select userseq from central.users where user_id = '{0}')) and product = 'F' and priority = 'U'
                and a.assigned_to IS NOT NULL and a.assigned_to in (select userseq from central.users where status = 'A') and a.appealseq not in
                (select appealseq from appeal_restriction ar where appealseq = a.appealseq)";

        public const string GetClientSpecificRealTimeQCClaimsCount = @"SELECT
                OVERDUE,
                THIRTY_MINUTES,
                ONE_HOUR,
                TWO_HOURS,
                FOUR_HOURS,
                SIX_HOURS,
                TOTAL_CLAIMS
            FROM
                CENTRAL.Dashboard_RT_QC_Claim
            WHERE
                CLIENTCODE IN ('{0}')";

        public const string GetTotalPaidForCOBDashboardTotalsByFlag =
            @"select nvl(to_char(paid, 'TM'),0) from central.dashboard_claim_cob_detail dcd
            join
            hciuser.hciclient hc
            on dcd.clientcode = hc.clientcode
            join 
            edit e
            on e.editflg = dcd.editflg
            join 
            central.client_user c
            on dcd.clientcode = c.clientcode 
            where active = 'T' and demo_client = 'F' and cob_active = 'T' and nucleus_client = 'T' and status = 'A'
            and userseq = (select userseq from central.users where user_id = '{0}')
            order by dcd.clientcode, dcd.editflg";

        public const string GetStatusOfEdits = @"select status from central.dashboard_claim_cob_detail dcd
            join edit e 
            on e.editflg = dcd.editflg
            where clientcode = '{0}' and product = 'C'";

        public const string GetTotalPaidPerClient = @"select nvl(to_char(sum(paid), 'TM'),0) from central.dashboard_claim_cob_detail dcd
            join
            hciuser.hciclient hc
            on dcd.clientcode = hc.clientcode
            join 
            edit e
            on e.editflg = dcd.editflg
            join 
            central.client_user c
            on dcd.clientcode = c.clientcode 
            where active = 'T' and demo_client = 'F' and cob_active = 'T' and nucleus_client = 'T' and status = 'A' and dcd.clientcode = '{0}'
            and userseq = (select userseq from central.users where user_id = '{1}')
            order by dcd.clientcode, dcd.editflg";
    }
}
