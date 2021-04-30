using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nucleus.Service.PageObjects.Default;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Utils;

namespace Nucleus.Service.PageObjects.Reference
{
    public class ReferenceManagerPageObjects: NewDefaultPageObjects
    {
        #region PRIVATE/PUBLIC FIELDS

        public const string SearchByLabelInSearchGridXPath = "//label[text()='Reference Manager']/../../..//label[contains(@title, '{0}')]/../span";

        public const string SearchRowHavingTricProcXPath =
            "//ul[contains(@class,'component_item_list')]/li/ul[li[label[text()='Trig Code:']]/span[text()!='']]";


        public const string ReferenceManagerRowCategoryByFlagProcTrigClientTemplate =
           "//ul[contains(@class,'component_item_list')]/li [ul/li[2]/span[text()='{0}']] [ul/li[3]/span[text()='{1}']] [ul/li[4]/span[text()='{2}']] [ul/li[5]/span[text()='{3}']] ";
      

        public const string ReferenceManagerRowCategoryByFlagProcClientTemplate =
            "//ul[contains(@class,'component_item_list')]/li [ul/li[2]/span[text()='{0}']] [ul/li[3]/span[text()='{1}']]  [ul/li[5]/span[text()='{2}']] ";
        public const string DeleteButtonofReferenceManagerByFlagProcTrigClientTemplate =
           ReferenceManagerRowCategoryByFlagProcTrigClientTemplate + "/ul/li[1]//li[2]";

        public const string DeleteButtonofAppealRationaleRowCategoryByFlagProcClientTemplate =
            ReferenceManagerRowCategoryByFlagProcClientTemplate + "/ul/li[1]//li[2]";

        public const string AuditHistoryValueTemplateByRowXpathSelector = "(//label[text()='Claim Reference Audit History']/ancestor::section[contains(@class,'component_right')]//ul[contains(@class, 'component_item_row')])[{0}]";

        public const string AuditHistoryTemplateFieldsByRowColXpathSelector = AuditHistoryValueTemplateByRowXpathSelector + "/li[{1}]/span";

        public const string ReferenceManagerAuditSideWindowXpathSelector = "//label[text()='Claim Reference Audit History']/ancestor::section[contains(@class,'component_right')]";


        public const string ReferenceRationaleAuditHistoryTemplate = ReferenceManagerRowCategoryByFlagProcTrigClientTemplate + "/ul/li[2]";

 /*       public const string ReferenceManagerRowByFlagProcTrigClientTemplate = "//ul[contains(@class,'component_item_list')]/li " +
                                                                                          "[ul/li[2]/span[text()='{0}']] [ul/li[3]/span[text()='{1}']] " +
                                                                                          "[ul/li[4]/span[text()='{2}']] [ul/li[5]/span[text()='{3}']]";
*/
        public const string EditIconReferenceManagerByFlagProcTrigClientTemplate = ReferenceManagerRowCategoryByFlagProcTrigClientTemplate + "/ul/li[1]//li[1]";

        public const string EditIconByRow = "(//span[contains(@class, 'small_edit_icon')])[{0}]";

        public const string EditReferenceFormHeader = "//span/label[text()='Edit Claim Reference']";

        public const string ClickCancelButtonOnEditConfirmWindow =
                "//div[contains(@class, 'modal_header')]/span[text()='Confirm Edit Claim Reference']/../..//span[text()='Cancel']";
        public const string ReferenceManagereAuditValueListByCol =
            "//label[text()='Claim Reference Audit History']/ancestor::section[contains(@class,'component_right')]//ul[contains(@class, 'component_item_row')]/li[{0}]/span";
        public const string EditFieldInputBoxXpathTemplatebyLabel = "//*/ul[contains(@class, 'component_item')]//label[text()='{0}']/..//input";
        public const string EditFieldReviewGuidelineXpathTemplate = "//section[contains(@class,'form_component')]//label[text()='Review Guideline']/../textarea";

        public const string ClickOkButtonOnEditConfirmWindow = "//div[contains(@class, 'modal_header')]/span[text()='Confirm Edit Claim Reference']" +
                                                               "/../..//div[contains(@class, 'work_button')]";


        #region CSSSelector

        public const string AddButtonCssSelector = "span.add_icon";

        public const string SearchIconCssLocator = "span.sidebar_icon";

        public const string AddClaimReferenceDivCssSelector = "section.form_component";

        #endregion

        #endregion
        #region CONSTRUCTOR

        public ReferenceManagerPageObjects()
            : base(PageUrlEnum.ReferenceManager.GetStringValue())
        {
        }
        #endregion
    }
}
