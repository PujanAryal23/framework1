using UIAutomation.Framework.Core.Driver;
namespace UIAutomation.Framework.Core.Base
{
    /// <summary>
    /// Provides the base for PageObjects.
    /// </summary>
    public abstract class PageBase
    {
        #region PRIVATE FIELDS

        /// <summary>
        /// Read only page url.
        /// </summary>
        private readonly string _pageUrl;

        #endregion


        #region CONSTRUCTORS

        /// <summary>
        /// Initializes the instance of page.
        /// </summary>
        protected PageBase()
            : this(string.Empty)
        {
        }

        /// <summary>
        /// Initializes the instance of page.
        /// </summary>
        /// <param name="pageUrl">Url of a page.</param>
        protected PageBase(string pageUrl)
            : this(SiteDriver.BaseUrl, pageUrl)
        {
        }

        /// <summary>
        /// Intializes the instance of page.
        /// </summary>
        /// <param name="baseUrl">A base url of a page.</param>
        /// <param name="pageUrl">A page url.</param>
        protected PageBase(string baseUrl, string pageUrl)
        {
            _pageUrl = baseUrl + pageUrl;
            if (null != PageTitle)
                SiteDriver.WaitForCorrectPageToLoad(PageTitle);
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
    }
}