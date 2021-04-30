using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Legacy.Service.PageObjects.Default;
using Legacy.Service.Support.Enum;
using Legacy.Service.Support.Utils;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Elements;

namespace Legacy.Service.PageObjects.Pre_Authorizations.DCI
{
    public class SearchPageObjects : DefaultPageObjects
    {
        #region PRIVATE/PUBLIC PROPERTIES
      
        private const string PreAuthSeqName = "txtPreAuthSeq";
        private const string PatSequenceName = "txtPatSeq";
        private const string PatFirstName = "txtPatFirst";
        private const string PatLastName = "txtPatLast";
        private const string PatNumberName = "txtPatNo";
        private const string PatDobName = "txtPatDOB";
        private const string PrvSequenceName = "txtPrvSeq";
        private const string TinName = "txtTIN";
        private const string PrvNameName = "txtPrvName";
        private const string PrvNumberName = "txtPrvNo";
        private const string DocNumberName = "txtDocNo";
        private const string AuthIdName = "txtPreAuthID";
        private const string Ssn1Name = "txtSSN1";
        private const string Ssn2Name = "txtSSN2";
        private const string Ssn3Name = "txtSSN3";
        private const string InsuredSsn1 = "txtEmpSSN1";
        private const string InsuredSsn2 = "txtEmpSSN2"; 
        private const string InsuredSsn3 = "txtEmpSSN3";

        private const string SearchName = "Imagebutton1";

        private const string ClearXPath = ".//img[contains(@src, '_Images/Btn_Clear.jpg')]";

        public const string TableXpathTemplate = ".//table[@id='dgResults']//tbody//tr[{0}]//td[{1}]";

        public const string NoMatchingRecordsXPath = ".//span[contains(@id, 'Results1')]//tr//td//span";

        #endregion
        
        #region PAGEOBJECTS PROPERTIES

        [FindsBy(How = How.XPath, Using = ClearXPath)] 
        public ImageButton ClearButton;

        [FindsBy(How = How.Name, Using = PreAuthSeqName)]
        public TextField PreAuthSeqTextField;

        [FindsBy(How = How.Name, Using = PatFirstName)]
        public TextField PatFirstTextField;

        [FindsBy(How = How.Name, Using = PatSequenceName)]
        public TextField PatSequenceTextField;

        [FindsBy(How = How.Name, Using = PatLastName)]
        public TextField PatLastTextField;

        [FindsBy(How = How.Name, Using = PatNumberName)]
        public TextField PatNumberTextField;

        [FindsBy(How = How.Name, Using = PatDobName)]
        public TextField PatDobTextField;

        [FindsBy(How = How.Name, Using = PrvSequenceName)]
        public TextField PrvSequenceTextField;

        [FindsBy(How = How.Name, Using = TinName)] 
        public TextField TinTextField;

        [FindsBy(How = How.Name, Using = PrvNameName)]
        public TextField PrvNameTextField;

        [FindsBy(How = How.Name, Using = PrvNumberName)]
        public TextField PrvNumberTextField;

        [FindsBy(How = How.Name, Using = DocNumberName)]
        public TextField DocNumberTextField;

        [FindsBy(How = How.Name, Using = AuthIdName)]
        public TextField AuthIdTextField;

        [FindsBy(How = How.Name, Using = Ssn1Name)]
        public TextField Ssn1TextField;

        [FindsBy(How = How.Name, Using = Ssn2Name)]
        public TextField Ssn2TextField;

        [FindsBy(How = How.Name, Using = Ssn3Name)]
        public TextField Ssn3TextField;

        [FindsBy(How = How.Name, Using = InsuredSsn1)]
        public TextField InsuredSsn1TextField;

        [FindsBy(How = How.Name, Using = InsuredSsn2)]
        public TextField InsuredSsn2TextField;

        [FindsBy(How = How.Name, Using = InsuredSsn3)]
        public TextField InsuredSsn3TextField;

        [FindsBy(How = How.Name, Using = SearchName)]
        public ImageButton SearchButton;

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
