using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using UIAutomation.Framework.Elements;

namespace Nucleus.Service.SqlScriptObjects.Settings
{
	public class SettingsSqlScriptObject
	{
		public const string client = "SMTST";


		public const string GetAvailabeRestrictionsList =
			@"select description from ref_restriction";

	    public const string HardDeleteUserIfExists = @"
        begin
        delete from central.USER_RESTRICTION_ACCESS where userseq=(select userseq  from central.users where user_id='{0}');" +
	    "delete from central.users where user_id = '{0}';" +
        "end;";

        public const string RestrictedClaimCountForClaimSequence = @"
            select count(*) from " + client + ".claim_line  where  claseq={0} and clasub={1} and patseq in " +
          "(select patseq from "+client+".HCIPAT where restriction in " +
          "(select restriction from REF_RESTRICTION where description in ({2})) )";

	    public const string UpdateProcessingTypeOfClient =
	        @"update client set processing_type='{0}' where clientcode = '{1}'";

	    public const string WhiteListedIp =
            @"select CIDR from Central.WHITELISTED_IP_CIDR where clientcode='" + client+"'";

	    public const string RevertIpFiltering =
            @"begin
               update hciuser.client set enable_ip_filter='F', ALLOW_VERSCEND_IP='T'  where clientcode='" + client+"';"+
               " delete from Central.WHITELISTED_IP_cidr where clientcode='" + client + "';" +
                "end;";

	    public const string CotivitiUserIPList =
            @"select CIDR from CENTRAL.WHITELISTED_IP_CIDR where clientcode ='INTERNAL'";

	    public const string GetAccessListByClient = @"select * from ref_restriction where clientcode in ('ALL', '{0}') AND USER_TYPE in ( 1, {1}) order by description";

        public const string GetAccessListByClientList = @"select * from ref_restriction where clientcode in ('ALL', {0}) AND USER_TYPE in ( 1, {1}) order by description";

        public const string GetAuthorityCountByPrivilege =
	        @"select count(1) from CENTRAL.USER_AUTHORIZATION where userseq=(select userseq from central.users where user_id='{0}') 
                and authority_id= (select id from central.REF_AUTHORIZATION where description='{1}')";
        public const string GetAuthorityCountByRole=
            @"select count(1) from CENTRAL.USER_ROLE where userseq=(select userseq from central.users where user_id='{0}') 
                and ROLE_ID= (select id from central.REF_ROLE where description='{1}')";


        public const string GetRoleCountForUser =
            @"select count(1) from CENTRAL.USER_ROLE where userseq=(select userseq from central.users where user_id='{0}') 
                and role_id in (select id from Central.REF_ROLE)";

	    public const string SubscriberIdForUser = "SELECT SUBSCRIBER_ID FROM central.USERs WHERE USER_ID='{0}'";
	    public const string RemoveSubscriberIdValue = "UPDATE CENTRAL.USERS SET SUBSCRIBER_ID=null WHERE USER_ID='{0}'";

	    public const string ProcessingTypeForCleint = "SELECT PROCESSING_TYPE FROM CLIENT WHERE CLIENTCODE='{0}'";

	    #region ClientSearch

	    public const string GetAvailableClientCode = "select distinct Clientcode from Client order by clientcode";
	    public const string GetPrimaryDataForClientSearch = "select clientcode,clientname,DECODE(client_type,'H','Group Health','P','Property & Casualty') from client where clientname like '%{0}%' and clientcode='{1}' order by clientcode";
	    public const string ActiveProductList =
            @"select PCI_ACTIVE,FFP_ACTIVE,FCI_ACTIVE,DCI_ACTIVE,NEG_ACTIVE,RXI_ACTIVE,COB_ACTIVE from client where clientcode = " + "'" + client + "'" +
	        " ";



	    public const string GetClientCodeFromDatabaseByProduct = "select clientcode from client where PCI_active='T' order by clientcode";

	    public const string GetClientCodeForPartialMatchesOnClientName =
            "select count(*) from client where clientname like '%{0}%' order by clientcode";

