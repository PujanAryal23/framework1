using Legacy.Service.PageObjects.Default;
using Legacy.Service.Support;
using Legacy.Service.Support.Enum;
using Legacy.Service.Support.Utils;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Elements;

namespace Legacy.Service.PageObjects.Product
{
    public class ClaimSummaryPageObjects : DefaultPageObjects
    {
        #region PRIVATE/PUBLIC FIELDS

        private const string SButtonXPath = "//img[contains(@src, '_images/S.jpg')]";
        private const string AButtonXPath = "//img[contains(@src, '_images/A.jpg')]";
        private const string MButtonXPath = "//img[contains(@title, 'Modify All Edits')]";
        private const string PButtonXPath = "//img[contains(@src, '_images/P.jpg')]";
        private const string ClaimSequenceXPath = "//body//table[2]//tr[1]//table[1]//tr[1]//td[2]//span[2]";
        private const string BatchIdXPath = "//body//table[2]//tr[1]//table[1]//tr[1]//td[2]//span[3]";
        private const string InvoiceButtonXPath = "//img[contains(@src, '_Images/Btn_ViewInvoice.jpg')]";
        private const string AppealButtonXPath = "//img[contains(@src, '_Images/Btn_Appeal.jpg')]";
        private const string TwelveMonthButtonXPath = "//img[contains(@src, '_Images/Btn_12Month.jpg')]";
        private const string AllButtonXPath = "//img[contains(@src, '_Images/Btn_All.jpg')]";
        private const string SixtyDaysButtonXPath = "//img[contains(@src, '_Images/Btn_60Days.jpg')]";
        private const string SameDayButtonXPath = "//img[contains(@src, '_Images/Btn_SameDay.jpg')]";
        private const string ProviderButtonXPath = "//img[contains(@src, '_Images/Btn_Provider.jpg')]";
        private const string DocumentButtonXPath = "//img[contains(@src, '_Images/Btn_Menu_Documents')]";
        private const string DataButtonXPath = "//img[contains(@src, '_Images/Btn_OriginalData.jpg')]";

        public const string CloseButtonXPath = "//img[contains(@src, '_Images/Btn_Close_Gray.jpg')]";
        public const string SaveButtonXPath = "//img[contains(@src, '_Images/Btn_Save_Gray.jpg')]";

        #region PROCESSING SECTION
        private const string ProcessingButtonXPath = "//img[contains(@src, '_Images/Btn_Processing.jpg')]";
        private const string ProcessingSectionFrameId = "history";
        private const string ClaimHistoryTableTitleXPath = "//body[@id='pgBody']/form/div/table/tbody/tr[1]/td/fieldset/legend";
        public const string SystemModificationsTableTitleXPath = "//body[@id='pgBody']/form/div/table/tbody/tr[3]/td/fieldset/legend";
        private const string LineModificationsTableTitleXPath = "//body[@id='pgBody']/form/div/table/tbody/tr[5]/td/fieldset/legend";
        private const string ProcessingHistoryTableTitleXPath = "//body[@id='pgBody']/form/div/table/tbody/tr[7]/td/fieldset/legend";
        private const string ProcessingCloseButtonXPath = "//img[contains(@src, '_Images/Btn_Close_Gray_Big.jpg')]";

        #endregion

        #region NEGOTIATION SECTION

        private const string NegotiationTitleXPath = "//div[@id='Label4']//table[3]//tr[2]//table[1]//table[1]//tr[1]//td";
        private const string NotesXPath = "//div[@id='Label4']//table[3]//tr[2]//table[1]//table[1]//tr[3]//td[1]";

        public const string NegotiationTextAreaId = "Neg_Notes";
        #endregion

        #region PEND SECTION

        private const string PendSectionTitleXPath = "//div[contains(@id, '0')]//table[1]//table[1]//tr[1]//td[1]";
        public const string PendSectionXPath = "//div[contains(@id, '0')]/table/tbody/tr/td[@class='lightTextP']";
        #endregion

        #region MODIFYALL SECTION

        private const string ModifyAllTitleXPath = "//div[@id='Label4']//table[3]//tr[2]//table[1]//table[1]//tr[1]//td[2]";
        private const string ReasonLabelXPath = "//div[@id='Label4']//table[3]//tr[2]//table[1]//table[1]//tr[4]//td[1]";
        private const string DeleteLabelXPath = "//div[@id='Label4']//table[3]//tr[2]//table[1]//table[1]//tr[4]//td[3]";
        private const string RestoreLabelXPath = "//div[@id='Label4']//table[3]//tr[2]//table[1]//table[1]//tr[4]//td[5]";
        private const string NotesLabelXPath = "//div[@id='Label4']//table[3]//tr[2]//table[1]//table[1]//tr[6]//td[1]";
        
