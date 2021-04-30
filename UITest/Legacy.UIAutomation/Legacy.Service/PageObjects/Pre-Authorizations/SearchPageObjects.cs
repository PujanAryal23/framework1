using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Legacy.Service.PageObjects.Default;
using Legacy.Service.Support.Enum;
using Legacy.Service.Support.Utils;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Elements;

namespace Legacy.Service.PageObjects.Pre_Authorizations
{
    public class SearchPageObjects : DefaultPageObjects
    {
        #region PRIVATE/PUBLIC PROPERTIES

        #region DCI

        public const string PreAuthSeqName = "txtPreAuthSeq";
        public const string PatSequenceName = "txtPatSeq";
        public const string PatFirstName = "txtPatFirst";
        public const string PatLastName = "txtPatLast";
        public const string PatNumberName = "txtPatNo";
        public const string PatDobName = "txtPatDOB";
        public const string PrvSequenceName = "txtPrvSeq";
        public const string DciTinName = "txtTIN";
        public const string PrvNameName = "txtPrvName";
        public const string PrvNumberName = "txtPrvNo";
        public const string DocNumberName = "txtDocNo";
        public const string DciAuthIdName = "txtPreAuthID";
        public const string DciSSN1Name = "txtSSN1";
        public const string DciSSN2Name = "txtSSN2";
        public const string DciSSN3Name = "txtSSN3";
        public const string DciInsuredSsn1 = "txtEmpSSN1";
        public const string DciInsuredSsn2 = "txtEmpSSN2"; 
        public const string DciInsuredSsn3 = "txtEmpSSN3";

        public const string DciSearchName = "Imagebutton1";

        #endregion

        #region PCI

        public const string AuthSeqName = "AuthSeq";
        public const string DocNoName = "DocNo";
        public const string PreAuthIdName = "PreAuthID";
        public const string PatNameName = "PatName";
        public const string DobName = "Dob";
        public const string PatNoName = "PatNo";
        public const string SSN1Name = "PatSSN1";
        public const string SSN2Name = "PatSSN2";
        public const string SSN3Name = "PatSSN3";
        public const string PatSeqName = "PatSeq";
        public const string ProviderNameName = "PrvName";
        public const string TinName = "Tin";
        public const string PrvNoName = "PrvNo";
        public const string PrvSeqName = "PrvSeq";

        public const string SearchXPath = ".//img[contains(@src, '_Images/Btn_Search_Green.jpg')]";
        #endregion

        #region COMMON
        private const string ClearXPath = ".//img[contains(@src, '_Images/Btn_Clear.jpg')]";
        
        public const string NoMatchingRecordsXPath = ".//span[contains(@id, 'Results1')]//tr//td//span";

        #endregion

        #endregion
        
        #region PAGEOBJECTS PROPERTIES

        [FindsBy(How = How.XPath, Using = ClearXPath)] 
        public ImageButton ClearButton;

        #endregion
        
        #region OVERRIDE PROPERTIES

        public override string PageTitle
        {
            get { return string.Format(PageTitleEnum.PreAuthorizationSearch.GetStringValue(), StartLegacy.Product.GetStringValue()); }
        }

        #endregion

        #region CONSTRUCTOR

        public SearchPageObjects()
            : base(string.Format(PageUrlEnum.PreAuthorizationSearch.GetStringValue(), StartLegacy.PreAuthorizationProduct))
        {

        }

        #endregion
    }
}
