using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.SqlServer.Server;
using Nucleus.Service.Support.Enum;

namespace Nucleus.Service.SqlScriptObjects.Claim
{
    public static class ClaimSqlScriptObjects
    {
        public const string client = "SMTST";

        public const string GetStatusOfClaimSeq = @"select status from " + client +" .hcicla where claseq = {0}";
        public const string RevertScriptForSwitchFunctionalityCoreFlag = @"
            begin

            delete  FROM " + client +@".hciflag WHERE claseq={0} and clasub={1} and deleted='H';
            delete  FROM " + client +@".hciflag WHERE claseq={2} and clasub={3} and deleted='H';
            delete  FROM " + client +@".hciflag WHERE claseq={4} and clasub={5} and deleted='H';

                       
            update " + client + @".hcicla
            set status='{6}',
            reltoclient='T'
            where claseq={0} and clasub={1};
            
            
            update " + client + @".hcicla
            set status='{6}',
            reltoclient='T'
            where claseq={2} and clasub={3};
            


            end;

            ";

        

        public const string GetLongDescOfToothNoFromDb = @"
            select longdesc from HCIUSER.dentaltooth where toothno='{0}'
            ";


        public const string GetLongDescOfOralCavityFromDb = @"
           select longdesc from HCIUSER.dentalquad where quadrant='{0}'
            ";


        public const string UpdateProductTypeToInactiveByClientCode = @"update HCIUSER.HCICLIENT
            set FIRST_INSIGHT_ACTIVE = 'F',FACILITY_INSIGHT_ACTIVE = 'F',FRAUDFINDERPRO_ACTIVE = 'F'
            where clientcode='{0}'
           
            ";

        public const string UpdateProductTypeToActiveByClientCode = @"update HCIUSER.HCICLIENT
            set FIRST_INSIGHT_ACTIVE = 'T',FACILITY_INSIGHT_ACTIVE = 'T',FRAUDFINDERPRO_ACTIVE = 'T'
            where clientcode='{0}'
        
            ";


        public const string GetDentalDataPointsValuesFromDb =
            @"select toothno,toothsurf,oralcavity from " + client + @".hcilin where claseq = '{0}'";

       

        public const string GetDataPointValueFromDb = @"
            select distinct TO_CHAR(a.DOS, 'MM/DD/YYYY') DOS,a.POS,a.PRVSPEC,a.TOOTHNO,a.TOOTHSURF,a.ORALCAVITY,a.PROC,a.M1,
            a.UNITS,TO_CHAR(a.BILLED,'$9,999.99') BILLED,TO_CHAR(a.ALLOWED,'$9,999.99') ALLOWED,
            TO_CHAR(a.PAID,'$9,999.99')AdjPaid,TO_CHAR(b.SUGPAID,'$0.99')SugPaid,TO_CHAR(b.SAVINGS,'$9,999.99')Savings,b.editflg
            FROM " + client + @".hcilin a 
            inner join " + client + @".hciflag b
            on a.LINSEQ = b.LINSEQ 
           
            WHERE a.CLASEQ='{0}'       
            ";

        public const string GetProcDescFromDb = @"
            select short_description from REPOSITORY.MEDICAL_CODE_DESCRIPTION_MV where code='{0}' and code_type='{1}' and EXPDATE is  null";

        public const string GetLongProcDescFromDb = @"
            select long_description from REPOSITORY.MEDICAL_CODE_DESCRIPTION_MV where code='{0}' and code_type='{1}' and EXPDATE is  null";

        public const string GetAllProcDescFromDb = @"
            select short_description from REPOSITORY.MEDICAL_CODE_DESCRIPTION_MV where code='{0}' and code_type='{1}'";

        public const string DeleteAllDeletedFlag = @"delete  FROM " + client +
                                                   @".hciflag WHERE claseq={0} and clasub={1} and deleted='H'";

        /// <summary>
        /// delete flag line
        /// </summary>
        public const string DeleteFlagLine = @"
        begin
        DELETE FROM " + client + @".hciflag WHERE claseq={0} and clasub={1} and editflg='{2}';
        DELETE FROM " + client + @".claim_audit WHERE claseq={0} and clasub={1} ;  
        end;";

        /// <summary>
        /// delete all claim audit record of specific flag
        /// </summary>
        public const string DeleteClaimAuditRecord = @"
        begin
        DELETE FROM {4}.line_flag_audit WHERE flagseq={0} ;
        DELETE FROM {4}.claim_audit WHERE claseq={1} AND  audit_action='LINEEDIT' AND  clasub={2} AND line_number={3};  
        end;";

        public const string DeleteClaimAuditRecordByClaseq = @"
        begin
        DELETE FROM {3}.line_flag_audit WHERE flagseq = (select flagseq from smtst.hciflag where claseq = {0});
        DELETE FROM {3}.claim_audit WHERE claseq={0} AND  audit_action='LINEEDIT' AND  clasub={1} AND line_number={2};  
        end;";


        public const string EditFlagList =
            @"select editflg from REPOSITORY.edit where product='F' and edit_type!='M' and status='A' order by editflg";

    


        public const string DeleteClaimAuditRecordExceptAdd = @"
        begin
        delete  from " + client + @".line_flag_audit where flagseq in (select flagseq from " + client + @".hciflag where claseq={0} and clasub={1}) and audit_action <> 'A';

        delete from " + client + @".claim_audit where claseq={0} and clasub={1}
        and TRUNC(audit_date,'MI') not in
        (select TRUNC(audit_date,'MI') from " + client + @".line_flag_audit 
        where flagseq in (select flagseq from " + client + @".hciflag where claseq={0} and clasub={1}) and audit_action =  'A'
        ) and userseq <>-1;

        end;";

        public const string DeleteClaimAuditRecordExceptApproveAndAdd = @"
        begin
        delete  from " + client + @".line_flag_audit where flagseq in (select flagseq from " + client + @".hciflag where claseq={0} and clasub={1}) and  audit_action not in ( 'A','V');

        delete from " + client + @".claim_audit where claseq={0} and clasub={1}
        and to_char(audit_date,'mm/dd/yyyy') not in
        (select to_char(audit_date,'mm/dd/yyyy') from " + client + @".line_flag_audit 
        where flagseq in (select flagseq from " + client + @".hciflag where claseq={0} and clasub={1}) and (audit_action =  'A' or audit_action='V')
        ) and userseq <>-1 and audit_action not in ('QA Pass' ,'QA Fail');

        end;";

        public const string DeleteClaimAuditOnly =
            "delete from " + client + ".claim_audit where claseq={0} and clasub={1} and trunc(audit_date)>=trunc(to_date('{2}','dd-Mon-yyyy')) ";

        public const string DeleteClaimNotesofNoteTypeClaimOnly =
            @"delete from " + client + @".note where key1={0} and key2={1}
            and created_by = (select userseq from central.users where user_id = '{2}')";

        public const string InternalUserClaimSubStatusList = @"select ss.description_text from REF_SUB_STATUS ss join sub_status_client sc on sc.sub_status_code=ss.sub_status_code 
        where ss.user_type in ('B','H') and sc.clientcode in ('ALL','{0}') and product in ('S','D') order by upper(description_text)";

        public const string ClientWiseClaimSubStatusList = @"select ss.description_text from REF_SUB_STATUS ss join sub_status_client sc on sc.sub_status_code=ss.sub_status_code 
        where ss.user_type in ('B','C') and sc.clientcode in ('ALL','{0}')  order by description_text";

        public const string TotalCountOfNotes =
            @"select count(*) from " + client + ".note where key1 in ({0} , {1}, {2})";

        public const string TotalCountOfClaimAndPatientNotes =
            @"select count(*) from " + client + ".note where key1 in ({0} , {1})";

        public const string TotalCountOfClaimNotes =
            @"select count(*) from " + client + ".note where key1={0} and key2={1}";


        public const string TotalCountOfProviderNotes =
            @"select count(*) from " + client + ".note where key1={0}";

        public const string ProviderSpecialty = @"SELECT DISTINCT PRVSPEC FROM SMTST.HCILIN WHERE CLASEQ={0}";
        public const string providerSpec = "select specialty from smtst.hciprov where prvseq={0}";
        public const string ProviderDetailSpec = @"select S.SPECIALTY from hciuser.specialty  S join  SMTST.HCIPROV_SPECIALTY P ON S.specialty=P.SPECIALTY WHERE P.PRVSEQ={0}";

        public const string ClientWisePlanList = "Select plan_name from {0}.plans order by plan_name";

        /*The list of users with 'Claims can be assigned to user' authority */

        public const string AssignedToList =
            //@"SELECT  u.first_name || ' ' || TRIM(u.last_name) assigned_to_name FROM users u where status = 'A' and exists 
            //   (select 1 from user_authorization where userseq = u.userseq and authority_id = 25) and user_type = 2 ";
            @"SELECT u.first_name || ' ' || TRIM(u.last_name) assigned_to_name FROM users u  LEFT JOIN  user_role ua ON u.userseq = ua.userseq WHERE  u.status = 'A'  AND u.userseq IN 
                (SELECT userseq FROM client_user where clientcode = '{0}') AND  ua.role_id= 8 AND  user_type = 2 ";

        public const string ClientUserAssignedToList =
        //@"SELECT  u.first_name || ' ' || TRIM(u.last_name) assigned_to_name FROM users u where status = 'A' and exists 
        //   (select 1 from user_authorization where userseq = u.userseq and authority_id = 25) and user_type = 2 ";
        @"SELECT u.first_name || ' ' || TRIM(u.last_name) assigned_to_name FROM users u  LEFT JOIN  user_role ua ON u.userseq = ua.userseq WHERE  u.status = 'A'  AND u.userseq IN 
            (SELECT userseq FROM client_user where clientcode = '{0}') AND  ua.role_id= 9 AND  user_type = 8";

        