        public const string NotesTextAreaId = "Notes0";
        public const string ReasonComboBoxId = "Reason0";
        public const string RestoreCheckBoxId = "ResFlag0";
        public const string DeleteCheckBoxId = "DelFlag0";

        #endregion

        #region ADDEDITS SECTION

        private const string AddEditsTitleXPath = "//div[@id='Label4']//table[3]//tr[2]//table[1]//table[1]//tr[1]//td[2]";
        private const string NotesLabelAddEditsSectionXPath = "//div[@id='Label4']//table[3]//tr[2]//table[1]//table[1]//tr[13]//td[1]";
        private const string EditSrcLabelXPath = "//div[@id='Label4']//table[3]//tr[2]//table[1]//table[1]//tr[9]//td[2]//tr[1]";

        public const string EditSrcComboBoxId = "Add-EditSrc";
        public const string LabelXPath = "//div[@id='Label4']//table[3]//tr[2]//table[1]//table[1]//tr[4]//td";
        public const string AddLineComboBoxId = "Add-Line";
        public const string AddEditComboBoxId = "Add-Edit";
        public const string AddSugPaidTextAreaId = "Add-SugPaid";
        public const string AddSugProcTextAreaId = "Add-SugProc";
        public const string NotesTextAreaAddEditSectionId = "ADD-Notes";

        #endregion

        #region LOGIC SECTION

        private const string LButtonXPath = "//img[contains(@src, '_images/L.jpg')]";
        public const string LButtonXPathTemplate = "//div[@id='Label4']//table[3]//tr[3]//table[1]//tr[{0}]//td[8]//img[contains(@src, '_images/L.jpg')]";
        public const string LogicSectionDivXPathTemplate = "//div[contains(@id, '{0}')]";
        public const string LogicSectionTitleXPathTemplate = "//div[contains(@id, '{0}')]//table[1]//table[1]//tr[1]//td[2]";

        #endregion

        #region DXCODE

        public const string DxCodeXPathTemplate = "//div[@id='Label4']//table[1]//table[1]//tr[{0}]//td[2]//a";

        #endregion

        #region PROCCODE

        public const string ProcCodeXPathTemplate = "//div[@id='Label4']//table[2]//table[1]//tr[{0}]//td[7]//a";
        #endregion

        #region REVCODE
        public const string RevCodeXPathTemplate = "//div[@id='Label4']//table[2]//table[1]//tr[{0}]//td[6]//a";
        #endregion

        #region INVOICE SECTION

        private const string InvoiceTitleXPath = "//fieldSet//legend";
        private const string InvoiceHeaderXPath = "//fieldSet//th[contains(@class,'darkText')]";
        
        public const string InvoiceCloseButtonXPath = "//img[contains(@src, '_Images/Btn_Close_Gray_Big.jpg')]";
        public const string InvoiceLabelsXPath = "//fieldSet//tr//span[contains(@class,'Titles')]";
        public const string InvoiceLightTextLabelsXPath = "//fieldSet//td[contains(@class,'lightText')]";

        #endregion

        public const string EditLinkXPath = "//div[@id='Label4']//table[3]//tr[3]//table[1]//tr//td[2]//a";
        public const string EditLinkXPathTemplate = "//a[text() = '{0}']";
        private const string SearchLinkXPath = "//span[@id='PreAuthSearchID']/span/a";
        public const string PreAuthorizationSearchWindowName = "Auth";
        public const string OriginalDataWindowName = "Header";

        #endregion

        #region PAGEOBJECT PROPERTIES

        [FindsBy(How = How.XPath, Using = ProcessingCloseButtonXPath)]
        public ImageButton ProcessingCloseButton;

        [FindsBy(How = How.Id, Using = ProcessingSectionFrameId)]
        public TextLabel ProcessingSectionFrame;

        [FindsBy(How = How.XPath, Using = LineModificationsTableTitleXPath)]
        public TextLabel LineModificationsTableTitle;

        [FindsBy(How = How.XPath, Using = ProcessingHistoryTableTitleXPath)]
        public TextLabel ProcessingHistoryTableTitle;

        [FindsBy(How = How.XPath, Using = SystemModificationsTableTitleXPath)]
        public TextLabel SystemModificationsTableTitle;

        [FindsBy(How = How.XPath, Using = ClaimHistoryTableTitleXPath)]
        public TextLabel ClaimHistoryTableTitle;

