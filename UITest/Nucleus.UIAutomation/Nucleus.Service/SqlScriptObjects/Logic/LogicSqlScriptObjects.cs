using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nucleus.Service.Support.Enum;

namespace Nucleus.Service.SqlScriptObjects.Logic
{


    public static class LogicSqlScriptObjects
    {
        public const string client = "SMTST";

        public const string GetAssignedClientList = @"select clientcode from client_user where userseq=(select userseq from central.users where user_id='{0}') and
                                                    clientcode in (select clientcode from hciuser.hciclient_nucleus where client_uses_claim_logics = 'T') order by clientcode";
    
      

        public const string ActiveProductList =
            @"select DCI_ACTIVE,FCI_ACTIVE,FFP_ACTIVE,NEG_ACTIVE,RXI_ACTIVE,PCI_ACTIVE from client where clientcode = '{0}'";

        public const string GetSecondaryDataForLogicSearch =
            "select DISTINCT cla.claimno, DECODE(lf.product,'F','CV','D',    'DCA', 'U',    'FCI', 'R','FFP','X','RxI','N','NEG','O','OCI')PRODUCT, to_char(ls.create_date,'MM/DD/YYYY HH:MI:SS PM'),cla.reviewgroup" +
            " from " + client + ".LINE_FLAG_LOGIC_REQUEST ls join "+client+".line_flag lf on ls.linseq= lf.linseq" +
            " join " + client + ".claim cla on ls.claseq= cla.claseq" +
            " where ls.claseq= {0} and ls.clasub= {1} ";

        public const string ModifiedDateForLogic =
            " SELECT TO_CHAR(max(CREATE_DATE),'MM/DD/YYYY HH:MI:SS AM') FROM  " + client +
            ".LINE_FLAG_LOGIC_REQUEST_COMM   WHERE LOGICSEQ IN(SELECT LOGICSEQ FROM " + client +
            ".LINE_FLAG_LOGIC_REQUEST WHERE claseq={0} and CLASUB={1})" ;


        public const string GetPrimaryDataForLogicSearch =
            "select LFL_REQ.claseq||'-'||LFL_REQ.CLASUB as claseq,lF.editflg, DECODE(LFL_REQ.assigned_to,'C','Client','H','Cotiviti') ASSIGNED_TO,"+
       " DECODE(LFL_REQ.status,'C','Closed', 'O', 'Open') STATUS,"+
        " (SELECT TO_CHAR(max(CREATE_DATE),'MM/DD/YYYY HH:MI:SS AM')FROM "+client+".LINE_FLAG_LOGIC_REQUEST_COMM   WHERE LOGICSEQ = LFL_REQ.LOGICSEQ) AS MODIFIED_DATE,"+
        "  (SELECT CAST(MAX(CREATE_DATE) AS TIMESTAMP) FROM "+client+".LINE_FLAG_LOGIC_REQUEST_COMM WHERE LOGICSEQ = LFL_REQ.LOGICSEQ) AS MODDATE,"+
        " (select first_name||' '||last_name from central.users where userseq = (select created_by from " + client + ".LINE_FLAG_LOGIC_REQUEST_COMM LFLAG where LOGICSEQ = LFL_REQ.LOGICSEQ and " +
            "LOGICNOTESEQ = (select min(LOGICNOTESEQ) from " + client + ".LINE_FLAG_LOGIC_REQUEST_COMM where LFLAG.LOGICSEQ =LINE_FLAG_LOGIC_REQUEST_COMM.LOGICSEQ ))) as CREATOR_NAME " +
            "FROM " + client +".CLAIM CLA "+
           " inner join "+client+" .LINE_FLAG_LOGIC_REQUEST LFL_REQ ON CLA.CLASEQ = LFL_REQ.CLASEQ AND CLA.CLASUB = LFL_REQ.CLASUB"+
            " INNER JOIN " + client + ".LINE_FLAG LF ON LF.FLAGSEQ = LFL_REQ.FLAGSEQ"+
            " WHERE LFL_REQ.claseq= {0} and LFL_REQ.CLASUB={1} AND  LFL_REQ.create_date between '25-sep-2015' and '26-NOV-2015' and LFL_REQ.assigned_to='H' and LFL_REQ.status='O' And LF.product='F' " +
        " ORDER BY MODDATE DESC" ;