        public const string CountOfUnreviewedClaimsForInternalUser =
            //@"SELECT count(*) from {0}.hcicla  where status = 'U' and RELTOCLIENT = 'F'";
            @"SELECT COUNT(*) FROM 
            (SELECT DISTINCT  c.claseq, c.clasub, c.altclaimno, c.claimno, c.status, c.reltoclient, c.reviewgroup, hn.sub_status  
            FROM {0}.hcicla c join {0}.hcicla_nucleus hn on c.claseq=hn.claseq 
join smtst.hcilin hl on c.claseq=hl.claseq and c.clasub=hl.clasub
where c.status='U' and c.reltoclient='F' and hl.patseq NOT IN (SELECT  patseq from smtst.PATIENT_RESTRICTION pr
                                                  JOIN CENTRAL.REF_RESTRICTION rr on pr.RESTRICTION = rr.RESTRICTION
                                            WHERE 
                                                  rr.USER_TYPE in (1, 2)  and
                                                  pr.RESTRICTION NOT IN (SELECT restriction FROM user_restriction_access where userseq = (select userseq from central.users where user_id = '{1}')  )) and c.claseq||'-'||c.clasub not in (select distinct ca.claseq||'-'||ca.clasub from smtst.claim_audit ca 
inner join smtst.claim_qa_audit cqa on ca.claseq = cqa.claseq and ca.clasub = cqa.clasub
where ca.audit_action = 'QAREADY' and cqa.audit_completed = 'F'))";
                 

        public const string CountOfUnreviewedClaimsForClientUser =
            //@"SELECT count(*) from {0}.hcicla  where status = 'U' and RELTOCLIENT = 'T'";
            @"SELECT count(*) from 
            (select distinct c.claseq from {0}.hcicla c join {0}.hcicla_nucleus hn on c.claseq = hn.claseq and 
            hn.clasub = c.clasub 
join smtst.hcilin hl on c.claseq=hl.claseq and c.clasub=hl.clasub
where c.status = 'U' and c.reltoclient='T' and hl.patseq NOT IN (SELECT  patseq from smtst.PATIENT_RESTRICTION pr
                                                  JOIN CENTRAL.REF_RESTRICTION rr on pr.RESTRICTION = rr.RESTRICTION
                                            WHERE 
                                                  rr.USER_TYPE in (1, 8)  and
                                                  pr.RESTRICTION NOT IN (SELECT restriction FROM user_restriction_access where userseq = (select userseq from central.users where user_id = '{1}')  )))";

        public const string CountOfPendedClaims =
            @"SELECT count(*) from 
            (select distinct C.CLASEQ,HP.PATNUM from {0}.HCICLA  C JOIN {0}.claim_line CY ON C.CLASEQ=CY.CLASEQ AND C.CLASUB=CY.CLASUB
            JOIN {0}.HCIPAT HP ON CY.PATSEQ=HP.PATSEQ
            where c.status = 'P'  and HP.patseq NOT IN (SELECT  patseq from smtst.PATIENT_RESTRICTION pr
                                                  JOIN CENTRAL.REF_RESTRICTION rr on pr.RESTRICTION = rr.RESTRICTION
                                            WHERE 
                                                  rr.USER_TYPE in (1, 2)  and
                                                  pr.RESTRICTION NOT IN (SELECT restriction FROM user_restriction_access where userseq = (select userseq from central.users where user_id = '{1}')  )))
            
            ";         

        public const string CountOfPendedClaimsClient =
            @"SELECT count(*) from 
            (select distinct C.CLASEQ,HP.PATNUM from {0}.HCICLA  C JOIN {0}.claim_line CY ON C.CLASEQ=CY.CLASEQ AND C.CLASUB=CY.CLASUB
JOIN {0}.HCIPAT HP ON CY.PATSEQ=HP.PATSEQ
where c.status = 'P' and c.reltoclient='T' and HP.patseq NOT IN (SELECT  patseq from smtst.PATIENT_RESTRICTION pr
                                                  JOIN CENTRAL.REF_RESTRICTION rr on pr.RESTRICTION = rr.RESTRICTION
                                            WHERE 
                                                  rr.USER_TYPE in (1, 8)  and
                                                  pr.RESTRICTION NOT IN (SELECT restriction FROM user_restriction_access where userseq = (select userseq from central.users where user_id = '{1}')  )))
            
            ";

        public const string ReviewGroupList =
            @"SELECT reviewgroup from {0}.reviewgroups order by reviewgroup";

        public const string ActiveProductList =
            @"select PCI_ACTIVE,COB_ACTIVE,DCI_ACTIVE,FCI_ACTIVE,FFP_ACTIVE,NEG_ACTIVE,OCI_ACTIVE,RXI_ACTIVE from client where clientcode = '{0}'";

        public const string ActiveProduct = @"select {0}_ACTIVE from client where clientcode='{1}'";
        public const string ActiveOrDisableProductByClient= @"update client set {0}_ACTIVE='{2}' where clientcode='{1}'";

        public const string EnableOnlyDCIForClient =
            @"UPDATE CLIENT SET dci_active='T',cob_active='F',pci_active='F',FCI_active='F',FFP_Active='F',neg_active='F',rxi_active='F' where clientcode='{0}'";
        public const string RestoreProductsForClients =
            @"UPDATE CLIENT SET dci_active='T',cob_active='T',pci_active='T',FCI_active='T',FFP_Active='T',neg_active='F',rxi_active='F' where clientcode='{0}'";

        public const string CountOfFlaggedClaims =
            @"SELECT count(DISTINCT c.claseq || c.clasub) FROM {0}.HCICLA c INNER JOIN {0}.line_flag lfg ON C.ClASEQ = lfg.CLASEQ AND C.CLASUB=lfg.CLASUB
                where  c.CLEARED='F' and LFG.DELETED =  'N'  AND LFG.EDIT_TYPE = 'R' and 
                    lfg.batchseq = (select batchseq from {0}.batch where batch_id = 'LoadTestBatch') and lfg.patseq    
NOT IN (SELECT  patseq from smtst.PATIENT_RESTRICTION pr
                                                  JOIN CENTRAL.REF_RESTRICTION rr on pr.RESTRICTION = rr.RESTRICTION
                                            WHERE 
                                                  rr.USER_TYPE in (1, 2)  and
                                                  pr.RESTRICTION NOT IN (SELECT restriction FROM user_restriction_access where userseq = (select userseq from central.users where user_id = '{1}')  ))
";

        public const string CountOfFlaggedClaimsClient =
            @"SELECT count(DISTINCT c.claseq || c.clasub) FROM {0}.HCICLA c INNER JOIN {0}.line_flag lfg ON C.ClASEQ = lfg.CLASEQ AND C.CLASUB=lfg.CLASUB
                where  c.CLEARED='F' and LFG.DELETED =  'N'  AND LFG.EDIT_TYPE = 'R' and c.reltoclient = 'T' and
                    lfg.batchseq = (select batchseq from {0}.batch where batch_id = 'LoadTestBatch') and lfg.patseq    
NOT IN (SELECT  patseq from smtst.PATIENT_RESTRICTION pr
                                                  JOIN CENTRAL.REF_RESTRICTION rr on pr.RESTRICTION = rr.RESTRICTION
                                            WHERE 
                                                  rr.USER_TYPE in (1, 8)  and
                                                  pr.RESTRICTION NOT IN (SELECT restriction FROM user_restriction_access where userseq = (select userseq from central.users where user_id = '{1}')  ))
";

        public const string LockedClaimListByUser =
           @"SELECT  claseq || '-' || clasub FROM {0}.locked_claims " +
            "WHERE userseq=(Select userseq from users where user_id='{1}')";

        public const string LockedClaims = "select count(*) from {0}.locked_claims where claseq={1} and clasub={2}" +
                                           "WHERE userseq=(Select userseq from users where user_id='{0}')";

        public const string LockedClaimCountByUser =
            @"SELECT  count(*) FROM {0}.locked_claims " +
            "WHERE userseq=(Select userseq from users where user_id='{1}')";

        public const string UpdateStatusToUnreviewed =
            @"UPDATE " + client + ".hcicla set status='U' where claimno in ('{0}')";

        public const string ClaimByClaimNoForClientUser =
            @"SELECT claseq || '-' || clasub FROM  " + client + ".hcicla WHERE claimno='{0}' and RELTOCLIENT='T' order by claseq desc,clasub desc";

        public const string ClaimByClaimNo = @"SELECT claseq || '-' || clasub FROM  " + client + ".hcicla WHERE claimno='{0}' order by claseq desc,clasub desc";

        public const string DeleteClaimAuditOnlyByClaimNo =
            @"delete from " + client + ".claim_audit where (claseq || '-' || clasub) in (select (claseq || '-' || clasub) " +
            "from " + client + ".hcicla where claimno='{0}') and trunc(audit_date)>=trunc(to_date('{1}','dd-Mon-yyyy'))";

        public const string TotalCountOfClaimDocuments =
            @"select count(*) from " + client + ".claimdocs where claseq = {0} and clasub = {1}";

        public const string DeleteClaimDocumentRecord =
            @"begin
            delete from " + client + ".claimdocs where claseq={0} and clasub={1};" +
            "delete from " + client + ".documents where docseq in (select docseq from  " + client + ".claimdocs where claseq={0} and clasub={1});" +
            "delete from " + client + ".document_action_audit where docseq in (select docseq from " + client + ".claimdocs where claseq={0} and clasub={1});" +
            "delete from " + client + ".claim_audit where claseq = {0} and clasub = {1} and trunc(audit_date)>=trunc(to_date('{2}','dd-Mon-yyyy'));" +
            "end;";

