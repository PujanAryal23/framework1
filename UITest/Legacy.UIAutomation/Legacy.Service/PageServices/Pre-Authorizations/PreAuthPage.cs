using Legacy.Service.PageObjects.Pre_Authorizations;
using DCIPageObjects = Legacy.Service.PageObjects.Pre_Authorizations.DCI;
using PCIPageObjects = Legacy.Service.PageObjects.Pre_Authorizations.PCI;
using Legacy.Service.PageServices.Base;
using Legacy.Service.PageServices.Default;
using Legacy.Service.Support.Enum;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Core.Navigation;
using UIAutomation.Framework.Elements;

namespace Legacy.Service.PageServices.Pre_Authorizations
{
    public class PreAuthPage : DefaultPage
    {
        #region PRIVATE FIELDS

        private PreAuthPageObjects _preAuthPage;

        #endregion

        #region CONSTRUCTOR

        public PreAuthPage(INavigator navigator, PreAuthPageObjects preAuthPage)
            : base(navigator, preAuthPage)
        {
            _preAuthPage = (PreAuthPageObjects)PageObject;
        }

        #endregion

        #region PUBLIC METHODS

        /// <summary>
        /// Get Pre-Authorizations window handle
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        public PreAuthPage GetWindowHandle(out string handle)
        {
            return GetCurrentWindowHandle<PreAuthPage>(_preAuthPage, out handle);
        }

        /// <summary>
        /// Click unreviewed button
        /// </summary>
        /// <returns></returns>
        public UnreviewedPage ClickUnreviewedButton()
        {
            _preAuthPage.UnreviewedButton.Click();
            return new UnreviewedPage(Navigator, new UnreviewedPageObjects());
        }

        /// <summary>
        /// Click document review button
        /// </summary>
        /// <returns></returns>
        public DocumentReviewPage ClickDocumentReviewButton()
        {
            _preAuthPage.DocumentReviewButton.Click();
            return new DocumentReviewPage(Navigator, new DocumentReviewPageObjects());
        }

        /// <summary>
        /// Click Pended Button
        /// </summary>
        /// <returns></returns>
        public PendedPage ClickPendedButton()
        {
            _preAuthPage.PendedButton.Click();
            return new PendedPage(Navigator, new PendedPageObjects());
        }

        /// <summary>
        /// Click Consult Required Button
        /// </summary>
        /// <returns></returns>
        public DCI.ConsultRequiredPage ClickConsultRequiredButton()
        {
            _preAuthPage.ConsultRequiredButton.Click();
            
            return new DCI.ConsultRequiredPage(Navigator, new DCIPageObjects.ConsultRequiredPageObjects());
        }

        /// <summary>
        /// Click Consult Complete Button
        /// </summary>
        /// <returns></returns>
        public DCI.ConsultCompletePage ClickConsultCompleteButton()
        {
            _preAuthPage.ConsultCompleteButton.Click();
            
            return new DCI.ConsultCompletePage(Navigator, new DCIPageObjects.ConsultCompletePageObjects());
        }

        /// <summary>
        /// Click Hci Consult Complete Button
        /// </summary>
        /// <returns></returns>
        public DCI.HCIConsultCompletePage ClickHciConsultCompleteButton()
        {
            _preAuthPage.HciConsultCompleteButton.Click();
            
            return new DCI.HCIConsultCompletePage(Navigator, new DCIPageObjects.HCIConsultCompletePageObjects());
        }
       
        /// <summary>
        /// Click Hci Consult Required Button
        /// </summary>
        /// <returns></returns>
        public DCI.HCIConsultRequiredPage ClickHciConsultRequiredButton()
        {
            _preAuthPage.HciConsultRequiredButton.Click();
            
            return new DCI.HCIConsultRequiredPage(Navigator, new DCIPageObjects.HCIConsultRequiredPageObjects());
        }

        /// <summary>
        /// Click search button
        /// </summary>
        /// <returns></returns>
        public IPageService ClickSearchButton()
        {
            _preAuthPage.SearchButton.Click();
            if(StartLegacy.Product.Equals(ProductEnum.PCI))
                return new PCI.SearchPage(Navigator, new PCIPageObjects.SearchPageObjects());
            return new DCI.SearchPage(Navigator, new DCIPageObjects.SearchPageObjects());
        }

        /// <summary>
        /// Click Logic Requests Button
        /// </summary>
        /// <returns></returns>
        public LogicRequestsPage ClickLogicRequestsButton()
        {
            _preAuthPage.LogicRequestsButton.Click();
            return new LogicRequestsPage(Navigator, new LogicRequestsPageObjects());
        }

        /// <summary>
        /// Click Closed Button
        /// </summary>
        /// <returns></returns>
        public IPageService ClickClosedButton()
        {
            _preAuthPage.ClosedButton.Click();
            if (StartLegacy.Product.Equals(ProductEnum.PCI))
                return new PCI.ClosedPage(Navigator, new PCIPageObjects.ClosedPageObjects());
            return new DCI.ClosedPage(Navigator, new DCIPageObjects.ClosedPageObjects());
        }

        #endregion
    }
}
