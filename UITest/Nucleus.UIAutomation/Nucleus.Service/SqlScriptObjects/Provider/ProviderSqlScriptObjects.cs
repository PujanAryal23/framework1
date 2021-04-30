using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nucleus.Service.Support.Enum;

namespace Nucleus.Service.SqlScriptObjects.Provider
{
    public static class ProviderSqlScriptObjects
    {

        public const string client = "SMTST";

        public const string ReasonCodeInNewProviderAction =
            @"select code ||  ' - ' || shortdesc  from HCIUSER.overreason  reasoncode
        where 
        product='R' and clientcode='ALL' and user_type in ('{0}','A')
       and action='L'
        order by code";


        public const string DeleteConditionActionForProviderSequenceAndDate = @"
        BEGIN
        DELETE FROM " + client +
                                                                              @".triggered_condition_action WHERE prvseq={0} and trunc(action_date) > to_date('{1}','DD-MON-YYYY');
        DELETE from " + client +
                                                                              @".note where note_type='PRV' and key1='{0}' and trunc(created) > to_date('{1}','DD-MON-YYYY');
        END;";

        public const string DeleteConditionActionForProviderSequenceForClient = @"
        BEGIN
        DELETE FROM " + client +
                                                                                @".triggered_condition_action WHERE prvseq={0} and  actioned_by =(select  userseq from central.users where user_id='{1}') and trunc(action_date) > to_date('{2}','DD-MON-YYYY');
        DELETE from " + client +
                                                                                @".note where note_type='PRV' and key1='{0}' and  created_by =(select  userseq from central.users where user_id='{1}') and trunc(created) > to_date('{2}','DD-MON-YYYY');
        END;";

        public const string TotalCountOfProviderClaimNoteForInternalUsers =
            @"SELECT count(*) FROM " + client + ".NOTE WHERE " +
            "((NOTE_TYPE='CLM' AND (KEY1,KEY2) IN (SELECT CLASEQ, CLASUB FROM " + client +
            ".HCILIN WHERE PRVSEQ = {0}))) ";

        public const string TotalCountOfProviderAndClaimNotesInternalUsers =
            @"SELECT count(*) FROM " + client + ".NOTE WHERE " +
            "((NOTE_TYPE='PRV' AND KEY1={0}) OR (NOTE_TYPE='CLM' AND (KEY1,KEY2) IN (SELECT CLASEQ, CLASUB FROM " +
            client + ".HCILIN WHERE PRVSEQ = {0}))) ";

        public const string DeleteProviderNotesOnly =
            @"delete from " + client + @".note where key1={0} 
            and created_by = (select userseq from central.users where user_id = '{1}')";

        public const string ProviderTriggeredConditions =
            @"select distinct condition_id from " + client + ".TRIGGERED_CONDITION where prvseq={0}";

        public const string UpdateTriggeredCondition =
            @"update " + client + @".triggered_condition set action = 'X', action_date = NULL where prvseq = {0}";

        public const string UpdateTriggeredConditionForClient =
            @"update " + client +
            @".triggered_condition set client_action = 'X', action_date = NULL where prvseq = {0}";

        public const string CountOfConditionNotesAssociatedToTheCondition =
            @"select count(*) from " + client + ".NOTE where key1 = {0} and note_type='PRV' and  sub_type = '{1}'";

        public const string SpecialtyList =
            @"SELECT CODE||'-'||SHORT_DESCRIPTION FROM Repository.MEDICAL_CODE_DESCRIPTION_MV where CODE_TYPE ='SPEC' order by code asc";

        public const string StateList =
            @"SELECT DESCRIPTION FROM REF_STATE order by DESCRIPTION asc";

        public static string MasterCodeByConditionId = @"SELECT DISTINCT  mc.code
        FROM suspect_provider_rules sp JOIN medical_code_description_mv mc ON mc.code >= sp.beg_proc AND mc.code <= sp.end_proc
            WHERE sp.condition_id IN ('{0}')
        AND mc.code_type IN ('HCPCS', 'CPT') AND sp.master_rule = 'T' 
        ORDER BY mc.code";

        public static string HundredMasterCodeByConditionId = @"SELECT * from (SELECT DISTINCT  mc.code
        FROM suspect_provider_rules sp JOIN medical_code_description_mv mc ON mc.code >= sp.beg_proc AND mc.code <= sp.end_proc
            WHERE sp.condition_id IN ('{0}')
        AND mc.code_type IN ('HCPCS', 'CPT') AND sp.master_rule = 'T' 
        ORDER BY mc.code) where  rownum<=100";

        public static string DeleteConditionId = "DELETE SMTST.TRIGGERED_CONDITION where prvseq={0}";


        public const string ConditionIDList =
            @"SELECT CONDITION_ID FROM SMTST.CONDITION_PARAMETERS WHERE ACTIVE_FLG = 'Y'
              order by CONDITION_ID";

        public static string AddProviderLockByProviderSeqUserId =
            "INSERT INTO " + client +
            ".PROVIDER_LOCK VALUES ({0},(select userseq from central.users where user_id='{1}'),NULL)";

        public static string DeleteProviderLockByProviderSeq =
            "DELETE SMTST.PROVIDER_LOCK WHERE PRVSEQ={0}";