        public const string ClaimAuditDocumentUploaded =
            "select * from " + client + ".claim_audit where claseq={0} and clasub={1} and AUDIT_ACTION='DOCUMENT_UPLOAD'";

        public const string ClaimAuditDocumentDeleted =
            "select * from " + client + ".claim_audit where claseq={0} and clasub={1} and AUDIT_ACTION='DOCUMENT_DELETE'";

        public const string ClaimAuditDocumentDownload =
            @"select * from " + client + ".DOCUMENT_ACTION_AUDIT where  docseq in " +
            "(select docseq from " + client + ".claimdocs where claseq={0} and clasub={1} and action_type = 'D')";

        public const string QaErrorReasonCode =
                @"select trim(code || ' - ' || shortdesc) reasoncode from HCIUSER.overreason where type='Q' order by reasoncode"
            ;

        public const string ListOfClaimsForPciWorklist =
            @"select cla.claseq || '-' || cla.clasub claimsequence, due_date, clientrecvddate from smtst.hcicla cla 
join smtst.hcicla_nucleus cla_nuc 
on cla.claseq = cla_nuc.claseq
where (cla.claseq || '-' || cla.clasub) in ({0})
order by cla_nuc.DUE_DATE asc, 
                                          cla.clientrecvddate asc, 
                                          cla.CLASEQ asc, cla.CLASUB asc";
        public const string ClaimSequenceByClaimNoInDescendingOrder=
            @"SELECT claseq || '-' || clasub FROM  " + client + ".hcicla WHERE claimno='{0}' and reltoclient = 'T' order by claseq desc,clasub desc";

        public const string ClaimSequenceByClaimNoInAscendingOrder =
            @"SELECT claseq || '-' || clasub FROM  " + client + ".hcicla WHERE claimno='{0}' and reltoclient = 'T' order by claseq,clasub";

        public const string BatchIDListByClient =
            @"select batch_id from (select * from {0}.batch where active='T' order by batch_Date desc) where rownum<=14";

        public const string UpdateHideAppeal = @"UPDATE CLIENT SET HIDE_APPEAL_FUNCTIONS ='{0}' WHERE CLIENTCODE='{1}'";
        /// <summary>
        /// delete logic in claim 
        /// </summary>
        public const string DeleteClaimLogicInFlagOfLogicManager = @"DELETE FROM " + client + @".hcilogic where claseq={0}  and clasub ={1}";

        public const string DeleteLineFlagAuditByClaimSequence =
            @"delete  from {2}.line_flag_audit where flagseq in (select flagseq from {2}.line_flag where claseq={0} and clasub={1})";

        public const string RestoreDeletedFlagsByClaimSequence = @"update  "+client+".line_flag set deleted='N' where claseq={0} and clasub={1}";
        public const string RestoreParticularFlagsByClaimSequence = @"update  " + client + ".line_flag set deleted='N' where claseq={0} and clasub={1} and editflg not in ('FRE')";
        public const string UpdateClaimQAAudit = "update " + client + ".CLAIM_QA_AUDIT set approved_by=null where claseq={0} and clasub={1}";

        public const string UpdateAuditCompletedInClaimQaAudit =
            "update {0}.claim_qa_audit set audit_completed = '{1}' where claseq = {2} and clasub ={3}";

        public const string UpdateStatusToUnreviewedByClaimSequence =
           @"begin
            UPDATE {2}.hcicla set status='U' where claseq={0} and clasub={1};
            UPDATE {2}.line_flag set deleted='N' where claseq={0} and clasub={1};
            end;";

        public const string DeleteClaimAuditOnlyByClient =
           "delete from {3}.claim_audit where claseq={0} and clasub={1} and trunc(audit_date)>=trunc(to_date('{2}','dd-Mon-yyyy')) ";

        public const string DeleteClaimAuditRecordByClient = @"
        begin
        DELETE FROM {4}.line_flag_audit WHERE flagseq={0} ;
        DELETE FROM {4}.claim_audit WHERE claseq={1} AND  audit_action='LINEEDIT' AND  clasub={2} AND line_number={3};  
        end;";

        public const string GetTriggerClaimDetails = @"select comments from SMTST.HCIFLAG where claseq='{0}' and EDITFLG='FOT'";

        public const string RevertScriptForCVP = @"
        begin
            update " + client + ".hcicla set status='U', reltoclient='F' where claseq in ({0});" +
            "update " + client + ".hciflag set deleted='N'  where claseq in ({1});"+
        "end;";

        public const string ResetFlagStatus = 
            @"begin
            UPDATE SMTST.hcicla set status='U',reltoclient='{3}' where claseq={2} ;
            update smtst.line_flag set hcidone='{0}',clidone='{1}' WHERE CLASEQ={2};
            end;";

        public const string GetFlagstatusForInternalUser =
            @"(SELECT DECODE(SUM(DECODE(product,'C',1,0)),0,'No Flag(s)',DECODE(SUM(DECODE(hcidone,'F',1,0)),0,'Reviewed','Unreviewed')) status FROM SMTST.line_flag WHERE edit_type='R' 
AND claseq={0} and clasub={1} AND deleted = 'N')";


        public const string GetFlagstatusForClientUser =
            @"(SELECT DECODE(SUM(DECODE(product,'C',1,0)),0,'No Flag(s)',DECODE(SUM(DECODE(clidone,'F',1,0)),0,'Reviewed','Unreviewed')) status FROM SMTST.line_flag WHERE edit_type='R' 
AND claseq={0} and clasub={1} AND deleted = 'N')";

        public const string FindRuleNoteForGivenClaimSeq = @"
                                                            SELECT rn.notes FROM 
                                                            smtst.hciflag f JOIN 
                                                            repository.mv_rule_note rn ON f.rule_seq = rn.rule_seq 
                                                            WHERE  f.claseq = {0} and f.clasub = {1} and f.editflg = '{2}'";

        public const string CountOfTotalFlagsAssociatedToAClaim = @"
                                                                   SELECT count(*) FROM smtst.hciflag where claseq = {0} and clasub = {1}";


        public const string GetAvailabeRestrictionsList =
            @"select description from ref_restriction where restriction in (select 
                restriction from user_restriction_access where userseq=(select userseq from central.users where user_id='{0}'))";

        //Query casts claims with no restriction (null) with 0
        public const string GetRestrictionForClaimseq =
                @"select NVL(restriction,0) from smtst.patient_restriction where patseq in (select patseq from SMTST.claim_line where claseq={0} and clasub={1})"
            ;

        public const string GetRestrictionFromDec =
            @"select restriction from ref_restriction where description='{0}'";

        public const string GetCoderReviewByClaimSequence = @"SELECT CODER_REVIEW from SMTST.HCICLA where CLASEQ ={0} and CLASUB={1}";
        public const string GetEditFlagProductByClaimSequence = @"SELECT PRODUCT from EDIT where editflg in (select editflg from smtst.hciflag where CLASEQ ={0} and CLASUB={1})";

        public const string GetCountOfClaseqFromBatchId = @" SELECT count(distinct claseq)from smtst.hcicla where batchseq = 
                                                                (select batchseq from smtst.hcibatch where batchid  = '{0}') ";

        public const string GetDefenseRationaleOfFlagseqBasedOnEffectiveDateAndDOS =
        //@"select d.description from smtst.hcilin l join smtst.hciflag f on l.linseq=f.linseq 
        //join repository.defense_code d on f.defense_code=d.code where f.flagseq={0} and
        // l.dos between d.effective_date and nvl(d.expire_date,'01-JAN-9999')";

            @"select d.description from rpe.hcilin l join rpe.hciflag f on l.linseq=f.linseq 
            join repository.defense_code d on f.defense_code=d.code where f.claseq='{0}' and f.clasub = '{1}' and
             l.dos between d.effective_date and nvl(d.expire_date,'01-JAN-9999')";

        public const string DefenseCodeByFlagSeq=
            @" select defense_code from smtst.hciflag where  flagseq={0}";

        public const string GetFlagDetailInformation =
            @"select RULE_CATEGORY_ID, PRIMARY_DEFENSE_AUTHOR, PRIMARY_DEFENSE_LOCATION, EDIT_TEXT from {0}.LINE_FLAG where CLASEQ = '{1}' and CLASUB='{2}'";

        public const string GetEditTextlInformation =
            @"select EDIT_TEXT from {0}.LINE_FLAG where CLASEQ = '{1}' and CLASUB='{2}' and editflg = '{3}'";

        public const string GetEOBMessageForFlag =
            @"select description_text from edit_description where editflg = '{0}' and type = '{1}'";

        public const string FlagList =
            @"select editflg from REPOSITORY.edit where product='{0}' and status='A' and edit_type!= 'M' order by editflg";

        public const string AllFlagList =
       @"select editflg from REPOSITORY.edit where status='A' and edit_type!= 'M' order by editflg";

        public const string GetAppealCountByClaseqAppealStatus =
        @"SELECT NVL(MAX(APPEALCOUNT),0) FROM (
        SELECT COUNT(DISTINCT A.APPEALSEQ) APPEALCOUNT, CLASEQ, CLASUB, LINSEQ
            FROM ATS.APPEAL A
        JOIN ATS.APPEAL_LINE AL ON A.APPEALSEQ = AL.APPEALSEQ
            and a.clientcode=  '" + client + @"' 
        WHERE A.STATUS<> 'R' and A.appeal_type<>'D' and (claseq,clasub) in ({0})
            GROUP BY CLASEQ, CLASUB, LINSEQ)LINEAPPEALCOUNT
            where
        claseq in 
        (select claseq from ats.appeal a join ats.appeal_line al on a.appealseq= al.appealseq where a.status in ('{1}')
        and (claseq,clasub) in ({0})) group by claseq order by claseq desc";

        
        //        public const string GetAppealCountByClaimnoAppealStatus =
        //            @"select count(*) from (select  distinct(a.appealseq), al.altclaimno from ats.appeal_line al join ats.appeal a on al.appealseq = a.appealseq where 
        //status = '{0}' and a.clientcode = '" + client + "' and altclaimno in ({1}))";

        //        public const string GetAppealCountByClaimnoAppealStatus =
        //            @"select count(*) from (select  distinct(a.appealseq), al.altclaimno from ats.appeal_line al join ats.appeal a on al.appealseq = a.appealseq where 
        //status = '{0}' and a.clientcode = '" + client + "' and altclaimno in ({1}))";


        public const string GetPenedClaimsFromReviewGroup =
            "select distinct c.claimno from" + client + ".hcicla c join " + client +
            "hcicla_nucleus hn on c.claseq = hn.claseq and " +
            @" hn.clasub = c.clasub 
