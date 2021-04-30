using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Utils;
using UIAutomation.Framework.Utils;

namespace Nucleus.Service.SqlScriptObjects.Pre_Auth
{
    class PreAuthSQLScriptObjects
    {
        public const string client = "SMTST";

        #region "PreAuth Creator"

        public const string GetPatientInfoFromDBByMemberID =
            @"select patnum, first_name, last_name, patseq, TO_CHAR(patdob, 'MM/DD/YYYY') as patdob from " + client +
            ".hcipat where patnum = '{0}'";

        public const string GetProviderInfoFromDBByProvNum =
            @"select provnum, provname, TIN, prvseq from " + client + ".hciprov where prvseq = '{0}'";

        #endregion

        #region PreAuth Search

        public const string DeleteLockByAuthSeq = @"delete from " + client + ".locked_preauth where preauthseq = {0}";

        public const string PreAuthLockCountByUser =
            "select count(*) from {0}.locked_preauth where userseq in(select userseq from users where user_id='{1}')";


        public const string PreauthLock = "select preauthseq from {0}.locked_preauth where userseq in(select userseq from users where user_id='{1}')";

        #endregion

        public const string DeleteFlagAndAuditByAuthSeqFlagsLinNo = @"begin
        delete from smtst.dentpreauthflag where preauthseq={0} and editflg in ({1}) and linno={2};
        delete from smtst.DENTPREAUTHOVER where preauthseq={0} and editflg in ({1}) and linno={2};
        end;";

        public const string DeleteFlagAuditByAuthSeqFlagsLinNo =
            @"delete from smtst.DENTPREAUTHOVER where preauthseq={0} and editflg in ({1}) and linno={2} and overdate>sysdate-2 ";

        public const string DeleteFlagAuditHistory =
            "delete from " + client + ".dentpreauthover where preauthSeq = '{0}' and action<> 'A'";

        public const string AuthSequenceByPrvSeqInDescendingOrder =
            @"SELECT PREAUTHSEQ FROM SMTST.DENTPREAUTH WHERE PRVSEQ={0} ORDER BY PREAUTHSEQ DESC";

        public const string AuthSequenceByPrvSeqInAscendingOrder =
            @"SELECT PREAUTHSEQ FROM SMTST.DENTPREAUTH WHERE PRVSEQ={0} ORDER BY PREAUTHSEQ";

        public const string PreAuthActionUpperLeftQuadrantValues =
            @"select dentpreauth.preauthseq, decode(dentpreauth.professional_review, 'T', 'Professional', 'Standard') review_type,concat(concat(dentpreauth.patfirst,' '),dentpreauth.patlast) patient_name,
            dentpreauth.patseq, TO_CHAR(dentpreauth.patdob, 'MM/DD/YYYY'),dentpreauth.prvseq, dentpreauth.prvname, 
            dentpreauth.tin, prov.state, dentpreauth.groupid, dentpreauth.plan_id, dentpreauth.patnum, 
            dentpreauth.preauthid, dentpreauth.docrefnum
            from " + client + ".DENTPREAUTH  dentpreauth " +
            "join " + client + ".hciprov  prov on dentpreauth.prvseq = prov.prvseq where dentpreauth.preauthseq = {0}";

        public const string ProvStateValue =
            @"SELECT STATE FROM " + client + ".HCIPROV WHERE PROVNUM = '{0}'";

        public const string PreAuthSearchValues =
            //    @"SELECT PREAUTHSEQ, PATSEQ, PRVSEQ, CREATEDATE, STATUS, PREAUTHID, (PATLAST || ', ' ||PATFIRST) AS PATNAME, PRVNAME, PROVNUM FROM " + client + ".DENTPREAUTH WHERE PREAUTHID = {0} ORDER BY PREAUTHSEQ DESC";
            @"
            SELECT PREAUTHSEQ, 
            (CASE
                 WHEN PROFESSIONAL_REVIEW = 'T' THEN 'Professional'
                 WHEN PROFESSIONAL_REVIEW = 'F' THEN 'Standard' 
                 END) AS REVIEW_TYPE,
            PATSEQ, PRVSEQ, CREATEDATE, 
            (CASE
	            WHEN STATUS = 'N' THEN 'New'
                WHEN STATUS = 'U' THEN 'Cotiviti Unreviewed'
	            WHEN STATUS = 'B' THEN 'Documents Required'
	            WHEN STATUS = 'R' THEN 'Documents Requested'
	            WHEN STATUS = 'C' THEN 'Closed'
	            WHEN STATUS = 'V' THEN 'Client Unreviewed'
	            WHEN STATUS = 'W' THEN 'Cotiviti Consultant Required'
	            WHEN STATUS = 'K' THEN 'Cotiviti Consultant Complete'
	            WHEN STATUS = 'A' THEN 'State Consultant Required'
	            WHEN STATUS = 'S' THEN 'State Consultant Complete'
	            WHEN STATUS = 'D' THEN 'Document Review'
            END) AS Status
            , PREAUTHID, 
            (PATLAST || ', ' ||PATFIRST) AS PATNAME, PRVNAME, PROVNUM FROM " + client + ".DENTPREAUTH WHERE PREAUTHID = '{0}' AND trunc(CREATEDATE) between trunc(to_date('{1}','mm/dd/yyyy')) AND trunc(to_date('{2}','mm/dd/yyyy')) ORDER BY PREAUTHSEQ DESC";

