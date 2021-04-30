using System;
using System.Collections.Generic;
using System.Configuration;
using Nucleus.Service.Data;
using Nucleus.Service.Support.Enum;
using UIAutomation.Framework.Core.Driver;

namespace Nucleus.Service.Support.Environment
{
    [Serializable]
    public class EnvironmentManager
    {
        #region PRIVATE FIELDS

        private static readonly EnvironmentManager instance = new EnvironmentManager();

        public static IList<int> LstRndNumbers;
        public static Random MRandom = new Random();
        public static int MNewRandomNumber;
        public static string Username;
        public static string UserFullName;
        public ISiteDriver SiteDriver;

        #endregion

        #region ENVIRONVARIABLES KEYS

        private const string ApplicationUrlKey = "ApplicationUrl";
        private const string HciAdminUsernameKey = "HCIAdminUsername";
        private const string HciClaimViewRestrictionUsernameKey = "HciClaimViewRestrictionUsername";
        private const string ClientUserWithClaimViewRestrictionKey = "ClientUserWithClaimViewRestrictionUsername";
        private const string HCIFirstLoginUserKey = "HCIFirstLoginUser";
        private const string ClientFirstLoginUserKey = "ClientFirstLoginUser";
        private const string HciAdminUsername8Key = "HCIAdminUsername8";
        private const string HciAdminUsername1Key = "HCIAdminUsername1";
        private const string HciAdminUsername2Key = "HCIAdminUsername2";
        private const string HciAdminUsername3Key = "HCIAdminUsername3";
        private const string HciAdminUsername4Key = "HCIAdminUsername4";
        private const string HciAdminUsername5Key = "HCIAdminUsername5";
        private const string HciAdminUsernameClaim5Key = "HciAdminUsernameClaim5";
        private const string HciAdminPasswordKey = "HCIAdminPassword";
        private const string HciUsernameKey = "HCIUsername";
        private const string HciPasswordKey = "HCIPassword";
        private const string ClientUserNameKey = "ClientUserName";
        private const string ClientUserNameKey1 = "ClientUserName1";
        private const string ClientUserNameKey4 = "ClientUserName4";
        private const string ClientPasswordKey = "ClientPassword";
        private const string AppealClientUserKey = "AppealClientUser";
        private const string AppealClientUserPasswordKey = "AppealClientUserPassword";
        private const string HciUserWithReadWriteManageAppealsKey = "HCIUserWithReadWriteManageAppeals";
        private const string HciIUserWithReadWriteManageAppealsPasswordKey = "HCIUserWithReadWriteManageAppealsPassword";
        private const string HciUserWithReadOnlyManageAppealsKey = "HCIUserWithReadOnlyManageAppeals";
        private const string HciUserWithReadOnlyManageAppealsPasswordKey = "HCIUserWithReadOnlyManageAppealsPassword";
        private const string ClientUserWithReadWriteManageAppealsKey = "ClientUserWithReadWriteManageAppeals";
        private const string ClientUserWithReadWriteManageAppealsPasswordKey = "ClientUserWithReadWriteManageAppealsPassword";
        private const string ConnectionStringKey = "ConnectionString";
        private const string UserTypeKey = "UserType";
        private const string TestClientKey = "TestClient";
        private const string IsInvoicePresentKey = "IsInvoicePresent";
        private const string IsHciUserAuthorizedToManageAppealsKey = "IsHCIUserAuthorizedToManageAppeals";
        private const string IsClientUserAuthorizedToManageAppealsKey = "IsClientUserAuthorizedToManageAppeals";
        private const string HciUserWithOutManageEdit = "HCIUserWithoutManageEdit";
        private const string HciUserWithoutManageAppealAuthority = "HCIUserWithoutManageAppealAuthority";
        private const string LoginTestUserKey = "LoginTestUser";
        private const string LoginTestPasswordKey = "LoginTestPassword";
        private const string HciUserPassowrdWithoutManageEdit = "HCIUserWithoutManageEditPassword";
        private const string HciUserPassowrdWithoutManageAppealAuthority = "HCIUserWithoutManageAppealAuthorityPassword";
        private const string HciUserWithReadWriteManageCategoryKey = "HCIUserWithReadWriteManageCategory";
        private const string HciIUserWithReadWriteManageCategoryPasswordKey = "HCIUserWithReadWriteManageCategoryPassword";
        private const string HciUserWithReadOnlyManageCategoryKey = "HCIUserWithReadOnlyManageCategory";
        private const string HciUserWithReadOnlyManageCategoryPasswordKey = "HCIUserWithReadOnlyManageCategoryPassword";
        private const string HciUserWithOutManageCategory = "HCIUserWithoutManageCategory";
        private const string HciUserPassowrdWithoutManageCategory = "HCIUserWithoutManageCategoryPassword";
        private const string ClientUserHavingFfpEditOfPciFlagsAuthorityUserName = "ClientUserHavingFfpEditOfPciFlagsAuthority";
        private const string ClientUserHavingFfpEditOfPciFlagsAuthorityPasswordKey = "ClientUserHavingFfpEditOfPciFlagsAuthorityPassword";
        private const string HCIUserWithReadOnlyRetroClaimSearchAuthorityKey = "HCIUserWithReadOnlyRetroClaimSearchAuthority";
        private const string HCIUserWithReadOnlyRetroClaimSearchAuthorityPasswordKey = "HCIUserWithReadOnlyRetroClaimSearchAuthorityPassword";
        private const string ClientUserWithNoAnyAuthorityKey = "ClientUserWithNoAnyAuthority";
        private const string ClientUserWithNoAnyAuthorityPasswordKey = "ClientUserWithNoAnyAuthorityPassword";
        private const string ClientUserWithoutManageEditsKey = "ClientUserWithoutManageEditsAuthority";
        private const string ClientUserWithoutManageEditsPasswordKey = "ClientUserWithoutManageEditsAuthorityPassword";
        private const string HCIUserWithReadOnlyToAllAuthorities = "HCIUserWithReadOnlyToAllAuthorities";
        private const string HCIUserWithReadOnlyToAllAuthoritiesPassword = "HCIUserWithReadOnlyToAllAuthoritiesPassword";
        private const string ClientUserWithAllReadOnlyAuthoritiesKey = "ClientUserWithAllReadOnlyAuthorities";
        private const string HciAdminUserWithUserMaintainenaceAuthorityToUpdatePasswordKey = "HciAdminUserWithUserMaintainenaceAuthorityToUpdatePassword";
        private const string HciAdminUserWithReadOnlyUserMaintainenaceAuthorityToUpdatePasswordKey = "HciAdminUserWithReadOnlyUserMaintainenaceAuthorityToUpdatePassword";
        private const string HciAdminUserWithNoUserMaintainenaceAuthorityToUpdatePasswordKey = "HciAdminUserWithNoUserMaintainenaceAuthorityToUpdatePassword";
        private const string UpdatePasswordKey = "UpdatePassword";
        private const string UpdateCurrentPasswordAndSetToNewKey = "UpdateCurrentPasswordAndSetToNew";
        private const string PasswordUpdatedByOtherUserKey = "PasswordUpdatedByOtherUser";
        private const string HciUserWithManageEditAuthorityKey = "HCIUserWithManageEditAuthority";
        private const string HciUserWithSuspectProviderDefaultPageKey = "HCIUserWithSuspectProviderDefaultPage";
        private const string HciUserPasswordWithSuspectProviderDefaultPageKey = "HCIUserPasswordWithSuspectProviderDefaultPage";
        private const string HciUserWithProviderSearchDefaultPageKey = "HCIUserWithProviderDefaultPage";
        private const string HciUserPasswordWithProviderSearchDefaultPageKey = "HCIUserPasswordWithProviderDefaultPage";
        private const string ClientUserWithSuspectProviderDefaultPageKey = "ClientUserWithSuspectProviderDefaultPage";
        private const string ClientUserPasswordWithSuspectProviderDefaultPageKey = "HCIUserPasswordWithSuspectProviderDefaultPage";
        private const string ClientUserWithProviderSearchDefaultPageKey = "ClientUserWithProviderSearchDefaultPage";
        private const string ClientUserPasswordWithProviderSearchDefaultPageKey = "ClientUserPasswordWithProviderSearchDefaultPage";
        private const string SampleUserKey = "SampleUser";
        private const string MstrUserKey = "MstrUser";
        private const string MstrUserPasswordKey = "MstrUserPassword";
        private const string ClientMstrUserKey = "ClientMstrUser";
        private const string ClientMstrUserPasswordKey = "ClientMstrUserPassword";
        private const string MstrUser1Key = "MstrUserwithManagerRole";
        private const string MstrUser1PasswordKey = "MstrUserPasswordwithManagerRole";
        private const string ClientMstrUser1Key = "ClientMstrUserwithManagerRole";
        private const string ClientMstrUser1PasswordKey = "ClientMstrUserPasswordwithManagerRole";
        private const string HCIUserWithOnlyDciWorklistAuthorityKey = "HCIUserWithOnlyDciWorklistAuthority";
        private const string ClientUserWithOnlyDciWorklistAuthorityKey = "ClientUserWithOnlyDciWorklistAuthority";
        private const string HCIUserWithoutPCIProductAuthorityKey = "HCIUserWithoutPCIProductAuthority";
        private const string HCIUsername7key = "HCIAdminUsername7";
        private const string ClientUserName7key = "ClientUsername7";
        private const string HCIUserNameForSecurityCheckKey = "HCIUserNameForSecurityCheck";
        private const string HCIPasswordForSecurityCheckKey = "HCIPasswordForSecurityCheck";
        private const string HCIUserForMyProfile = "HCIAdminUsernameForMyProfile";
        private const string ClientUserForMyProfile = "ClientUsernameForMyProfile";
        private const string HCIPasswordForMyProfile = "HCIAdminUsernameForMyProfilePassword";
        private const string ClientPasswordForMyProfile = "ClientUsernameForMyProfilePassword";
       
        
        #endregion