        [FindsBy(How = How.XPath, Using = ProcessingButtonXPath)]
        public Link ProcessingLink;

        [FindsBy(How = How.XPath, Using = InvoiceCloseButtonXPath)]
        public ImageButton InvoiceCloseButton;

        [FindsBy(How = How.XPath, Using = InvoiceTitleXPath)]
        public TextLabel InvoiceTitleTxtLbl;

        [FindsBy(How = How.XPath, Using = InvoiceHeaderXPath)]
        public TextLabel InvoiceHeaderTxtLbl;

        [FindsBy(How = How.XPath, Using = NotesLabelAddEditsSectionXPath)]
        public TextLabel NotesLabelAddEditsSection;

        [FindsBy(How = How.XPath, Using = DataButtonXPath)]
        public Link DataButton;

        [FindsBy(How = How.XPath, Using = SearchLinkXPath)]
        public Link SearchLink;

        [FindsBy(How = How.XPath, Using = AddEditsTitleXPath)]
        public TextLabel AddEditsTitleTxtLbl;

        [FindsBy(How = How.XPath, Using = EditSrcLabelXPath)]
        public TextLabel EditsSrcTxtLbl;

        [FindsBy(How = How.XPath, Using = RestoreLabelXPath)]
        public TextLabel RestoreTxtLbl;

        [FindsBy(How = How.XPath, Using = NotesLabelXPath)]
        public TextLabel NotesTxtLbl;

        [FindsBy(How = How.XPath, Using = ModifyAllTitleXPath)]
        public TextLabel ModifyAllTitleTxtLbl;

        [FindsBy(How = How.XPath, Using = ReasonLabelXPath)]
        public TextLabel ReasonTxtLbl;

        [FindsBy(How = How.XPath, Using = DeleteLabelXPath)]
        public TextLabel DeleteTxtLbl;

        [FindsBy(How = How.XPath, Using = PendSectionTitleXPath)]
        public TextLabel PendSectionTitleTxtLbl;

        [FindsBy(How = How.XPath, Using = NegotiationTitleXPath)]
        public TextLabel NegotiationTitleTxtLbl;

        [FindsBy(How = How.XPath, Using = NotesXPath)]
        public TextField NotesTxtFld;

        [FindsBy(How = How.XPath, Using = SixtyDaysButtonXPath)]
        public Link SixtyDaysButton;

        [FindsBy(How = How.XPath, Using = DocumentButtonXPath)]
        public Link DocumentButton;

        [FindsBy(How = How.XPath, Using = SameDayButtonXPath)]
        public Link SameDayButton;

        [FindsBy(How = How.XPath, Using = ProviderButtonXPath)]
        public Link ProviderButton;

        [FindsBy(How = How.XPath, Using = AllButtonXPath)]
        public Link AllButton;

        [FindsBy(How = How.XPath, Using = TwelveMonthButtonXPath)]
        public Link TwelveMonthButton;

        [FindsBy(How = How.XPath, Using = AppealButtonXPath)]
        public ImageButton AppealButton;

        [FindsBy(How = How.XPath, Using = InvoiceButtonXPath)]
        public ImageButton InvoiceButton;

        [FindsBy(How = How.XPath, Using = CloseButtonXPath)]
        public ImageButton CloseButton;

        [FindsBy(How = How.XPath, Using = LButtonXPath)]
        public ImageButton LButton;

        [FindsBy(How = How.XPath, Using = PButtonXPath)]
        public ImageButton PButton;

        [FindsBy(How = How.XPath, Using = SButtonXPath)]
        public ImageButton SButton;

        [FindsBy(How = How.XPath, Using = ClaimSequenceXPath)] 
        public TextLabel ClaimSequenceTextLabel;

        [FindsBy(How = How.XPath, Using = BatchIdXPath)] 
        public TextLabel BatchIdTextLabel;

        [FindsBy(How = How.XPath, Using = AButtonXPath)]
        public ImageButton AButton;

        [FindsBy(How = How.XPath, Using = MButtonXPath)]
        public ImageButton MButton;

        #endregion

        #region OVERRIDE PROPERTIES

        public override string PageTitle
        {
            get
            {
                var product = StartLegacy.Product.GetStringValue();
                return product.Equals("Fraud Finder Pro") ? "Claim Summary" : string.Format(PageTitleEnum.ClaimSummary.GetStringValue(), product);
            }
        }

        #endregion

        #region CONSTRUCTOR

        public ClaimSummaryPageObjects()
            : base(ProductPageUrlEnum.ClaimSummary.GetStringValue())
        {
        }

        #endregion
    }
}
