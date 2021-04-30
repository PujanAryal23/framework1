using System;
using Legacy.Service.PageObjects.Pre_Authorizations;
using Legacy.Service.PageObjects.Welcome;
using Legacy.Service.PageServices.Base;
using Legacy.Service.PageServices.Default;
using Legacy.Service.PageServices.Pre_Authorizations;
using Legacy.Service.Support.Utils;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Core.Navigation;
using Legacy.Service.PageObjects.Product;
using Legacy.Service.PageServices.Welcome;
using UIAutomation.Framework.Elements;
using LogicRequestsPageObjects = Legacy.Service.PageObjects.Product.LogicRequestsPageObjects;
using Legacy.Service.Support.Enum;

namespace Legacy.Service.PageServices.Product
{
    public class ProductPage : DefaultPage
    {
        #region PRIVATE FIELDS

        private readonly ProductPageObjects _productPage;

        #endregion

        #region CONSTRUCTORS

        public ProductPage(INavigator navigator, ProductPageObjects productPage)
            : base(navigator, productPage)
        {
            _productPage = (ProductPageObjects)PageObject;
        }

        #endregion

        #region PUBLIC METHODS

        public override IPageService GoBack()
        {
            var welcomePage = Navigator.Navigate<WelcomePageObjects>(() => Navigator.Back());
            return new WelcomePage(Navigator, welcomePage);
        }

        public BatchListPage NavigateToBatchListPage()
        {
            var batchList = Navigator.Navigate<BatchListPageObjects>(() =>
            {
                _productPage.BatchListBtn.Click();
                
            });
            return new BatchListPage(Navigator, batchList);
        }

        public LogicRequestsPage NavigateToLogicRequestsPage()
        {
            var logicRequest = Navigator.Navigate<LogicRequestsPageObjects>(() => _productPage.LogicBtn.Click());
            return new LogicRequestsPage(Navigator, logicRequest);
        }

        public SearchProductPage NavigateToSearchProductPage()
        {
            var searchProduct = Navigator.Navigate<SearchProductPageObjects>(() => _productPage.SearchProductBtn.Click());
            return new SearchProductPage(Navigator, searchProduct);
        }

        public SearchUnreviewedPage NavigateToSearchUnreviewedPage()
        {
            var searchUnreviewed = Navigator.Navigate<SearchUnreviewedPageObjects>(() => _productPage.UnreviewedClaimsBtn.Click());
            return new SearchUnreviewedPage(Navigator, searchUnreviewed);
        }

        public ModifiedEditsPage NavigateToModifiedEditsPage()
        {
            var modifiedEdits = Navigator.Navigate<ModifiedEditsPageObjects>(() => _productPage.ModifiedEditsBtn.Click());
            return new ModifiedEditsPage(Navigator, modifiedEdits);
        }

        public SearchPendedPage NavigateToSearchPendedPage()
        {
            var searchUnreviewed = Navigator.Navigate<SearchPendedPageObjects>(() => _productPage.PendedClaimsBtn.Click());
            return new SearchPendedPage(Navigator, searchUnreviewed);
        }

        public PreAuthPage NavigateToPreAuthorizationsPage()
        {
            var preAuthorization = Navigator.Navigate<PreAuthPageObjects>(() => _productPage.PreAuthorizationLink.Click());
            return new PreAuthPage(Navigator, preAuthorization);
        }

        public DocClaimListPage NavigateToDocClaimListPage(DocumentTypeEnum docType)
        {
            DocClaimListPageObjects docClaimListPage = null;
            switch (docType)
            {
                case DocumentTypeEnum.DocRequired:
                    docClaimListPage = ClickOnDocsToOpenDocClaimListPage(_productPage.DocsRequiredBtn, docType);
                    break;
                case DocumentTypeEnum.DocRequested:
                    docClaimListPage = ClickOnDocsToOpenDocClaimListPage(_productPage.DocsRequestedBtn, docType);
                    break;
                case DocumentTypeEnum.DocReceived:
                    docClaimListPage = ClickOnDocsToOpenDocClaimListPage(_productPage.DocsReceivedBtn, docType);
                    break;
            }
            return new DocClaimListPage(Navigator, docClaimListPage);
        }

        public SearchClearedPage NavigateToSearchClearedPage()
        {
            var searchCleared = Navigator.Navigate<SearchClearedPageObjects>(() => _productPage.SearchClearedLink.Click());
            return new SearchClearedPage(Navigator, searchCleared);
        }

        #endregion

        #region PRIVATE METHODS

        public DocClaimListPageObjects ClickOnDocsToOpenDocClaimListPage(ImageButton docs, DocumentTypeEnum docType)
        {
            return Navigator.Navigate<DocClaimListPageObjects>(() =>
                                          {
                                              docs.Click();
                                              DocClaimListPageObjects.DocClaimListPageTitle = string.Format(PageTitleEnum.DocClaimList.GetStringValue(), docType.GetStringValue());
                                              Console.Out.WriteLine("Click on Docs {0}", docType.GetStringValue());
                                          }
                                         );
        }

        #endregion
    }
}
