using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Legacy.Service.PageObjects;
using UIAutomation.Framework.Core.Navigation;

namespace Legacy.Service.PageActions
{
    public class BatchListPageAction : BasePageAction
    {
        private  BatchListPage _batchListPage;

        public BatchListPageAction(INavigator navigator, BatchListPage batchListPage)
            : base(navigator, batchListPage)
        {
            // Just for performance!
            _batchListPage = (BatchListPage)_pageObject;
        }


        public override IPageAction GoBack()
        {
            Navigator.Back();
            if (Navigator.CurrentUrl.StartsWith(_pageObject.PageUrl))
                return this;
            return new PhysicianClaimInsightPageAction(Navigator, new PhysicianClaimInsightPage());
        }
    }
}
