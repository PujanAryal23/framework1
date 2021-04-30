using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Security.Policy;
using Nucleus.Service.PageObjects.Appeal;
using Nucleus.Service.PageObjects.Login;
using Nucleus.Service.PageObjects.Settings;
using Nucleus.Service.PageServices.Base.Default;
using Nucleus.Service.PageServices.Login;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Core.Navigation;
using Nucleus.Service.PageObjects.Settings.User;
using Nucleus.Service.SqlScriptObjects.Settings;
using UIAutomation.Framework.Elements;
using Nucleus.Service.Support.Common.Constants;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Menu;
using UIAutomation.Framework.Database;
using UIAutomation.Framework.Utils;

namespace Nucleus.Service.PageServices.Settings
{
    public class ClientProfileManagerPage : DefaultPage
    {
        #region PRIVATE FIELDS

        private ClientProfileManagerPageObjects _clientprofileManagerPage;
        private OracleStatementExecutor _executor;

        #endregion

        #region PUBLIC PROPERTIES

        public ClientProfileManagerPageObjects MyClientProfile
        {
            get { return _clientprofileManagerPage; }
        }

        #endregion

        #region CONSTRUCTOR

        public ClientProfileManagerPage(INavigator navigator, ClientProfileManagerPageObjects clientprofileManagerPage)
            : base(navigator, clientprofileManagerPage)
        {
            _clientprofileManagerPage = (ClientProfileManagerPageObjects)PageObject;
            _executor = new OracleStatementExecutor();
        }

        #endregion

        #region PUBLIC METHODS


        public void CheckUncheckConfigurationSettingsCheckBox(string configuration, bool check = true)
        {


            var currentStatus = SiteDriver
                .FindElement<CheckBox>(
                    string.Format(ClientProfileManagerPageObjects.OptionXpath,
                        configuration), How.XPath).Checked;

            if (currentStatus ^ check)
            {
                SiteDriver.FindElement<CheckBox>(string.Format(ClientProfileManagerPageObjects.OptionXpath, configuration),
                    How.XPath).ClickJS();
                StringFormatter.PrintMessage(string.Format("{0} is now {1}", configuration, check ? "Turned On" : "Turned Off"));
                return;
            }
            StringFormatter.PrintMessage(string.Format("{0} is now {1}", configuration, check ? "Turned On" : "Turned off"));


        }

        ///// <summary>
        ///// logout
        ///// </summary>
        //public new LoginPage Logout()
        //{
        //    _clientprofileManagerPage.LogoutButton.Click();
        //    return new LoginPage(Navigator, new LoginPageObjects());
        //}

        public ClientProfileManagerPage ClickOnSecurityTab()
        {
            var clientProfileManagerPage = Navigator.Navigate<ClientProfileManagerPageObjects>(() =>
            {
                _clientprofileManagerPage.SecurityTabLink.Click();
                StringFormatter.PrintMessage("Click on Security Tab");
            });
            return new ClientProfileManagerPage(Navigator, clientProfileManagerPage);
        }

        public ClientProfileManagerPage EnterPasswordDurationDays(string durationDays)
        {
            var clientProfileManagerPage = Navigator.Navigate<ClientProfileManagerPageObjects>(() =>
            {
                SiteDriver.WaitForCondition(() => _clientprofileManagerPage.PasswordDurationInputBox.Displayed);
                _clientprofileManagerPage.PasswordDurationInputBox.Clear();
                _clientprofileManagerPage.PasswordDurationInputBox.SendKeys(durationDays + "KEYS.ENTER");

                Console.WriteLine("Entered password duration days = {0}", durationDays);
            });
            return new ClientProfileManagerPage(Navigator, clientProfileManagerPage);
        }

        public void SelectPhiAcessibility(string value)
        {
            SiteDriver.FindElement<ImageButton>(ClientProfileManagerPageObjects.PhiAccessibilityDropDownSelectorCssLocator, How.CssSelector).Click();
            JavaScriptExecutor.ExecuteClick(
                string.Format(ClientProfileManagerPageObjects.PhiAccessibilityDropdownValuesXPathTemplate, value),
                How.XPath);
            ;
            Console.WriteLine("PHI accessibility set to {0}", SiteDriver.FindElement<TextLabel>(ClientProfileManagerPageObjects.PhiAccesibilitySelectedOptionValueId, How.Id).Text);
        }