        #region PUBLIC PROPERTIES

        public static EnvironmentManager Instance
        {
            get { return instance; }
        }

        public static string ScreenshotFileName
        {
            get { return UIAutomation.Framework.Core.Driver.SiteDriver.GetScreenshotFilename(); }
        }

        public static string EncryptionKey { get; set; }

        #endregion

        #region CONSRUCTOR


        #endregion

        #region ENVIRONMENT VARIABLES

        public  string ApplicationUrl
        {
            get { return  DataHelper.EnviromentVariables[ApplicationUrlKey]; }
        }

        public string Browser
        {
            get { return ConfigurationManager.AppSettings["TestBrowser"].ToUpperInvariant(); }
        }

        public string HCIAdminUsernameForMyProfile
        {
            get { return GetCredentials(HCIUserForMyProfile); }
        }

        public string HCIPasswordForUserForMyProfile
        {
            get { return GetCredentials(HCIPasswordForMyProfile); }
        }

        public string ClientUserNameForMyProfile
        {
            get { return GetCredentials(ClientUserForMyProfile); }
        }

        public string ClientPasswordForUserForMyProfile
        {
            get { return GetCredentials(ClientPasswordForMyProfile); }
        }

        public string HciAdminUsername8
        {
            get { return GetCredentials(HciAdminUsername8Key); }
        }

      
        public string HciFirstLoginUser
        {
            get { return GetCredentials(HCIFirstLoginUserKey); }
        }
        public string ClientFirstLoginUser
        {
            get { return GetCredentials(ClientFirstLoginUserKey); }
        }
        public string HciAdminUsername1
        {
            get { return GetCredentials(HciAdminUsername1Key); }
        }
        public string HciAdminUsername7
        {
            get { return GetCredentials(HCIUsername7key); }
        }
        public string ClientUsername7
        {
            get { return GetCredentials(ClientUserName7key); }
        }

