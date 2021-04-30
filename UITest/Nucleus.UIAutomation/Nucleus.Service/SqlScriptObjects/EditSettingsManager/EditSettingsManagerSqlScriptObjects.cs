using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nucleus.Service.Support.Enum;
using UIAutomation.Framework.Utils;

namespace Nucleus.Service.SqlScriptObjects.EditSettingsManager
{
    public static class EditSettingsManagerSqlScriptObjects
    {
        public const string client = "SMTST";

        public const string AllResultsInEditSettingsManagerGrid = @"select a.*, h.edittype product, decode(h.editflg, null, 'F', 'T') allows_reporting  FROM
                                                                    (SELECT d.column_name, d.column_name || '_HCI_REVIEW_REQ' HCI_REVIEW_REQ, d.column_name || '_CLI_REVIEW_REQ' CLI_REVIEW_REQ, 
                                                                    d.column_name || '_AUTOAPPROVE' AUTOAPPROVE FROM dba_tab_columns d  WHERE  d.table_name = 'EDITSETTINGS' AND 
                                                                    d.column_name IN (SELECT h.editflg  FROM hcieditdesc h WHERE h.edittype = 'F' AND h.active = 'T' AND h.editflg NOT IN ( 'ANT', 'SUB'))
                                                                    and d.column_name in (select editflg from edit where nvl(use_new_edit_settings, 'F') = 'F'  )                                      
                                                                    union
                                                                    select editflg,  review_req review_req, cli_review_req, auto_approve autoapprove
                                                                    from repository.edit_settings where clientcode='SMTST' and editflg in (select editflg from edit where nvl(use_new_edit_settings, 'F') = 'T')
                                                                    )a 
                                                                    LEFT JOIN hcieditdesc h ON a.column_name = h.editflg where h.edittype not in ('D')  order by COLUMN_NAME";

       
        public const string GetAllEdits = @"select a.column_name  FROM
                                                                    (SELECT d.column_name, d.column_name || '_HCI_REVIEW_REQ' HCI_REVIEW_REQ, d.column_name || '_CLI_REVIEW_REQ' CLI_REVIEW_REQ, 
                                                                    d.column_name || '_AUTOAPPROVE' AUTOAPPROVE FROM dba_tab_columns d  WHERE  d.table_name = 'EDITSETTINGS' AND 
                                                                    d.column_name IN (SELECT h.editflg  FROM hcieditdesc h WHERE h.edittype = 'F' AND h.active = 'T' AND h.editflg NOT IN ( 'ANT', 'SUB'))
                                                                    )a 
                                                                    LEFT JOIN hcieditdesc h ON a.column_name = h.editflg order by COLUMN_NAME";

        public const string EditFlagList = @"select editflg from REPOSITORY.edit where product='{0}' and status='A' order by editflg";

        public const string IsNewEditByEdit = @"select use_new_edit_settings from REPOSITORY.edit where editflg='{0}'";

        public const string EditSettingAudit =
            @"select * from central.edit_settings_audit where clientcode='"+client+@"' and editflg='{0}' ";

        public const string ReportingFlagList = @"select editflg
            from   ( select {0} from 
            HCIUSER.EDITSETTINGS where clientcode='SMTST')       
             unpivot
             ( status
                for editflg in ({0})
            ) where status = 'R'";

        public const string GetDataSetForFallBackOrderSettings = @"select dataset, rownum from fallback_order_settings fos join hcieditdesc h on fos.editor = h.editor
                                                                   where clientcode = '"+ client + "' and h.editflg = '{0}' order by fos.fb_order";

        public const string GetEditorByFlag = @"select editor from hcieditdesc where editflg = '{0}'";

        public const string GetUseDefaultOrderList = @"select DATASET from FALLBACK_DEFAULT_ORDER where editor = (select editor from hcieditdesc where editflg = '{0}') 
                                                        order by default_order";

        public const string GetLatestFallbackOrderSettingsAuditForDataSets = @"select dataset, max(modtimestamp) from FALLBACK_ORDER_SETTINGS_AUDIT 
                                                                                where clientcode = '" + client + "'" +
                                                                             " and DATASET in ({0}) and userseq = (select userseq from central.users where user_id = '{1}') group by dataset";

        public const string GetListOfEditsRequiringFallBackOrder = @"select editflg from hcieditdesc where active = 'T' and edittype = 'F' and editflg = primary_editflg and editor is not null
                                                                    and editflg not in ('ADD', 'TSN', 'ANT', 'SUB')";