        public ClientProfileManagerPage ClickOnSaveButton()
        {
            var clientProfileManagerPage = Navigator.Navigate<ClientProfileManagerPageObjects>(() =>
            {
                _clientprofileManagerPage.SecurityTabSaveButton.Click();
                SiteDriver.WaitToLoadNew(1000);
                SiteDriver.WaitForPageToLoad();
                Console.WriteLine("Clicked on Save Button");
            });
            return new ClientProfileManagerPage(Navigator, clientProfileManagerPage);
        }

        public bool IsSecurityAlertIconDisplayed()
        {
            return SiteDriver.FindElement<ImageButton>(ClientProfileManagerPageObjects.AlertIconXPath, How.XPath).Displayed;
        }

        public bool IsSecurityAlertIcon2Displayed()
        {
            return SiteDriver.FindElement<ImageButton>(ClientProfileManagerPageObjects.AlertIcon2XPath, How.XPath).Displayed;
        }

        public ClientProfileManagerPage PageRefresh()
        {
            var clientProfileManagerPage = Navigator.Navigate<ClientProfileManagerPageObjects>(SiteDriver.Refresh);
            return new ClientProfileManagerPage(Navigator, clientProfileManagerPage);
        }



        public string GetAlertMessageTooltip()
        {
            JavaScriptExecutor.ExecuteMouseOver(ClientProfileManagerPageObjects.AlertIconXPath, How.XPath);

            return SiteDriver.FindElement<TextLabel>(ClientProfileManagerPageObjects.AlertMessageXPath, How.XPath).Text.Trim();
        }

        public ClientProfileManagerPage CloseModalPopup()
        {
            var clientProfileManagerPage = Navigator.Navigate<ClientProfileManagerPageObjects>(() => _clientprofileManagerPage.ModalCloseButton.Click());

            return new ClientProfileManagerPage(Navigator, clientProfileManagerPage);
        }



        /// <summary>
        /// Click on Privileges by clicking on Privileges tab
        /// </summary>
        /// <returns>An instance of ProfileManagerPage</returns>s
        public void ClickOnPrivileges()
        {
            _clientprofileManagerPage.PrivilegesTabLink.Click();
            StringFormatter.PrintMessage("Click on Privileges Tab");
        }

        public ClientProfileManagerPage ClickOnProductSettings()
        {
            var clientProfileManagerPage = Navigator.Navigate<ClientProfileManagerPageObjects>(() =>
            {
                _clientprofileManagerPage.ProductSettingsTabLink.Click();
                StringFormatter.PrintMessage("Click on Product Settings Tab");
            });
            return new ClientProfileManagerPage(Navigator, clientProfileManagerPage);
        }

        public ClientProfileManagerPage ClickOnGeneralInformation()
        {
            var clientProfileManagerPage = Navigator.Navigate<ClientProfileManagerPageObjects>(() =>
            {
                _clientprofileManagerPage.GeneralInformationTabLink.Click();

                StringFormatter.PrintMessage("Click on General Information Tab");
            });
            return new ClientProfileManagerPage(Navigator, clientProfileManagerPage);
        }

        public ClientProfileManagerPage ClickOnInteropSetting()
        {
            var clientProfileManagerPage = Navigator.Navigate<ClientProfileManagerPageObjects>(() =>
            {
                _clientprofileManagerPage.InteropSettingTabLink.Click();

                StringFormatter.PrintMessage("Click on Interop Settings Tab");
            });
            return new ClientProfileManagerPage(Navigator, clientProfileManagerPage);
        }
        public bool IsEnableQuickDeleteRadioButtonDisplayed()
        {
            SiteDriver.WaitForCondition(() => _clientprofileManagerPage.EnableQuickDeleteRadioButton.Displayed);
            return _clientprofileManagerPage.EnableQuickDeleteRadioButton.Displayed;
        }