        public string HciAdminUsername2
        {
            get { return GetCredentials(HciAdminUsername2Key); }
        }

        public string HciAdminUsername3
        {
            get { return GetCredentials(HciAdminUsername3Key); }
        }

        public string HciAdminUsername4
        {
            get { return GetCredentials(HciAdminUsername4Key); }
        }

        public string HciAdminUsername5
        {
            get { return GetCredentials(HciAdminUsername5Key); }
        }
        public string HciAdminUsernameClaim5
        {
            get { return GetCredentials(HciAdminUsernameClaim5Key); }
        }
        public string HciAdminUsername
        {
            get { return GetCredentials(HciAdminUsernameKey); }
        }
        public string HciClaimViewRestrictionUsername
        {
            get { return GetCredentials(HciClaimViewRestrictionUsernameKey); }
        }

        public string SampleUserUserName
        {
            get { return GetCredentials(SampleUserKey); }
        }
        public string HciAdminPassword
        {
            get { return GetCredentials(HciAdminPasswordKey); }
        }

        public string HciUsername
        {
            get { return GetCredentials(HciUsernameKey); }
        }

        public string HciPassword
        {
            get { return GetCredentials(HciPasswordKey); }
        }

        public string ClientUserName
        {
            get { return GetCredentials(ClientUserNameKey); }
        }

