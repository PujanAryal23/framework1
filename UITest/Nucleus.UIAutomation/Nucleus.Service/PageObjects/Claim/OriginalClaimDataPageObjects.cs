using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Utils;
using UIAutomation.Framework.Core.Base;

namespace Nucleus.Service.PageObjects.Claim
{
    public class OriginalClaimDataPageObjects:NewPageBase
    {

        #region PRIVATE FIELDS


        public const string PopupTitleCssLocator = "label.PopupPageTitle";

        public const string ClaimSequenceCssLocator = "div.SubNavPopupLeft>span";

        public const string ColumnNamesCssLocator = "table>thead>tr>th";

        public const string TableColumnHeaderXPathTemplate = "//th[text()='{0}']";

        public const string OriginalClamDataXpath = "//table[contains(@class,'nucleus_table')]//tbody//tr[{0}]/td";
       
        #endregion

        #region PROTECTED PROPERTIES

        public override string PageTitle
        {
            get { return PageTitleEnum.OriginalClaimDataPage.GetStringValue(); }
        }

        #endregion

        #region CONSTRUCTOR

        public OriginalClaimDataPageObjects()
            : base(PageUrlEnum.OrignalClaimData.GetStringValue())
        {
        }

        

        #endregion
    }
}