        public const string SuspectProviderCount =
            //  @"select count (distinct prvseq) from " + client + ".TRIGGERED_CONDITION where action = 'S' and ACTIVE_FLG='Y'";
            @"select count(distinct prvseq) FROM " + client + ".triggered_condition tc " +
            "WHERE tc.active_flg = 'Y' AND  tc.action = 'S'" +
            "AND NOT EXISTS (SELECT 1 FROM smtst.hcibatch WHERE active != 'T' AND batchseq = tc.batchseq)";

        public const string GetSuspectProviderScoresSQL =
            @"SELECT SCORE, FFP_SCORE,PCI_SCORE, BILL_SCORE, SPECIALTY_SCORE, STATE_SCORE             
             FROM CENTRAL.PROVIDER_SCORE WHERE prvseq = {0} AND CLIENTCODE = '{1}'";

        public const string GetSuspectProviderSpecialtySQL = "";




        public const string CotivitiFlaggedProviderCount =
            @"select count (distinct prvseq) from " + client + ".TRIGGERED_CONDITION where action in  ('R','D','Q') and ACTIVE_FLG = 'Y'";

        public const string ClientFlaggedProviderCount =
            @"select count (distinct prvseq) from " + client +
            ".TRIGGERED_CONDITION where client_action in  ('R','D','Q') and ACTIVE_FLG = 'Y'";

        public const string UpdateFlagStatus = @"BEGIN
                    update SMTST.TRIGGERED_CONDITION set active_flg = '{0}' where {1} in ('R','D','Q') and 
                    triggered_date = '21-JUL-16' and condition_id='SUSC';
                    END;";


        public const string GetCotivitiFlaggedProvidersByTriggeredDateCount = "select count (distinct prvseq) from " + client + ".TRIGGERED_CONDITION where action in  ('R','D','Q') and  triggered_date = '21-JUL-16' and condition_id='SUSC'";

        public const string GetClientFlaggedProvidersByTriggeredDateCount = "select count (distinct prvseq) from " + client + ".TRIGGERED_CONDITION where client_action in ('R','D','Q') and  triggered_date = '21-JUL-16' and condition_id='SUSC'";

        public static string GetJobFlag = "select FRAUD_CTI_ENABLED from HCIUSER.HCICLIENT_NUCLEUS where clientcode='{0}'";

        public static string GetRealTimeProviderClaimQueueCount =
            "select count(*) from CENTRAL.REALTIME_PRV_QUEUE where condition_id='{1}' and prvseq={0} and clientcode='SMTST'";

        public static string DeletePRVQueueData =
            "delete from CENTRAL.REALTIME_PRV_QUEUE where condition_id='{0}' and prvseq={1} and clientcode='{2}'";

        public const string GetOpenCTICaseForProvider =
            "select OPEN_CTI_CASE from " + client + ".TRIGGERED_CONDITION where prvseq={0}";

        public const string UpdateProviderActionAndOpenCTICase =
            "update " + client + ".TRIGGERED_CONDITION set client_action='D', open_cti_case='F' where prvseq={0}";

        public const string GetPhoneNumberForUser =
            "select phone from  central.users where user_id = '{0}'";

        public const string GetProviderHistoryCount =
            @"select count(*) from " + client + ".claim_line where prvseq={0} and DOS between '{1}' and '{2}'";

        public const string TotalCountOfProviderNotes =
            @"select count(*) from " + client + ".note where key1={0}";

        public const string ClientFlaggedProviderCountByClientAction =
            @"select count (distinct prvseq) from " + client +
            ".TRIGGERED_CONDITION where client_action = '{0}' and ACTIVE_FLG='Y'";

        public const string ClientSuspectProviderCount =
            @"select count (distinct prvseq) from " + client + ".TRIGGERED_CONDITION t " +
            "where action not in ('S','N','X') and client_action='X' and ACTIVE_FLG='Y' " +
            "and  NOT EXISTS (SELECT 1 FROM smtst.hcibatch WHERE active != 'T' AND batchseq = t.batchseq)";

        public const string UpdateEdosDosForPrvSeqwithProcAndClaseqInHcilinTable =
            @"UPDATE " + client +
            ".hcilin SET dos=trunc(sysdate),edos=trunc(sysdate) WHERE prvseq={0} and proc={1} and claseq={2} ";

        public const string UpdateEdosDosIfOlderThanYearForProvSeqProcClaseq =
            @"DECLARE
            dosDate  DATE;
                BEGIN
                 SELECT dos INTO dosDate FROM smtst.hcilin 
                  WHERE prvseq={0} and proc={1} and claseq={2} ;
                  IF dosDate <= add_months( trunc(sysdate), -13 ) THEN
                    Update smtst.hcilin 
                     set dos=trunc(sysdate)
                     ,edos=trunc(sysdate)
                     where prvseq={0} and proc={1} and claseq={2} ;
                END IF;
            END;";

        public const string CheckForProviderConditionInSuspectProviderRulesTable =
            "SElECT COUNT(*) FROM HCIUSER.SUSPECT_PROVIDER_RULES where condition_id = '{0}'";

        public const string UpdateVactionCactionInTriggeredCondition =
            @"update smtst.TRIGGERED_CONDITION
            set action='{0}'
            ,CLIENT_ACTION='{1}'
            where prvseq={2} and condition_id='{3}'";

