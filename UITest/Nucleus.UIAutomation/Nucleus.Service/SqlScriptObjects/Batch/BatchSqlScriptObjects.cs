using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nucleus.Service.SqlScriptObjects.Batch
{
    public class BatchSqlScriptObjects
    {
        public const string client = "SMTST";

        public const string GetFirstFiftyActiveBatches = @"select batchid from  (
            select batchid from " + client +
                                                         ".HCIBATCH where active = 'T' order by TO_DATE(BATCHDATE,'DD-MON-YYYY')desc ) where rownum <=50"
            ;

        public const string ActiveProductList =
            @"select PCI_ACTIVE,FFP_ACTIVE,FCI_ACTIVE,DCI_ACTIVE,COB_ACTIVE from client where clientcode = " + "'" + client + "'" +
            " ";

        public const string TotalClaimsInBatch =
            // @"select initflgclm from " + client + ".hcibatch WHERE BATCHID = '{0}'";
            @"select count(1) from " + client +
            ".hcicla c where c.CLEARED='F' and (c.claseq, c.clasub) in (select f.claseq, f.clasub from " + client +
            ".line_flag f where f.DELETED = 'N'  AND f.EDIT_TYPE = 'R') and c.batchseq=(select batchseq from " + client +
            ".hcibatch where batchid='{0}')";

        public const string ReleaseUserReleaseDate =
            @"SELECT RELEASEUSER || ' ' || TO_CHAR(releasedate,'MM/DD/YYYY HH:MI AM') FROM  
            (SELECT batchseq, releasedate, releaseuser, rank() OVER (PARTITION BY batchseq ORDER BY releasedate DESC) rnk
            FROM " + client + ".hcibatchhistory)hbh WHERE rnk=1 and BATCHSEQ IN " +
            "(SELECT BATCHSEQ FROM " + client + ".HCIBATCH WHERE BATCHID = '{0}')";


        public const string ProductWiseUnreviewedClaims =
            @"SELECT COUNT (DISTINCT f.claseq || f.clasub)
               
                FROM smtst.hcicla c  INNER JOIN smtst.line_Flag f ON (f.claseq = c.claseq AND f.clasub = c.clasub)
              WHERE c.batchseq IN (SELECT BATCHSEQ FROM smtst.HCIBATCH WHERE BATCHID = '{0}') AND
                  f.product = '{1}' AND
                  f.edit_type = 'R' AND
                  f.deleted = 'N' AND
                  c.reltoclient =  'F' AND
                  f.hcidone = 'F' AND
                  c.status IN ('U','P')";

        public const string ProductWiseUnreviewedClaimsByClient =
            //@"select CLIENT_UNREVIEWED_CLAIMS_PCI,CLIENT_UNREVIEWED_CLAIMS_FFP,CLIENT_UNREVIEWED_CLAIMS_FCI,CLIENT_UNREVIEWED_CLAIMS_DCI,CLIENT_UNREVIEWED_CLAIMS_COB
            //from " + client + ".BATCH_STATISTICS  WHERE BATCHSEQ = (SELECT BATCHSEQ FROM " + client +
            //".HCIBATCH WHERE BATCHID = '{0}')";
            @"SELECT COUNT (DISTINCT f.claseq || f.clasub)                  
            FROM smtst.hcicla c  INNER JOIN smtst.line_Flag f ON (f.claseq = c.claseq AND f.clasub = c.clasub)
            WHERE c.batchseq IN (SELECT BATCHSEQ FROM smtst.HCIBATCH WHERE BATCHID = '{0}') AND
            f.product = '{1}' AND
            f.edit_type = 'R' AND
            f.deleted = 'N' AND
            c.reltoclient =  'T' AND
            f.clidone = 'F' AND
            c.status IN ('U','P') AND
            c.cleared = 'F'
            AND f.edit_type = 'R'";


        public const string UnreviewedClaims =
            //@"select HCI_UNREVIEWED_CLAIMS_TOTAL from " + client +
            //".BATCH_STATISTICS  WHERE BATCHSEQ IN (SELECT BATCHSEQ FROM " + client + ".HCIBATCH WHERE BATCHID = '{0}')";
            @"SELECT COUNT (unique c.claseq || c.clasub) FROM " + client + ".hcicla c WHERE c.batchseq IN (SELECT BATCHSEQ FROM " + client + ".HCIBATCH WHERE BATCHID = '{0}') AND c.reltoclient = 'F' AND c.status = 'U'";

        public const string UnreviewedClaimsByClient =
            //@"select CLIENT_UNREVIEWED_CLAIMS_TOTAL from " + client +
            //".BATCH_STATISTICS  WHERE BATCHSEQ IN (SELECT BATCHSEQ FROM " + client + ".HCIBATCH WHERE BATCHID = '{0}')";
            @"SELECT COUNT (unique c.claseq || c.clasub) FROM " + client + ".hcicla c WHERE c.batchseq IN (SELECT BATCHSEQ FROM " + client + ".HCIBATCH WHERE BATCHID = '{0}') AND c.reltoclient = 'T' AND c.status = 'U'";

        public const string BatchCompleteDate =
            @"SELECT to_CHAR(BATCH_COMPLETE_DATE,'MM/DD/YYYY HH:MI AM') FROM " + client +
            ".BATCH WHERE batch_id = '{0}'";

        public const string BatchDate = @"SELECT to_CHAR(BATCH_DATE,'MM/DD/YYYY') FROM " + client +
                                        ".BATCH WHERE batch_id = '{0}'";

        public const string CotivitiANDClientCreateDate =
            @"SELECT to_CHAR(createdate,'MM/DD/YYYY') as CotivitiCreatedate, to_CHAR(batchdate,'MM/DD/YYYY') as ClientCreateDate 
            FROM " + client + ".HCIBATCH  where batchid = '{0}'";

        public const string BatchesReceivedThisWeek =
            @"SELECT batchid FROM " + client +
            ".HCIBATCH WHERE CREATEDATE<SYSDATE AND CREATEDATE>SYSDATE-7 AND ACTIVE='T' order by BATCHDATE desc";

        public const string IncompleteBatches =

            //@"SELECT b.batchid FROM " + client + @".hcibatch b LEFT JOIN(SELECT hc.batchseq, 
            //count(reltoclient) total_reltoclient
            //FROM " + client + @".hcicla hc
            //WHERE reltoclient = 'F'
            //GROUP BY hc.batchseq) incomplete on incomplete.batchseq = b.batchseq
            //LEFT JOIN(SELECT batchseq,
            //HCI_UNREVIEWED_CLAIMS_TOTAL total_unreviewed
            //FROM " + client + @".batch_statistics)stats ON b.batchseq = stats.batchseq
            //WHERE active = 'T'
            //AND total_reltoclient > 0
            //AND total_unreviewed > 0 order by b.BATCHDATE desc";
            @"select batchid from " + client + ".hcibatch  where batchseq in (select distinct batchseq from " + client +
            ".hcicla where reltoclient='F') and active='T' order by batchdate desc";

        public const string BatchInformationInQ1 =

            @"SELECT TO_CHAR(BATCHDATE,'MM/DD/YYYY'), TO_CHAR(HN.BATCH_COMPLETE_DATE,'MM/DD/YYYY HH:MI:SS AM')  HCIREVIEWDATE,TO_CHAR(CREATEDATE,'MM/DD/YYYY HH:MI:SS AM') RECEIVED_BY_VERSCEND,ORIGFILE AS FTP_FILE_RECEIVED,
            TO_CHAR(STARTTIME,'MM/DD/YYYY HH:MI:SS AM') PROCESSING_STARTED,TO_CHAR(ENDTIME,'MM/DD/YYYY HH:MI:SS AM') PROCESSING_COMPLETED,           
            ORIGCLMS RECEIVED_CLAIMS,ORIGLINS RECEIVED_LINES,  NVL(ORIGPAID,0) RECEIVED_PAID,PROCCLMS ANALYZED_CLAIMS,  PROCLINS ANALYZED_LINES,NVL(PROCPAID,0) ANALYZED_PAID 
            FROM " + client + ".HCIBATCH B JOIN " + client +
            ".HCIBATCH_NUCLEUS HN ON B.BATCHSEQ=HN.BATCHSEQ WHERE BATCHID='{0}'";

        public const string BatchReleaseUserDateTimeList =
            @"select releaseuser,TO_CHAR(releasedate,'MM/DD/YYYY HH:MI:SS AM') from " + client + ".HCIBATCHHISTORY hb join " + client + ".hcibatch h on hb.batchseq=h.batchseq " +
            "where h.BATCHID='{0}' order by hb.releasedate desc";

        public const string ReturnFiles =
            @" SELECT (batch_id || segment) returnfile, return_date as returndate, rec_count returncount
                FROM " + client +
            ".return_file WHERE batchseq = {0} UNION SELECT 'Not Included in Return Files' returnfile, null returndate, count(*) returncoun FROM " +
            client + ".claim  WHERE dateretrn is null and batchseq = {0}";

        public const string BatchReleaseRevert =
            @" BEGIN
                    update " + client + @".hcicla set status = 'U', reltoclient = 'F' where claseq in ({0},{1},{2},{3}) and clasub = 0;
                update  " + client + @".hciflag set hcidone = 'F', clidone = 'F', hcisugdate = null, clisugdate = null 
                    where claseq in ({0},{1},{2},{3}) and clasub = 0  ;
                update  " + client + @".batch set pci_review_complete = null where batchseq in ({4});
                update  " + client + @".hcibatch_nucleus set oci_review_complete= null, rxi_review_complete= null,  batch_complete_date=null
                    where batchseq in ({4});
                 " + client + @".CompleteReview.UpdateBatchStatisticsProcess({4});                
                END; ";

        public const string PCIDataValues =
            @"SELECT BS.HCI_ACCEPTED_CLAIMS_PCI,BS.HCI_UNREVIEWED_CLAIMS_PCI,BS.RELEASE_TO_CLIENT_CLAIMS_PCI,BS.HCI_ACCEPTED_LINES_PCI,BS.HCI_UNREVIEWED_LINES_PCI,
            BS.RELEASE_TO_CLIENT_LINES_PCI,BS.CLIENT_ACCEPTED_CLAIMS_PCI,BS.CLIENT_UNREVIEWED_CLAIMS_PCI,BS.CLIENT_ACCEPTED_LINES_PCI,
            BS.CLIENT_UNREVIEWED_LINES_PCI, FI_INITFLGCLM,FI_INITFLGLIN from " + client + ".HCIBATCH B JOIN " + client + ".BATCH_STATISTICS BS ON B.BATCHSEQ=BS.BATCHSEQ WHERE BATCHID='{0}'";

        public const string FCIDataValues =
            @"SELECT BS.HCI_ACCEPTED_CLAIMS_FCI,BS.HCI_UNREVIEWED_CLAIMS_FCI,BS.RELEASE_TO_CLIENT_CLAIMS_FCI,BS.HCI_ACCEPTED_LINES_FCI,BS.HCI_UNREVIEWED_LINES_FCI,
            BS.RELEASE_TO_CLIENT_LINES_FCI,BS.CLIENT_ACCEPTED_CLAIMS_FCI,BS.CLIENT_UNREVIEWED_CLAIMS_FCI,BS.CLIENT_ACCEPTED_LINES_FCI,
            BS.CLIENT_UNREVIEWED_LINES_FCI,FAC_INITFLGCLM,FAC_INITFLGLIN from " + client + ".HCIBATCH B JOIN " + client + ".BATCH_STATISTICS BS ON B.BATCHSEQ=BS.BATCHSEQ WHERE BATCHID='{0}'";

        public const string DCIDataValues =
            @"SELECT BS.HCI_ACCEPTED_CLAIMS_DCI,BS.HCI_UNREVIEWED_CLAIMS_DCI,BS.RELEASE_TO_CLIENT_CLAIMS_DCI,BS.HCI_ACCEPTED_LINES_DCI,BS.HCI_UNREVIEWED_LINES_DCI,
            BS.RELEASE_TO_CLIENT_LINES_DCI,BS.CLIENT_ACCEPTED_CLAIMS_DCI,BS.CLIENT_UNREVIEWED_CLAIMS_DCI,BS.CLIENT_ACCEPTED_LINES_DCI,
            BS.CLIENT_UNREVIEWED_LINES_DCI,DENT_INITFLGCLM,DENT_INITFLGLIN from " + client + ".HCIBATCH B JOIN " + client + ".BATCH_STATISTICS BS ON B.BATCHSEQ=BS.BATCHSEQ WHERE BATCHID='{0}'";

        public const string COBDataValues =
            @"SELECT BS.HCI_ACCEPTED_CLAIMS_COB,BS.HCI_UNREVIEWED_CLAIMS_COB,BS.RELEASE_TO_CLIENT_CLAIMS_COB,BS.HCI_ACCEPTED_LINES_COB,BS.HCI_UNREVIEWED_LINES_COB,
            BS.RELEASE_TO_CLIENT_LINES_COB,BS.CLIENT_ACCEPTED_CLAIMS_COB,BS.CLIENT_UNREVIEWED_CLAIMS_COB,BS.CLIENT_ACCEPTED_LINES_COB,
            BS.CLIENT_UNREVIEWED_LINES_COB,COB_INITFLGCLM,COB_INITFLGLIN from " + client + ".HCIBATCH B JOIN " + client + ".BATCH_STATISTICS BS ON B.BATCHSEQ=BS.BATCHSEQ WHERE BATCHID='{0}'";

        public const string FFPDataValues =
            @"SELECT BS.HCI_ACCEPTED_CLAIMS_FFP,BS.HCI_UNREVIEWED_CLAIMS_FFP,BS.RELEASE_TO_CLIENT_CLAIMS_FFP,BS.HCI_ACCEPTED_LINES_FFP,BS.HCI_UNREVIEWED_LINES_FFP,
            BS.RELEASE_TO_CLIENT_LINES_FFP,BS.CLIENT_ACCEPTED_CLAIMS_FFP,BS.CLIENT_UNREVIEWED_CLAIMS_FFP,BS.CLIENT_ACCEPTED_LINES_FFP,
            BS.CLIENT_UNREVIEWED_LINES_FFP,FRD_INITFLGCLM,FRD_INITFLGLIN from " + client + ".HCIBATCH B JOIN " + client + ".BATCH_STATISTICS BS ON B.BATCHSEQ=BS.BATCHSEQ WHERE BATCHID='{0}'";

        public const string PCIDollarValues =
            @"SELECT BS.HCI_ACCEPTED_PAID_PCI, BS.HCI_UNREVIEWED_PAID_PCI,BS.RELEASE_TO_CLIENT_PAID_PCI,BS.HCI_ACCEPTED_SAVINGS_PCI,BS.HCI_UNREVIEWED_SAVINGS_PCI,
            BS.RELEASE_TO_CLIENT_SAVINGS_PCI,BS.CLIENT_ACCEPTED_PAID_PCI,BS.CLIENT_UNREVIEWED_PAID_PCI,BS.CLIENT_ACCEPTED_SAVINGS_PCI,
            BS.CLIENT_UNREVIEWED_SAVINGS_PCI,FI_INITFLGPD,FI_INITFLGSAV FROM " + client + ".HCIBATCH B JOIN " + client + ".BATCH_STATISTICS BS ON B.BATCHSEQ=BS.BATCHSEQ WHERE BATCHID='{0}'";


        public const string FCIDollarValues =
            @"SELECT  BS.HCI_ACCEPTED_PAID_FCI, BS.HCI_UNREVIEWED_PAID_FCI,BS.RELEASE_TO_CLIENT_PAID_FCI,BS.HCI_ACCEPTED_SAVINGS_FCI,BS.HCI_UNREVIEWED_SAVINGS_FCI,
            BS.RELEASE_TO_CLIENT_SAVINGS_FCI,BS.CLIENT_ACCEPTED_PAID_FCI,BS.CLIENT_UNREVIEWED_PAID_FCI,BS.CLIENT_ACCEPTED_SAVINGS_FCI,
            BS.CLIENT_UNREVIEWED_SAVINGS_FCI,FAC_INITFLGPD,FAC_INITFLGSAV FROM " + client + ".HCIBATCH B JOIN " + client + ".BATCH_STATISTICS BS ON B.BATCHSEQ=BS.BATCHSEQ WHERE BATCHID='{0}'";

        public const string DCIDollarValues =
            @"SELECT  BS.HCI_ACCEPTED_PAID_DCI, BS.HCI_UNREVIEWED_PAID_DCI,BS.RELEASE_TO_CLIENT_PAID_DCI,BS.HCI_ACCEPTED_SAVINGS_DCI,BS.HCI_UNREVIEWED_SAVINGS_DCI,
            BS.RELEASE_TO_CLIENT_SAVINGS_DCI,BS.CLIENT_ACCEPTED_PAID_DCI,BS.CLIENT_UNREVIEWED_PAID_DCI,BS.CLIENT_ACCEPTED_SAVINGS_DCI,
            BS.CLIENT_UNREVIEWED_SAVINGS_DCI,DENT_INITFLGPD,DENT_INITFLGSAV FROM " + client + ".HCIBATCH B JOIN " + client + ".BATCH_STATISTICS BS ON B.BATCHSEQ=BS.BATCHSEQ WHERE BATCHID='{0}'";

        public const string COBDollarValues =
            @"SELECT  BS.HCI_ACCEPTED_PAID_COB, BS.HCI_UNREVIEWED_PAID_COB,BS.RELEASE_TO_CLIENT_PAID_COB,BS.HCI_ACCEPTED_SAVINGS_COB,BS.HCI_UNREVIEWED_SAVINGS_COB,
            BS.RELEASE_TO_CLIENT_SAVINGS_COB,BS.CLIENT_ACCEPTED_PAID_COB,BS.CLIENT_UNREVIEWED_PAID_COB,BS.CLIENT_ACCEPTED_SAVINGS_COB,
            BS.CLIENT_UNREVIEWED_SAVINGS_COB,COB_INITFLGPD,COB_INITFLGSAV FROM " + client + ".HCIBATCH B JOIN " + client + ".BATCH_STATISTICS BS ON B.BATCHSEQ=BS.BATCHSEQ WHERE BATCHID='{0}'";

        public const string FFPDollarValues =
            @"SELECT  BS.HCI_ACCEPTED_PAID_FFP, BS.HCI_UNREVIEWED_PAID_FFP,BS.RELEASE_TO_CLIENT_PAID_FFP,BS.HCI_ACCEPTED_SAVINGS_FFP,BS.HCI_UNREVIEWED_SAVINGS_FFP,
            BS.RELEASE_TO_CLIENT_SAVINGS_FFP,BS.CLIENT_ACCEPTED_PAID_FFP,BS.CLIENT_UNREVIEWED_PAID_FFP,BS.CLIENT_ACCEPTED_SAVINGS_FFP,
            BS.CLIENT_UNREVIEWED_SAVINGS_FFP,FRD_INITFLGPD,FRD_INITFLGSAV FROM " + client + ".HCIBATCH B JOIN " + client + ".BATCH_STATISTICS BS ON B.BATCHSEQ=BS.BATCHSEQ WHERE BATCHID='{0}'";

        public const string NewProductList =
                @"select decode(PCI_ACTIVE,'T','CV','-1'),decode(FFP_ACTIVE,'T','FFP','-1'),decode(FCI_ACTIVE,'T','FCI','-1'),decode(DCI_ACTIVE,'T','DCA','-1'),decode(COB_ACTIVE,'T','COB','-1') from client where clientcode='" + client + "'"
            ;

        public const string RELTOCLIENT =
            @"SELECT distinct(reltocLIENT) FROM SMTST.HCICLA WHERE BATCHSEQ = (SELECT BATCHSEQ FROM SMTST.HCIBATCH WHERE BATCHID = '{0}')";

    }
}