join smtst.hcilin hl on c.claseq=hl.claseq and c.clasub=hl.clasub
where c.status = 'P' and c.reltoclient='T' and C.REVIEWGROUP IN ('Permapend','Pre-Pay') and hl.patseq NOT IN (SELECT  patseq from smtst.PATIENT_RESTRICTION pr
        JOIN CENTRAL.REF_RESTRICTION rr on pr.RESTRICTION = rr.RESTRICTION
            WHERE
        rr.USER_TYPE in (1, 2)  and
        pr.RESTRICTION NOT IN (SELECT restriction FROM user_restriction_access where userseq = (select userseq from central.users where user_id = '{0}')  )))";

        public const string GetUnreviewedClaimsFromReviewGroup =
            "select distinct c.claimno from " + client + ".hcicla c join " + client +
            ".hcicla_nucleus hn on c.claseq = hn.claseq and " +
            " hn.clasub = c.clasub join " + client + ".hcilin hl on c.claseq = hl.claseq and hl.clasub = c.clasub where c.status = 'U' and c.reltoclient='T' and C.REVIEWGROUP IN ('Permapend','Pre-Pay')" +
            @" and hl.proc in (select code from medical_code_description_mv where code = '{1}' and code_type in ('CPT', 'HCPCS', 'DEN')) and   hl.patseq NOT IN (SELECT  patseq from smtst.PATIENT_RESTRICTION pr
        JOIN CENTRAL.REF_RESTRICTION rr on pr.RESTRICTION = rr.RESTRICTION
            WHERE
        rr.USER_TYPE in (1, 8)  and
        pr.RESTRICTION NOT IN (SELECT restriction FROM user_restriction_access where userseq = (select userseq from central.users where user_id = '{0}')  ))";


        public const string GetFlaggedClaimForReviewGroup= "SELECT distinct c.claimno FROM " + client + ".HCICLA c INNER JOIN " + client + ".line_flag lfg ON C.ClASEQ = lfg.CLASEQ AND C.CLASUB=lfg.CLASUB" +
                                                           " join " + client + ".hcilin hl on c.claseq = hl.claseq and hl.clasub = c.clasub where  c.reviewgroup in ('Permapend','Pre-Pay') and c.CLEARED='F' and LFG.DELETED =  'N'  AND LFG.EDIT_TYPE = 'R' and  hl.proc in (select code from medical_code_description_mv where code = '{1}' and code_type in ('CPT','HCPCS','DEN')) and " +
        "lfg.batchseq = (select batchseq from "+ client+".batch where batch_id = 'LoadTestBatch') and lfg.patseq "+
        " NOT IN (SELECT patseq from "+ client+".PATIENT_RESTRICTION pr JOIN CENTRAL.REF_RESTRICTION rr on pr.RESTRICTION = rr.RESTRICTION WHERE rr.USER_TYPE in (1, 8)  and pr.RESTRICTION NOT IN (SELECT restriction FROM user_restriction_access where userseq = (select userseq from central.users where user_id = '{0}')))" ;
        
        public const string GetUnreviewedClaimsFromReviewGroupForInternal =
            "select distinct c.claseq||'-'||c.clasub from " + client + ".hcicla c join " + client +".hcilin hl on c.claseq = hl.claseq and hl.clasub = c.clasub" +
            @" where c.status = 'U' and C.REVIEWGROUP IN ('Permapend','Pre-Pay') and hl.proc in (select code from medical_code_description_mv where code = '{1}' and code_type in ('CPT', 'HCPCS', 'DEN')) and  hl.patseq NOT IN (SELECT  patseq from smtst.PATIENT_RESTRICTION pr
        JOIN CENTRAL.REF_RESTRICTION rr on pr.RESTRICTION = rr.RESTRICTION
            WHERE
        rr.USER_TYPE in (1, 2)  and
        pr.RESTRICTION NOT IN (SELECT restriction FROM user_restriction_access where userseq = (select userseq from central.users where user_id = '{0}')  )) and c.claseq||'-'||c.clasub not in (select distinct ca.claseq||'-'||ca.clasub from smtst.claim_audit ca inner join smtst.claim_qa_audit cqa on ca.claseq = cqa.claseq and ca.clasub = cqa.clasub where ca.audit_action = 'QAREADY' " +
        "and cqa.audit_completed = 'F')";


        public const string GetFlaggedClaimForReviewGroupForInternal = "SELECT distinct c.claseq||'-'||c.clasub FROM " + client + ".HCICLA c INNER JOIN " + client + ".line_flag lfg ON C.ClASEQ = lfg.CLASEQ AND C.CLASUB=lfg.CLASUB join " + client + ".hcilin hl on c.claseq = hl.claseq and hl.clasub = c.clasub where c.reviewgroup in ('Permapend','Pre-Pay') and c.CLEARED='F' and LFG.DELETED =  'N'  AND LFG.EDIT_TYPE = 'R' and  hl.proc in (select code from medical_code_description_mv where code = '{1}' and code_type in ('CPT','HCPCS','DEN')) and " +
                                                            "lfg.batchseq = (select batchseq from " + client + ".batch where batch_id = 'LoadTestBatch') and lfg.patseq " +
                                                            " NOT IN (SELECT patseq from " + client + ".PATIENT_RESTRICTION pr JOIN CENTRAL.REF_RESTRICTION rr on pr.RESTRICTION = rr.RESTRICTION WHERE rr.USER_TYPE in (1, 2)  and pr.RESTRICTION NOT IN (SELECT restriction FROM user_restriction_access where userseq = (select userseq from central.users where user_id = '{0}')))";

        public const string GetFlagCountByClaimSeqAndProduct =
            @"select count(*) from " + client + ".line_flag where claseq={0} and clasub={1} and product='{2}'";

        public const string GetExcelvalueForFlaggedClaims = "SELECT distinct c.claseq,  c.claseq||'-'|| c.clasub, " +
                                                            "c.claimno, to_char(c.CLIENTRECVDDATE,'MM/DD/YYYY')recvdDate, hn.form_type, NVL(p.PROVNAME, p.GROUPNAME),  p.prvseq,(select patnum from smtst.hcipat where patseq=l.patseq)memberid," +
                                                            " (SELECT LTRIM(SHARED.STRAGG(' '|| EDITFLG))" +
                                                            "  FROM(SELECT DISTINCT EDITFLG, CLASEQ, CLASUB FROM " + client + ".LINE_FLAG LF" +
                                                            " WHERE LF.DELETED = 'N' AND LF.TOPFLAG = 'T' AND LF.EDIT_TYPE = 'R')" +
                                                            " WHERE CLASEQ = C.CLASEQ AND CLASUB = C.CLASUB) TOPFLAGS," +
                                                            "(SELECT SUM(NVL(F.PAID + F.DEDUCT - F.CLISUGPAID,0)) SAVINGS " +
                                                            " FROM " + client + ".hcilin lin join " + client + ".hciflag f on lin.linseq=f.linseq " +
                                                            "    JOIN EDIT E ON F.EDITFLG = E.EDITFLG WHERE DELETED = 'N' AND TOPFLAG = 'T' " +
                                                            "AND e.edit_type != 'M' AND lin.claseq= c.claseq and lin.clasub= c.clasub)SAVINGS," +
                                                            " b.batchid, " +" rlob.description line_of_business_description, " +
                                                            " Decode(c.reltoclient,'T',decode(c.status,'R','Client Reviewed','U','Client Unreviewed','P','Pended','I','In Process'),Decode(c.status,'R','Cotiviti Reviewed','U','Cotiviti Unreviewed','P','Pended','I','In Process'))status "+
                                                            " FROM " + client + ".hcicla c " +
                                                            " JOIN " + client + ".hcicla_nucleus hn ON c.claseq = hn.claseq AND c.clasub = hn.clasub " +
                                                            " JOIN " + client + ".hcibatch b ON c.batchseq = b.batchseq " +
                                                            " JOIN " + client + ".hcilin l ON c.claseq = l.claseq AND c.clasub = l.clasub " +
                                                            "  JOIN " + client + ".LINE_FLAG LF ON L.LINSEQ= LF.LINSEQ " +
                                                            "  JOIN " + client + ".hciprov p ON l.prvseq = p.prvseq " +
                                                            "  LEFT JOIN central.ref_line_of_business rlob ON c.line_of_business = rlob.line_of_business " +
                                                            "  LEFT JOIN central.users u on hn.assigned_to = u.userseq " +
                                                            "  WHERE  rlob.line_of_business='C'and b.batchid='LoadTestBatch'  and lf.patseq    NOT IN (SELECT  patseq from " + client +".PATIENT_RESTRICTION pr " +
                                                            "  JOIN CENTRAL.REF_RESTRICTION rr on pr.RESTRICTION = rr.RESTRICTION " +
                                                            "   WHERE rr.USER_TYPE in (1, 2)  and " +
                                                            "  pr.RESTRICTION NOT IN (SELECT restriction FROM central.user_restriction_access where userseq = (select userseq from central.users where user_id = '{0}')))  ORDER BY CLASEQ DESC";


        public const string GetExcelValueFOrFlaggedClaimsClient= "SELECT distinct c.claseq,  c.claseq||'-'|| c.clasub, " +
                                                            "c.claimno,to_char(c.CLIENTRECVDDATE,'MM/DD/YYYY')recvdDate, hn.form_type, NVL(p.PROVNAME, p.GROUPNAME),  p.prvseq, (select patnum from smtst.hcipat where patseq=l.patseq)memberid," +
                                                            " (SELECT LTRIM(SHARED.STRAGG(' '|| EDITFLG))" +
                                                            "  FROM(SELECT DISTINCT EDITFLG, CLASEQ, CLASUB FROM " + client + ".LINE_FLAG LF" +
                                                            " WHERE LF.DELETED = 'N' AND LF.TOPFLAG = 'T' AND LF.EDIT_TYPE = 'R')" +
                                                            " WHERE CLASEQ = C.CLASEQ AND CLASUB = C.CLASUB) TOPFLAGS," +
                                                            "(SELECT SUM(NVL(F.PAID + F.DEDUCT - F.CLISUGPAID,0)) SAVINGS " +
                                                            " FROM " + client + ".hcilin lin join " + client + ".hciflag f on lin.linseq=f.linseq " +
                                                            "    JOIN EDIT E ON F.EDITFLG = E.EDITFLG WHERE DELETED = 'N' AND TOPFLAG = 'T' " +
                                                            "AND e.edit_type != 'M' AND lin.claseq= c.claseq and lin.clasub= c.clasub)SAVINGS," +
                                                            " b.batchid, " + " rlob.description line_of_business_description, " +
                                                            " Decode(c.reltoclient,'T',decode(c.status,'R','Client Reviewed','U','Client Unreviewed','P','Pended','I','In Process'),Decode(c.status,'R','Cotiviti Reviewed','U','Cotiviti Unreviewed','P','Pended','I','In Process'))status " +
                                                            " FROM " + client + ".hcicla c " +
                                                            " JOIN " + client + ".hcicla_nucleus hn ON c.claseq = hn.claseq AND c.clasub = hn.clasub " +
                                                            " JOIN " + client + ".hcibatch b ON c.batchseq = b.batchseq " +
                                                            " JOIN " + client + ".hcilin l ON c.claseq = l.claseq AND c.clasub = l.clasub " +
                                                            "  JOIN " + client + ".LINE_FLAG LF ON L.LINSEQ= LF.LINSEQ " +
                                                            "  JOIN " + client + ".hciprov p ON l.prvseq = p.prvseq " +
                                                            "  LEFT JOIN central.ref_line_of_business rlob ON c.line_of_business = rlob.line_of_business " +
                                                            "  LEFT JOIN central.users u on hn.assigned_to = u.userseq " +
                                                            "  WHERE  rlob.line_of_business='C'and b.batchid='LoadTestBatch'  and lf.patseq    NOT IN (SELECT  patseq from " + client + ".PATIENT_RESTRICTION pr " +
                                                            "  JOIN CENTRAL.REF_RESTRICTION rr on pr.RESTRICTION = rr.RESTRICTION " +
                                                            "   WHERE rr.USER_TYPE in (1, 2)  and " +
                                                            "  pr.RESTRICTION NOT IN (SELECT restriction FROM central.user_restriction_access where userseq = (select userseq from central.users where user_id = '{0}')))  ORDER BY c.claimno DESC";

        public const string GetExcelDownloadAudit = "select * from(select Action_source from " + client +
                                                    ".document_action_audit where userseq in (select userseq from users where user_id = '{0}') and action_type='D' order by action_time desc) where rownum<=5";


        public const string CountOfSpecificFlaggedClaims =
            @"SELECT count(DISTINCT c.claseq || c.clasub) FROM {0}.HCICLA c INNER JOIN {0}.line_flag lfg ON C.ClASEQ = lfg.CLASEQ AND C.CLASUB=lfg.CLASUB
                where  c.CLEARED='F' and LFG.DELETED =  'N'  AND LFG.EDIT_TYPE = 'R' and 
                    lfg.batchseq = (select batchseq from {0}.batch where batch_id = 'LoadTestBatch') and lfg.patseq    
 NOT IN (SELECT  patseq from smtst.PATIENT_RESTRICTION pr
                                                  JOIN CENTRAL.REF_RESTRICTION rr on pr.RESTRICTION = rr.RESTRICTION
                                            WHERE 
                                                  rr.USER_TYPE in (1, 2)  and
                                                  pr.RESTRICTION NOT IN (SELECT restriction FROM user_restriction_access where userseq = (select userseq from central.users where user_id = '{1}')  ))
                                                  and lfg.editflg in ({2})";

        public const string GetCountOfUnreviewedClaimsForSpecificFlag = @"SELECT COUNT(*) FROM 
            (SELECT DISTINCT  c.claseq, c.clasub, c.altclaimno, c.claimno, c.status, c.reltoclient, c.reviewgroup, 
            hn.sub_status  
            FROM {0}.hcicla c join {0}.hcicla_nucleus hn
            on c.claseq=hn.claseq  
             join {0}.hciflag flg 
             on flg.claseq = c.claseq 
join smtst.hcilin hl on c.claseq=hl.claseq and c.clasub=hl.clasub
where c.status='U' and c.reltoclient='F' and flg.editflg in ({2})
             and flg.deleted = 'N' and flg.EDITFLG <> 'PS'
            and hl.patseq NOT IN (SELECT  patseq from smtst.PATIENT_RESTRICTION pr
                                                  JOIN CENTRAL.REF_RESTRICTION rr on pr.RESTRICTION = rr.RESTRICTION
                                            WHERE 
                                                  rr.USER_TYPE in (1, 2)  and
                                                  pr.RESTRICTION NOT IN (SELECT restriction FROM user_restriction_access where userseq = (select userseq from central.users where user_id = '{1}')  )) and c.claseq||'-'||c.clasub not in (select distinct ca.claseq||'-'||ca.clasub from smtst.claim_audit ca 
inner join smtst.claim_qa_audit cqa on ca.claseq = cqa.claseq and ca.clasub = cqa.clasub
where ca.audit_action = 'QAREADY' and cqa.audit_completed = 'F'))";

        public const string GetCountOfSpecificFlaggedClaimsForClient = @"SELECT count(DISTINCT c.claseq || c.clasub) FROM {0}.HCICLA c INNER JOIN {0}.line_flag lfg
ON C.ClASEQ = lfg.CLASEQ AND C.CLASUB=lfg.CLASUB
                where  c.CLEARED='F' and LFG.DELETED =  'N'  AND LFG.EDIT_TYPE = 'R' and c.reltoclient = 'T' and
                    lfg.batchseq = (select batchseq from {0}.batch where batch_id = 'LoadTestBatch') and lfg.patseq   NOT IN (SELECT  patseq from smtst.PATIENT_RESTRICTION pr
                                                  JOIN CENTRAL.REF_RESTRICTION rr on pr.RESTRICTION = rr.RESTRICTION
                                            WHERE 
                                                  rr.USER_TYPE in (1, 8)  and
                                                  pr.RESTRICTION NOT IN (SELECT restriction FROM user_restriction_access where userseq = (select userseq from central.users where user_id = '{1}')  ))
        and lfg.editflg in ({2})";

        public const string GetCountOfUnreviewedClaimsForSpecificFlagForClient = @"SELECT COUNT(*) FROM 
            (SELECT DISTINCT  c.claseq, c.clasub, c.altclaimno, c.claimno, c.status, c.reltoclient, c.reviewgroup, 
            hn.sub_status  
            FROM {0}.hcicla c join {0}.hcicla_nucleus hn
            on c.claseq=hn.claseq  
             join {0}.hciflag flg 
             on flg.claseq = c.claseq 
join smtst.hcilin hl on c.claseq=hl.claseq and c.clasub=hl.clasub
where c.status='U' and c.reltoclient='T' and flg.editflg in ({2})
             and flg.deleted = 'N' and flg.EDITFLG <> 'PS'
            and hl.patseq NOT IN (SELECT  patseq from smtst.PATIENT_RESTRICTION pr
                                                  JOIN CENTRAL.REF_RESTRICTION rr on pr.RESTRICTION = rr.RESTRICTION
                                            WHERE 
                                                  rr.USER_TYPE in (1, 8)  and
                                                  pr.RESTRICTION NOT IN (SELECT restriction FROM user_restriction_access where userseq = (select userseq from central.users where user_id = '{1}')  )))";

        #region Original Claim Data
        public const string GetOriginalClaimDataFromDatabase = @"SELECT CD.* FROM " + client + ".CLIENT_DATA CD " +
                                                               "JOIN " + client + ".CLAIM_LINE LIN " +
                                                               "ON CD.HCILINSEQ = LIN.LINSEQ " +
                                                               "WHERE CD.HCICLASEQ = '{0}' AND CD.HCICLASUB = '{1}' " +
                                                               "ORDER BY LIN.LINNO, CD.HCILINSEQ";
        public const string GetClientProcessingType = "SELECT PROCESSING_TYPE FROM CLIENT WHERE CLIENTCODE='{0}'";

        public const string GetProcessingDataForClaim = "SELECT  to_char(PROCESSDATE,'fmMM/DD/YYYY HH:MI:SS PM')CLIENTpROCESSDATE ," +
                                                        "  to_char(CLIENTRECVDDATE,'fmMM/DD/YYYY HH:MI:SS PM') CLIENTRECEIVEDDATE ," +
                                                        "  B.BATCH_ID,FILERECVD, " +
                                                        "  to_char(DATERECVD,'fmMM/DD/YYYY HH:MI:SS PM') COTIVITIRECEIVED , " +
                                                        "  to_char(B.START_TIME,'fmMM/DD/YYYY HH:MI:SS PM') COTIVITIRUNDATE ," +
                                                        "  nvl(FILERETRN,'Pend'), to_char(DATERETRN,'MM/DD/YYYY HH:MI:SS PM') DATERETURNED " +
                                                        "     FROM SMTST.CLAIM C JOIN SMTST.BATCH B " +
                                                        "      ON C.BATCHSEQ= B.BATCHSEQ WHERE CLASEQ = {0} AND CLASUB ={1}";

        public const string GetClaimHistory = "select to_char(CA.audit_date,'MM/DD/YYYY HH:MI:SS PM') INITCAP, " +
                                              " INITCAP(CA.audit_action) ,CA.Line_number,DECODE(U.username,'System',U.username,U.username ||' '||'('||U.USER_ID||')' )username, " +
                                              " DECODE(U.USER_TYPE,'2','Internal','8','Client','System') USER_TYPE, " +
                                              " DECODE(ca.status,'U','Unreviewed','R','Reviewed','X','Editor Analyzed','I','In System', (select description_text from ref_sub_status where sub_status_code= ca.sub_status))status " +
                                              " , (select username||'('||USER_ID||')' from users where userseq= CA.assigned_to)assigned_to, " +
                                              "Decode(CA.product,'*', null ,(SELECT DESCRIPTION_TEXT FROM CENTRAL.REF_PRODUCTS WHERE PRODUCT_CODE=CA.product))product, " +
                                              " decode(to_char(to_date(CA.elapseD_time,'sssss'),'mi:ss'),'00:00','0',to_char(to_date(CA.elapseD_time,'sssss'),'mi:ss'))RESPONSE_TIME , " +
                                              " CA.ACTION_DESCRIPTION NOTES " +
                                              "  FROM {0}.claim_audit CA " +
                                              " JOIN USERS U ON U.USERSEQ= CA.USERSEQ AND CLASEQ = {1} AND CLASUB = {2} {3} ORDER BY CA.audit_date DESC";

        public const string ProductSpecificInvoiceData = @" SELECT 
                TRIM(to_char(NVL(CI.{0}_REVIEWFEE, 0),'$90.00')) FEE,
                TRIM(to_char(NVL(CI.{0}_APPEALFEE, 0),'$90.00')) APPEAL_FEE,
                TRIM(to_char(NVL(CI.{0}_FRAUDFEE, 0),'$90.00')) FRAUD_FEE,
                TRIM(to_char((NVL(CI.{0}_APPEALFEE,0)+NVL(CI.{0}_REVIEWFEE,0)+NVL(CI.{0}_FRAUDFEE,0)),'$90.00'))TOTALS
                FROM smtst.CLAIM C
                INNER JOIN  smtst.hciINVOICE CI
                ON C.CLASEQ = CI.CLASEQ
                AND C.CLASUB = CI.CLASUB
                WHERE C.CLASEQ||'-'||C.clasub = '{1}'
                ORDER BY CI.INVOICESEQ desc";

        public const string InvoiceDateFromDb =
            @"SELECT c.Altclaimno, 'Group '||c.GROUPSEQ1||'-'||g.DESCRIPTION, CI.INVRECTYP TYPE,DECODE(CI.INVSTATUS,'N','New' )STATUS, CI.INVOICESEQ,CI.INVNUM INVOICE#, CI.INVDATE, to_char(CI.INVAMT,'$90.99'),
 to_char(CI.CREATEDATE,'fmMM/dd/yyyy') CREATED,CI.POSTDATE POSTED,C.FAC_RESULTCODE, CI.PREINVNUM PREVIOUS_INVOICE#, to_char(CI.INVDATE,'fmMM/dd/yyyy'),
 to_char(NVL(CI.PREINVAMT, 0),'$90.99') PREVIOUS_TOTAL_FEE
               FROM smtst.CLAIM C
                INNER JOIN  smtst.hciINVOICE CI
                ON C.CLASEQ = CI.CLASEQ
                AND C.CLASUB = CI.CLASUB
                LEFT OUTER JOIN SMTST.GROUPS G
                ON C.GROUPSEQ1 = G.GROUPSEQ
                WHERE C.CLASEQ||'-'||C.clasub = '{0}'";
        
        #endregion

        public const string GetSugModifierValueByClaimSeqAndEditFlag = @"select distinct suggested_m1, suggested_m2, suggested_m3, suggested_m4 from smtst.hciflag where claseq = {0} and editflg = '{1}'";

        public const string GetClaimSequenceInClaimSearchByMemberId =
            "select distinct claseq||'-'||clasub,claseq from SMTST.hcilin where patseq in (select patseq from smtst.hcipat where patnum='{0}') order by claseq desc";

        public const string GetPatientRestrictionDescriptionByPatNum = @"SELECT RR.DESCRIPTION FROM REF_RESTRICTION RR
            JOIN " + client + ".PATIENT_RESTRICTION PR ON RR.RESTRICTION= PR.RESTRICTION JOIN " + client +
                                                                       ".HCIPAT P ON PR.PATSEQ= P.PATSEQ WHERE P.PATNUM= '{0}'";

        public const string GetClaimsWithQAAuditRecordFromDatabase = @"select distinct c.claseq||'-'||c.clasub from smtst.hcicla c 
                                                                    inner join smtst.claim_audit ca on c.claseq = ca.claseq and c.clasub = ca.clasub 
                                                                    inner join smtst.claim_qa_audit cqa on c.claseq = cqa.claseq and c.clasub = cqa.clasub
                                                                    join smtst.hcilin hl on ca.claseq=hl.claseq and ca.clasub=hl.clasub
                                                                    where c.status = 'U' and 
                                                                    ca.audit_action = 'QAREADY' and  c.reltoclient = 'F' and cqa.audit_completed = 'F' and 
hl.patseq NOT IN (SELECT  patseq from smtst.PATIENT_RESTRICTION pr
                                                  JOIN CENTRAL.REF_RESTRICTION rr on pr.RESTRICTION = rr.RESTRICTION
                                            WHERE 
                                                  rr.USER_TYPE in (1, 2)  and
                                                  pr.RESTRICTION NOT IN (SELECT restriction FROM user_restriction_access where userseq = (select userseq from central.users where user_id = '{0}')  ))";

        public const string GetDentalProfileReview = "select dental_profile_review from smtst.hciprov where prvseq in (select prvseq from smtst.hcilin where claseq={0} and clasub={1})";
        public const string GetConfidenceScoreForClaimAndFlag = "select caspian_ml_score from smtst.hciflag where claseq={0} and editflg='{1}'";

        public const string UpdateRWROAccessForAuthorities = @"update CENTRAL.USER_AUTHORIZATION set attribute= {0} where userseq = (select userseq from central.users where user_id = '{1}')
and authority_id=(select id from central.ref_authorization where description = '{2}')";

        public const string UpdateRWRORoleForUser = @" update central.user_role 
 set role_id=(select id from central.ref_role where role_name='{2}' and applies_to={3})
 where 
 userseq=(select userseq from central.users where user_id='{0}' ) and role_id=(select id from central.ref_role where role_name='{1}' and applies_to={3})";

        public const string GetRoleForUser= @"select * from central.user_role where userseq=(select userseq from central.users where user_id='{0}' )
        and ROLE_ID = (select id from central.ref_role where role_name='{1}' and applies_to = {2})";

        public const string InsertRoleForUser = @"
        Insert into central.user_role(USERSEQ, ROLE_ID)
            values((select userseq from central.users where user_id= '{0}') ,(select id from central.ref_role where role_name='{1}' and applies_to = {2}))";

        public const string DeleteRoleForUser = @"
        delete from central.user_role
            where userseq=(select userseq from central.users where user_id='{0}' )
        and ROLE_ID = (select id from central.ref_role where role_name='{1}' and applies_to = {2})";

        public const string GetEditFlagForClaim =
            "SELECT PRODUCT from EDIT where editflg='{0}'";

        public const string InternalUserClaimSubStatusListForDCIInactiveClient = @"select ss.description_text from REF_SUB_STATUS ss join sub_status_client sc on sc.sub_status_code=ss.sub_status_code 
        where ss.user_type in ('B','H') and sc.clientcode in ('ALL','{0}') and (product='S' or (product='D' and ss.user_type='B'))  order by upper(description_text)";

        public const string UnreviewedClaimByBatchDate = @"select distinct c.claseq||'-'||c.clasub from smtst.hcibatch b
                                                            join smtst.hcicla c on
                                                            c.BATCHSEQ= b.BATCHSEQ
                                                            join smtst.hcilin hl on
                                                            (c.claseq||'-'||c.clasub) = (hl.claseq||'-'||hl.clasub)
                                                            where 
                                                                trunc(batchdate) = TO_DATE('{0}', 'MM/DD/YYYY')  AND
                                                                c.status='U' AND C.RELTOCLIENT='F' and c.claseq||'-'||c.clasub not in (select distinct ca.claseq||'-'||ca.clasub from smtst.claim_audit ca 
            inner join smtst.claim_qa_audit cqa on ca.claseq = cqa.claseq and ca.clasub = cqa.clasub
            where ca.audit_action = 'QAREADY' and cqa.audit_completed = 'F')
                                                                and hl.patseq not in (SELECT  patseq from smtst.PATIENT_RESTRICTION pr
                                                                  JOIN CENTRAL.REF_RESTRICTION rr on pr.RESTRICTION = rr.RESTRICTION
                                                                    WHERE 
                                                                  rr.USER_TYPE in (1, 2) and
                                                                  pr.RESTRICTION NOT IN 
                                                                  (SELECT restriction FROM user_restriction_access 
                                                                  where userseq = (select userseq from central.users where user_id = '{1}')))";

        public const string RestoreDeletedFlagsFromDatabase = @"
                                            BEGIN
                                            update smtst.hciflag set deleted='N' WHERE CLASEQ in ({0},{1});
                                            delete from smtst.hciflag where claseq={1} and linno=1;
                                            end;
                                       ";

        public const string UpdateStatusByClaimSequence =
           @"UPDATE {2}.hcicla set status='{3}',reltoclient='{4}' where claseq ={0} and clasub={1}";


        public const string HciDoneAndCliDoneValuesByAuthSeqLinNoFlagName = @"SELECT {0} FROM {1}.hciflag WHERE claseq = {2} AND linno = {3} AND editflg = '{4}'";

        public const string GetClientReviewFlag = @"select distinct client_review_flg from smtst.hciflag where claseq = {0} and topflag ='T'";


        public const string GetSystemDateFromDatabase = "select to_char(sysdate,'MM/DD/YYYY HH:MI:SS AM') from dual";

        public const string GetPendedClaimsFromProcCodeandTin = @"select C.CLAIMNO from smtst.HCICLA 
            C JOIN smtst.claim_line CY ON C.CLASEQ=CY.CLASEQ AND C.CLASUB=CY.CLASUB
            join smtst.hcilin hl on c.claseq = hl.claseq and c.clasub = hl.clasub
            where c.status = 'P' and c.reltoclient = 'T' and hl.proc = '{1}' and hl.prvseq in (select prvseq from smtst.hciprov where tin = '{2}')
            and hl.patseq NOT IN (SELECT  patseq from smtst.PATIENT_RESTRICTION pr
                                                  JOIN CENTRAL.REF_RESTRICTION rr on pr.RESTRICTION = rr.RESTRICTION
                                            WHERE 
                                                  rr.USER_TYPE in (1, 8)  and
                                                  pr.RESTRICTION NOT IN (SELECT restriction FROM user_restriction_access where userseq = (select userseq from central.users where user_id = '{0}')  ))";

        public const string GetInternalAndClientSugUnitsByClaseqAndFlag =
            "select HCISUGUNITS,CLISUGUNITS  from smtst.hciflag where claseq={0} and clasub={1} and editflg='{2}' and deleted='N'";

        public const string PreAuthCountByPatSequence = @"select count(distinct(l.preauthseq)) from smtst.dentpreauth d join smtst.dentpreauthlin l on d.preauthseq = l.preauthseq where d.patseq = {0}";

        public const string GetCountOfClaimQueueRecord = @"select count(*) from central.realtime_claim_queue_audit where claseq = {0} and clasub = {1} and audit_action = 'I' and clientcode = 'RPE'";

        public const string DeleteRealTimeClaimQueueRecord = @"delete from realtime_claim_queue where claseq = {0} and clasub = {1}";

        public const string DeleteRealTimeClaimQueueAuditRecord = @"delete from central.realtime_claim_queue_audit where claseq = {0} and clasub = {1} and clientcode = 'RPE'";

        public const string DeleteClaimAuditRecordByClientAndClaimSeq = @"delete from {0}.CLAIM_AUDIT where claseq = {1} and clasub = {2}";

        public const string UpdateAuditCompletedAndCompletedDate = @"update {0}.claim_qa_audit set audit_completed='{1}' , completed_date = {2} where claseq = {3} and clasub = {4}";


        public const string GetExpectedReasonCodesForAllFlagsOnClaimOption = @"select trim(code || ' - ' || shortdesc) reasoncode 
from HCIUSER.overreason o where clientcode in ('ALL','{1}','SMTST') and o.product in ('A','R','D','F','C','U')
and type in ('O','L','Q') and user_type in ('A','{2}') and action in ('{0}','L') order by reasoncode";

        public const string GetExpectedReasonCodesForAFlag = @"select trim(code || ' - ' || shortdesc) reasoncode 
from HCIUSER.overreason o where clientcode in ('ALL','{2}','SMTST') and o.product in ('A','{0}')
and type in ('O','L','Q') and user_type in ('A','{3}') and action in ('{1}','L') order by reasoncode";

        public const string GetExpectedReasonCodesForAllFlagsOnTheLine = @"select trim(code || ' - ' || shortdesc) reasoncode 
from HCIUSER.overreason o where clientcode in ('ALL','{1}','SMTST') and o.product in ('A','R','D','F','C','U')
and type in ('O','L','Q') and user_type in ('A','{2}') and action in ('{0}','L') order by reasoncode";

        public const string GetExpectedReasonCodesForAllFlagsOnClaimOptionNoAction = @"select trim(code || ' - ' || shortdesc) reasoncode 
from HCIUSER.overreason o where clientcode in ('ALL','{0}','SMTST') and o.product in ('A','R','D','F','C','U')
and type in ('O','L','Q') and user_type in ('A','{1}') order by reasoncode";

        public const string GetExpectedReasonCodesForAFlagNoAction = @"select trim(code || ' - ' || shortdesc) reasoncode 
from HCIUSER.overreason o where clientcode in ('ALL','{1}','SMTST') and o.product in ('A','{0}')
and type in ('O','L','Q') and user_type in ('A','{2}') order by reasoncode";

        public const string GetExpectedReasonCodesForAllFlagsOnTheLineNoAction = @"select trim(code || ' - ' || shortdesc) reasoncode 
from HCIUSER.overreason o where clientcode in ('ALL','{0}','SMTST') and o.product in ('A','R','D','F','C','U')
and type in ('O','L','Q') and user_type in ('A','{1}') order by reasoncode";

        public const string GetDxCodesValuesDetailsinLineDetails = @"SELECT dx,
                CASE WHEN (length(DX_CODE) >3 and substr(DX_CODE,1,1)<>'E') THEN concat(concat(substr(DX_CODE,0,3),'.'),substr(DX_CODE,4)) 
                ELSE DX_CODE end DX_CODE,DX_IND,DESCRIPTION DXCODE_DESCRIPTION FROM 
                (SELECT * FROM 
                                (select 'Dx1:' dx, patseq, dx1 DX_CODE, dx1_ind dx_ind
                                from smtst.claim_line
                                where claseq = {0} and clasub = {1} and linno ={2} and dx1 is not null
                                union all
                                select 'Dx2:' dx, patseq, dx2 DX_CODE, dx2_ind dx_ind
                                from smtst.claim_line
                                where claseq = {0} and clasub = {1} and linno ={2} and dx2 is not null
                                union all
                                select 'Dx3:' dx, patseq, dx3 DX_CODE, dx3_ind dx_ind
                                from smtst.claim_line
                                where claseq = {0} and clasub = {1} and linno ={2} and dx3 is not null
                                union all
                                select 'Dx4:' dx,patseq, dx4 DX_CODE, dx4_ind dx_ind
                                from smtst.claim_line
                                where claseq = {0} and clasub = {1} and linno ={2} and dx4 is not null
                                union all
                                select 'Dx5:' dx,patseq, dx5 DX_CODE, dx5_ind dx_ind
                                from smtst.claim_line
                                where claseq = {0} and clasub = {1} and linno ={2} and dx5 is not null
                                union all
                                select 'Dx6:' dx,patseq, dx6 DX_CODE, dx6_ind dx_ind
                                from smtst.claim_line
                                where claseq = {0} and clasub = {1} and linno ={2} and dx6 is not null
                                union all
                                select 'Dx7:' dx,patseq, dx7 DX_CODE, dx7_ind dx_ind
                                from smtst.claim_line
                                where claseq = {0} and clasub = {1} and linno ={2} and dx7 is not null
                                union all
                                select 'Dx8:' dx,patseq, dx8 DX_CODE, dx8_ind dx_ind
                                from smtst.claim_line
                                where claseq = {0} and clasub = {1} and linno ={2} and dx8 is not null
                                union all
                                select 'Dx9:' dx,patseq, dx9 DX_CODE, dx9_ind dx_ind
                                from smtst.claim_line
                                where claseq = {0} and clasub = {1} and linno ={2} and dx9 is not null) PAT_DX,
                            smtst.PATIENT_DIAGNOSIS_HISTORY PDH
                        WHERE PAT_DX.DX_CODE = PDH.CODE AND
                              PAT_DX.PATSEQ = PDH.PATSEQ) CLAIM_PAT_DX,
                   DIAGCODES
                WHERE CLAIM_PAT_DX.DX_CODE = DIAGCODES.CODE  AND CLAIM_PAT_DX.dx_ind = DIAGCODES.CODE_IND";

        public const string GetClaimsForHugeDataLoad =
            @"select distinct claseq||'-'||clasub, max(dos) dos from loadt.claim_line where patseq = {0} and prvseq = {1} group by claseq,clasub order by dos desc";

        public const string GetClaimFlagNotesClientUser = @"SELECT LF.LINNO,LF.EDITFLG, 
            TO_CHAR(AUDIT_DATE, 'mm/dd/yyyy fmHH:MI:SS PM')AUDITDATE ,u.username ,REGEXP_REPLACE (NOTES,'<|>|p|/','')
            FROM SMTST.LINE_FLAG_AUDIT LFA 
            JOIN SMTST.LINE_FLAG LF
            ON LF.FLAGSEQ=LFA.FLAGSEQ
            JOIN USERS U
            ON U.USERSEQ=LFA.USERSEQ 
            WHERE  lf.claseq||'-'||lf.clasub='{0}' and deleted='N'  order by LF.LINNO ,LF.EDITFLG";

        public const string GetClaimFlagNotes = @"SELECT LF.LINNO,LF.EDITFLG, 
            TO_CHAR(AUDIT_DATE, 'mm/dd/yyyy fmHH:MI:SS PM')AUDITDATE ,u.username ,REGEXP_REPLACE (NOTES,'<|>|p|/','')
            FROM SMTST.LINE_FLAG_AUDIT LFA 
            JOIN SMTST.LINE_FLAG LF
            ON LF.FLAGSEQ=LFA.FLAGSEQ
            JOIN USERS U
            ON U.USERSEQ=LFA.USERSEQ 
            WHERE  lf.claseq||'-'||lf.clasub='{0}' and deleted='N' AND client_display='T' and audit_action not in ('D','V') order by LF.LINNO ,LF.EDITFLG";

        public const string GetDxLvlValueFromDatabase = @"select {0}_lvl from {3}.claim_line where claseq = '{1}' and linno = {2}";

        public const string GetRecReasonCodeAndDpKeyFromDb = @"select distinct rec_reason_code, dp_key from rpe.HCIFLAG where claseq = {0} and clasub = 0";

        public const string OutstandingClaimsQAResultForNoRestriction = @"SELECT DISTINCT  c.claseq ||'-'|| c.clasub
            FROM smtst.hcicla c join smtst.hcicla_nucleus hn on c.claseq=hn.claseq
            join smtst.hcilin hl on c.claseq=hl.claseq and c.clasub=hl.clasub
            LEFT JOIN smtst.claim_audit ca on c.claseq = ca.claseq and c.clasub = ca.clasub
                                    LEFT JOIN smtst.claim_qa_audit cqa on c.claseq = cqa.claseq and c.clasub = cqa.clasub
            where c.status='U' and c.reltoclient='F' and hl.patseq NOT IN (SELECT  patseq from smtst.PATIENT_RESTRICTION pr
                                                              JOIN CENTRAL.REF_RESTRICTION rr on pr.RESTRICTION = rr.RESTRICTION
                                                        WHERE
                                                              rr.USER_TYPE in (1, 2)  and
                                                              pr.RESTRICTION NOT IN (SELECT restriction FROM user_restriction_access where userseq = (select userseq from central.users where user_id = '{0}')  ))
             and ca.claseq is not null and ca.audit_action= 'QAREADY'
             and c.reltoclient = 'F'
             and c.status = 'U' and cqa.audit_completed ='F'
             AND NOT EXISTS (SELECT 1 FROM smtst.PATIENT_RESTRICTION pr WHERE pr.patseq = hl.patseq)";


        public const string OutstandingClaimsQAResultForAllRestriction = @"SELECT DISTINCT  c.claseq ||'-'|| c.clasub
            FROM smtst.hcicla c join smtst.hcicla_nucleus hn on c.claseq=hn.claseq
            join smtst.hcilin hl on c.claseq=hl.claseq and c.clasub=hl.clasub
            LEFT JOIN smtst.claim_audit ca on c.claseq = ca.claseq and c.clasub = ca.clasub
                                    LEFT JOIN smtst.claim_qa_audit cqa on c.claseq = cqa.claseq and c.clasub = cqa.clasub
            where c.status='U' and c.reltoclient='F' and hl.patseq NOT IN (SELECT  patseq from smtst.PATIENT_RESTRICTION pr
                                                              JOIN CENTRAL.REF_RESTRICTION rr on pr.RESTRICTION = rr.RESTRICTION
                                                        WHERE
                                                              rr.USER_TYPE in (1, 2)  and
                                                              pr.RESTRICTION NOT IN (SELECT restriction FROM user_restriction_access where userseq = (select userseq from central.users where user_id = '{0}')  ))
             and ca.claseq is not null and ca.audit_action= 'QAREADY'
             and c.reltoclient = 'F'
             and c.status = 'U' and cqa.audit_completed ='F'";

        public const string OutstandingClaimsQAResultForSpecificRestriction =
            @"SELECT DISTINCT  c.claseq ||'-'|| c.clasub
            FROM smtst.hcicla c join smtst.hcicla_nucleus hn on c.claseq=hn.claseq
            join smtst.hcilin hl on c.claseq=hl.claseq and c.clasub=hl.clasub
            LEFT JOIN smtst.claim_audit ca on c.claseq = ca.claseq and c.clasub = ca.clasub
                                    LEFT JOIN smtst.claim_qa_audit cqa on c.claseq = cqa.claseq and c.clasub = cqa.clasub
            where c.status='U' and c.reltoclient='F' and hl.patseq NOT IN (SELECT  patseq from smtst.PATIENT_RESTRICTION pr
                                                              JOIN CENTRAL.REF_RESTRICTION rr on pr.RESTRICTION = rr.RESTRICTION
                                                        WHERE
                                                              rr.USER_TYPE in (1, 2)  and
                                                              pr.RESTRICTION NOT IN (SELECT restriction FROM user_restriction_access where userseq = (select userseq from central.users where user_id = '{0}')  ))
             and ca.claseq is not null and ca.audit_action= 'QAREADY'
             and c.reltoclient = 'F'
             and c.status = 'U' and cqa.audit_completed ='F'
             AND EXISTS (SELECT 1 FROM smtst.PATIENT_RESTRICTION pr WHERE pr.patseq = hl.patseq AND pr.restriction = {1})";

        public const string UnreviewedClaimsResultForSpecificRestriction = @"SELECT DISTINCT  c.claseq||'-'|| c.clasub
            FROM smtst.hcicla c join smtst.hcicla_nucleus hn on c.claseq=hn.claseq 
            join smtst.hcilin hl on c.claseq=hl.claseq and c.clasub=hl.clasub
            where c.status='U' and c.reltoclient='F' and hl.patseq NOT IN (SELECT  patseq from smtst.PATIENT_RESTRICTION pr
                                                              JOIN CENTRAL.REF_RESTRICTION rr on pr.RESTRICTION = rr.RESTRICTION
                                                        WHERE 
                                                              rr.USER_TYPE in (1, 2)  and
                                                              pr.RESTRICTION NOT IN (SELECT restriction FROM user_restriction_access where userseq = (select userseq from central.users where user_id = '{0}')  )) and c.claseq||'-'||c.clasub not in (select distinct ca.claseq||'-'||ca.clasub from smtst.claim_audit ca 
            inner join smtst.claim_qa_audit cqa on ca.claseq = cqa.claseq and ca.clasub = cqa.clasub
            where ca.audit_action = 'QAREADY' and cqa.audit_completed = 'F')
            AND EXISTS (SELECT 1 FROM smtst.PATIENT_RESTRICTION pr WHERE pr.patseq = hl.patseq AND pr.restriction = {1})";

        public const string CountOfUnreviewedClaimsResultForNoRestriction = @"SELECT count (DISTINCT  c.claseq||'-'|| c.clasub)
            FROM smtst.hcicla c join smtst.hcicla_nucleus hn on c.claseq=hn.claseq 
            join smtst.hcilin hl on c.claseq=hl.claseq and c.clasub=hl.clasub
            where c.status='U' and c.reltoclient='F' and hl.patseq NOT IN (SELECT  patseq from smtst.PATIENT_RESTRICTION pr
                                                              JOIN CENTRAL.REF_RESTRICTION rr on pr.RESTRICTION = rr.RESTRICTION
                                                        WHERE 
                                                              rr.USER_TYPE in (1, 2)  and
                                                              pr.RESTRICTION NOT IN (SELECT restriction FROM user_restriction_access where userseq = (select userseq from central.users where user_id = '{0}')  )) and c.claseq||'-'||c.clasub not in (select distinct ca.claseq||'-'||ca.clasub from smtst.claim_audit ca 
            inner join smtst.claim_qa_audit cqa on ca.claseq = cqa.claseq and ca.clasub = cqa.clasub
            where ca.audit_action = 'QAREADY' and cqa.audit_completed = 'F')
            AND NOT EXISTS (SELECT 1 FROM smtst.PATIENT_RESTRICTION pr WHERE pr.patseq = hl.patseq)";

        public const string GetProductOfSystemDeletedFlagByClaseq =
            @"select product from edit where editflg in (select editflg from smtst.hciflag where claseq = {0} and deleted = 'S')";

        public const string GetCountOfSystemDeletedFlagsByEditFlgClaseqPrepep =
            @"SELECT count(*) FROM smtst.hciflag where deleted = 'S'
and editflg = '{0}' and claseq = {1} and prepep = {2}";

        public const string GetStatusRelToClientByClaseq =
            @"select status, reltoclient from smtst.hcicla where claseq in ({0})";

        public const string GetEditTypeOfSystemDeletedFlagByClaimSeq = @"select active from edit_settings where editflg = (select editflg from smtst.hciflag
where claseq = {0} and deleted = 'S') and clientcode = 'SMTST'";

        public const string GetNDCValueByClaimSeq = @"select distinct NDC from smtst.claim_line where claseq = {0} and clasub = {1}";

    }

}