        public bool EnableQuickDeleteDefaultOption()
        {
            return _clientprofileManagerPage.GetEnableQuickDeleteDefaultOption.IsChecked();
        }

        public bool DentalClaimInsightChecked()
        {
            return _clientprofileManagerPage.DentalClaimInsightCheckBox.Selected;
        }

        public void ClickOnAllowBillableAppeals(bool yesNo)
        {
            if (yesNo) //click on yes
            {
                JavaScriptExecutor.ExecuteClick(
                    string.Format(ClientProfileManagerPageObjects.AllowBillableAppealsCssTemplate, 1), How.CssSelector);
                Console.WriteLine("Yes Button Clicked");
            }
            else

            {
                JavaScriptExecutor.ExecuteClick(
                    string.Format(ClientProfileManagerPageObjects.AllowBillableAppealsCssTemplate, 2), How.CssSelector);
                Console.WriteLine("No Button Clicked");
            }

        }

        public bool ClickOnDisplayDocumentIDCheckBox(bool select)
        {
            //if select =true then click on checkbox
            if (_clientprofileManagerPage.DisplayExternalDocumentIDCheckBox.Selected)
            {
                if (select)
                    return true;

                _clientprofileManagerPage.DisplayExternalDocumentIDCheckBox.Click();
                return false;

            }

            if (!select) return false;
            _clientprofileManagerPage.DisplayExternalDocumentIDCheckBox.Click();
            return true;
        }

        public bool IsCotivitiUploadAppealDocumentCheckBoxDisabled()
        {
            return _clientprofileManagerPage.CotivitiUploadAppealDocumentCheckBox.Enabled;
        }

        public string GetCotivitiUploadAppealDocumentCheckBoxTooltip()
        {
            JavaScriptExecutor.ExecuteMouseOver(ClientProfileManagerPageObjects.CotivitiUploadAppealDocumentId, How.Id);

            return SiteDriver.FindElement<TextLabel>(ClientProfileManagerPageObjects.AlertMessageXPath, How.XPath).Text.Trim();
        }

        public void ClickOnClientProcessAppealsiwthCotivitiHealthCheckBox(bool click)
        {
            if (_clientprofileManagerPage.ClientProcessAppealsiwthCotivitiHealthCheckBox.Checked)
            {
                _clientprofileManagerPage.ClientProcessAppealsiwthCotivitiHealthCheckBox.Click();
                if (click)
                {
                    _clientprofileManagerPage.ClientProcessAppealsiwthCotivitiHealthCheckBox.Click();

                }

            }
            else if (click)
            {
                _clientprofileManagerPage.ClientProcessAppealsiwthCotivitiHealthCheckBox.Click();

            }
        }

        public bool IsDisplayExternalDocuemntIdEnabled()
        {
            return _clientprofileManagerPage.DisplayExternalDocumentIDCheckBox.Enabled;
        }

        public bool IsDisplayExternalDocuemntIdChecked()
        {
            return _clientprofileManagerPage.DisplayExternalDocumentIDCheckBox.Checked;
        }

        public void ClickOnCotivitiUploadAppealDocumentCheckBox(bool click)
        {
            if (_clientprofileManagerPage.CotivitiUploadAppealDocumentCheckBox.Checked)
            {
                if (!click)
                    _clientprofileManagerPage.CotivitiUploadAppealDocumentCheckBox.Click();
            }
            else if (click)
            {
                _clientprofileManagerPage.CotivitiUploadAppealDocumentCheckBox.Click();

            }
        }

        public void ClickOnAutoGenerateAppealEmail(bool yesNo)
        {
            if (yesNo) //click on yes
            {
                JavaScriptExecutor.ExecuteClick(
                   ClientProfileManagerPageObjects.YesAutoGenerateAppealEmailCssSelector, How.CssSelector);
                Console.WriteLine("Yes Button Clicked");
            }
            else
            {
                JavaScriptExecutor.ExecuteClick(
                  ClientProfileManagerPageObjects.NoAutoGenerateAppealEmailCssSelector, How.CssSelector);
                Console.WriteLine("No Button Clicked");
            }

        }

