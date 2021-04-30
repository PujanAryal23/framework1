using Nucleus.Service.Support.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Nucleus.Service.PageObjects.Claim;
using Nucleus.Service.PageObjects.Default;
using Nucleus.Service.PageObjects.Settings.User;
using Nucleus.Service.PageServices.Base.Default;
using Nucleus.Service.SqlScriptObjects.Common_SQL;
using UIAutomation.Framework.Core.Navigation;
using UIAutomation.Framework.Database;
using Nucleus.Service.SqlScriptObjects.Settings;
using Nucleus.Service.SqlScriptObjects.User;
using Nucleus.Service.Support.Common.Constants;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Environment;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Elements;
using UIAutomation.Framework.Utils;
using static System.String;
using Extensions = Nucleus.Service.Support.Utils.Extensions;

namespace Nucleus.Service.PageObjects.Settings.User
{
    public class MyProfilePageObjects : NewDefaultPageObjects
    {
        #region CONSTRUCTOR
        public MyProfilePageObjects()
            : base(PageUrlEnum.UserProfileSearch.GetStringValue())
        {
        }
        #endregion

        #region PROTECTED PROPERTIES
        public override string PageTitle
        {
            get { return PageTitleEnum.MyProfile.GetStringValue(); }
        }
        #endregion

        #region PUBLIC METHODS

        public const string SaveButtonCssSelector = "button:contains(Save)";
        public const string CancelButtonCssSelector = "button:contains(Cancel)";
        public const string ExtFieldByPhoneFaxOrAltPhoneCssSelector = "div li:has(label:contains({0})) li:has(label:contains(Ext)) input";
        public const string ProfileTabXPath = "//div[text()='{0}']";
        public const string DropdownByLabelXPath = "//label[text()='{0}']/../section/div";
        public const string DropdownListByLabelXPath = "//label[text()='{0}']/../section/ul/li";
        public const string DropdonwValueByTextXPath = "//label[text()='{0}']/../section/ul/li[text()='{1}']";
        public const string TextBoxByLabelXPath = "//h2[text()='{0}']/..//label[text()='{1}']/../input";
        public const string TextBoxByQuestionLabelXPath = "//h2[text()='{0}']/..//label[text()='{1}']/../../..//input[@maxlength='100']";
        public const string ButtonByTextXPath = "//button[text()='{0}']";
        public const string InputFieldByLabelXpathTemplate = "//section[not(contains(@class,'is_hidden'))]//section[contains(@class,'component_header')]/..//label[text()='{0}']//..//input";
        public const string DropDownInputListValueByLabelAndValueXPathTemplate = "//label[text()='{0}']/../section//ul//li[text()='{1}']";
        public const string InputFieldByLabelCssTemplate = "section.form_component div:has(>label:contains({0})) input ,ul div:has(>label:contains({0})) input,section.is_slider:not(.is_hidden) div:has(>label:contains({0})) input";
        #endregion

    }
}
