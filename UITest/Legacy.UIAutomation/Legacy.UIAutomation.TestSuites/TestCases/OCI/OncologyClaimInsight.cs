using Legacy.Service.Support.Enum;
using Legacy.UIAutomation.TestSuites.Base;
using NUnit.Framework;

namespace Legacy.UIAutomation.TestSuites.TestCases.OCI
{
    [Category("OCI")]
    public class OncologyClaimInsight : AutomatedBase
    {
        #region PROTECTED PROPERTIES

        protected override string FullyQualifiedClassName
        {
            get
            {
                return GetType().FullName;
            }
        }

        protected override ProductEnum TestProduct
        {
            get
            {
                return ProductEnum.OCI;
            }

        }

        #endregion
    }
}
