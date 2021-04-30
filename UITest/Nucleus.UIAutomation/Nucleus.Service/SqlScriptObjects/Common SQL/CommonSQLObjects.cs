using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms.VisualStyles;
using UIAutomation.Framework.Database;
using Nucleus.Service.SqlScriptObjects.QA;
using Nucleus.Service.Support.Common.Constants;
using Nucleus.Service.Support.Enum;
using UIAutomation.Framework.Utils;
using static System.String;

namespace Nucleus.Service.SqlScriptObjects.Common_SQL
{
    public class CommonSQLObjects
    {
        
        public const string client = "SMTST";
        #region CONSTRUCTOR

        public CommonSQLObjects(IOracleStatementExecutor executor)
        {
            _executor = executor;
        }
        #endregion

        #region PRIVATE FIELDS
        private readonly IOracleStatementExecutor _executor;
        #endregion

        #region SQLStatements

        public const string RemoveLockFromQaClaim = @"update CENTRAL.qa_review_claim set lock_user = null, lock_date = null  where claseq = {0}";

        public const string GetAppealTypeByClaseq = @"select appeal_type from ats.appeal a
            join 
            ats.appeal_line al
            on 
            al.appealseq = a.appealseq
            where al.claseq = '{0}' and al.clasub = '{1}'";

        public const string UpdateDCIAppealDayType = @"update client set APPEAL_DAY_TYPE ='{0}' where clientcode='SMTST'";

        public const string GetSystemDateFromDatabase = "select to_char(sysdate,'MM/DD/YYYY HH:MI:SS AM') from dual";

        private const string GetAssignedClientList = @"select clientcode from client_user where userseq=(select  userseq from central.users 
                                                    where user_id='{0}') order by clientcode";

        public const string WhiteListedIp =
            @"select CIDR from Central.WHITELISTED_IP_CIDR where clientcode='" + client + "'";

        private const string FlagList = "select editflg from REPOSITORY.edit where status='A' order by editflg";

        private const string SpecialtyList = "select SPECIALTY ||' - '|| description spec from HCIUSER.SPECIALTY order by SPECIALTY";

        private const string ActiveProductList =
            @"select PCI_ACTIVE,COB_ACTIVE,DCI_ACTIVE,FCI_ACTIVE,FFP_ACTIVE,NEG_ACTIVE,OCI_ACTIVE,RXI_ACTIVE from client where clientcode = '{0}'";
        //@"select DCI_ACTIVE,FCI_ACTIVE,FFP_ACTIVE,NEG_ACTIVE,OCI_ACTIVE,RXI_ACTIVE,PCI_ACTIVE,COB_ACTIVE from client where clientcode = '{0}'";

        private const string AllProductValueList =
            @"select DCI_ACTIVE,FCI_ACTIVE,FFP_ACTIVE,NEG_ACTIVE,OCI_ACTIVE,PCI_ACTIVE,RXI_ACTIVE,COB_ACTIVE from client where clientcode = '{0}'";

        private const string AppealCategoryIDList =
            @"select distinct category from CENTRAL.APPEAL_CATEGORY_ASSIGNMENT where deleted ='F' order by category";

        private const string GetAllAssignedCategoriesByUserId =
            @"select distinct(category) from appeal_category_assignment
               where REGEXP_LIKE(assigned_analysts , '{0}([^\d]|$)') and deleted = 'F'
               order by category";

        private const string AppealCategoryOrderList =
            @"select distinct cat_order from CENTRAL.APPEAL_CATEGORY_ASSIGNMENT where deleted ='F' order by cat_order";

        private const string AllProductList =
            @"select description_text,upper(abbreviation) from central.ref_products order by DESCRIPTION_TEXT";

        private const string DeleteAllLockedClaimsByClient = @"delete from {0}.locked_claims";

        private const string AllowedClaimByClaimSequenceAndUserId = @"select count(*) from " + client + ".patient_restriction  pr "+
        " join ref_restriction rr on pr.restriction=rr.RESTRICTION and user_type=1"+
        " where  patseq = (select distinct patseq from " + client + ".hcilin where claseq={0} and clasub={1}) and pr.restriction in ("+
        " select restriction from central.user_restriction_access where userseq=(select userseq from central.users where user_id = '{2}'))";

        private const string AllowedClaimNotAllUserTypeByClaimSequenceAndUserId = @"select count(*) from " + client + ".patient_restriction  pr " +
                                                                                  " join ref_restriction rr on pr.restriction=rr.RESTRICTION and user_type<>1 " +
                                                                                  " where  patseq = (select distinct patseq from " + client + ".hcilin where claseq={0} and clasub={1}) and pr.restriction in (" +
                                                                                  " select restriction from central.user_restriction_access where userseq=(select userseq from central.users where user_id = '{2}'))";

        private const string AllowedClaimForAssignedAccessForUserIdAndClaimSequenceExcludeAll =
            @"select count(*) from " + client + ".patient_restriction  pr " +
            "join ref_restriction rr on pr.restriction=rr.RESTRICTION and user_type<>1 " +
            "where  patseq  = (select distinct patseq from " + client + ".hcilin where claseq={0} and clasub={1}) and pr.restriction in (" +
            "select ra.restriction from central.user_restriction_access ra " +
            "join ref_restriction rr on ra.restriction = rr.restriction " +
            "where rr.description = '{3}' and userseq=(select userseq from central.users where user_id = '{2}'))";


        private const string AllowedClaimForAssignedAccessForUserIdAndClaimSequenceIncludeAll =
            @"select count(*) from " + client + ".patient_restriction  pr " +
            " join ref_restriction rr on pr.restriction=rr.RESTRICTION  " +
            " where  patseq  = (select distinct patseq from " + client + ".hcilin where claseq={0} and clasub={1}) and pr.restriction in (" +
            " select ra.restriction from central.user_restriction_access ra " +
            " join ref_restriction rr on ra.restriction = rr.restriction " +
            " where rr.description = '{3}' and userseq=(select userseq from central.users where user_id = '{2}'))";

        private const string SetCoderReviewValueForClaimSeqeunce =
            @"UPDATE " + client + ".hcicla SET CODER_REVIEW = '{1}' where claseq = {0}";