        public const string GetPrimarydataForOpenLogics=
            @"select * from(
                select to_char(LFL_REQ.claseq||'-'||LFL_REQ.CLASUB) as claseq,   (SELECT CAST(MAX(CREATE_DATE) AS TIMESTAMP) 
                FROM " + client + @".LINE_FLAG_LOGIC_REQUEST_COMM 
                WHERE LOGICSEQ = LFL_REQ.LOGICSEQ) AS MODDATE 
                FROM  " + client + @".LINE_FLAG_LOGIC_REQUEST LFL_REQ 
                WHERE   LFL_REQ.status='O' and  LFL_REQ.ASSIGNED_TO= '{0}' 
                union all
                select to_char(LFL_REQ.preauthseq) as claseq,   (SELECT CAST(MAX(CREATE_DATE) AS TIMESTAMP) 
                FROM " + client + @".preauth_logic_note 
                WHERE LOGICSEQ = LFL_REQ.LOGICSEQ) AS MODDATE 
                FROM  " + client + @" .PREAUTH_LOGIC LFL_REQ 
                WHERE   LFL_REQ.status='O' and  LFL_REQ.ASSIGNED_TO= '{0}') order by MODDATE desc";


        public const string GetprimarydataForPciOpenLogic =
            "select LFL_REQ.claseq||'-'||LFL_REQ.CLASUB as claseq, " +
            "  (SELECT CAST(MAX(CREATE_DATE) AS TIMESTAMP) FROM " + client + ".LINE_FLAG_LOGIC_REQUEST_COMM WHERE LOGICSEQ = LFL_REQ.LOGICSEQ) AS MODDATE" +
            " FROM " + client + ".CLAIM CLA " +
            " inner join " + client + " .LINE_FLAG_LOGIC_REQUEST LFL_REQ ON CLA.CLASEQ = LFL_REQ.CLASEQ AND CLA.CLASUB = LFL_REQ.CLASUB" +
            " INNER JOIN " + client + ".LINE_FLAG LF ON LF.FLAGSEQ = LFL_REQ.FLAGSEQ" +
            " WHERE   LFL_REQ.status='O' and  LFL_REQ.ASSIGNED_TO= '{0}' AND LF.product='F' " +
            " ORDER BY MODDATE DESC";