        public void ClickOnAllowSwitchFlagsOnAppealActionCheckBox(bool check)
        {
            if (_clientprofileManagerPage.AllowSwitchFlagsOnAppealActionCheckBox.Checked)
            {
                if (!check)
                {
                    _clientprofileManagerPage.AllowSwitchFlagsOnAppealActionCheckBox.Click();
                    Console.WriteLine("unchecked <Allow Switch Flags on Appeal Action checkbox>");
                }
                else
                    Console.WriteLine("Allow Switch Flags on Appeal Action checkbox already checked");
            }
            else
            {
                if (check)
                {
                    _clientprofileManagerPage.AllowSwitchFlagsOnAppealActionCheckBox.Click();
                    Console.WriteLine("checked <Allow Switch Flags on Appeal Action checkbox>");
                }
                else
                    Console.WriteLine("Allow Switch Flags on Appeal Action checkbox already unchecked");
            }
        }

        public void SelectAllowAddSwitchFlagsOnClaimAction(string value)
        {
            SiteDriver.FindElement<Section>(ClientProfileManagerPageObjects.AllowAddSwitchFlagsOnClaimActionDropDownId, How.Id).Click();
            JavaScriptExecutor.ExecuteClick(JavaScriptExecutor.FindElementByText(
                ClientProfileManagerPageObjects.AllowAddSwitchFlagsOnClaimActionDropDownListCssLocator, value));
            Console.WriteLine("Select <{0}> on Allow Add/Switch Flags on Claim Action", value);
        }

        public List<String> GetAllOptionsOfAllowAddSwitchFlagsOnClaimAction()
        {
            SiteDriver.FindElement<Section>(ClientProfileManagerPageObjects.AllowAddSwitchFlagsOnClaimActionDropDownId, How.Id).Click();
            SiteDriver.WaitToLoadNew(300);
            return JavaScriptExecutor.FindElements(
                ClientProfileManagerPageObjects.AllowAddSwitchFlagsOnClaimActionDropDownListCssLocator, "Text");
        }

        public string GetAllowAddSwitchFlagsOnClaimActionValue()
        {
            return SiteDriver.FindElement<TextLabel>(ClientProfileManagerPageObjects.AllowAddSwitchFlagsOnClaimActionDropDownId, How.Id).GetAttribute("value");
        }

        public string GetAppealEmailCC()
        {
            return
                SiteDriver.FindElement<TextArea>(ClientProfileManagerPageObjects.AppealEmailCCCssLocator,
                    How.CssSelector).Text;
        }

        public bool IsAllowClientUserstoModifyAutoReviewedFlagsCheckboxEnabled()
        {
            return _clientprofileManagerPage.AllowClientUserstoModifyAutoReviewedFlags.Enabled;
        }

        public bool IsClientUsesClaimLogicsCheckboxChecked()
        {
            return _clientprofileManagerPage.ClientUsesClaimLogics.Checked;
        }

        public string GetClientCode()
        {
            return _clientprofileManagerPage.ClientCodeBox.GetAttribute("Value");
        }

        public void ClickOnCancel()
        {
             _clientprofileManagerPage.CancelButton.Click();
        }
        
        public void ClickToToggleClientUsesClaimLogic()
        {

            if (IsClientUsesClaimLogicsCheckboxChecked())
            {
                _clientprofileManagerPage.ClientUsesClaimLogics.Click();
                StringFormatter.PrintMessage("unchecked client uses claim logic");
            }

            else
            {
                {
                    _clientprofileManagerPage.ClientUsesClaimLogics.Click();
                    
                    StringFormatter.PrintMessage("checked client uses claim logic");
                }
            }

        }


        public bool IsAllowClientUserstoModifyAutoReviewedFlagsCheckboxChecked()
        {
            return _clientprofileManagerPage.AllowClientUserstoModifyAutoReviewedFlags.Checked;
        }

