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
    public class ClosedPageObjects: DefaultPageObjects
    {
        #region PRIVATE PROPERTIES

        private const string SearchName = "btnSearch";

        #endregion

        #region PAGEOBJECTS PROPERTIES

        [FindsBy(How = How.Name, Using = SearchName)]
        public ImageButton SearchButton;

        #endregion

        #region OVERRIDE PROPERTIES

        public override string PageTitle
        {
            get { return string.Format(PageTitleEnum.Closed.GetStringValue(), StartLegacy.PreAuthorizationProduct); }
        }

        #endregion

        #region CONSTRUCTOR

        public ClosedPageObjects()
            : base(string.Format(PageUrlEnum.Closed.GetStringValue(), StartLegacy.PreAuthorizationProduct))
        {

        }

        #endregion
    }
}