        public const string SuspectProviderList = @"select  a.prvseq , nvl(ps.score,0) score from (
        (select * from central.provider_score  where clientcode='" + client + @"' ) ps right join
        (select distinct prvseq  FROM " + client +
                                                  @".triggered_condition tc WHERE tc.active_flg = 'Y' AND  tc.action = 'S'
        AND NOT EXISTS (SELECT 1 FROM " + client + @".hcibatch WHERE active != 'T' AND batchseq = tc.batchseq)) a 
        on a.prvseq=ps.prvseq  ) order by score desc";

        public const string GetProviderExposure = "SELECT * FROM SMTST.MV_BILLsUMMARY_ALL WHERE PRVSEQ = {0}";
        public const string GetVisitAllDetail = "SELECT VISITSALL FROM SMTST.MV_BILLsUMMARY_ALL WHERE PRVSEQ = {0}";

        public const string GetProviderSpecialityAverages =
            "SELECT * FROM SMTST.MV_SPECIALTY_AVERAGES WHERE SPECIALTY = '{0}'";

        public const string GetSuspectProviderAverageForEachScore =
            @" select round(avg(nvl(FFP_SCORE,0))) as Avg_Condition_Score,
             round(avg(nvl(PCI_SCORE,0))) as Avg_RuleEngine_Score,
             round(avg(nvl(BILL_SCORE,0))) as Avg_Bill_Score,
             round(avg(nvl(SPECIALTY_SCORE,0))) as Avg_Specialty,
             round(avg(nvl(STATE_SCORE,0))) as Avg_Geographic_Score
             from smtst.provider p left join smtst.ps_provider_scores ps on p.prvseq = ps.prvseq where p.specialty = '{0}'";

        public const string GetVisibleToInNotesForPrvNote =
            "SELECT visible_to from " + client +
            @".note where note_type='PRV' and key1='{0}' and  created_by =(select  userseq from central.users where user_id='{1}') and trunc(created) > to_date('{2}','DD-MON-YYYY')";

        public const string GetFullOrFacilitynameFromFirstName = "select ps.FULL_OR_FACILITY_NAME from " + client +
                                                                 @".provider ps where FIRST_NAME='{0}' AND  prvseq in (select  a.prvseq  from ((select * from central.provider_score  where clientcode='" +
                                                                 client +
                                                                 "' order by score)ps right join(select distinct prvseq FROM " +
                                                                 client +
                                                                 @".triggered_condition tc WHERE tc.active_flg = 'Y' AND tc.action = 'S' AND NOT EXISTS(SELECT 1 FROM " +
                                                                 client +
                                                                 @".hcibatch WHERE active != 'T' AND batchseq = tc.batchseq)) a on a.prvseq=ps.prvseq  ))";



        public const string GetFullOrFacilitynameForClient= "select ps.FULL_OR_FACILITY_NAME from " + client +
                                                            @".provider ps where LAST_OR_FACILITY_NAME='{0}' AND  prvseq in (select  a.prvseq  from ((select * from central.provider_score  where clientcode='" +
                                                            client +
                                                            "' order by score)ps right join(select distinct prvseq FROM " +
                                                            client +
                                                            @".triggered_condition tc WHERE tc.active_flg = 'Y' AND tc.action IN ('Q','R','D') AND CLIENT_ACTION='X' AND NOT EXISTS(SELECT 1 FROM " +
                                                            client +
                                                            @".hcibatch WHERE active != 'T' AND batchseq = tc.batchseq)) a on a.prvseq=ps.prvseq  ))";

        public const string GetFullorFacilitynameForClientFromFirstName= "select ps.FULL_OR_FACILITY_NAME from " + client +
                                                                        @".provider ps where FIRST_NAME='{0}' AND  prvseq in (select  a.prvseq  from ((select * from central.provider_score  where clientcode='" +
                                                                        client +
                                                                        "' order by score)ps right join(select distinct prvseq FROM " +
                                                                        client +
                                                                        @".triggered_condition tc WHERE tc.active_flg = 'Y' AND tc.action IN ('Q','R','D') AND CLIENT_ACTION='X' AND NOT EXISTS(SELECT 1 FROM " +
                                                                        client +
                                                                        @".hcibatch WHERE active != 'T' AND batchseq = tc.batchseq)) a on a.prvseq=ps.prvseq  ))";



        public const string GetFullOrFacilitynameFromLastName = "select ps.FULL_OR_FACILITY_NAME from " + client +
                                                                @".provider ps where LAST_OR_FACILITY_NAME='{0}' AND  prvseq in (select  a.prvseq  from ((select * from central.provider_score  where clientcode='" +
                                                                client +
                                                                "' order by score) ps right join (select distinct prvseq FROM " +
                                                                client +
                                                                @".triggered_condition tc WHERE tc.active_flg = 'Y' AND tc.action = 'S' AND NOT EXISTS(SELECT 1 FROM " +
                                                                client +
                                                                @".hcibatch WHERE active != 'T' AND batchseq = tc.batchseq)) a on a.prvseq=ps.prvseq  ))";



        public const string GetProvidersByFirstNameAndLastName =
            @"SELECT PRVSEQ from " + client +
            @".provider where first_name like '%{0}%' and last_or_facility_name like '%{1}%'";

        public const string GetCotivitiFlaggedProvidersByFirstNameAndLastName =
            @"select distinct p.prvseq from " + client +
            @".provider p join " + client + @".TRIGGERED_CONDITION t on p.prvseq= t.prvseq
            where t.action in ('R','D','Q') and ACTIVE_FLG='Y' and p.first_name like '%{0}%' and p.last_or_facility_name like '%{1}%'";

        public const string GetClientFlaggedProvidersByFirstNameAndLastName = 
            @"select distinct p.prvseq from " + client +  @".provider p join " + client + @".TRIGGERED_CONDITION t on p.prvseq= t.prvseq
            where t.action in ('R','D','Q') and ACTIVE_FLG='Y' and p.first_name like '%{0}%' and p.last_or_facility_name like '%{1}%'";

        public const string GetSuspectProvidersByFirstNameAndLastName =
            @"select * from " + client + @".provider ps
            join (select distinct prvseq FROM " + client + @".triggered_condition tc WHERE tc.active_flg = 'Y' AND tc.action = 'S'
        AND NOT EXISTS
            (SELECT 1 FROM " + client + @".hcibatch WHERE active != 'T' AND batchseq = tc.batchseq))a
            on a.prvseq=ps.prvseq where ps.first_name like '%{0}%' and ps.last_or_facility_name like '%{1}%'";


        public const string GetBasicProviderInfo =
            @"select provnum,firstname,lastname,tin,provname from smtst.hciprov where prvseq={0}";

        public const string GetBasicProviderInformation =
            @"SELECT PRVSEQ,
            (select max(case_id) from central.fraud_cti_event_audit where clientcode = 'SMTST' and prvseq = {0}) CASE_TRACKING_ID,
            hp.SPECIALTY ||' - '|| sp.DESCRIPTION SPECIALTY,
            TIN,
            STATE,
            ZIP,
            GROUPTIN,
            GROUPNAME,
            PROVNUM
            FROM " + client + ".hciprov hp LEFT JOIN specialty sp " +
            "ON hp.specialty = sp.specialty AND expdate IS NULL where prvseq='{0}'";

        /*public const string ProviderClaimResultsFromDb = "select LINNO,CLASEQ|| '-' || clasub from " + client +
                                                   ".HCILIN WHERE PRVSEQ={0} AND DOS >='{1}' AND EDOS<'{2}' ORDER BY CLASEQ ,LINNO ASC";*/

        public const string ProviderClaimresultsFromDbDescOrder= @"select l.LINNO,l.CLASEQ|| '-' || l.CLASUB  from " + client + ".hcilin l JOIN " + client + ".hcicla c on l.claseq = c.claseq and l.clasub = c.clasub " +
                                                                 "JOIN " + client + ".hciprov p on l.prvseq = p.prvseq JOIN " + client + ".hcipat pat on l.patseq = pat.patseq " +
                                                                 " where l.prvseq = '{0}' and " +
                                                                 " l.dos between to_date('{1}', 'mm/dd/yyyy') and to_date('{2}','mm/dd/yyyy')" +
                                                                 " and l.edos between to_date('{1}', 'mm/dd/yyyy') and to_date('{2}','mm/dd/yyyy') and l.adjstatus in ('O','C') " +
                                                                 " order by l.claseq desc, l.linno DESC";

        public const string ProviderClaimResultsFromDb = @"select l.LINNO,l.CLASEQ|| '-' || l.CLASUB  from " + client + ".hcilin l JOIN " + client + ".hcicla c on l.claseq = c.claseq and l.clasub = c.clasub " +
                                                        "JOIN " + client + ".hciprov p on l.prvseq = p.prvseq JOIN " + client + ".hcipat pat on l.patseq = pat.patseq " +
                                                        " where l.prvseq = '{0}' and " +
                                                        " l.dos between to_date('{1}', 'mm/dd/yyyy') and to_date('{2}','mm/dd/yyyy')" +
                                                        " and l.edos between to_date('{1}', 'mm/dd/yyyy') and to_date('{2}','mm/dd/yyyy') and l.adjstatus in ('O','C') " +
                                                        " order by l.claseq, l.linno ASC";

        public const string Export12MonthsHistoryExcelData =
            @"SELECT l.linno line_number, concat(Concat(l.claseq,'-'), l.clasub) claim_seq, c.altclaimno claim_number, l.pos, l.prvseq, nvl(p.provname, p.groupname) provider_name,
             p.specialty, c.status claim_status, to_CHAR(l.dos,'fmMM/DD/YYYY')||' '|| to_CHAR(l.dos,'fmHH:')||to_char(l.dos,'MI:SS AM')  begin_dos, to_CHAR(l.dos,'fmMM/DD/YYYY')||' '|| to_CHAR(l.dos,'fmHH:')||to_char(l.dos,'MI:SS AM')  end_dos, l.patseq,  pat.patnum patient_number, 
             concat(Concat(pat.first_name,', '), pat.last_name) patient_name, pat.patdob patient_dob, 
             case pat.patsex 
             when 'M' then 'Male'
             when 'F' then 'Female'
             else 'Unknown'
             end Patient_gender,
            (select editflg from smtst.line_flag where deleted = 'N' and l.linseq = linseq and topflag = 'T' and edit_type = 'R') as TopFlag,
            l.proc,nvl((SELECT short_description FROM REPOSITORY.medical_code_description_mv WHERE code = l.proc AND code_type in ('DEN', 'CPT', 'HCPCS') and expdate is null and rownum < 2),'No description available.') proc_code_description,
            l.revcode, l.m1, l.m2, l.m3, l.units,
			l.dx1,decode(l.dx1,null,'',nvl((SELECT nvl(long_description,short_description )FROM REPOSITORY.medical_code_description_mv WHERE code = l.dx1 and code_type='DX'),'No description available.')) dx1desc,
            l.dx2,decode(l.dx2,null,'',nvl((SELECT nvl(long_description,short_description )FROM REPOSITORY.medical_code_description_mv WHERE code = l.dx2 and code_type='DX'),'No description available.')) dx2desc,
            l.dx3,decode(l.dx3,null,'',nvl((SELECT nvl(long_description,short_description )FROM REPOSITORY.medical_code_description_mv WHERE code = l.dx3 and code_type='DX'),'No description available.')) dx3desc,
			l.dx4,decode(l.dx4,null,'',nvl((SELECT nvl(long_description,short_description )FROM REPOSITORY.medical_code_description_mv WHERE code = l.dx4 and code_type='DX'),'No description available.')) dx4desc,
            l.dx5,decode(l.dx5,null,'',nvl((SELECT nvl(long_description,short_description )FROM REPOSITORY.medical_code_description_mv WHERE code = l.dx5 and code_type='DX'),'No description available.')) dx5desc,
            l.dx6,decode(l.dx6,null,'',nvl((SELECT nvl(long_description,short_description )FROM REPOSITORY.medical_code_description_mv WHERE code = l.dx6 and code_type='DX'),'No description available.')) dx6desc,
            l.dx7,decode(l.dx7,null,'',nvl((SELECT nvl(long_description,short_description )FROM REPOSITORY.medical_code_description_mv WHERE code = l.dx7 and code_type='DX'),'No description available.')) dx7desc,
            l.dx8,decode(l.dx8,null,'',nvl((SELECT nvl(long_description,short_description )FROM REPOSITORY.medical_code_description_mv WHERE code = l.dx8 and code_type='DX'),'No description available.')) dx8desc,
            l.dx9,decode(l.dx9,null,'',nvl((SELECT nvl(long_description,short_description )FROM REPOSITORY.medical_code_description_mv WHERE code = l.dx9 and code_type='DX'),'No description available.')) dx9desc,
			l.toothno tooth_number, toothsurf tooth_surface, oralcavity oral_cavity,
            l.billed, l.allowed, l.paid, l.paid + l.deduct adjpaid,
            (select nvl(clisugpaid, sugpaid) from smtst.line_flag where deleted = 'N' and topflag = 'T' and edit_type = 'R' and l.linseq = linseq) sugpaid,
            (select (paid + deduct - clisugpaid) from smtst.line_flag where deleted = 'N' and topflag = 'T' and edit_type = 'R' and l.linseq = linseq) savings,
            l.adjstatus, l.proctype, (Select description from central.ref_line_of_business where  central.ref_line_of_business.Line_of_business = c.Line_of_business)  as Line_Of_Business,       
            p.npi rendering_npi, p.tin,c.clientrecvddate          
             FROM smtst.hcilin l 
             JOIN smtst.hcicla c on l.claseq = c.claseq and l.clasub = c.clasub 
             JOIN smtst.hciprov p on l.prvseq = p.prvseq 
             JOIN smtst.hcipat pat on l.patseq = pat.patseq where l.prvseq = {0} and l.claseq = {1} and l.linno = {2}";

        public const string Export3yearsHistoryExcelData =
            @"SELECT l.linno line_number, concat(Concat(l.claseq,'-'), l.clasub) claim_seq, c.altclaimno claim_number, l.pos, l.prvseq, nvl(p.provname, p.groupname) provider_name,
             p.specialty, c.status claim_status, to_CHAR(l.dos,'fmMM/DD/YYYY')||' '|| to_CHAR(l.dos,'fmHH:')||to_char(l.dos,'MI:SS AM')  begin_dos, to_CHAR(l.dos,'fmMM/DD/YYYY')||' '|| to_CHAR(l.dos,'fmHH:')||to_char(l.dos,'MI:SS AM')  end_dos, l.patseq,  pat.patnum patient_number, 
             concat(Concat(pat.first_name,', '), pat.last_name) patient_name, pat.patdob patient_dob, 
             case pat.patsex 
             when 'M' then 'Male'
             when 'F' then 'Female'
             else 'Unknown'
             end Patient_gender,
            (select editflg from smtst.line_flag where deleted = 'N' and l.linseq = linseq and topflag = 'T' and edit_type = 'R') as TopFlag,
            l.proc,nvl((SELECT short_description FROM REPOSITORY.medical_code_description_mv WHERE code = l.proc AND code_type in ('DEN', 'CPT', 'HCPCS') and expdate is null and rownum < 2),'No description available.') proc_code_description,
            l.revcode, l.m1, l.m2, l.m3, l.units,
			l.dx1,decode(l.dx1,null,'',nvl((SELECT nvl(long_description,short_description )FROM REPOSITORY.medical_code_description_mv WHERE code = l.dx1 and code_type='DX'),'No description available.')) dx1desc,
            l.dx2,decode(l.dx2,null,'',nvl((SELECT nvl(long_description,short_description )FROM REPOSITORY.medical_code_description_mv WHERE code = l.dx2 and code_type='DX'),'No description available.')) dx2desc,
            l.dx3,decode(l.dx3,null,'',nvl((SELECT nvl(long_description,short_description )FROM REPOSITORY.medical_code_description_mv WHERE code = l.dx3 and code_type='DX'),'No description available.')) dx3desc,
			l.dx4,decode(l.dx4,null,'',nvl((SELECT nvl(long_description,short_description )FROM REPOSITORY.medical_code_description_mv WHERE code = l.dx4 and code_type='DX'),'No description available.')) dx4desc,
            l.dx5,decode(l.dx5,null,'',nvl((SELECT nvl(long_description,short_description )FROM REPOSITORY.medical_code_description_mv WHERE code = l.dx5 and code_type='DX'),'No description available.')) dx5desc,
            l.dx6,decode(l.dx6,null,'',nvl((SELECT nvl(long_description,short_description )FROM REPOSITORY.medical_code_description_mv WHERE code = l.dx6 and code_type='DX'),'No description available.')) dx6desc,
            l.dx7,decode(l.dx7,null,'',nvl((SELECT nvl(long_description,short_description )FROM REPOSITORY.medical_code_description_mv WHERE code = l.dx7 and code_type='DX'),'No description available.')) dx7desc,
            l.dx8,decode(l.dx8,null,'',nvl((SELECT nvl(long_description,short_description )FROM REPOSITORY.medical_code_description_mv WHERE code = l.dx8 and code_type='DX'),'No description available.')) dx8desc,
            l.dx9,decode(l.dx9,null,'',nvl((SELECT nvl(long_description,short_description )FROM REPOSITORY.medical_code_description_mv WHERE code = l.dx9 and code_type='DX'),'No description available.')) dx9desc,
			l.toothno tooth_number, toothsurf tooth_surface, oralcavity oral_cavity,
            l.billed, l.allowed, l.paid, l.paid + l.deduct adjpaid,
            (select nvl(clisugpaid, sugpaid) from smtst.line_flag where deleted = 'N' and topflag = 'T' and edit_type = 'R' and l.linseq = linseq) sugpaid,
            (select (paid + deduct - clisugpaid) from smtst.line_flag where deleted = 'N' and topflag = 'T' and edit_type = 'R' and l.linseq = linseq) savings,
            l.adjstatus, l.proctype, (Select description from central.ref_line_of_business where  central.ref_line_of_business.Line_of_business = c.Line_of_business)  as Line_Of_Business,       
            p.npi rendering_npi, p.tin,c.clientrecvddate          
             FROM smtst.hcilin l 
             JOIN smtst.hcicla c on l.claseq = c.claseq and l.clasub = c.clasub 
             JOIN smtst.hciprov p on l.prvseq = p.prvseq 
             JOIN smtst.hcipat pat on l.patseq = pat.patseq where l.prvseq = {0} and l.dos >= add_months(trunc(sysdate),-36) 
             AND nvl(l.edos,l.dos) < trunc(sysdate)";


        public const string FlagList = @"select editflg from REPOSITORY.edit where  status='A' order by editflg ";

        public const string ProviderClaimLineDetailsSecondaryDataFromDb = @"SELECT c.altclaimno claim_number, l.m1, l.m2, l.m3, l.dx1, l.dx2, l.dx3, l.dx4, l.dx5, l.dx6, l.dx7, l.dx8, l.dx9,