        public const string FlagOrderSequenceList =
            @"select editflg from edit where editflg in ({0}) order by edit_order";

        public const string AllFlag =
            @"select editflg from smtst.dentpreauthflag where preauthseq={0}  and linno={1} order by orderno";

        public const string ClientFlag =
            @"select editflg from smtst.dentpreauthflag where preauthseq={0}  and deleted not in ('S', 'H') and linno={1} order by orderno";

        public const string PreAuthLineValues =
            @"SELECT LINNO, DOS, PROC, BILLED, PAID, (CASE WHEN SCENARIO = 'X' THEN  'All' ELSE SCENARIO END)AS SCENARIO, TOOTHNO, TOOTHSURF, ORALCAVITY FROM " +
            client + ".DENTPREAUTHLIN WHERE PREAUTHSEQ = {0} order by linno";

        public const string PreAuthProcCodeDescription =
            @"SELECT LONG_DESCRIPTION FROM REPOSITORY.MEDICAL_CODE_DESCRIPTION_MV WHERE CODE IN ('{0}') and CODE_TYPE='{1}' and EXPDATE is  null";

        public const string PreAuthShortProcCodeDescription =
            @"SELECT short_description FROM REPOSITORY.MEDICAL_CODE_DESCRIPTION_MV WHERE CODE IN ('{0}') and CODE_TYPE='{1}' and EXPDATE is  null";

        public const string PreAuthReasonCodeListForInternalUser =
            @"select trim(code || ' - ' || shortdesc) reasoncode  from HCIUSER.overreason where type = 'O' and clientcode in ('" +
            client + "', 'ALL', 'HCI') " +
            "and user_type in ('A', 'H') and product = 'D' order by reasoncode";

        public const string PreAuthReasonCodeListForClientUser =
            @"select trim(code || ' - ' || shortdesc) reasoncode  from HCIUSER.overreason where type = 'O' and clientcode in ('" +
            client + "', 'ALL') " +
            "and user_type in ('A', 'C') and product = 'D' order by reasoncode";


        public const string HciDoneAndCliDoneValuesByAuthSeqLinNoFlagName =
            @"SELECT hcidone, clidone FROM smtst.dentpreauthflag WHERE preauthseq = {0} AND linno = {1} AND editflg = '{2}'";

        public const string UnDeleteAnyFlagByPreAuthSeq =
            @"UPDATE smtst.dentpreauthflag SET deleted = 'N' WHERE preauthseq = {0}";

        public const string OutstandingPreAuthsCountForInternalUser = @"SELECT * FROM " + client + ".dentpreauth d " +
                                                                      "JOIN " + client +
                                                                      ".hcipat pat ON d.patseq = pat.patseq " +
                                                                      "JOIN " + client +
                                                                      ".hciprov prv ON d.prvseq = prv.prvseq " +
                                                                      "WHERE status in ('K', 'D', 'U', 'S')";

        public const string ConsultantReviewPreAuthsCountForInternalUser =
            @"SELECT * FROM " + client + ".dentpreauth d " +
            "JOIN " + client + ".hcipat pat ON d.patseq = pat.patseq " +
            "JOIN " + client + ".hciprov prv ON d.prvseq = prv.prvseq " +
            "WHERE status in ('A','W')";

        public const string DocumentsNeededPreAuthsCountForClientUser = @"SELECT * FROM " + client + ".dentpreauth d " +
                                                                        "JOIN " + client +
                                                                        ".hcipat pat ON d.patseq = pat.patseq " +
                                                                        "JOIN " + client +
                                                                        ".hciprov prv ON d.prvseq = prv.prvseq " +
                                                                        "WHERE status in ('B','R')";

        public const string PreAuthCreatedAudit =
            @"select notes from " + client + ".dentpreauthhist where preauthseq = {0} and status = 'N'";