	    public const string GetSecondaryDataForClientSearch = "select DECODE(c.processing_type,'R','Real-time','B','Batch','C','CVP Batch'),DECODE(c.allow_billable_appeals,'Y','Yes','N','No'),  DECODE(c.auto_generate_appeal_email,'Y','Yes','N','No'),DECODE(c.allow_client_user_quick_delete,'T','Yes','F','No'),CONCAT(CONCAT(u.first_name,' '),u.last_name) as last_mod_by,to_char(c.last_updated,'MM/DD/YYYY HH:MI:SS PM') from client c left join central.users u on c.LAST_UPDATED_BY=u.userseq where clientcode='" + client + "'";

	    public const string UpdateDefaultSchemaValue = @"update hciuser.client
        set clientname='Selenium Test Client',
        active='T'
        , processing_type='B',
        client_type='H'
        where clientcode='" + client + @"'";

	    public const string GetClientSettingAudit =
            @"select userseq,user_id,last_updated,column_name,old_value,new_value from hciuser.client_settings_audit where clientcode='{0}' order by last_updated desc FETCH NEXT {1} ROWS ONLY";
	    public const string LockedClaimCountByUser =
	        @"SELECT  count(*) FROM " + client + ".locked_claims " +
	        "WHERE userseq=(Select userseq from users where user_id='{0}')";


        public const string GetCustomFieldList = @"select column_name,field_label,contains_pii from " + client + ".USER_DEFINED_FIELD order by field_order";
        public const string DeleteCustomFieldList = @"delete  from {0}.USER_DEFINED_FIELD";

        #endregion


        public const string GetCIWLastUpdatedNameFromDb =
            @"select username from central.users where userseq = (select CIW_MODIFIED_BY from hciclient_nucleus where clientcode = 'SMTST')";

        public const string GetCIWLastUpdatedDateFromDb =
            @"select to_char(CIW_MODIFIED_DATE,'mm/dd/yyyy hh:mm:ss AM') from hciclient_nucleus where clientcode ='SMTST'";


        public const string GetCIWModifiedDateInformationFromAuditTable =
            @"select old_value,new_value from hciuser.client_settings_audit 
                where id = (select max(id) from hciuser.client_settings_audit where column_name = 'CIW_MODIFIED_DATE')";

        public const string GetClientGeneralSettingFromDataBase =
            "select active,clientname,DECODE(client_type,'H','Group Health','P',('Property '||chr(38)||' Casualty')),DECODE(processing_type,'R','Real-time','B','Batch','C','CVP Batch','PR','PCA Real-Time','PB','PCA Batch') from client where clientcode = 'SMTST'";

	    public const string GetLogoModifiedDateFromDb =
            "select to_char(LOGO_MODIFIED_DATE,'mm/dd/yyyy hh:mm:ss AM') from hciuser.hciclient_nucleus where clientcode='{0}'";

        public const string GetLogoModifiedByFromDb = "select first_name||' '||last_name from users where userseQ in (select Logo_modified_by from hciuser.hciclient_nucleus where clientcode='{0}')";

	    public const string GetLogoModifiedDateInformationFromAuditTable =
	        @"select old_value,new_value from hciuser.client_settings_audit 
                where id = (select max(id) from hciuser.client_settings_audit where column_name = 'LOGO_MODIFIED_DATE')";

	    public const string UpdateEmailAddressForClientInDB =
	        "UPDATE CLIENT SET ATS_CC='{1}' WHERE CLIENTCODE='{0}'";

	    public const string UpdateSendAppealEmailStatus =
	        "UPDATE CLIENT SET AUTO_GENERATE_APPEAL_EMAIL='{1}' WHERE CLIENTCODE='{0}'";

        public const string GetClientSettingAuditByColumnForToday =
            @"select column_name,old_value,new_value from hciuser.client_settings_audit where clientcode='{0}' 
        and column_name in ({1})
            and trunc(last_updated)=trunc(sysdate)
        order by last_updated desc FETCH NEXT {2} ROWS ONLY";

        public const string GetUserSeqFromUserId = "select userseq from users where user_id='{0}'";

        public const string GetDCIAppealDayType = @"select APPEAL_DAY_TYPE from client where clientcode='SMTST'";

        public const string GetExcludeHolidayDataFromDb =
            @"SELECT exclude_internal_holidays, exclude_client_holidays FROM hciclient c WHERE c.clientcode = 'SMTST'";

        public const string GetUserCountForUserTypeAndEmail =
            @"select count(*) from users where email_address='{0}' and user_type='2'";

        public const string GetRedirectURLFromDatabase =
            @"select redirect_url  from hciclient where clientcode = '{0}'";


    }
}
