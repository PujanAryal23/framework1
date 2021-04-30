using Nucleus.Service.Support.Utils;
using UIAutomation.Framework.Core.Base;
using Nucleus.Service.Support.Enum;


namespace Nucleus.Service.PageObjects.Claim
{
    public class PatientBillHistoryPageObjects : PageBase
    {
        #region PRIVATE/PUBLIC FIELDS

        
        #endregion

        #region PAGEOBJECTS PROPERTIES

        

        #endregion

        #region PROTECTED PROPERTIES

        public override string PageTitle
        {
            get { return PageTitleEnum.ExtendedPageBillHistory.GetStringValue(); }
        }

        #endregion

        #region CONSTRUCTOR

        public PatientBillHistoryPageObjects()
            : base(PageUrlEnum.ClaimPatientHistory.GetStringValue())
        {
        }

        

        #endregion
    }
}