        public const string CountOfDentalProfReview =
            @"SELECT COUNT(*) FROM HCIUSER.DENTALPROFREVIEW WHERE CODE='{0}' AND APPLY='Y' AND CLIENTCODE='" + client +
            "'";

        public const string FlagListByHcidone =
            @"select editflg from smtst.dentpreauthflag where preauthseq={0} and hcidone='{1}'";

        public const string FlagListByClientdone =
            @"select editflg from smtst.dentpreauthflag where preauthseq={0} and clidone='{1}'";


        public const string PreAuthProcessingHistoryListByAuthSeq =
            @"SELECT TO_CHAR(MODDATE,'MM/DD/YYYY HH:MI:SS PM') , DECODE(MODUSER,'SYSTEM','System',(SELECT FIRST_NAME || ' ' || LAST_NAME FROM CENTRAL.USERS WHERE USER_ID=MODUSER)) AS MODUSERNAME,STATUS,NOTES FROM " +
            client + ".DENTPREAUTHHIST WHERE PREAUTHSEQ={0} ORDER BY MODDATE DESC";



        public const string DeleteHistoryAndUpdateStatus = @"
        begin
        delete from SMTST.DENTPREAUTHHIST where PREAUTHSEQ={0} and  trunc(moddate)>trunc(to_date('{1}','dd-Mon-yy'));
        update smtst.dentpreauth set status = '{2}' where preauthseq={0};
        
        end;";



        public const string UpdateStatusOfPreAuthSeq =
            @"update " + client + ".dentpreauth set status = '{0}' where preauthseq in ({1})";

        public const string GetHciDoneStatusOfPreAuthSeq =
            @"select hcidone from " + client + ".dentpreauthflag where preauthseq in ({0})";

        public const string GetCliDoneStatusOfPreAuthSeq =
            @"select clidone from " + client + ".dentpreauthflag where preauthseq in ({0})";

        public const string UpdateHciDoneOrClientDoneToFalse =
            @"update " + client + ".dentpreauthflag set {0} = 'F' where preauthseq in ({1})";

        public const string GetPatientPreAuthHistoryData =
            @"SELECT p.linno Linno, to_char(l.valid_begdate, 'MM/DD/YYYY') BegDate, nvl(h.specialty, 'null') Specialty, nvl(p.scenario, 'null') Scenario, 
            nvl(p.toothno, '') TN, nvl(p.toothsurf, '') TS, nvl(p.oralcavity, '') OC, nvl(p.proc, 'null') Proc,
            nvl((SELECT short_description FROM repository.medical_code_description_mv WHERE code = p.proc AND code_type in ('DEN') and expdate is null and rownum < 2),'') proc_code_description,
            '$'||to_char(c.billed,'FM999990.00') Billed,'$'|| to_char(c.paid-c.sugpaid,'FM999990.00') Allowed,'$'|| to_char(c.paid,'FM999990.00') Paid,
            '$'||to_char(c.sugpaid,'FM999990.00') SugPaid,'$'|| to_char(c.savings,'FM999990.00') Savings, nvl(c.editflg, 'null') Flag
            FROM " + client + ".dentpreauth l JOIN " + client + ".dentpreauthlin p on l.preauthseq = p.preauthseq JOIN "
            + client + ".dentpreauthflag c  on l.preauthseq = c.preauthseq and p.linno = c.linno JOIN "
            + client + ".hciprov h on l.prvname = h.provname where l.preauthseq = '{0}'";


        public const string GetPatientPreAuthHistoryTotalData =
            @"SELECT 
l.prvname PrvName, l.preauthseq PreAuthSeq,
(case l.status
WHEN 'B' THEN 'Documents Requested'
WHEN 'C' THEN 'Closed'
WHEN 'D' THEN 'Document Review'
WHEN 'K' THEN 'Cotiviti Consultant Complete'
WHEN 'N' THEN 'New'
WHEN 'R' THEN 'Documents Requested'
WHEN 'S' THEN 'State Consultant Complete'
WHEN 'U' THEN 'Cotiviti Unreviewed'
WHEN 'V' THEN 'Client Unreviewed'
WHEN 'W' THEN 'Cotiviti Consultant Required'
END) AS Status,
h.prvseq ProvSeq,  h.tin TIN, l.patseq PatSeq,
nvl((SELECT long_description FROM repository.medical_code_description_mv WHERE code = p.proc AND code_type in ('DEN') and expdate is null and rownum < 2),'') proc_code_long_description
FROM " + client + ".dentpreauth l JOIN " + client + ".dentpreauthlin p on l.preauthseq = p.preauthseq JOIN "
            + client + ".dentpreauthflag c  on l.preauthseq = c.preauthseq and p.linno = c.linno JOIN "
            + client + ".hciprov h on l.prvname = h.provname where l.preauthseq = '{0}'";