        public bool IsCloseClientAppealsCheckboxChecked()
        {
            return _clientprofileManagerPage.CloseClientAppeals.Checked;
        }
        public void CheckCloseClientAppealCheckbox(bool check)
        {
            if (_clientprofileManagerPage.CloseClientAppeals.Checked)
            {
                if (check) return;
                _clientprofileManagerPage.CloseClientAppeals.Click();
                ClickOnSaveButton();
            }
            else if (check)
            {
                _clientprofileManagerPage.CloseClientAppeals.Click();
                ClickOnSaveButton();

            }
        }

        public string GetCloseClientAppealCheckBoxTooltip()
        {
            JavaScriptExecutor.ExecuteMouseOver(ClientProfileManagerPageObjects.CloseClientAppealsId, How.Id);

            return SiteDriver.FindElement<TextLabel>(ClientProfileManagerPageObjects.AlertMessageXPath, How.XPath).Text.Trim();
        }

        public void CheckAllowClientUsersToModifyAutoReviewedFlagsCheckbox(bool check)
        {
            if (_clientprofileManagerPage.AllowClientUserstoModifyAutoReviewedFlags.Checked)
            {
                if (check) return;
                _clientprofileManagerPage.AllowClientUserstoModifyAutoReviewedFlags.Click();
                ClickOnSaveButton();
            }
            else if (check)
            {
                _clientprofileManagerPage.AllowClientUserstoModifyAutoReviewedFlags.Click();
                ClickOnSaveButton();

            }
        }

        public void SelectScoutCaseTrackerCheckbox(bool check)
        {
            if (_clientprofileManagerPage.ScoutCaseTracker.Checked)
            {
                if (check) return;
                _clientprofileManagerPage.ScoutCaseTracker.Click();
                ClickOnSaveButton();
            }
            else if (check)
            {
                _clientprofileManagerPage.ScoutCaseTracker.Click();
                ClickOnSaveButton();

            }
        }

        public bool IsScoutCaseTrackerCheckboxChecked()
        {
            return _clientprofileManagerPage.ScoutCaseTracker.Checked;
        }

        public void SelectEnableProviderFlaggingCheckbox(bool check)
        {
            if (_clientprofileManagerPage.EnableProviderFlagging.Checked)
            {
                if (check) return;
                _clientprofileManagerPage.EnableProviderFlagging.Click();
                Console.WriteLine("Enable Provider Flagging checkbox unchecked.");
                ClickOnSaveButton();
            }
            else if (check)
            {
                _clientprofileManagerPage.EnableProviderFlagging.Click();
                Console.WriteLine("Enable Provider Flagging checkbox checked.");
                ClickOnSaveButton();

            }
        }

        public string GetProviderFlaggingText()
        {
            return _clientprofileManagerPage.ProviderFlaggingText.GetAttribute("value");
        }

        public bool IsProviderFlaggingCheckboxChecked()
        {
            return _clientprofileManagerPage.EnableProviderFlagging.Checked;
        }
        #endregion

        public bool EnableorDisableCheckBoxByOption(string configuration, bool disable = false)
        {
            /*
           * Function Enables Check Box if Disabled (when disable is False) regardless of initial condition
           * Function Disables Check Box if Enabled (when disable is True) regardless of initial condition
           * Returns true if Check Box is initially disabled 
           */

            var condition1 = SiteDriver
                .FindElement<CheckBox>(
                    string.Format(ClientProfileManagerPageObjects.OptionXpath,
                        configuration), How.XPath).Checked;



            var condition2 = disable;

            if (!(condition1 ^ condition2))
            {
                SiteDriver.FindElement<CheckBox>(string.Format(ClientProfileManagerPageObjects.OptionXpath, configuration),
                    How.XPath).ClickJS();
                StringFormatter.PrintMessage(string.Format("{0} is now {1}", configuration, disable ? "Turned Off" : "Turned on"));
                return false;
            }
            StringFormatter.PrintMessage(string.Format("{0} is now {1}", configuration, disable ? "Turned Off" : "Turned on"));
            return true;

        }

        public bool IsCheckBoxDisabled(string option)
        {

            return !SiteDriver.FindElement<CheckBox>(String.Format(ClientProfileManagerPageObjects.OptionXpath, option),
                How.XPath).Enabled;
        }