        public const string GetAncillarySettingValueForFOTandFRE = @"SELECT 
                                                                        CASE {0} 
                                                                        WHEN 'B' THEN 'Billed'
                                                                        WHEN 'F' THEN 'Allowed'
                                                                        WHEN 'P' THEN 'Adj Paid'
                                                                        END {0}
                                                                    FROM EDITSETTINGS WHERE CLIENTCODE = '"+client+"'";

        public const string ReportingSettings = @"select reporting from hciuser.hcieditdesc where editflg = '{0}'";

        public const string StatusValue = @"select case {0} 
                                            when 'T' THEN 'On'
                                            WHEN 'F' THEN 'Off'
                                            WHEN 'R' THEN 'Reporting'
                                            WHEN 'P' THEN 'Prototype'
                                            END
                                            from hciuser.editsettings where clientcode = '" + client + "'";

        public const string StatusValueForCOB = @"select case active
                                            when 'T' THEN 'On'
                                            WHEN 'F' THEN 'Off'
                                            WHEN 'R' THEN 'Reporting'
                                            WHEN 'P' THEN 'Prototype'
                                            END
                                            from repository.edit_settings where editflg='{0}' and clientcode = '" + client + "'";


        public const string EditDetailsByFlag =
            @" select GENERAL_DESC,EDITOR,EDIT_ORDER,REQUIRES_SUGGESTED_CODE,REQUIRES_SUGGESTED_UNITS,REQUIRES_SUGGESTED_PAID,
 REQUIRES_HISTORY_LINE,HISTORY_INTERVAL,ALLOW_SWITCH,REVERSE_FLAG,REVERSE_EDIT_SWITCH_FLAG,REDUCTION_FLAG,ALLOW_SUGPD_EXCEED_ADJPD
 from hciuser.HCIEDITDESC h join repository.edit e on h.editflg=e.editflg  where h.editflg in ({0})";

        public const string COBEditFlagDetails = @"select EDITFLG, AUTO_APPROVE, REVIEW_REQ, CLI_REVIEW_REQ,
                                                case active
                                                    when 'T' THEN 'On'
                                                    WHEN 'F' THEN 'Off'
                                                    WHEN 'R' THEN 'Reporting'
                                                    WHEN 'P' THEN 'Prototype'
                                                    END 
                                                AS STATUS
                                                from REPOSITORY.EDIT_SETTINGS where clientcode = '"+ client +"'" +
                                                "AND EDITFLG IN (SELECT EDITFLG FROM EDIT WHERE PRODUCT = 'C')";

        public const string AncillarySettingsStatus = "select * from " +
                                                       " (select Key, value from ANCILLARY_EDIT_SETTINGS where clientcode = 'SMTST') " +
                                                       " pivot(listagg(value, ',') within group(order by null)  for (key) in ('INVD_ALLDXCODES_INVALID','INVD_INCOMPLETE_DX','INVD_INACTIVE_DX','INVD_NONEXISTENT_DX'))";

        public const string UpdateAncillarySettingsStatus =
            "update ANCILLARY_EDIT_SETTINGS set value='T' where clientcode='SMTST' and editflg='INVD'";

        public const string UpdateLOBForClient = "update hciclient set line_of_business='{0}' where clientcode='{1}'";


        public const string GetEditlabelAncillarySettings= "select LABEL1 from ANCILLARY_EDIT_SETTINGS_MAPPING where editflg='{0}'";

        public const string GetToolTipValueForLookBackPeriod = @"select additional_info from ancillary_edit_settings_mapping where editflg in ('{0}') ORDER BY case when column_name = 'CLAIM_MIN_PAID_THRESHOLD' then 1 
when column_name = '{1}' then 2 
when column_name = 'MEMBER_MIN_PAID_THRESHOLD' then 3 
else 4 
end,column_name";

        public const string GetDUPMacthOptionsAncillarySettingFromDb = " select DECODE(DUP_MATCH_OPTIONS,'B','Billed','A','Allowed','Y','Any','U','Units')  from hciuser.editsettings where clientcode='SMTST'";
        public const string GetSPECoverrideAncillarySettingsFromDb = @"select DECODE(SPECOVRD,'S','Same','D','Different') from hciuser.editsettings where clientcode='SMTST'";


    }
}
