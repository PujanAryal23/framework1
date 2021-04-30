using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nucleus.Service.Support.Enum;
using Nucleus.Service.Support.Utils;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Base;
using UIAutomation.Framework.Elements;

namespace Nucleus.Service.PageObjects.Provider
{
    public class NewProviderScoreCardPageObjects : NewPageBase
    {

        #region PRIVATE FIELDS

        private const string ScoreCardId = "";

        #endregion

        #region PAGEOBJECT PROPERTIES

        //[FindsBy(How = How.Id, Using = ScoreCardId)]
        //public TextLabel ScoreCard;

        #endregion

        #region OVERRIDE PROPERTIES

        public override string PageTitle
        {
            get { return PageTitleEnum.ProviderScoreCard.GetStringValue(); }
        }

        #endregion

        #region CONSTRUCTOR

        public NewProviderScoreCardPageObjects()
            : base(PageUrlEnum.NewProviderScoreCard.GetStringValue())
        {
        }

        #endregion
    }
}