        public const string GetCLientLogoFromDB = @"select LOGO from client where clientcode='{0}'";

        private const string UpdateClaimSeqeunceForCoderWorklistPopulation =
            @"BEGIN
            UPDATE " + client + ".hciflag set hcidone = 'F', hcisugdate = null, clisugdate = null, clidone = 'F' where deleted = 'N' and claseq = {0};" +
            "UPDATE " + client + ".hcicla c set status = 'U', reltoclient= 'F' ,coder_review = 'T' where claseq = {0};" +
            "END";

        private const string UpdateClaimStatus = "update "+client+".hcicla set status = '{0}' where claseq = {1}";


        private const string GetRestrictionAppliedToClaim =
            @"SELECT Description from " + client + ".patient_restriction pr join ref_restriction rr on pr.restriction=rr.RESTRICTION  where patseq = (select distinct patseq from SMTST.hcilin where claseq={0}) and user_type in (1,{1})";


        private const string GetUserTypeListByAppealSequence =
            @"select user_type from ATS.APPEAL_RESTRICTION ar join CENTRAL.REF_RESTRICTION rr
              on ar.restriction=rr.restriction
              where appealseq in ({0})";
        private const string GetRestrictionIDAndUserTypeForSelectedAppealSeqSQL =
            @"select rr.restriction,rr.user_type from
(select * from ats.appeal_restriction where appealseq={0}) ar join central.ref_restriction
rr on ar.restriction=rr.restriction";


        private const string GetPrvNameForGivenClaimNo =
            @"SELECT DISTINCT NVL(PROVNAME, GROUPNAME) PROVIDER_FULL_OR_FACILITY_NAME 
                FROM " + client + ".hcilin hc " +
            "join " + client + ".hciprov p on p.prvseq = hc.prvseq " +
            "join  " + client + ".hcicla c on c.claseq = hc.claseq " +
            "WHERE c.claimno = '{0}'";


        private const string GetPrvNameForGivenClaimSeq =
            @"SELECT DISTINCT NVL(PROVNAME, GROUPNAME) PROVIDER_FULL_OR_FACILITY_NAME 
                FROM " + client + ".hcilin hc " +
            "join " + client + ".hciprov p on p.prvseq = hc.prvseq " +
            "join  " + client + ".hcicla c on c.claseq = hc.claseq " +
            "WHERE c.claseq = {0}";


        private const string InsertLockForClaim = @"BEGIN
        Delete from  " + client + ".LOCKED_CLAIMS where claseq={0} and clasub={1};"+@"
        Insert into " + client + ".LOCKED_CLAIMS values ({0},{1},(select userseq from central.users where user_id='{2}'),sysdate);" +
                                                  "END;";


        private const string StateIro =
            @"select HAS_IRO,PHRASE,CARE_OF,DEPARTMENT_NAME,STREET || SUITE,CITY || ', ' || STATE || ' ' || ZIP,'Phone: ' || PHONE,'Toll-Free: ' || TOLL_FREE_PHONE,'Fax: ' || FAX,EMAIL,WEBSITE_1,WEBSITE_2
         from repository.state_iro
         where STATE_CODE='{0}'";

        private const string UpdateStateOfProvider = @"update smtst.hciprov 
         set state='{0}'
         where prvseq={1}";

        private const string UpdateStateIRO = @"update repository.state_iro 
         set has_iro='{0}'
         where STATE_CODE='{1}'";

        private const string DCIFlag =
            "select editflg from repository.edit where product='D' and status='A' order by editflg";

        private const string GetAppealSettings= @"  
                            select  CLIENT_PROCESS_APPEALS,
                            HIDE_APPEAL_FUNCTIONS,
                            ALLOW_BILLABLE_APPEALS,
                            CLIENT_CAN_APPEAL_CORE_FLAGS,
                            DISABLE_RECORD_REVIEWS,
                            ENABLE_MEDICAL_RECORD_REVIEWS,
                            DISPLAY_EXTERNAL_DOC_ID,
                            ALLOW_APPEALS_EXT_ID_SETTING,
                            AUTO_GENERATE_APPEAL_EMAIL,
                            ATS_CC,
                            CLOSE_CLIENT_APPEALS from hciuser.client where clientcode='{0}'";

        private const string GetClientSettingsByColumn = @"select {0} from hciuser.client where clientcode='{1}'";

        private const string GetAppealDueDatesValue= @"  
                                                        select
                                                            DCI_RECORD_REVIEW_TAT,
                                                            'NA',
                                                            DCI_URGENT_APPEAL_TAT,
                                                            DCI_APPEAL_TAT,
                                                            
                                                            FCI_RECORD_REVIEW_TAT,
                                                            FCI_MEDICAL_RECORD_REVIEW_TAT,
                                                            FCI_URGENT_APPEAL_TAT,
                                                            FCI_APPEAL_TAT,
                                                            
                                                            FFP_RECORD_REVIEW_TAT,
                                                            FFP_MEDICAL_RECORD_REVIEW_TAT,
                                                            FFP_URGENT_APPEAL_TAT,
                                                            FFP_APPEAL_TAT,
                                                            
                                                            NEG_RECORD_REVIEW_TAT,
                                                            'NA',
                                                            NEG_URGENT_APPEAL_TAT,
                                                            NEG_APPEAL_TAT,
                                                            
                                                            OCI_RECORD_REVIEW_TAT,
                                                            'NA',
                                                            OCI_URGENT_APPEAL_TAT,
                                                            OCI_APPEAL_TAT,
                                                            
                                                            PCI_RECORD_REVIEW_TAT,
                                                            PCI_MEDICAL_RECORD_REVIEW_TAT,
                                                            PCI_URGENT_APPEAL_TAT,
                                                            PCI_APPEAL_TAT,
                                                            
                                                            RXI_RECORD_REVIEW_TAT,
                                                            'NA',
                                                            RXI_URGENT_APPEAL_TAT,
                                                            RXI_APPEAL_TAT,
                                                            
                                                            COB_RECORD_REVIEW_TAT,
                                                            COB_MEDICAL_RECORD_REVIEW_TAT,
                                                            COB_URGENT_APPEAL_TAT,
                                                            COB_APPEAL_TAT
                                                        from
                                                            HCIUSER.client
                                                        where
                                                            CLIENTCODE = '{0}'";

