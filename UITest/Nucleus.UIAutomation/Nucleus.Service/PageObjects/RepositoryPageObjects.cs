using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nucleus.Service.PageObjects.Default;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Utils;

namespace Nucleus.Service.PageObjects
{
   

    public class RepositoryPageObjects : NewDefaultPageObjects
    {
        #region PRIVATE/PUBLIC FIELDS

        public const string referencesSubMenuCSS = "li.top_nav:has(header:contains(Reference)) li>a";
        public const string referenceLabelXpath = "//header[text()='Reference']";       
        public const string FindReferencePanel = "//label[contains(text(),\'Find Reference\')]/../../.."; //Delibrately used Xpath instead of a more convinient CSS selector to verify both presence of panel and the panel title using a single selector.
        public const string searchResultRowCSS = "ul.component_item_list>div.component_item>ul.component_item_row";
        public const string searchResultGridRowCSS = "ul.component_item_list>div.component_item:nth-of-type({0})>ul.component_item_row";
        public const string addOnCodeDetailsLabelCSS =
            "section.component_left.column_40>section.component_content>ul.component_item_list>div.component_item>ul.component_item_row:nth-of-type({0})>li.component_data_point:nth-of-type({1})>label";
        public const string addOnCodeDetailsValueCSS =
            "section.component_left.column_40>section.component_content>ul.component_item_list>div.component_item>ul.component_item_row:nth-of-type({0})>li.component_data_point:nth-of-type({1})>span";

        public const string SearchResultHeaderCssSector = "h2.component_list_header";
        public const string ClearLinkXPath = "//span[text()='Clear']";


        #endregion

        #region PROTECTED PROPERTIES
        public override string PageTitle
        {
            get { return PageTitleEnum.Repository.GetStringValue(); }
        }
        #endregion
        #region CONSTRUCTOR

        public RepositoryPageObjects()
            : base(PageUrlEnum.Repository.GetStringValue())
        {
        }
        #endregion
    }
}