        public string GetClientInformationProcessingDropdownValue(string label)
        {
            return SiteDriver.FindElement<TextLabel>(string.Format(
                ClientProfileManagerPageObjects.ClientInformationDropdownIDValueXpathTemplate, label), How.XPath).GetAttribute("value");
        }

        public NewClientSearchPage ClickOnBackButton()
        {
            JavaScriptExecutor.ExecuteClick(ClientProfileManagerPageObjects.BackButtonXpathSelector, How.XPath);
            return new NewClientSearchPage(Navigator, new NewClientSearchPageObjects());
        }


        public string GetInputBoxValueByLabel(string label)
        {
            return SiteDriver.FindElement<TextLabel>(
                string.Format(ClientProfileManagerPageObjects.TextValueByLabelXPathTemplate, label), How.XPath).GetAttribute("value");
        }

        public void SetInputBoxValueByLabel(string label, string value)
        {
            SiteDriver.FindElement<TextLabel>(
                string.Format(ClientProfileManagerPageObjects.TextValueByLabelXPathTemplate, label), How.XPath).Clear();
            SiteDriver.FindElement<TextLabel>(
                string.Format(ClientProfileManagerPageObjects.TextValueByLabelXPathTemplate, label), How.XPath).SendKeys(value);
        }

        public void ClearInputBoxValueByLabel(string label)
        {
            SiteDriver.FindElement<TextLabel>(
                string.Format(ClientProfileManagerPageObjects.TextValueByLabelXPathTemplate, label), How.XPath).Clear();
        }

        public void ClickOnEnableIpFilterCheckBox(bool check)
        {
            if (_clientprofileManagerPage.EnableIpFilterCheckbox.Checked)
            {
                if (!check)
                {
                    _clientprofileManagerPage.EnableIpFilterCheckbox.Click();
                    Console.WriteLine("unchecked <Enable IP Filtering checkbox>");
                }
                else
                    Console.WriteLine("Enable IP Filtering is already checked");
            }
            else
            {
                if (check)
                {
                    _clientprofileManagerPage.EnableIpFilterCheckbox.Click();
                    Console.WriteLine("checked <Enable IP Filtering checkbox>");
                }
                else
                    Console.WriteLine("Enable IP Filtering checkbox already unchecked");
            }

        }

        public bool IsIpFilterTextBoxEnable()
        {
            var t = _clientprofileManagerPage.IpFilterTextbox.Displayed;
            return _clientprofileManagerPage.IpFilterTextbox.Enabled;

        }

        public bool IsCIDRTextBoxEnable()
        {
            return _clientprofileManagerPage.CIDRTextbox.Enabled;
        }
        public List<string> GetIpList()
        {
            var list = new List<string>();
            if (_clientprofileManagerPage.IpFilterTextbox.Value.Contains(','))
            {
                list = _clientprofileManagerPage.IpFilterTextbox.Value.Split(',').ToList();
            }
            else
                list.Add(_clientprofileManagerPage.IpFilterTextbox.Value);
            return list;
        }

        public void SetIPAddress(string ip)
        {
            _clientprofileManagerPage.IpFilterTextbox.Clear();
            _clientprofileManagerPage.IpFilterTextbox.SetText(ip);
            //_clientprofileManagerPage.CIDRTextbox.Clear();
            //_clientprofileManagerPage.CIDRTextbox.SetText(cidr);

        }

        public void SetMultipleIPAddress(List<string> ipList)
        {
            var ip = "";
            for (int i = 0; i < ipList.Count; i++)
            {
                ip += ipList[i] + ',';

            }
            ip = ip.Remove(ip.Length - 1);
            _clientprofileManagerPage.IpFilterTextbox.Clear();
            _clientprofileManagerPage.IpFilterTextbox.SetText(ip);

            Console.Write("Comma separated ips: " + ip + " entered");
        }

        public void SetCotivitiIPAddress(string ip)
        {
            _clientprofileManagerPage.CotivitiIpTextBox.Clear();
            _clientprofileManagerPage.CotivitiIpTextBox.SetText(ip);

        }

