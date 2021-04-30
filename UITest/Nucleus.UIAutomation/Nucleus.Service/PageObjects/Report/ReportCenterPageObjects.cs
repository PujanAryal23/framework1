using Nucleus.Service.PageObjects.Default;
using Nucleus.Service.Support.Enum;
using UIAutomation.Framework.Utils;

namespace Nucleus.Service.PageObjects.Report
{
    public class ReportCenterPageObjects : NewDefaultPageObjects
    {
        #region PROTECTED PROPERTIES

        public override string PageTitle
        {
            get { return PageTitleEnum.ReportCenter.GetStringValue(); }
        }

        #endregion

        #region CONSTRUCTOR

        public ReportCenterPageObjects()
            : base(PageUrlEnum.ReportCenter.GetStringValue())
        {
        }

        #endregion
    }
}