        public string ClientUserName1
        {
            get { return GetCredentials(ClientUserNameKey1); }
        }
        public string ClientUserName4
        {
            get { return GetCredentials(ClientUserNameKey4); }
        }

        public string ClientPassword
        {
            get { return GetCredentials(ClientPasswordKey); }
        }

        public string HciUserWithReadWriteManageAppeals
        {
            get { return GetCredentials(HciUserWithReadWriteManageAppealsKey); }
        }

        public string HciIUserWithReadWriteManageAppealsPassword
        {
            get { return GetCredentials(HciIUserWithReadWriteManageAppealsPasswordKey); }
        }

        public string HciUserWithReadOnlyManageAppeals
        {
            get { return GetCredentials(HciUserWithReadOnlyManageAppealsKey); }
        }

        public string HciUserWithReadOnlyManageAppealsPassword
        {
            get { return GetCredentials(HciUserWithReadOnlyManageAppealsPasswordKey); }
        }

        public string HciUserWithNoManageEdit
        {
            get { return GetCredentials(HciUserWithOutManageEdit); }
        }

        public string HciUserWithReadWriteManageCategory
        {
            get { return GetCredentials(HciUserWithReadWriteManageCategoryKey); }
        }

        public string HciIUserWithReadWriteManageCategoryPassword
        {
            get { return GetCredentials(HciIUserWithReadWriteManageCategoryPasswordKey); }
        }

        public string HciUserWithReadOnlyManageCategory
        {
            get { return GetCredentials(HciUserWithReadOnlyManageCategoryKey); }
        }

        public string HciUserWithReadOnlyManageCategoryPassword
        {
            get { return GetCredentials(HciUserWithReadOnlyManageCategoryPasswordKey); }
        }

        public string HciUserWithNoManageCategory
        {
            get { return GetCredentials(HciUserWithOutManageCategory); }
        }

        public string HciUserPasswordWithNoManageCategory
        {
            get { return GetCredentials(HciUserPassowrdWithoutManageCategory); }
        }
        public string HciUserWithNoManageAppealAuthority
        {
            get { return GetCredentials(HciUserWithoutManageAppealAuthority); }
        }

        public string HciUserPasswordWithNoManageAppealAuthority
        {
            get { return GetCredentials(HciUserPassowrdWithoutManageAppealAuthority); }
        }

        public string ClientUserHavingFfpEditOfPciFlagsAuthority
        {
            get { return GetCredentials(ClientUserHavingFfpEditOfPciFlagsAuthorityUserName); }
        }
        

        public string ClientUserWithoutManageEditsAuthority
        {
            get { return GetCredentials(ClientUserWithoutManageEditsKey); }
        }

        public string ClientUserWithoutManageEditsAuthorityPassword
        {
            get { return GetCredentials(ClientUserWithoutManageEditsPasswordKey); }
        }


        public string ClientUserHavingFfpEditOfPciFlagsAuthorityPassword
        {
            get { return GetCredentials(ClientUserHavingFfpEditOfPciFlagsAuthorityPasswordKey); }
        }

        public string AppealClientUser
        {
            get { return GetCredentials(AppealClientUserKey); }
        }

        public string AppealClientUserPassword
        {
            get { return GetCredentials(AppealClientUserPasswordKey); }
        }

        public string HciUserPasswordWithNoManageEdit
        {
            get { return GetCredentials(HciUserPassowrdWithoutManageEdit); }
        }

        public string ClientUserPasswordWithoutManageEdit
        {
            get { return GetCredentials(HciUserPassowrdWithoutManageEdit); }
        }

        public string ClientUserWithReadWriteManageAppeals
        {
            get { return GetCredentials(ClientUserWithReadWriteManageAppealsKey); }
        }

        public string ClientUserWithReadWriteManageAppealsPassword
        {
            get { return GetCredentials(ClientUserWithReadWriteManageAppealsPasswordKey); }
        }

        public string LoginTestUser
        {
            get { return GetCredentials(LoginTestUserKey); }
        }

        public string LoginTestPassword
        {
            get { return GetCredentials(LoginTestPasswordKey); }
        }

