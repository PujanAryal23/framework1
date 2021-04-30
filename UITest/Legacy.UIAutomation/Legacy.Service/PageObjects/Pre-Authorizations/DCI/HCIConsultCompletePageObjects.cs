﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Legacy.Service.PageObjects.Default;
using Legacy.Service.Support.Enum;
using Legacy.Service.Support.Utils;

namespace Legacy.Service.PageObjects.Pre_Authorizations.DCI
{
    public class HCIConsultCompletePageObjects : DefaultPageObjects
    {
        #region OVERRIDE PROPERTIES

        public override string PageTitle
        {
            get { return PageTitleEnum.HciConsultComplete.GetStringValue(); }
        }

        #endregion

        #region CONSTRUCTOR

        public HCIConsultCompletePageObjects()
            : base(string.Format(PageUrlEnum.ListPage.GetStringValue(), StartLegacy.PreAuthorizationProduct))
        {

        }

        #endregion
    }
}