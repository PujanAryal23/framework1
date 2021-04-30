using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nucleus.Service.PageObjects.Default;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Utils;

namespace Nucleus.Service.PageObjects.Batch
{
    public class BatchSummaryPageObjects : NewDefaultPageObjects
    {
        #region PAGEOBJECTPROPERTIES

        public const string BackIconCssTemplate = "span.search_icon";
        public const string QuadrantTitlesCssTemplate = "label.component_title";
        public const string BatchIDValueXpath = "//label[text()='Batch ID:']/../span";
        public const string ValueByLabelXpathTemplate = "//label[text()='{0}']/../div";
        public const string FirstRowHeaderXpathTempate = "(//table[@class = 'batch_stats_table']//tr)[1]/th"; 
        public const string FirstColumnXpathTemplate = "//table[@class = 'batch_stats_table']//tbody//th"; 
        public const string RowValueByLabelXpath =
            "//span[contains(@class,'batch_summary_label') and text()='{0}']/../../div[2]/span";

        //public const string ToggleIconInStatsByProductXpathTemplate = "//span[contains(@title,'Switch')]";
        public const string DollarIconCssTemplate = "span.dollars";
        public const string DataIconCssTemplate = "span.data_icon";
        public const string SelectedDollarIconCssSelector =
            "li.is_selected span.dollars";
        public const string SelectedDataValueIconXpath= "//li[contains(@class,'is_selected')]/span[contains(@class,'data_icon')]";
        public const string DataValuesByProductCssTemplate = @"//div[ul/li/label[text()='{0}']]//li[not(contains(@class,'no_value'))]/span";
        public const string ActiveProductListCssTemplate = @"div.claim_dollar_detail>ul>div>ul>li>label";
        ////div[ul/li/label[text()='CV']]/ul[2]//li[position()>1]//label
        
        public const string FirstContainerFirstColumnLabelsXpathTemplate =
            @"//div[ul/li/label[text()='CV']]/ul[position()>1]//li[1]//label";
        public const string FirstContainerLabelsByRowXpathTemplate =
            @"//div[ul/li/label[text()='CV']]/ul[{0}]//li[position()>1]//label";
        
        public const string ProcessingHistoryValueListByColumnCssTemplate =
            "div#bottom_section section.component_right ul.component_item_list>.component_item>ul>li:nth-of-type({0})>span";
        public const string ProcessingHistoryLabelInGridByRowColumnCssTemplate =
            "div#bottom_section section.component_right ul.component_item_list>.component_item:nth-of-type({0})>ul>li:nth-of-type({1})>label";
        public const string ProcessingHistoryValueInGridByRowColumnCssTemplate =
            "div#bottom_section section.component_right ul.component_item_list>.component_item:nth-of-type({0})>ul>li:nth-of-type({1})>span";

        public const string EmptyMessageXpathTemplate = "//label[text()='Return Files']/../../../section[2]/label[contains(@class,'empty_message')]";
        public const string BatchFileValueListByColumnCssTemplate =
            "div#top_section section.component_right ul.component_item_list>.component_item>ul>li:nth-of-type({0})>span";
        public const string BatchFileLabelInGridByRowColumnCssTemplate =
            "div#top_section section.component_right ul.component_item_list>.component_item:nth-of-type(1)>ul>li:nth-of-type(1)>label";

        public const string BatchInformationTopDivValueXPathTemplate =
            "//li[label[{0}]]/span";

        public const string BatchInformationRightDivValueCssLocator = "//table[@class='batch_stats_table']//tbody//td";
        

        #endregion
        #region CONSTRUCTOR

        public BatchSummaryPageObjects()
            : base(PageUrlEnum.BatchSummary.GetStringValue())
        {
        }

        #endregion
    }
}