        private const string GetAppealDueDatesForProducts = @"  
                                                        select
                                                            {0}
                                                        from
                                                            HCIUSER.client
                                                        where
                                                            CLIENTCODE = '{1}'";

        public const string ActiveOrDisableSingleProductByClient = @"update client set {0}_ACTIVE='{2}' where clientcode='{1}'";

        private const string RevertProductStatus = @"update hciuser.client set
DCI_ACTIVE='{0}',
FCI_ACTIVE='{1}',
FFP_ACTIVE='{2}',
NEG_ACTIVE='{3}',
OCI_ACTIVE='{4}',
PCI_ACTIVE='{5}',
RXI_ACTIVE='{6}'
where clientcode='{7}'";
        private const string RevertAppealData = @"
                                                update hciuser.client set

                                                CLIENT_PROCESS_APPEALS='{0}', 
                                                HIDE_APPEAL_FUNCTIONS='{1}', 
                                                ALLOW_BILLABLE_APPEALS='{2}', 
                                                CLIENT_CAN_APPEAL_CORE_FLAGS='{3}', 
                                                DISABLE_RECORD_REVIEWS = '{4}',
                                                ENABLE_MEDICAL_RECORD_REVIEWS = '{5}', 
                                                DISPLAY_EXTERNAL_DOC_ID='{6}',
                                                ALLOW_APPEALS_EXT_ID_SETTING='{7}',
                                                AUTO_GENERATE_APPEAL_EMAIL='{8}',
                                                ATS_CC='{9}',
                                                CLOSE_CLIENT_APPEALS='{10}',

                                                DCI_RECORD_REVIEW_TAT='{11}',
                                                DCI_URGENT_APPEAL_TAT='{12}',
                                                DCI_APPEAL_TAT='{13}',

                                                FCI_RECORD_REVIEW_TAT='{14}',
                                                FCI_MEDICAL_RECORD_REVIEW_TAT = '{15}',
                                                FCI_URGENT_APPEAL_TAT='{16}',
                                                FCI_APPEAL_TAT='{17}',


                                                FFP_RECORD_REVIEW_TAT='{18}',
                                                FFP_MEDICAL_RECORD_REVIEW_TAT = '{19}',
                                                FFP_URGENT_APPEAL_TAT='{20}',
                                                FFP_APPEAL_TAT='{21}',


                                                NEG_RECORD_REVIEW_TAT='{22}',
                                                NEG_URGENT_APPEAL_TAT='{23}',
                                                NEG_APPEAL_TAT='{24}',

                                                OCI_RECORD_REVIEW_TAT='{25}',
                                                OCI_URGENT_APPEAL_TAT='{26}',
                                                OCI_APPEAL_TAT='{27}',

                                                PCI_RECORD_REVIEW_TAT='{28}',
                                                PCI_MEDICAL_RECORD_REVIEW_TAT = '{29}',
                                                PCI_URGENT_APPEAL_TAT='{30}',
                                                PCI_APPEAL_TAT='{31}',


                                                COB_RECORD_REVIEW_TAT='{32}',
                                                COB_MEDICAL_RECORD_REVIEW_TAT = '{33}',
                                                COB_URGENT_APPEAL_TAT='{34}',
                                                COB_APPEAL_TAT='{35}'


                                                where clientcode='{36}'
";

        private const string UpdateSpecificClientData = @"update hciuser.client set {0} where clientcode='{1}'";

        private const string RevertConfigSettings = @"update hciuser.client
                                                            set 
                                                            HCI_CREATE_RETURN_FILE = '{0}',
                                                            CLIENT_CREATE_RETURN_FILE = '{1}',
                                                            ACCEPTS_MULTI_RETURN_FILES = '{2}',
                                                            SEPARATE_FRAUD_REVIEW = '{3}',
                                                            CTI_JOB_FLAG = '{4}',
                                                            PROVIDER_FLAGGING_ENABLED = '{5}',
                                                            PROVIDER_FLAGGING_TITLE = '{6}',
                                                            PAID_EXPOSURE_THRESHOLD = {7},
                                                            PROVIDER_SCORE_THRESHOLD = {8},
                                                            ALTERNATE_CLAIM_NUMBER_TITLE = '{9}',
                                                            CLIENT_USES_CLAIM_LOGICS = '{10}',
                                                            VIEW_RATIONALE = '{11}',
                                                            MODIFY_AUTO_REVIEWED_FLAGS = '{12}',
                                                            ALLOW_CLIENT_USER_QUICK_DELETE = '{13}',
                                                            CAN_MODIFY_CORE_FLAGS = '{14}',
                                                            ALLOW_ADD_SWITCH = '{15}',
                                                            ALLOW_SWITCH_REVERSE_EDITS = '{16}',
                                                            ALLOW_ADD_SWITCH_EDITS_APPEALS = '{17}',
                                                            INV_ONLY_CLIENT_REV_EDITS = '{18}',
                                                            INVOICE_CORE_EDITS = '{19}'
                                                        where clientcode = '{20}'";

        public const string GetConfigSettings = @"select 
                                                    HCI_CREATE_RETURN_FILE, 
                                                    CLIENT_CREATE_RETURN_FILE,
                                                    ACCEPTS_MULTI_RETURN_FILES,
                                                    SEPARATE_FRAUD_REVIEW,
                                                    CTI_JOB_FLAG  ScoutCaseTracker,
                                                    PROVIDER_FLAGGING_ENABLED,
                                                    PROVIDER_FLAGGING_TITLE,
                                                    PAID_EXPOSURE_THRESHOLD,
                                                    PROVIDER_SCORE_THRESHOLD,
                                                    ALTERNATE_CLAIM_NUMBER_TITLE,
                                                    CLIENT_USES_CLAIM_LOGICS,
                                                    VIEW_RATIONALE,
                                                    MODIFY_AUTO_REVIEWED_FLAGS,
                                                    ALLOW_CLIENT_USER_QUICK_DELETE,
                                                    CAN_MODIFY_CORE_FLAGS,
                                                    ALLOW_ADD_SWITCH,
                                                    ALLOW_SWITCH_REVERSE_EDITS,
                                                    ALLOW_ADD_SWITCH_EDITS_APPEALS,
                                                    INV_ONLY_CLIENT_REV_EDITS,
                                                    INVOICE_CORE_EDITS
                                                from hciuser.client 
                                                where clientcode = '{0}'";

