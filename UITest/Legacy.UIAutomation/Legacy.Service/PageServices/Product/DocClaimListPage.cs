using System;
using Legacy.Service.PageObjects.Product;
using Legacy.Service.PageServices.Default;
using Legacy.Service.Support.Enum;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Core.Navigation;
using UIAutomation.Framework.Elements;

namespace Legacy.Service.PageServices.Product
{
    public class DocClaimListPage : DefaultPage
    {
        #region PRIVATE/PUBLIC FIELDS

        private readonly DocClaimListPageObjects _docClaimListPage;

        #endregion

        #region CONSTRUCTOR

        public DocClaimListPage(INavigator navigator, DocClaimListPageObjects docClaimListPage)
            : base(navigator, docClaimListPage)
        {
            _docClaimListPage = (DocClaimListPageObjects)PageObject;
        }

        #endregion

        #region PUBLIC METHODS

        /// <summary>
        /// Get search product window handle
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        public DocClaimListPage GetWindowHandle(out string handle)
        {
            return GetCurrentWindowHandle<DocClaimListPage>(_docClaimListPage, out handle);
        }

        /// <summary>
        /// Click claim sequence
        /// </summary>
        /// <param name="claimSeq"></param>
        /// <returns></returns>
        public ClaimSummaryPage ClickClaimSequence(string claimSeq)
        {
            var claimSequence = Navigator.Navigate<ClaimSummaryPageObjects>(()=>
                                                    {
                                                        SiteDriver.FindElement<Link>(string.Format(DocClaimListPageObjects.ClaimSeqXpath, claimSeq), How.XPath).Click();
                                                        Console.WriteLine("Clicked ClaimSequence "+ claimSeq);
                                                    });
            SiteDriver.SwitchFrame("View");
            return new ClaimSummaryPage(Navigator, claimSequence);
        }

        #endregion

        #region PRIVATE METHODS

        #endregion
    }
}
