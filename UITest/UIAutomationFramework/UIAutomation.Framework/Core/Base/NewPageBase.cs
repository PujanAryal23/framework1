using System.Security.Policy;
using UIAutomation.Framework.Core.Driver;
namespace UIAutomation.Framework.Core.Base
{
    /// <summary>
    /// Provides the base for PageObjects.
    /// </summary>
    public abstract class NewPageBase
    {
        #region PRIVATE FIELDS

        /// <summary>
        /// Read only page url.
        /// </summary>
        private string _pageUrl;

        public ISiteDriver SiteDriver;

        #endregion


        #region CONSTRUCTORS

        /// <summary>
        /// Initializes the instance of page.
        /// </summary>
        protected NewPageBase()
            : this(string.Empty)
        {
        }

        /// <summary>
        /// Initializes the instance of page.
        /// </summary>
        /// <param name="pageUrl">Url of a page.</param>
        protected NewPageBase(string pageUrl)
            : this(string.Empty, pageUrl)
        {
        }

        /// <summary>
        /// Intializes the instance of page.
        /// </summary>
        /// <param name="baseUrl">A base url of a page.</param>
        /// <param name="pageUrl">A page url.</param>
        protected NewPageBase(string baseUrl, string pageUrl)
        {
            _pageUrl = baseUrl + pageUrl;
        }

        #endregion

        #region PUBLIC PROPERTIES

        /// <summary>
        /// Gets a page url.
        /// </summary>
        public string PageUrl
        {
            get { return _pageUrl; }
        }

        /// <summary>
        /// Gets or sets page title. 
        /// </summary>
        public virtual string PageTitle { get; set; }

        #endregion

        #region Methods


        public void SetPageBase()
        {
            if (_pageUrl != "chrome://downloads/")
            {
                _pageUrl =  SiteDriver.BaseUrl + _pageUrl;
            }
            if (null != PageTitle)
                SiteDriver.WaitForCorrectPageToLoad(PageTitle);
        }
        #endregion
    }
}