        public const string LastModifiedUsernameAndDate = @"select u.first_name ||  ' ' || u.last_name Username,  to_char(c.last_updated , 'mm/dd/yyyy')
                                                            from hciuser.client c join central.users u on c.last_updated_by = u.userseq
                                                            where c.clientcode = '{0}'";

        public const string SetPasswordDurationInSecurityTab = @"update hciuser.client set PASSWORD_CHANGE_FREQUENCY = {0} 
                                                                 where clientcode = '{1}'";

        public const string RevertCanModifyCoreFlags = @"update hciuser.client set can_modify_core_flags = 'F' where clientcode = '{0}'";

        public const string GetUserSequenceForCurrentlyLoggedInUser = @"select userseq from central.users where user_id = '{0}'";

        public const string GetBatchCompleteStatusForProductsSQL = @"SELECT 
                                                                                    {0}
                                                                                FROM HCIUSER.CLIENT WHERE CLIENTCODE = '{1}'";

        public const string UpdateRevertBatchCompleteStatusSQL = @"UPDATE HCIUSER.CLIENT 
                                                                    SET
                                                                    DCI_BATCH_COMPLETE_STATUS = NULL,
                                                                    FCI_BATCH_COMPLETE_STATUS = NULL,
                                                                    FFP_BATCH_COMPLETE_STATUS = NULL,
                                                                    NEG_BATCH_COMPLETE_STATUS = NULL,
                                                                    OCI_BATCH_COMPLETE_STATUS = NULL,
                                                                    PCI_BATCH_COMPLETE_STATUS = NULL,
                                                                    RXI_BATCH_COMPLETE_STATUS = NULL,
                                                                    COB_BATCH_COMPLETE_STATUS = NULL
                                                                    WHERE CLIENTCODE = '{0}'";

        public const string UpdateHciDoneOrClientDoneToFalse = @"update {0}.hciflag set {1} = 'F' where claseq in ({2}) and editflg in ({3})";

        public const string LineOfBusinessList =
            @"SELECT description from central.ref_line_of_business order by description";

        public const string GetClientCodeFromDatabaseByClientStatus = "select clientcode from client where active = '{0}' order by clientcode";

        public const string UpdateProcessingTypeOfClientSQL = @"update client set processing_type='{0}' where clientcode = '{1}'";

        public const string GetSpecificClientData = @"select {0} from client where clientcode ='{1}'";

        public const string DeletePTOByUserId = @"delete from central.user_pto
              WHERE userseq = (select userseq from users where user_id = '{0}')";

        public const string GetClaimStatusFromDb = @"select status from {1}.hcicla where claseq = {0}";

        public const string GetReleaseToClientStatusFromDb = @"select reltoclient from {1}.hcicla where claseq = {0}";