nvl((SELECT concat(code, concat(' - ', long_description)) FROM repository.medical_code_description_mv WHERE code = l.proc AND linno = {2} and code_type in ('DEN', 'CPT', 'HCPCS') and expdate is null and rownum < 2),'No description available.') proc_code_description,
nvl((SELECT concat(code, concat(' - ', short_description)) FROM repository.medical_code_description_mv WHERE code = l.revcode AND linno = {2} and revcodetype in ('R','U') and expdate is null and rownum < 2),'') RevCode_description,
nvl((SELECT concat('Mod 1: ', concat(code, concat(' - ', short_description))) FROM repository.medical_code_description_mv WHERE code = l.m1 AND linno = {2} AND code_type = 'MOD' AND expdate is null and rownum < 2),'') Modifier1_description, 
nvl((SELECT concat('Mod 2: ', concat(code, concat(' - ', short_description))) FROM repository.medical_code_description_mv WHERE code = l.m2 AND linno = {2} AND code_type = 'MOD' AND expdate is null and rownum < 2),'') Modifier2_description,
nvl((SELECT concat('Mod 3: ', concat(code, concat(' - ', short_description))) FROM repository.medical_code_description_mv WHERE code = l.m3 AND linno = {2} AND code_type = 'MOD' AND expdate is null and rownum < 2),'') Modifier3_description,
nvl((SELECT concat('Dx 1: ', concat(CASE WHEN (length(code) >3 and substr(code,1,1)<>'E') THEN concat(concat(substr(code,0,3),'.'),substr(code,4)) ELSE code end, concat(' - ', short_description))) FROM repository.medical_code_description_mv WHERE code = l.dx1 AND linno = {2} AND code_type = 'DX'),'No description available.') DXCode1_description,
nvl((SELECT concat('Dx 2: ', concat(CASE WHEN (length(code) >3 and substr(code,1,1)<>'E') THEN concat(concat(substr(code,0,3),'.'),substr(code,4)) ELSE code end, concat(' - ', short_description))) FROM repository.medical_code_description_mv WHERE code = l.dx2 AND linno = {2} AND code_type = 'DX'),'No description available.') DXCode2_description,
nvl((SELECT concat('Dx 3: ', concat(CASE WHEN (length(code) >3 and substr(code,1,1)<>'E') THEN concat(concat(substr(code,0,3),'.'),substr(code,4)) ELSE code end, concat(' - ', short_description))) FROM repository.medical_code_description_mv WHERE code = l.dx3 AND linno = {2} AND code_type = 'DX'),'No description available.') DXCode3_description, 
nvl((SELECT concat('Dx 4: ', concat(CASE WHEN (length(code) >3 and substr(code,1,1)<>'E') THEN concat(concat(substr(code,0,3),'.'),substr(code,4)) ELSE code end, concat(' - ', short_description))) FROM repository.medical_code_description_mv WHERE code = l.dx4 AND linno = {2} AND code_type = 'DX'),'No description available.') DXCode4_description,
nvl((SELECT concat('Dx 5: ', concat(CASE WHEN (length(code) >3 and substr(code,1,1)<>'E') THEN concat(concat(substr(code,0,3),'.'),substr(code,4)) ELSE code end, concat(' - ', short_description))) FROM repository.medical_code_description_mv WHERE code = l.dx5 AND linno = {2} AND code_type = 'DX'),'No description available.') DXCode5_description,
nvl((SELECT concat('Dx 6: ', concat(CASE WHEN (length(code) >3 and substr(code,1,1)<>'E') THEN concat(concat(substr(code,0,3),'.'),substr(code,4)) ELSE code end, concat(' - ', short_description))) FROM repository.medical_code_description_mv WHERE code = l.dx6 AND linno = {2} AND code_type = 'DX'),'No description available.') DXCode6_description,
nvl((SELECT concat('Dx 7: ', concat(CASE WHEN (length(code) >3 and substr(code,1,1)<>'E') THEN concat(concat(substr(code,0,3),'.'),substr(code,4)) ELSE code end, concat(' - ', short_description))) FROM repository.medical_code_description_mv WHERE code = l.dx7 AND linno = {2} AND code_type = 'DX'),'No description available.') DXCode7_description,
nvl((SELECT concat('Dx 8: ', concat(CASE WHEN (length(code) >3 and substr(code,1,1)<>'E') THEN concat(concat(substr(code,0,3),'.'),substr(code,4)) ELSE code end, concat(' - ', short_description))) FROM repository.medical_code_description_mv WHERE code = l.dx8 AND linno = {2} AND code_type = 'DX'),'No description available.') DXCode8_description,
nvl((SELECT concat('Dx 9: ', concat(CASE WHEN (length(code) >3 and substr(code,1,1)<>'E') THEN concat(concat(substr(code,0,3),'.'),substr(code,4)) ELSE code end, concat(' - ', short_description))) FROM repository.medical_code_description_mv WHERE code = l.dx9 AND linno = {2} AND code_type = 'DX'),'No description available.') DXCode9_description,
case l.adjstatus
WHEN 'O' THEN 'Original'
WHEN 'C' THEN 'Correction'
WHEN 'R' THEN 'Reversal'
WHEN 'V' THEN 'Voided'
WHEN 'P' THEN 'Partial Reversal'
END AS ADJSTATUS
FROM smtst.hcilin l
JOIN smtst.hcicla c on l.claseq = c.claseq and l.clasub = c.clasub
JOIN smtst.hciprov p on l.prvseq = p.prvseq
JOIN smtst.hcipat pat on l.patseq = pat.patseq where l.prvseq = {0} and l.claseq = {1} and l.linno = {2}";


        public const string GetExport12MonthsHistoryExcelFileName = @"select concat(concat(concat(Concat(nvl(p.provname, p.groupname), '_'), l.prvseq),'_'),p.specialty) file_name
             from smtst.hcilin l
             join smtst.hciprov p on l.prvseq = p.prvseq 
             where l.prvseq = {0} and l.claseq = {1} and l.linno = {2}";

        public const string Get3YearsHistoryExcelFileName = @"select concat(concat(concat(Concat(nvl(p.provname, p.groupname), '_'), l.prvseq),'_'),p.specialty) file_name
             from smtst.hcilin l
             join smtst.hciprov p on l.prvseq = p.prvseq 
             where l.prvseq = {0}";

        public const string DeleteDocumentAuditAction = @"delete from SMTST.document_action_audit where prvseq in({0}) and userseq = (select  userseq from central.users where user_id='{1}')";
        public const string SelectDocumentAuditAction = @"select * from SMTST.document_action_audit where prvseq = {0} and userseq = (select  userseq from central.users where user_id='{1}')";

        public const string GetAllClasubForProvider = @"select claseq ||'-'|| clasub from smtst.hcilin where prvseq = {0} and claseq = {1}";

        public const string GetNpiForProvider = "select npi from " + client + ".hciprov where prvseq='{0}'";

        public const string GetSearchResultForSingleProcCodeInUserSpecifiedCondition = @"select concat(concat(code,' - '),short_description) from repository.medical_code_description_mv where code = '{0}' and code_type = '{1}'";

        public const string GetExcelExport =
            " select PR.SCORE,PROVNAME, p.prvseq,provnum,Npi,TIN,SPECIALTY,STATE,GROUPNAME,GROUPTIN " +
            " FROM " + client + ".HCIPROV P  join central.provider_score PR " +
            " ON PR.PRVSEQ=P.PRVSEQ " +
            " where p.prvseq={0} and PR.clientcode='"+ client +"'";



        public const string GetExcelExportForSuspectProviders =   "SELECT nvl(ps.score,0) composite_score, " +
                                                                  "p.full_or_facility_name, " +
                                                                  "p.prvseq, " +
                                                                  "p.provider_id," +
                                                                  "(SELECT LTRIM(STRAGG (' ' || condition_id || decode(proc, null, '', '-' || proc) ) ) " +
                                                                  "FROM smtst.triggered_condition t " +
                                                                  "WHERE prvseq = p.prvseq AND active_flg = 'Y' AND action = 'S' " +
                                                                  "AND NOT EXISTS (SELECT 1 FROM smtst.hcibatch WHERE active != 'T' AND batchseq = t.batchseq)) conditions," +
                                                                  " p.npi," +
                                                                  "p.tin," +
                                                                  "NVL(p.specialty,'99') specialty,p.state, p.group_name,p.group_tin," +
                                                                  "NVL(bsa.sumpaidall, 0.0) + NVL(bsa.sumdeductall, 0.0) paid,tc.triggered_date FROM smtst.provider p " +
                                                                  "LEFT OUTER JOIN smtst.mv_billsummary_all bsa ON (bsa.prvseq = p.prvseq)" +
                                                                  "LEFT JOIN central.provider_score ps ON ps.prvseq = p.prvseq AND CLIENTCODE='SMTST' inner join (select distinct prvseq, max(triggered_date) triggered_date FROM smtst.triggered_condition tc " +
                                                                  "WHERE tc.active_flg = 'Y' AND tc.action = 'S' And NOT EXISTS (SELECT 1 FROM smtst.hcibatch WHERE active != 'T' AND batchseq = tc.batchseq) " +
                                                                  "group by prvseq) tc on tc.prvseq=p.prvseq where p.state IN (SELECT STATE FROM REF_STATE) order by composite_score desc";

        public const string GetExcelExportForSuspectProvidersClient = "SELECT nvl(ps.score,0) composite_score, " +
                                                          "p.full_or_facility_name, " +
                                                          "p.prvseq, " +
                                                          "p.provider_id," +
                                                          "(SELECT LTRIM(STRAGG (' ' || condition_id || decode(proc, null, '', '-' || proc) ) ) " +
                                                          "FROM smtst.triggered_condition t " +
                                                          "WHERE prvseq = p.prvseq AND  client_action = 'X' AND action in ('Q','R','D') " +
                                                          "AND NOT EXISTS (SELECT 1 FROM smtst.hcibatch WHERE active != 'T' AND batchseq = t.batchseq)) conditions," +
                                                          " p.npi," +
                                                          "p.tin," +
                                                          "NVL(p.specialty,'99') specialty,p.state, p.group_name,p.group_tin," +
                                                          "NVL(bsa.sumpaidall, 0.0) + NVL(bsa.sumdeductall, 0.0) paid,tc.triggered_date FROM smtst.provider p " +
                                                          "LEFT OUTER JOIN smtst.mv_billsummary_all bsa ON (bsa.prvseq = p.prvseq)" +
                                                          "LEFT JOIN central.provider_score ps ON ps.prvseq = p.prvseq AND CLIENTCODE='SMTST' inner join (select distinct prvseq, max(triggered_date) triggered_date FROM smtst.triggered_condition tc " +
                                                          "WHERE tc.active_flg = 'Y' AND tc.action in ('Q','R','D') And NOT EXISTS (SELECT 1 FROM smtst.hcibatch WHERE active != 'T' AND batchseq = tc.batchseq) " +
                                                          "group by prvseq) tc on tc.prvseq=p.prvseq where p.state IN (SELECT STATE FROM REF_STATE) order by composite_score desc";




        public const string GetExcelDownloadAudit = "select * from(select Action_source from " + client +
                                            ".document_action_audit where userseq in (select userseq from users where user_id = '{0}') and action_type='D' order by action_time desc) where rownum<=5";

        public const string GetProviderCountByuser = "select count(*) from  {0}" + 
                                                     ".provider_lock where userseq in (select userseq from users where user_id='{1}')";

        public const string GetProviderLockByUser = "select prvseq from  {0}" +
                                           ".provider_lock where userseq in (select userseq from users where user_id='{1}')";

        public const string GetEditsForUserSpecifiedCondition =
            @"select distinct editflg, edit_order from edit where user_specified_condition = 'T' order by edit_order asc";

        
        public const string GetSearchInputOfGoogle =
            @"select case when provname is null then 'undefined'
                else provname 
                end ||' '||case when (select distinct specialty_description from hciuser.providers_specialty 
                where specialty = (select specialty from smtst.hciprov where prvseq = {0})) is null 
                then 'Physician Specialty Unknown'
                else 
                (select distinct specialty_description from hciuser.providers_specialty 
                where specialty = (select specialty from smtst.hciprov where prvseq = {0}))
                end
                ||' '||state from smtst.hciprov where prvseq = {0}";
    }
}