        public string ConnectionString
        {
            get { return GetCredentials(ConnectionStringKey); }
        }

        public string UserType
        {
            get { return  DataHelper.EnviromentVariables[UserTypeKey]; }
        }

        public ClientEnum TestClient
        {
            get { return (ClientEnum)System.Enum.Parse(typeof(ClientEnum),  DataHelper.EnviromentVariables[TestClientKey]); }
        }

        public bool IsInvoicePresent
        {
            get { return Convert.ToBoolean( DataHelper.EnviromentVariables[IsInvoicePresentKey]); }
        }

        public bool IsHciUserAuthorizedToManageAppeals
        {
            get { return Convert.ToBoolean( DataHelper.EnviromentVariables[IsHciUserAuthorizedToManageAppealsKey]); }
        }

        public bool IsClientUserAuthorizedToManageAppeals
        {
            get { return Convert.ToBoolean( DataHelper.EnviromentVariables[IsClientUserAuthorizedToManageAppealsKey]); }
        }

        public string HCIUserWithReadOnlyRetroClaimSearchAuthority
        {
            get { return GetCredentials(HCIUserWithReadOnlyRetroClaimSearchAuthorityKey); }
        }

        public string HCIUserWithReadOnlyRetroClaimSearchAuthorityPassword
        {
            get { return GetCredentials(HCIUserWithReadOnlyRetroClaimSearchAuthorityPasswordKey); }
        }

        public string ClientUserWithNoAnyAuthority
        {
            get { return GetCredentials(ClientUserWithNoAnyAuthorityKey); }
        }

        public string ClientUserWithNoAnyAuthorityPassword
        {
            get { return GetCredentials(ClientUserWithNoAnyAuthorityPasswordKey); }
        }

        public string HCIUserWithReadOnlyAccessToAllAuthorites
        {
            get { return GetCredentials(HCIUserWithReadOnlyToAllAuthorities); }
        }

        public string HCIUserWithReadOnlyAccessToAllAuthoritesPassword
        {
            get { return GetCredentials(HCIUserWithReadOnlyToAllAuthoritiesPassword); }
        }

        public string ClientUserWithAllReadOnlyAuthorities
        {
            get { return GetCredentials(ClientUserWithAllReadOnlyAuthoritiesKey); }
        }

        public string HciAdminUserWithUserMaintainenaceAuthorityToUpdatePassword
        {
            get { return GetCredentials(HciAdminUserWithUserMaintainenaceAuthorityToUpdatePasswordKey); }
        }

        public string HciAdminUserWithReadOnlyUserMaintainenaceAuthorityToUpdatePassword
        {
            get { return GetCredentials(HciAdminUserWithReadOnlyUserMaintainenaceAuthorityToUpdatePasswordKey); }
        }

        public string HciAdminUserWithSuspectProviderDefaultPage
        {

            get { return GetCredentials(HciUserWithSuspectProviderDefaultPageKey); }
        }

        public string HciAdminPasswordWithSuspectProviderDefaultPage
        {

            get { return GetCredentials(HciUserPasswordWithSuspectProviderDefaultPageKey); }
        }

        public string ClientUserWithSuspectProviderDefaultPage
        {

            get { return GetCredentials(ClientUserWithSuspectProviderDefaultPageKey); }
        }

        public string ClientPasswordWithSuspectProviderDefaultPage
        {

            get { return GetCredentials(ClientUserPasswordWithSuspectProviderDefaultPageKey); }
        }
        public string HciAdminUserWithProviderSearchDefaultPage
        {

            get { return GetCredentials(HciUserWithProviderSearchDefaultPageKey); }
        }

        public string HciAdminPasswordWithProviderSearchDefaultPage
        {

            get { return GetCredentials(HciUserPasswordWithProviderSearchDefaultPageKey); }
        }

        public string ClientUserWithProviderSearchDefaultPage
        {

            get { return GetCredentials(ClientUserWithProviderSearchDefaultPageKey); }
        }

        public string ClientPasswordWithProviderSearchDefaultPage
        {

            get { return GetCredentials(ClientUserPasswordWithProviderSearchDefaultPageKey); }
        }

        public string HciAdminUserWithNoUserMaintainenaceAuthorityToUpdatePassword
        {
            get { return GetCredentials(HciAdminUserWithNoUserMaintainenaceAuthorityToUpdatePasswordKey); }
        }