        public void SetMultipleCotivitiIPAddress(List<string> ipList)
        {
            var ip = "";
            for (int i = 0; i < ipList.Count; i++)
            {
                ip += ipList[i] + ',';

            }
            ip = ip.Remove(ip.Length - 1);
            _clientprofileManagerPage.CotivitiIpTextBox.Clear();
            _clientprofileManagerPage.CotivitiIpTextBox.SetText(ip);

            Console.Write("Comma separated Cotiviti ips: " + ip + " entered");
        }

        public int GetWhiteListedIpCount()
        {
            return _executor.GetSingleStringValue(SettingsSqlScriptObject.WhiteListedIp).Count();
        }

        public List<string> GetWhiteListedIpFromDatabase()
        {
            var t = _executor.GetTableSingleColumn(SettingsSqlScriptObject.WhiteListedIp).ToList();
            return t;
        }

        public void UpdateEnableIpFilterSettingToFalse()
        {
            _executor.ExecuteQuery(SettingsSqlScriptObject.RevertIpFiltering);
        }

        public List<string> GetCotivitiUserIpListFromDatabase()
        {
            List<string> list = new List<string>();
            list = _executor.GetTableSingleColumn(SettingsSqlScriptObject.CotivitiUserIPList);
            return (list == null) ? new List<string>() : list;


        }
        public void ClearIPFilter()
        {
            _clientprofileManagerPage.IpFilterTextbox.Clear();

        }

        public void ClearCotivitiIPFilter()
        {
            _clientprofileManagerPage.CotivitiIpTextBox.Clear();

        }

        public bool IsIpFilterValidationIconDisplayed(bool isRegexValidation = false, bool isDuplicateValidation = false)
        {
            if (isRegexValidation)
                return SiteDriver
                    .FindElement<ImageButton>(ClientProfileManagerPageObjects.IpFilterRegexValidationCssSelector, How.CssSelector)
                    .GetAttribute("style").Contains("visibility: visible");
            else if (isDuplicateValidation)
                return SiteDriver
                    .FindElement<ImageButton>(ClientProfileManagerPageObjects.IpFilterCustomValidationId, How.Id)
                    .GetAttribute("style").Contains("visibility: visible");
            else

                return SiteDriver.FindElement<ImageButton>(ClientProfileManagerPageObjects.IPFilterValidationId, How.Id)
                    .GetAttribute("style").Contains("visibility: visible");

        }

        public bool IsCotivitiIpFilterValidationIconDisplayed(bool isRegexValidation = false, bool isDuplicateValidation = false)
        {
            if (isRegexValidation)
                return SiteDriver
                    .FindElement<ImageButton>(ClientProfileManagerPageObjects.CotivitiIpFilterRegexValidationCssSelector, How.CssSelector)
                    .GetAttribute("style").Contains("visibility: visible");
            else if (isDuplicateValidation)
                return SiteDriver
                    .FindElement<ImageButton>(ClientProfileManagerPageObjects.CotivitiIpFilterCustomValidationId, How.Id)
                    .GetAttribute("style").Contains("visibility: visible");
            else

                return SiteDriver.FindElement<ImageButton>(ClientProfileManagerPageObjects.CotivitiIpFilterValidationId, How.Id)
                    .GetAttribute("style").Contains("visibility: visible");

        }
        public string GetIpFilterValidationTooltip()
        {

            SiteDriver.MouseOver(ClientProfileManagerPageObjects.IPFilterValidationId, How.Id);

            return SiteDriver.FindElement<TextLabel>(ClientProfileManagerPageObjects.AlertMessageXPath, How.XPath).Text.Trim();
        }

        public string GetCotivitiIpFilterValidationTooltip()
        {

            SiteDriver.MouseOver(ClientProfileManagerPageObjects.CotivitiIpFilterValidationId, How.Id);

            return SiteDriver.FindElement<TextLabel>(ClientProfileManagerPageObjects.AlertMessageXPath, How.XPath).Text.Trim();
        }

        public string GetClientIpFilterCheckboxTooltip()
        {

            SiteDriver.MouseOver(ClientProfileManagerPageObjects.EnableIpFilterID, How.Id);

            return SiteDriver.FindElement<TextLabel>(ClientProfileManagerPageObjects.AlertMessageXPath, How.XPath).Text.Trim();
        }