        public const string GetAssignedRestrictionDescriptionsForAUser = @"select description from ref_restriction where restriction in (select 
                restriction from user_restriction_access where userseq=(select userseq from central.users where user_id='{0}')) order by description";

        public const string GetAllFlagsForProducts = @"select editflg from repository.EDIT
                                                        where status ='A' and product  in ({0})
                                                        MINUS
                                                        select editflg from REPOSITORY.edit
                                                        where product in ('X', 'O', 'N')
                                                        order by editflg";

        public const string GetFlagListBasedOnProduct =
            @"select editflg from repository.edit where product = '{0}' and status ='A' order by editflg asc";

        #endregion

        #region PUBLIC METHODS

        public void UpdateClaimStatusFromDB(string status, string claseq)
        {
            _executor.ExecuteQuery(Format(UpdateClaimStatus, status, claseq));
        }

        public void RemoveQaClaimLockFromDB(string claseq) =>
            _executor.ExecuteQuery(Format(RemoveLockFromQaClaim, claseq));

        public void UpdateDCIAppealDayTypeDb(string appealDayType) => _executor.ExecuteQuery(Format(UpdateDCIAppealDayType, appealDayType));
        public void DeletePTOByUserIdFromDb(string userId) => _executor.ExecuteQuery(Format(DeletePTOByUserId, userId));

        public string GetClaimStatusFromDatabase(string claseq,string client="SMTST") => _executor.GetSingleStringValue(Format(GetClaimStatusFromDb, claseq,client));

        public string GetReleaseToClientStatusFromDatabase(string claseq, string client = "SMTST") => _executor.GetSingleStringValue(Format(GetReleaseToClientStatusFromDb, claseq, client));

        public List<string> GetAssignedRestrictionDescriptionsForAUserFromDb(string userId) => _executor.GetTableSingleColumn(Format(GetAssignedRestrictionDescriptionsForAUser, userId));

        public string GetSpecificClientDataFromDb(string data, string clientCode) =>
            _executor.GetSingleStringValue(Format(GetSpecificClientData, data, clientCode));

        public void UpdateProcessingTypeOfClient(string processingType, string clientCode)
        {
            _executor.ExecuteQuery(Format(UpdateProcessingTypeOfClientSQL, processingType, clientCode));
            Console.WriteLine($"Updated processing type of {clientCode} to {processingType}");
        }

        public void UpdateSpecificAppealDataInDB(List<string> appealSettings, List<string> values, string client)
        {
            var mapSettingToTableColumn = new Dictionary<string, string>
            {
                [ProductAppealsEnum.AppealsActive.GetStringValue()] = "CLIENT_PROCESS_APPEALS",
                [ProductAppealsEnum.HideAppeals.GetStringValue()] = "HIDE_APPEAL_FUNCTIONS",
                [ProductAppealsEnum.BillableAppeals.GetStringValue()] = "ALLOW_BILLABLE_APPEALS",
                [ProductAppealsEnum.AppealCoreFlags.GetStringValue()] = "CLIENT_CAN_APPEAL_CORE_FLAGS",
                [ProductAppealsEnum.DisableRecordReviews.GetStringValue()] = "DISABLE_RECORD_REVIEWS",
                [ProductAppealsEnum.DisplayExtDocIDField.GetStringValue()] = "DISPLAY_EXTERNAL_DOC_ID",
                [ProductAppealsEnum.CotivitiUploadsAppealDocuments.GetStringValue()] = "ALLOW_APPEALS_EXT_ID_SETTING",
                [ProductAppealsEnum.SendAppealEmail.GetStringValue()] = "AUTO_GENERATE_APPEAL_EMAIL",
                [ProductAppealsEnum.AppealEmailCC.GetStringValue()] = "ATS_CC",
                [ProductAppealsEnum.AutoCloseAppeals.GetStringValue()] = "CLOSE_CLIENT_APPEALS",
            };

            var updateSet = "";

            if (appealSettings.Count == 1)
                updateSet = $"{mapSettingToTableColumn[appealSettings[0]]} = '{values[0]}'";
            
            else
            {
                for (int i = 0; i < appealSettings.Count; i++)
                {
                    updateSet = Concat(updateSet,
                        $"{mapSettingToTableColumn[appealSettings[i]]} = '{values[i]}'",
                        (appealSettings[i] == appealSettings.Last() ? "" : " , "));
                }
            }

            Console.WriteLine($"Trying to update '{updateSet}' in client table");
            _executor.ExecuteQuery(Format(UpdateSpecificClientData, updateSet, client));
        }

        /// <summary>
        /// Update specific column/s in hciuser.client table
        /// </summary>
        /// <param name="updateSet">Pass update query chunk as string. For example, to set client_user_phi_access to 'U', pass "client_user_phi_access='U'". Send comma separated values to update multiple columns</param>
        /// <param name="client">Client code</param>
        public void UpdateSpecificClientDataInDB(string updateSet, string client)
        {
            Console.WriteLine($"Updating to '{updateSet}' in client table");
            _executor.ExecuteQuery(Format(UpdateSpecificClientData, updateSet, client));
        }

        public List<string> GetClientFromDatabaseByClientStatus(string status)
        {
            return _executor.GetTableSingleColumn(Format(GetClientCodeFromDatabaseByClientStatus, status));
        }

        public List<string> GetLineOfBusiness()
        {
            return
                _executor.GetTableSingleColumn(CommonSQLObjects.LineOfBusinessList);
        }

        public void UpdateHciDoneOrClientDoneToFalseFromDatabase(string client, string hcidoneOrClientDone, string claSeq, string editFlg)
        {
            _executor.ExecuteQuery(Format(UpdateHciDoneOrClientDoneToFalse, client, hcidoneOrClientDone, claSeq, editFlg));
            Console.WriteLine("Update {0} for claseq <{1}> with flag <{2}>", hcidoneOrClientDone, claSeq, editFlg);
        }

        public string GetSystemDateFromDB() =>
            _executor.GetSingleStringValue(GetSystemDateFromDatabase);

        public List<string> GetWhiteListedIpFromDatabase()
        {
            var t = _executor.GetTableSingleColumn(WhiteListedIp).ToList();
            return t;
        }

        public List<string> GetBatchCompleteStatusForProducts(string columns, string client)
        {
            var listOfStatus = new List<string>();

            var result = _executor.GetCompleteTable(Format(GetBatchCompleteStatusForProductsSQL, columns, client));

            foreach (var row in result)
            {
                listOfStatus = row.ItemArray.Select(x => x.ToString()).ToList();
            }

            return listOfStatus;
        }

        public void UpdateRevertBatchCompleteStatus(string client) =>
            _executor.ExecuteQuery(Format(UpdateRevertBatchCompleteStatusSQL, client));

        public void UpdateSingleProductStatusForClient(string product, string client, bool active = false)
        {
            _executor.ExecuteQuery(Format(ActiveOrDisableSingleProductByClient, product, client, active ? 'T' : 'F'));
        }

        public void UpdateRevertProductStatus(List<string> expectedData) => _executor.ExecuteQuery(Format(RevertProductStatus, expectedData.ToArray()));

        public void UpdateRevertAppealData(List<string> expectedData) => _executor.ExecuteQuery(Format(RevertAppealData, expectedData.ToArray()));

        public void UpdateRevertConfigSettings(List<string> expectedData, string clientcode)
        {
            var listOfExpectedData = new List<string>(expectedData);
            listOfExpectedData.Add(clientcode);
            _executor.ExecuteQuery(Format(RevertConfigSettings, listOfExpectedData.ToArray()));
        }

        public void UpdateSetPasswordDurationInSecurityTab(string durationValue, string clientcode ) => 
                     _executor.ExecuteQuery(Format(SetPasswordDurationInSecurityTab, durationValue, clientcode));

        public void UpdateRevertCanModifyCoreFlags(string clientcode) =>
            _executor.ExecuteQuery(Format(RevertCanModifyCoreFlags, clientcode));


        public string GetLastModifiedUsernameAndDate(string clientcode)
        {
            var result = _executor.GetCompleteTable(Format(LastModifiedUsernameAndDate, clientcode));
            string auditWithDateTime = "";

            foreach (var item in result)
            {
                auditWithDateTime = item.ItemArray[0].ToString() +" " +item.ItemArray[1].ToString();
            }

            return auditWithDateTime;
        }

        public List<string> GetAppealSettingsFromDatabase(string clientCode)
        {
            var newList = new List<string>();
            var list =
                _executor.GetCompleteTable(Format(GetAppealSettings, clientCode));
            foreach (var row in list)
            {
                newList = row.ItemArray.Select(x => x.ToString()).ToList();
            }


            return newList;
        }

        public List<string> GetConfigSettingsFromDatabase(string clientCode)
        {
            var newList = new List<string>();
            var list =
                _executor.GetCompleteTable(Format(GetConfigSettings, clientCode));
            foreach (var row in list)
            {
                newList = row.ItemArray.Select(x => x.ToString()).ToList();
            }
            return newList;
        }
        public List<string> GetAppealDueDatesValueFromDatabase(string clientCode)
        {
            var newList = new List<string>();
            var list =
                _executor.GetCompleteTable(Format(GetAppealDueDatesValue, clientCode));
            foreach (var row in list)
            {
                newList = row.ItemArray.Select(x => x.ToString()).ToList();
            }
            
            
            return newList;
        }

        /// <summary>
        /// Method returns the Appeal Due Dates values from the DB for the products passed on as the function parameters
        /// </summary>
        /// <param name="productList">List of products for which to fetch the turn around days from the DB</param>
        /// <param name="client">Client to which the products belong to</param>
        /// <returns>Dictionary<string,string> where Product is the key and its respective turn around days for (Record Review, MRR, Urgent and Appeal)
        /// </returns>
        public Dictionary<string, List<string>> GetAppealDueDatesForProductsFromDB(List<string> productList, string client)
        {
            var dictWithProductAndDueDateValue = new Dictionary<string, List<string>>();
            string queryTableColumns = "";

            foreach (var product in productList)
            {
                string colName = "";

                switch (product)
                {
                    case "DCA":
                        colName = "DCI_RECORD_REVIEW_TAT, 'NA', DCI_URGENT_APPEAL_TAT, DCI_APPEAL_TAT";
                        break;
                    case "FCI":
                        colName = "FCI_RECORD_REVIEW_TAT, FCI_MEDICAL_RECORD_REVIEW_TAT, FCI_URGENT_APPEAL_TAT, FCI_APPEAL_TAT";
                        break;
                    case "FFP":
                        colName = "FFP_RECORD_REVIEW_TAT, FFP_MEDICAL_RECORD_REVIEW_TAT, FFP_URGENT_APPEAL_TAT, FFP_APPEAL_TAT";
                        break;
                    case "NEG":
                        colName = "NEG_RECORD_REVIEW_TAT, 'NA', NEG_URGENT_APPEAL_TAT, NEG_APPEAL_TAT";
                        break;
                    case "OCI":
                        colName = "OCI_RECORD_REVIEW_TAT, 'NA', OCI_URGENT_APPEAL_TAT, OCI_APPEAL_TAT";
                        break;
                    case "CV":
                        colName = "PCI_RECORD_REVIEW_TAT, PCI_MEDICAL_RECORD_REVIEW_TAT, PCI_URGENT_APPEAL_TAT, PCI_APPEAL_TAT";
                        break;
                    case "RXI":
                        colName = "RXI_RECORD_REVIEW_TAT, 'NA', RXI_URGENT_APPEAL_TAT, RXI_APPEAL_TAT";
                        break;
                    case "COB":
                        colName = "COB_RECORD_REVIEW_TAT, COB_MEDICAL_RECORD_REVIEW_TAT, COB_URGENT_APPEAL_TAT, COB_APPEAL_TAT";
                        break;
                }

                if (productList.IndexOf(product) != 0)
                    queryTableColumns = $"{queryTableColumns}, {colName}";
                else
                    queryTableColumns = $"{colName}";
            }

            var list = _executor.GetCompleteTable(Format(GetAppealDueDatesForProducts, queryTableColumns, client)).ToList();

            for (int i = 0; i < productList.Count; i++)
            {
                dictWithProductAndDueDateValue.Add(productList[i], list[0].ItemArray.Select(s => s.ToString()).ToList().GetRange((i * 4), 4));
            }

            return dictWithProductAndDueDateValue;
        }

        public List<bool> GetAppealSettingsBoolValueFromDatabase(string clientCode)
        {
            var newList = new List<bool>();
            var list =
                _executor.GetCompleteTable(Format(GetAppealSettings, clientCode));
            foreach (var row in list)
            {
                newList = row.ItemArray.Select(x => x.ToString() == "T" || x.ToString() == "Y").ToList();
            }

            return newList;
        }

        /// <summary>
        /// Method returns a Dictionary<string,string> type where Appeal Setting Label is the key and its respective DB record as the value 
        /// </summary>
        /// <param name="appealSettingLabels">List of appeal settings label for which to fetch the DB values for</param>
        /// <param name="client">Client code for which the record needs to be fetched</param>
        /// <returns></returns>
        public Dictionary<string, string> GetAppealSettingsStatusByLabelInDB(List<string> appealSettingLabels, string client)
        {
            var dictWithLabelAndSettingValue = new Dictionary<string, string>();
            string queryColumns = "";

            foreach (var label in appealSettingLabels)
            {
                string colName = "";

                switch (label)
                {
                    case "Appeals Active":
                        colName = "CLIENT_PROCESS_APPEALS";
                        break;
                    case "Hide Appeals":
                        colName = "HIDE_APPEAL_FUNCTIONS";
                        break;
                    case "Billable Appeals":
                        colName = "ALLOW_BILLABLE_APPEALS";
                        break;
                    case "Appeal Core Flags":
                        colName = "CLIENT_CAN_APPEAL_CORE_FLAGS";
                        break;
                    case "Disable Record Reviews":
                        colName = "DISABLE_RECORD_REVIEWS";
                        break;
                    case "Enable Medical Record Reviews":
                        colName = "ENABLE_MEDICAL_RECORD_REVIEWS";
                        break;
                    case "Display Ext Doc ID Field":
                        colName = "DISPLAY_EXTERNAL_DOC_ID";
                        break;
                    case "Cotiviti Uploads Appeal Documents":
                        colName = "ALLOW_APPEALS_EXT_ID_SETTING";
                        break;
                    case "Send Appeal Email":
                        colName = "AUTO_GENERATE_APPEAL_EMAIL";
                        break;
                    case "Appeal Email CC":
                        colName = "ATS_CC";
                        break;
                    case "Auto-Close Appeals":
                        colName = "CLOSE_CLIENT_APPEALS";
                        break;
                }

                if(appealSettingLabels.IndexOf(label) != 0)
                    queryColumns = $"{queryColumns} , {colName} ";
                else
                    queryColumns = $"{colName} ";
            }

            var list = _executor.GetCompleteTable(Format(GetClientSettingsByColumn, queryColumns, client)).ToList();

            for (int i = 0; i < appealSettingLabels.Count; i++)
            {
                if (appealSettingLabels[i] == "Appeal Email CC")
                {
                    dictWithLabelAndSettingValue.Add(appealSettingLabels[i], list[0].ItemArray[i].ToString());
                    continue;
                }

                var boolValue = list[0].ItemArray[i].ToString() == "T" || list[0].ItemArray[i].ToString() == "Y";
                dictWithLabelAndSettingValue.Add(appealSettingLabels[i], boolValue.ToString());
            }
            
            return dictWithLabelAndSettingValue;
        }

        public List<string> GetClientSettingsByColNames(string colNames, string client)
        {
            var listOfClientSettings = new List<string>();
            var listOfDataRows = _executor.GetCompleteTable(Format(GetClientSettingsByColumn, colNames, client)).ToList();

            for (int count = 0; count < listOfDataRows.Count; count++)
            {
                listOfClientSettings.Add(listOfDataRows[0].ItemArray[count].ToString());
            }

            return listOfClientSettings;
        }

        public List<string> GetDCIFlag()
        {
            return
                _executor.GetTableSingleColumn(DCIFlag);
        }

        public void UpdateStateIroByStateCode(string hasIro,string stateCode)
        {
            _executor.ExecuteQuery(Format(UpdateStateIRO, hasIro, stateCode));
            StringFormatter.PrintMessage(Format("Update State IRO By HasIro=<{0}> where StateCode=<{1}>", hasIro, stateCode));
        }

        public void UpdateStateOfProviderByProviderSequence(string state,string providerSeq)
        {
            _executor.ExecuteQuery(Format(UpdateStateOfProvider, state, providerSeq));
            StringFormatter.PrintMessage(Format("Update State of Provider By State=<{0}> where ProviderSequnce=<{1}>", state, providerSeq));
        }

        public List<string> GetStateIroByStateCode(string stateCode)
        {
            var newList = new List<string>();
            var list =
                _executor.GetCompleteTable(Format(StateIro, stateCode));
            foreach (var row in list)
            {
                newList = row.ItemArray
                    .Select(x => x.ToString()).ToList();


            }
            return newList;
        }

        public List<string> GetRestrictedUserTypeListByAppealSequence(string appealSeq)
        {
            return
                _executor.GetTableSingleColumn(Format(GetUserTypeListByAppealSequence, appealSeq));
        }

        public List<string> GetAssignedClientListForUser(String userId)
        {
            return
                _executor.GetTableSingleColumn(Format(GetAssignedClientList, userId));
        }

        public List<string> GetFlagLists()
        {
            return
                _executor.GetTableSingleColumn(FlagList);
        }

        public List<string> GetSpecialtyList()
        {
            return
                _executor.GetTableSingleColumn(SpecialtyList);
        }


        public List<string> GetAppealCategoryIDList()
        {
            return
                _executor.GetTableSingleColumn(AppealCategoryIDList);
        }

        public List<string> GetAllAssignedCategoriesByUser(string userId)
        {
            var userSeq=_executor.GetSingleStringValue(Format(GetUserSequenceForCurrentlyLoggedInUser, userId));
            return _executor.GetTableSingleColumn(Format(GetAllAssignedCategoriesByUserId, userSeq));
        }

        public List<string> GetAllProductList(bool isAbbreviation=false)
        {
            var list = _executor.GetTableSingleColumn(AllProductList,isAbbreviation?1:0);

            if (isAbbreviation)
            {
                list = list.Select(s => s.Replace("PCI", "CV")).ToList();
                list = list.Select(s => s.Replace("DCI", "DCA")).ToList();
            }

            return list;
        }
        public List<string> GetAppealCategoryOrderList()
        {
            return
                _executor.GetTableSingleColumn(AppealCategoryOrderList);
        }
        public IList<string> GetActiveProductListForClient(string client)
        {
            var newList = new List<string>();
            var productList =
                _executor.GetCompleteTable(Format(ActiveProductList, client));
            foreach (DataRow row in productList)
            {
                newList = row.ItemArray
                    .Select(x => x.ToString()).ToList();
            }
            return newList;
        }
        public IList<bool> GetAllProductValueList(string client)
        {
            var newList = new List<bool>();
            var productList =
                _executor.GetCompleteTable(Format(AllProductValueList, client));
            foreach (DataRow row in productList)
            {
                newList = row.ItemArray
                    .Select(x => x.ToString()=="T"?true:false).ToList();
            }
            return newList;
        }

        public List<string> GetAllFlagsForAllActiveProductsByClient(string client)
        {
            var productCodesOfActiveProducts = GetAllActiveProductsAbbrvForClient(client, true);

            string commaSeparatedProductList = Join("','", productCodesOfActiveProducts);

            commaSeparatedProductList = commaSeparatedProductList.Insert(0, "'");
            commaSeparatedProductList = commaSeparatedProductList.Insert(commaSeparatedProductList.Length, "'");

            var data = _executor.GetCompleteTable(Format(GetAllFlagsForProducts, commaSeparatedProductList));

            List<string> listOfFlags = new List<string>();

            foreach (var dataRow in data)
            {
                listOfFlags.Add(dataRow.ItemArray[0].ToString());
            }

            return listOfFlags;
        }

        public List<string> GetAllActiveProductsAbbrvForClient(string client, bool returnProductCode = false)
        {
            var listOfProductActiveStatus = _executor.GetCompleteTable((Format(AllProductValueList, client)));
            var listOfActiveProductColumnNames = new List<string>();
            var listOfProductCodes = new List<string>
            {
                "D",
                "U",
                "R",
                "N",
                "O",
                "F",
                "X",
                "C"
            };
            
            foreach (DataRow productActiveStatus in listOfProductActiveStatus)
            {
                int i = 0;

                foreach (var item in productActiveStatus.ItemArray)
                {
                    var columnName = productActiveStatus.Table.Columns[i].ColumnName;

                    if (item.ToString() == "T")
                        listOfActiveProductColumnNames.Add(columnName);

                    else
                        listOfProductCodes[i] = "INACTIVE";

                    i++;
                }
            }

            if (returnProductCode)
            {
                //Removing product codes of inactive clients
                listOfProductCodes.RemoveAll((x) => x == "INACTIVE");
                return listOfProductCodes;
            }

            var listOfActiveProductAbbrv = listOfActiveProductColumnNames.Select(s => s.Remove(s.IndexOf('_'))).ToList();
            listOfActiveProductAbbrv = listOfActiveProductAbbrv.Select(s => s.Replace("PCI", "CV")).ToList();
            listOfActiveProductAbbrv = listOfActiveProductAbbrv.Select(s => s.Replace("DCI", "DCA")).ToList();
            
            return listOfActiveProductAbbrv;
        }

        public List<string> GetExpectedProductListFromDatabase(string client, bool isAllPresent = false)
        {
            List<string> _activeProductListForClient = GetActiveProductListForClient(client).ToList();
            List<string> expectedProductTypeList = new List<string>();
            var allProductList = GetAllProductList();

            for (int i = 0; i < _activeProductListForClient.Count; i++)
            {
                if (_activeProductListForClient[i] == "T")
                {
                    expectedProductTypeList.Add(allProductList[i]);
                }
            }

            /*if (_activeProductListForClient[0] == "T")
            {
                expectedProductTypeList.Add("Dental Claim Accuracy");
            }

            if (_activeProductListForClient[1] == "T")
            {
                expectedProductTypeList.Add("FacilityClaim Insight");
            }

            if (_activeProductListForClient[2] == "T")
            {
                expectedProductTypeList.Add("Fraud Finder Pro");
            }

            if (_activeProductListForClient[3] == "T")
            {
                expectedProductTypeList.Add("Negotiation");
            }

            if (_activeProductListForClient[4] == "T")
            {
                expectedProductTypeList.Add("PharmaClaim Insight");
            }

            if (_activeProductListForClient[5] == "T")
            {
                expectedProductTypeList.Add("Coding Validation");
            }

            if (_activeProductListForClient[6] == "T")
            {
                expectedProductTypeList.Add("Coordination of Benefits");
            }*/

            if (isAllPresent) expectedProductTypeList.Insert(0, "All");
            return expectedProductTypeList;
        }

        public void DeleteLockedClaimsByClient(string client)
        {
            _executor.ExecuteQuery(Format(DeleteAllLockedClaimsByClient,client));
            StringFormatter.PrintMessage(Format("All locked Claims deleted from {0}",client));
        }
        public void InsertLockForClaimByClaimSeqAndUserId(string claSeq,string userid)
        {
            var claimSeq = claSeq.Split('-').ToList();
            _executor.ExecuteQuery(Format(InsertLockForClaim, claimSeq[0],claimSeq[1],userid));
            StringFormatter.PrintMessage(Format("Add Lock for claim  {0}", claSeq));
        }
        public bool IsClaimAllowByClaimSequenceAndUserId(string claimSeq,string userId)
        {
            var claSeq = claimSeq.Split('-').ToList();
            return _executor.GetSingleValue(Format(AllowedClaimByClaimSequenceAndUserId, claSeq[0],
                       claSeq[1], userId)) > 0;
        }

        public bool IsClaimAllowButNotAllUserTypeByClaimSequenceAndUserId(string claimSeq, string userId)
        {
            var claSeq = claimSeq.Split('-').ToList();
            return _executor.GetSingleValue(Format(AllowedClaimNotAllUserTypeByClaimSequenceAndUserId, claSeq[0],
                       claSeq[1], userId)) > 0;
        }

       

        public bool IsClaimAllowedByClaimSequenceUserIdAndAccessExcludingAll(string claimSeq, string userId, string access)
        {
            var claSeq = claimSeq.Split('-').ToList();
            return _executor.GetSingleValue(Format(AllowedClaimForAssignedAccessForUserIdAndClaimSequenceExcludeAll, claSeq[0],
                       claSeq[1], userId, access)) > 0;
        }


        public bool IsClaimAllowedByClaimSequenceUserIdAndAccessIncludingAll(string claimSeq, string userId, string access)
        {
            var claSeq = claimSeq.Split('-').ToList();
            return _executor.GetSingleValue(Format(AllowedClaimForAssignedAccessForUserIdAndClaimSequenceIncludeAll, claSeq[0],
                       claSeq[1], userId, access)) > 0;
        }

         public void UpdateCoderReviewForClaimSequence(string claseq, string value)
        {
            _executor.ExecuteQuery(Format(SetCoderReviewValueForClaimSeqeunce,claseq, value));
            StringFormatter.PrintMessage(Format("Clasequnce: {0} has been updated and coder review is set to  {1}",claseq, value));
        }

         public void UpdateClaimSequenceToPopulateForCoderClaimList(string claseq)
         {
             _executor.ExecuteQuery(Format(UpdateClaimSeqeunceForCoderWorklistPopulation, claseq));
             StringFormatter.PrintMessage(Format("Clasequnce: {0} has been updated", claseq));
         }

         public List<string> GetAppliedRestrictionListInClaim(string claseq,bool isInternal=true)
         {
             return
                 _executor.GetTableSingleColumn(Format(GetRestrictionAppliedToClaim, claseq, isInternal?2:8));
         }


         public bool GetRestrictionIDAndUserTypeForSelectedAppealSeq(string appealseq, string restrictionType)
        {
            var restrictionList =
                _executor.GetCompleteTable(Format(GetRestrictionIDAndUserTypeForSelectedAppealSeqSQL, appealseq));
            return restrictionList.Select(row => row.ItemArray.Select(x => x.ToString()).ToList()).Select(newList => newList[1] == restrictionType).FirstOrDefault();
        }

         public string GetFullPrvNameForGivenClaimNo(string claimNo)
         {
             return _executor.GetSingleStringValue(Format(GetPrvNameForGivenClaimNo, claimNo));
         }

         public string GetFullPrvNameForGivenClaimSeq(string claseq)
         {
             return _executor.GetSingleStringValue(Format(GetPrvNameForGivenClaimSeq, claseq));
         }

         public string GetUserSeqForCurrentlyLoggedInUser(string userid)
         {
             return _executor.GetSingleStringValue(Format(GetUserSequenceForCurrentlyLoggedInUser, userid));
         }

         public List<string> GetFlagListBasedOnProductDb(string product) =>
             _executor.GetTableSingleColumn(Format(GetFlagListBasedOnProduct, product));


         public string GetAppealTypeByClaseqDb(string claseq) =>
             _executor.GetSingleStringValue(Format(GetAppealTypeByClaseq, claseq.Split('-')[0], claseq.Split('-')[1]));

         #endregion
    }
}