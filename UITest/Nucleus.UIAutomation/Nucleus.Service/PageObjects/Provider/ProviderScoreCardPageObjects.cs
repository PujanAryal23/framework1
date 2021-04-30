using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Base;
using Nucleus.Service.Support.Enum;
using UIAutomation.Framework.Utils;
using UIAutomation.Framework.Elements;

namespace Nucleus.Service.PageObjects.Provider
{
    public class ProviderScoreCardPageObjects : NewPageBase
    {
        #region PRIVATE FIELDS

        public const string ScoreCardId = "ctl00_MainContentPlaceHolder_lblTotalScore";
        public const string pageHeaderCSSLocator = "label.title";

        #endregion

        #region PAGEOBJECT PROPERTIES
       
        //[FindsBy(How = How.Id, Using = ScoreCardId)]
        //public TextLabel ScoreCard;

        //[FindsBy(How = How.CssSelector, Using = pageHeaderCSSLocator)]
        //public TextLabel PageHeader;
        
        #endregion

        #region OVERRIDE PROPERTIES

        public override string PageTitle
        {
            get { return PageTitleEnum.ProviderScoreCard.GetStringValue(); }
        }

        #endregion

        #region CONSTRUCTOR

        public ProviderScoreCardPageObjects()
            : base(PageUrlEnum.ProviderScoreCard.GetStringValue())
        {
        }

        #endregion
    }
}
