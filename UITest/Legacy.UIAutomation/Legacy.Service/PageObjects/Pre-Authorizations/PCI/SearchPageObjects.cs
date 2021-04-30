using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Legacy.Service.PageObjects.Default;
using Legacy.Service.Support.Enum;
using Legacy.Service.Support.Utils;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Elements;

namespace Legacy.Service.PageObjects.Pre_Authorizations.PCI
{

    public class SearchPageObjects : DefaultPageObjects
    {
        #region PRIVATE/PUBLIC PROPERTIES

        private const string AuthSeqName = "AuthSeq";
        private const string DocNoName = "DocNo";
        private const string PreAuthIdName = "PreAuthID";
        private const string PatNameName = "PatName";
        private const string DobName = "Dob";
        private const string PatNoName = "PatNo";
        private const string SSN1Name = "PatSSN1";
        private const string SSN2Name = "PatSSN2";
        private const string SSN3Name = "PatSSN3";
        private const string PatSeqName = "PatSeq";
        private const string ProviderNameName = "PrvName";
        private const string TinName = "Tin";
        private const string PrvNoName = "PrvNo";
        private const string PrvSeqName = "PrvSeq";
        private const string SearchXPath = ".//img[contains(@src, '_Images/Btn_Search_Green.jpg')]";
        private const string ClearXPath = ".//img[contains(@src, '_Images/Btn_Clear.jpg')]";

        public const string NoMatchingRecordsXPath = "//span[@class='highBG']";
        #endregion

        #region PAGEOBJECTS PROPERTIES

        [FindsBy(How = How.Name, Using = AuthSeqName)]
        public TextField AuthSeqTextField;

        [FindsBy(How = How.Name, Using = DocNoName)]
        public TextField DocNoTextField;

        [FindsBy(How = How.Name, Using = PreAuthIdName)]
        public TextField PreAuthIdTextField;

        [FindsBy(How = How.Name, Using = PatNameName)]
        public TextField PatNameTextField;

        [FindsBy(How = How.Name, Using = DobName)]
        public TextField DobTextField;

        [FindsBy(How = How.Name, Using = PatNoName)]
        public TextField PatNoTextField;

        [FindsBy(How = How.Name, Using = SSN1Name)]
        public TextField SSN1TextField;

        [FindsBy(How = How.Name, Using = SSN2Name)]
        public TextField SSN2TextField;

        [FindsBy(How = How.Name, Using = SSN3Name)]
        public TextField SSN3TextField;

        [FindsBy(How = How.Name, Using = PatSeqName)]
        public TextField PatSeqTextField;

        [FindsBy(How = How.Name, Using = ProviderNameName)]
        public TextField ProviderNameTextField;

        [FindsBy(How = How.Name, Using = TinName)]
        public TextField TinTextField;

        [FindsBy(How = How.Name, Using = PrvNoName)]
        public TextField PrvNoTextField;

        [FindsBy(How = How.Name, Using = PrvSeqName)]
        public TextField PrvSeqTextField;

        [FindsBy(How = How.XPath, Using = SearchXPath)]
        public ImageButton SearchButton;

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