        public const string GetSystemDateFromDatabase = "select to_char(sysdate,'MM/DD/YYYY HH:MI:SS AM') from dual";

        public const string DeleteLogic = @"delete from smtst.preauth_logic where preauthseq = {0}";

        public const string DeletePreauth = @"delete from smtst.dentpreauth where preauthseq = {0}";

        public const string DeletePreAuthDocumentRecord =
             @"begin
            delete from " + client + ".preauthdocs where preauthseq={0};" +
            "delete from " + client + ".documents where docseq in (select docseq from  " + client + ".preauthdocs where preauthseq={0});" +
            "delete from " + client + ".document_action_audit where docseq in (select docseq from " + client + ".preauthdocs where preauthseq={0});" +
            "delete from " + client + ".dentpreauthhist where preauthseq = {0} and trunc(moddate)>=trunc(to_date('{1}','dd-Mon-yyyy'));" +
            "end;";

        public const string GetToothRecordsByToothNumber = @"select to_char(p.dos, 'MM/DD/YYYY') DOS, nvl(p.proc, 'null') Proc,
nvl((SELECT short_description FROM repository.medical_code_description_mv WHERE code = p.proc AND code_type in ('DEN') and expdate is null and rownum < 2),'') proc_code_description, p.toothno TN,
p.toothsurf TS, p.oralcavity OC, '$'|| to_char(p.allowed,'FM999990.00') Allowed,(SELECT editflg FROM smtst.line_flag WHERE deleted = 'N' and topflag = 'T' AND p.linseq  = linseq) flag
from smtst.hcilin p where p.patseq = {0} and p.toothno = {1} order by flag desc";

        public const string PreAuthAuditDocumentUploaded = "select * from " + client + ".dentpreauthhist where preauthseq={0} and NOTES='Document Uploaded'";

        public const string PreAuthAuditDocumentDownload =
            @"select * from " + client + ".DOCUMENT_ACTION_AUDIT where docseq in " +
            "(select docseq from " + client + ".preauthdocs where preauthseq = {0}) and action_type = 'D'";

        public const string PreAuthAuditDocumentDeleted =
           "select * from " + client + ".dentpreauthhist where preauthseq={0} and NOTES='Document Deleted'";


        public const string GetDeletedDocumentsStatusFromDB =
            "select deleted from smtst.documents where docseq in (select docseq from  SMTST.preauthdocs where preauthseq={0})";

        public const string GetDocumentUploadValuesFromDb = @"select docname,to_char(docdate,'MM/DD/YYYY fmHH:MI:SS PM'),docdesc from " + client + ".documents where docseq in (select docseq from " + client + ".preauthdocs where preauthseq = '{0}') and deleted = 'F' order by docdate desc,docname desc";

        public const string PreAuthCountByPatSequence = @"select count(distinct(l.preauthseq)) from smtst.dentpreauth d join smtst.dentpreauthlin l on d.preauthseq = l.preauthseq where d.patseq = {0}";



        public const string GetNoteDetails =
            "select DECODE(note_type,'PAH','Pre-Auth'), (SELECT FIRST_NAME||' '||LAST_NAME FROM USERS WHERE USERSEQ= N.created_by) CREATEDBY,  " +
            " TO_CHAR(last_updated,'MM/DD/YYYY' )last_updated from SMTST.NOTE N  WHERE KEY1 = {0} and note_type='PAH' order by created desc";


        public const string deletePreAuthNotesFromDB = "delete from SMTST.NOTE WHERE KEY1={0} and note_type='PAH'";

        public const string DeleteOrRestorePreAuthFlag = @"begin
        update smtst.dentpreauthflag set deleted = '{0}', clidone = '{1}', hcidone = '{1}' where dpa_flagseq={2};
        end;";

        public const string GetTriggerProcCodeFromDb = @"select proc from smtst.dentpreauthlin where preauthseq = {0} and
            linno = (select triglinno from smtst.dentpreauthflag where preauthseq = {0} and linno = {1} and editflg = '{2}')";

        public const string DeleteToothInfoFromDB = @"update smtst.dentpreauthlin set TOOTHNO=NULL,TOOTHSURF=NULL,ORALCAVITY=NULL WHERE preauthseq={0} AND LINNO=1";
        public const string GetToothInfoForPreauthFromDB = @"select TOOTHNO, TOOTHSURF,ORALCAVITY from smtst.dentpreauthlin where preauthseq={0}";


    }
}