        public bool IsOptionCheckboxVisible(string label)
        {
            return (SiteDriver.FindElement<CheckBox>(string.Format(ClientProfileManagerPageObjects.OptionParentXpath, label), How.XPath)
                .GetAttribute("style").Contains("display: block") || SiteDriver.FindElement<TextArea>(string.Format(ClientProfileManagerPageObjects.OptionParentXpath, label), How.XPath)
                .GetAttribute("style").Contains(""));
        }

        public bool IsIpFilterTextboxVisible()
        {
            return (SiteDriver.FindElement<TextArea>(ClientProfileManagerPageObjects.IPFilterTextBoxWrapperId, How.Id)
                .GetAttribute("style").Contains("display: inline-block") || SiteDriver.FindElement<TextArea>(ClientProfileManagerPageObjects.IPFilterTextBoxWrapperId, How.Id)
                        .GetAttribute("style").Contains(""));
        }

        public bool IsIpFilterTextboxInvisible()
        {
            return SiteDriver.FindElement<CheckBox>(ClientProfileManagerPageObjects.IPFilterTextBoxWrapperId, How.Id)
                .GetAttribute("style").Contains("display: none");
        }
        public bool IsOptionCheckboxInvisible(string label)
        {
            return SiteDriver.FindElement<CheckBox>(string.Format(ClientProfileManagerPageObjects.OptionParentXpath, label), How.XPath)
                .GetAttribute("style").Contains("display: none");
        }

        public string GetOptionCheckboxTooltip(string label)
        {

            SiteDriver.MouseOver(string.Format(ClientProfileManagerPageObjects.OptionParentXpath, label), How.XPath);

            return SiteDriver.FindElement<TextLabel>(ClientProfileManagerPageObjects.AlertMessageXPath, How.XPath).Text.Trim();
        }

        public bool IsCotivitiIPTextBoxDisplayed()
        {
            return _clientprofileManagerPage.CotivitiIpTextBox.Displayed;
        }

        public bool IsCotivitiIPTextBoxReadonly()
        {
            return _clientprofileManagerPage.CotivitiIpTextBox.GetAttribute("readonly") == "true";
        }

        public List<string> GetCotivitiUserIpList()
        {
            var list = new List<string>();
            if (string.IsNullOrEmpty(_clientprofileManagerPage.CotivitiIpTextBox.Text))
            {
                return list;
            }
            if (_clientprofileManagerPage.CotivitiIpTextBox.Text.Contains(','))
            {
                list = _clientprofileManagerPage.CotivitiIpTextBox.Text.Split(',').ToList();
            }

            else
                list.Add(_clientprofileManagerPage.CotivitiIpTextBox.Text);
            return list;

        }


        public bool IsFailDQLafterValidatorDisplayed()
        {
            return _clientprofileManagerPage.FailDLQafterCompareValidator.GetAttribute("style").Contains("visibility: visible");
        }

        public string GetFailDQLafterValidationTooltip()
        {
            JavaScriptExecutor.ExecuteMouseOver(ClientProfileManagerPageObjects.FailDLQafterCompareValidatorId, How.Id);

            return SiteDriver.FindElement<TextLabel>(ClientProfileManagerPageObjects.AlertMessageXPath, How.XPath).Text.Trim();
        }


        public List<string> GetTurnaroundTimeByProduct(string product)
        {
            return SiteDriver
                .FindElementsAndGetAttribute("value",
                    string.Format(ClientProfileManagerPageObjects.TurnaroundTimesByProduct, product), How.XPath); ;
        }

        public void SelectClaimLogicCheckbox(bool check)
        {
            if (_clientprofileManagerPage.ClientUsesClaimLogics.Checked)
            {
                if (check) return;
                _clientprofileManagerPage.ClientUsesClaimLogics.Click();
                ClickOnSaveButton();
            }
            else if (check)
            {
                _clientprofileManagerPage.ClientUsesClaimLogics.Click();
                ClickOnSaveButton();

            }
        }
    }
}