        public string UpdatePassword
        {
            get { return GetCredentials(UpdatePasswordKey); }
        }
        public string UpdateCurrentPasswordAndSetToNew
        {
            get { return GetCredentials(UpdateCurrentPasswordAndSetToNewKey); }
        }

        public string PasswordUpdatedByOtherUser
        {
            get { return GetCredentials(PasswordUpdatedByOtherUserKey); }
        }

        public string ClientUserWithClaimViewRestriction
        {
            get { return GetCredentials(ClientUserWithClaimViewRestrictionKey); }
        }

        public string HciUserWithManageEditAuthority
        {
            get { return GetCredentials(HciUserWithManageEditAuthorityKey); }
        }

        public string MstrtUser1
        {
            get { return GetCredentials(MstrUserKey); }
        }

        public string MstrtUser1Password
        {
            get { return GetCredentials(MstrUserPasswordKey); }
        }
        public string MstrtUserWithManageRole
        {
            get { return GetCredentials(MstrUser1Key); }
        }

        public string MstrtUserWithManageRolePassword
        {
            get { return GetCredentials(MstrUser1PasswordKey); }
        }
        public string ClientMstrtUserWithManageRole
        {
            get { return GetCredentials(ClientMstrUser1Key); }
        }

        public string ClientMstrtUserWithManageRolePassword
        {
            get { return GetCredentials(ClientMstrUser1PasswordKey); }
        }

        public string ClientMstrtUser
        {
            get { return GetCredentials(ClientMstrUserKey); }
        }

        public string ClientMstrUserPassword
        {
            get { return GetCredentials(ClientMstrUserPasswordKey); }
        }

        public string HCIUserWithOnlyDciWorklistAuthorityAndNoOtherAuthority
        {
            get { return GetCredentials(HCIUserWithOnlyDciWorklistAuthorityKey); }
        }

        public string ClientUserWithOnlyDciWorklistAuthorityAndNoOtherAuthority
        {
            get { return GetCredentials(ClientUserWithOnlyDciWorklistAuthorityKey); }
        }

        public string HCIUserWithoutPCIProductAuthority
        {
            get { return GetCredentials(HCIUserWithoutPCIProductAuthorityKey); }
        }

        public string HCIUserNameForSecurityCheck
        {
            get { return GetCredentials(HCIUserNameForSecurityCheckKey); }
        }

        public string HCIPasswordForSecurityCheck
        {
            get { return GetCredentials(HCIPasswordForSecurityCheckKey); }
        }
        #endregion

        #region PUBLIC METHODS

        public void Init(ISiteDriver driver)
        {
            SiteDriver = driver;
        }
        
        public void CaptureScreenShot(string testName)
        {
            SiteDriver.CaptureScreenShot(testName);
        }

        //public static void CaptureScreenShot(string dirPath, string testName)
        //{
        //    SiteDriver.CaptureScreenShot(dirPath, testName);
        //}

        public void PageRefresh()
        {
            SiteDriver.Refresh();
        }

        /// <summary>
        /// Genereate unique random numbers
        /// </summary>
        /// <param name="minValue">minimum value</param>
        /// <param name="maxValue">maximum value</param>
        public static void GenereatetUniqueRndNumbers(int minValue, int maxValue)
        {
            int range = maxValue - minValue;
            var rangeNumbersArr = new int[range + 1];
            LstRndNumbers = new List<int>();
            for (var count = 0; count < rangeNumbersArr.Length; count++)
            {
                rangeNumbersArr[count] = minValue + count;
                LstRndNumbers.Add(rangeNumbersArr[count]);
            }
        }

        /// <summary>
        /// Get a new random number
        /// </summary>
        /// <returns></returns>
        public static int NewRandomNumber()
        {
            if (LstRndNumbers.Count > 0)
            {
                // Select random number from list
                int index = MRandom.Next(0, LstRndNumbers.Count);
                MNewRandomNumber = LstRndNumbers[index];
                // Remove selected number from Remaining Numbers List
                LstRndNumbers.RemoveAt(index);
                return MNewRandomNumber;
            }
            throw new InvalidOperationException("All numbers are used");
        }


        #endregion

        #region PRIVATE METHODS

        /// <summary>
        /// Decrypt encrypted data to plain text using encryption key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private string GetCredentials(string key)
        {
            return DataHelper.Credentials[key];
        }
        #endregion
    }
}

