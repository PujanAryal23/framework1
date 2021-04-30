using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nucleus.Service.PageObjects.Default;
using Nucleus.Service.Support.Enum;
using UIAutomation.Framework.Utils;

namespace Nucleus.Service.PageObjects.Batch
{
    public class BatchSearchPageObjects: NewDefaultPageObjects
    {
        #region PAGEOBJECTPROPERTIES
        public const string DisabledFindButtonCssLocator = "div.is_disabled>button.work_button";
        public const string FindButtonCssLocator = "button.work_button";
        public const string BatchReleaseIconCssSelector =
            "li.batch_release";
        //public const string LoadMoreCssLocator = "div.load_more_data span";
        public const string CheckMarkIconCssSelector = "ul.component_item_list>.component_item:nth-of-type(1)>ul>li:nth-of-type({0})>ul>li.ok_check ";
        public const string CheckMarkIconCssSelectorByLabel = "//section[contains(@class,'component column_60')]//ul[contains(@class,'component_item_row')]//label[text()='{0}:']/../following-sibling::li[1]/ul/li";
        public const string BatchDetailsHeaderCssSelector = "section.column_40>section>div>label";
        public const string BatchDetailsValueByLabelXpathTemplate =
            "//label[text()='Batch Details']/../../../section[2]/div/ul/li/label[contains(@title, '{0}')]/../span";

        public const string BatchIDXPathTemplate = "//span[text()='{0}']";

        public const string BatchReleaseIconToolTipCssSelector =
            "ul.component_item_list>.component_item:nth-of-type(1)>ul>li:nth-of-type(1)>ul>li";
        public const string FilterOptionsListCssLocator = "li.appeal_search_filter_options";
        public const string FilterOptionListByCss = "li.appeal_search_filter_options>ul>li>span";
        public const string FilterOptionValueByCss = "li.appeal_search_filter_options>ul>li:nth-of-type({0})>span";


        #endregion
        #region CONSTRUCTOR
        public BatchSearchPageObjects()
            : base(PageUrlEnum.BatchSearch.GetStringValue())
        {
        }

        #endregion
    }
}