        public const string GetprimarydataForDciOpenLogic =
            @"select * from(
                select to_char(LFL_REQ.claseq||'-'||LFL_REQ.CLASUB) as claseq,   (SELECT CAST(MAX(CREATE_DATE) AS TIMESTAMP) 
                FROM " + client + @".LINE_FLAG_LOGIC_REQUEST_COMM 
                WHERE LOGICSEQ = LFL_REQ.LOGICSEQ) AS MODDATE 
                FROM  " + client + @".LINE_FLAG_LOGIC_REQUEST LFL_REQ join " + client +
            @".hciflag hf on hf.flagseq=lfl_req.flagseq
                WHERE   LFL_REQ.status='O' and  LFL_REQ.ASSIGNED_TO= '{0}' and hf.edittype='D'
                union all
                select to_char(LFL_REQ.preauthseq) as claseq,   (SELECT CAST(MAX(CREATE_DATE) AS TIMESTAMP) 
                FROM " + client + @".preauth_logic_note 
                WHERE LOGICSEQ = LFL_REQ.LOGICSEQ) AS MODDATE 
                FROM  " + client + @" .PREAUTH_LOGIC LFL_REQ 
                WHERE   LFL_REQ.status='O' and  LFL_REQ.ASSIGNED_TO= '{0}') order by MODDATE desc";

        public const string GetprimarydataForFfpOpenLogic =
            "select LFL_REQ.claseq||'-'||LFL_REQ.CLASUB as claseq, " +
            "  (SELECT CAST(MAX(CREATE_DATE) AS TIMESTAMP) FROM " + client + ".LINE_FLAG_LOGIC_REQUEST_COMM WHERE LOGICSEQ = LFL_REQ.LOGICSEQ) AS MODDATE" +
            " FROM " + client + ".CLAIM CLA " +
            " inner join " + client + " .LINE_FLAG_LOGIC_REQUEST LFL_REQ ON CLA.CLASEQ = LFL_REQ.CLASEQ AND CLA.CLASUB = LFL_REQ.CLASUB" +
            " INNER JOIN " + client + ".LINE_FLAG LF ON LF.FLAGSEQ = LFL_REQ.FLAGSEQ" +
            " WHERE   LFL_REQ.status='O' and  LFL_REQ.ASSIGNED_TO= '{0}' AND LF.product='R' " +
            " ORDER BY MODDATE DESC";

        public const string deleteclaimLockFromDatabase =
            "delete  from SMTST.LOCKED_CLAIMS where claseq={0} and clasub={1}";

        public const string LogicResultOrderedInAscending =
            "select LFL_REQ.claseq||'-'||LFL_REQ.CLASUB as claseq FROM "
            +client+ ".CLAIM CLA " +
            " inner join " + client +
            " .LINE_FLAG_LOGIC_REQUEST LFL_REQ ON CLA.CLASEQ = LFL_REQ.CLASEQ AND CLA.CLASUB = LFL_REQ.CLASUB" +
            " INNER JOIN " + client + ".LINE_FLAG LF ON LF.FLAGSEQ = LFL_REQ.FLAGSEQ" +
            " WHERE   LFL_REQ.create_date BETWEEN '14-OCT-2011' AND '14-DEC-2011' and LFL_REQ.ASSIGNED_TO='{0}' " +
            " ORDER BY LFL_REQ.claseq,LFL_REQ.clasub ";

        public const string LogicResultOrderedInDecscending =
            "select LFL_REQ.claseq||'-'||LFL_REQ.CLASUB as claseq FROM " 
          +client+ ".CLAIM CLA " +
            " inner join " + client +
            " .LINE_FLAG_LOGIC_REQUEST LFL_REQ ON CLA.CLASEQ = LFL_REQ.CLASEQ AND CLA.CLASUB = LFL_REQ.CLASUB " +
            " INNER JOIN " + client + ".LINE_FLAG LF ON LF.FLAGSEQ = LFL_REQ.FLAGSEQ " +
            " WHERE   LFL_REQ.create_date BETWEEN '14-OCT-2011' AND '14-DEC-2011' and LFL_REQ.ASSIGNED_TO='{0}'" +
            " ORDER BY LFL_REQ.claseq DESC,LFL_REQ.clasub desc";


        /// <summary>
        /// delete logic in claim 
        /// </summary>
        public const string DeleteClaimLogicInFlagOfLogicManager = @"DELETE FROM " + client + @".hcilogic where claseq = {0}  and clasub ={1}";
        public const string GetExportlogicData= "select to_char(LFL_REQ.claseq||'-'||LFL_REQ.CLASUB)claseq,lF.editflg flag,DECODE(LF.PRODUCT,'F','Coding Validation','R','FraudFinder Pro','D','Dental Claim Accuracy') PRODUCT ,DECODE(LFL_REQ.assigned_to,'C','Client','H','Cotiviti') ASSIGNED_TO," +
         "DECODE(LFL_REQ.status,'C','Closed', 'O', 'Open') STATUS, "+
       " TO_CHAR(LFL_REQ.CREATE_DATE,'MM/DD/YYYY HH:MI:SS AM') AS create_date,"+
        " LFL_REQ.CREATE_DATE AS createdate" +
       " FROM SMTST.CLAIM CLA"+
         "  inner join SMTST.LINE_FLAG_LOGIC_REQUEST LFL_REQ ON CLA.CLASEQ = LFL_REQ.CLASEQ AND CLA.CLASUB = LFL_REQ.CLASUB "+
          "  INNER JOIN   SMTST.LINE_FLAG LF ON LF.FLAGSEQ = LFL_REQ.FLAGSEQ "+
                                                 " WHERE LFL_REQ.assigned_to= '{0}' and LFL_REQ.status= 'O'" +
                                                "UNION ALL"+
                                                 " select to_char(PL.PREAUTHSEQ) as claseq, F.editflg flag,'Dental Claim Accuracy' PRODUCT , DECODE(PL.assigned_to,'C','Client','H','Cotiviti') ASSIGNED_TO," +
                                                 "  DECODE(PL.status,'C','Closed', 'O', 'Open') STATUS, "+
                                                 " TO_CHAR(PL.CREATE_DATE, 'MM/DD/YYYY HH:MI:SS AM') AS create_date, "+
                                                 " PL.CREATE_DATE AS createdate "+
                                                 " FROM SMTST.DENTPREAUTH P "+
                                                 " INNER JOIN SMTST.PREAUTH_LOGIC PL ON P.PREAUTHSEQ = PL.PREAUTHSEQ "+
                                                 " INNER JOIN SMTST.DENTPREAUTHFLAG F ON F.DPA_FLAGSEQ = PL.FLAGSEQ "+
                                                 " LEFT JOIN SMTST.LOCKED_PREAUTHS LP ON P.PREAUTHSEQ = LP.PREAUTHSEQ "+
                                                 " WHERE PL.assigned_to= '{0}' and PL.status= 'O'"+
                                                 " order by createdate DESC, flag Asc";
        
    }
}
