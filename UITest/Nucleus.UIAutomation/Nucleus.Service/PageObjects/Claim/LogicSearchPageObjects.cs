using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nucleus.Service.PageObjects.Default;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Utils;

namespace Nucleus.Service.PageObjects.Claim
{
    public class LogicSearchPageObjects: NewDefaultPageObjects
    {

        #region CONSTRUCTOR
        public LogicSearchPageObjects()
            : base(PageUrlEnum.LogicSearch.GetStringValue())
        {
        }
        #endregion

        #region PROTECTED PROPERTIE
        public override string PageTitle
        {
            get { return PageTitleEnum.LogicSearch.GetStringValue(); }
        }
        #endregion

        #region public method


        public const string LogicSearchResultListLockForClaimXpathTemplate =
            "//li[contains(@class,'lock')] ";
        public const string OverDueIcon= @"li[title^='Logic response is past due']";
        public const string LogicDetailheaderByCSS = " section.column_40 .component_header_left label";

        public const string LogicSecondaryDetailsByCss =
            "//label[text()='Logic Details']/../../../section[2]/div/ul/li/label[contains(@title, '{0}')]/../span";

        public const string FilterOptionsListCssLocator = "li.appeal_search_filter_options";
        public const string DetailsLabelByCss = ".column_50 label ";
        public const string FilterOptionListByCss = "li.appeal_search_filter_options>ul>li>span";
        public const string FilterOptionValueByCss = "li.appeal_search_filter_options>ul>li:nth-of-type({0})>span";
        public const string SearchButtonCssLocator = "input.work_button";
        public const string PreAuthInLogicSearchXpathLocator = "//ul[contains(@class,'component_item_row ')]//li/span[text()='{0}']";
        public const string ClaimsearchExportByCss = ".print_icon";
        public const string DisabledExportIconCss = "li.is_disabled>span.print_icon";
        public const string EnabledExportIconCss = "li.is_active>span.print_icon";

        #endregion

    }
}
