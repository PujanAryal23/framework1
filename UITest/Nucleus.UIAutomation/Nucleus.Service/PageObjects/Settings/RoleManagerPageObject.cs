using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nucleus.Service.PageObjects.Default;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Utils;

namespace Nucleus.Service.PageObjects.Settings
{
    public class RoleManagerPageObject : NewDefaultPageObjects
    {
        #region Public Fields

        public const string EditIconByRowCssSelector = "ul.component_item_list div:nth-of-type({0}) span.small_edit_icon";
        public const string EditIconByRoleNameAndUserTypeXPath = "//span[@title = '{0}']/../../li//span[contains(text(),'{1}')]/../../li//span[contains(text(),'Edit Role')]";
        public const string DeleteIconByRowCssSelector = "ul.component_item_list div:nth-of-type({0}) span.small_delete_icon";
        public const string RoleNameValueByRowCssSelector = "ul.component_item_list div:nth-of-type({0}) label[title='Role Name']+span";
        public const string UserTypeValueByRowCssSelector = "ul.component_item_list div:nth-of-type({0}) label[title='User Type']+span";
        public const string FiltersIconCssSelector = "span.filters";
        public const string InputValueOfUserTypeXPath = "//label[contains(text(),'User Type')]/following-sibling::section//ul/li";
        public const string UserTypeDropDownValueByTextXPath = "//label[contains(text(),'User Type')]/following-sibling::section/ul/li[contains(text(),'{0}')]";
        public const string UserTypeInputFieldXPath =
            "//label[contains(text(),'User Type')]/following-sibling::section/div/input";
        public const string AddIconCssSelector = ".add_icon";
        public const string CreateNewRoleFormCssSelector = "form:has(label:contains(Create New Role))";
        public const string CreateNewRoleFormInputFieldByLabelCssSelector = "div.basic_input_component:has(label.form_label:contains({0})) input";
        public const string RoleNameValueCssSelector = "ul.component_item_list div label[title='Role Name']+span";
        public const string UserTypeValueCssSelector = "ul.component_item_list div label[title='User Type']+span";
        #region Modify Role

        public const string EditFormHeaderCssSelector = "span.form_header_left>label";
        public const string RoleDescriptionInputFieldCssSelector = "label:contains(Role Description)+input";
        public const string AssignedAuthoritiesCssSelector = "h2:contains(Assigned Authorities)+ul ul span";
        public const string AssignedAuthoritiesByRowCssSelector = "h2:contains(Assigned Authorities)+ul ul div:nth-of-type({0}) span";
        public const string AvailableAuthoritiesXPath = "//h2[text()='Available Authorities']/.. /ul/ul/div//span";
        public const string AssignedAuthoritiesXPath = "//h2[text()='Assigned Authorities']/.. /ul/ul/div//span";
        public const string AvailableAuthoritiesByRowXPath = "//h2[text()='Available Authorities']/.. /ul/ul/div[{0}]//span";
       
        public const string AuthorityByNameXPath = "//h2[text()='{0}']/..//span[text()='{1}']";
        public const string SelectAllCssSelector = "span:contains(Select All >>)";
        public const string DeselectAllCssSelector = "span:contains(<< Deselect All)";

        #endregion

        #region CreateNewRole

        public const string CreateNewRoleAvailableAuthoritiesByRowCssSelector = "div.left_list>ul>ul>div:nth-of-type({0}) ul";

        #endregion

        #endregion
    }
}
