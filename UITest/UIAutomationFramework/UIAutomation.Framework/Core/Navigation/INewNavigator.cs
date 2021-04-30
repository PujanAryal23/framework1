using System;
using System.Security.Policy;
using UIAutomation.Framework.Common.Constants;
using UIAutomation.Framework.Core.Base;
using UIAutomation.Framework.Core.Driver;

namespace UIAutomation.Framework.Core.Navigation
{
    /// <summary>
    /// Defines an interface implements <see cref="IDisposable"/>. It supports in Flow Pattern <see cref="Navigator"/>
    /// </summary>
    public interface INewNavigator : IDisposable
    {
        /// <summary>
        /// Initializes the current page.
        /// </summary>
        /// <typeparam name="T">The generice page <see cref="Type"/> to be initialized.</typeparam>
        /// <returns>The object of the current page.</returns>
        T Open<T>() where T : NewPageBase, new();

        /// <summary>
        /// Navigates to the page and intializes the current page.
        /// </summary>
        /// <typeparam name="T">The generic page <see cref="Type"/> to be navigaed and initialzed.</typeparam>
        /// <param name="action">The object of <see cref="Action"/> a delegate.</param>
        /// <returns>The object of the current page. </returns>
        T Navigate<T>(Action action) where T : NewPageBase, new();

        /// <summary>
        /// Navigatest to the page and initialized the current page having parameterized contructor.
        /// </summary>
        /// <typeparam name="T">The generic page <see cref="Type"/> to be navigaed and initialzed.</typeparam>
        /// <param name="action">The object of <see cref="Action"/> a delegate.</param>
        /// <param name="pageBase">A delegate that has one parameter and returns the value of the specified by the TResult parameter.</param>
        /// <returns>The object of the current page.</returns>
        T Navigate<T>(Action action, Func<T> pageBase) where T : NewPageBase, new();

        

        /// <summary>
        /// Switches handles to the current page.
        /// </summary>
        /// <typeparam name="T">The generice page <see cref="Type"/> to be initialized.</typeparam>
        /// <param name="windowName">The name of a page or handle.</param>
        /// <returns>The object of the current page.</returns>
        T SwitchWindow<T>(string windowName) where T : NewPageBase, new();

        /// <summary>
        /// Closes the current page.
        /// </summary>
        /// <param name="windowName">The name of the page to be opened or passed the handles after closing the current page.</param>
        void CloseWindow(string windowName);

        /// <summary>
        /// Explicit waits.
        /// </summary>
        /// <param name="t">Timeout value.</param>
        void Sleep(int t);

        /// <summary>
        /// Waits until ajax request completed.
        /// </summary>
        /// <param name="action">The object of <see cref="Action"/>.</param>
        /// <param name="library">The type of <see cref="ScriptConstants"/></param>
        void WaitForAjaxToLoad(Action action, string library);

        /// <summary>
        /// Waits until the certain function completed.
        /// </summary>
        /// <param name="f">An member of <see cref="Delegate"/>.</param>
        void WaitForCondition(Func<bool> f);

        /// <summary>
        /// Gets the window handles of the open browser window.
        /// </summary>
        /// <param name="index">The index of window handles.</param>
        /// <returns>The window handle of the open browser window.</returns>
        string GetWindow(int index);

        /// <summary>
        ///  Gets the <see cref="Url"/> the browser is currently displaying.
        /// </summary>
        string CurrentUrl { get; }

        /// <summary>
        /// Retrieves the message of a JavaScript alert.
        /// </summary>
        /// <returns>A value from alert box.</returns>
        string GetAlertBoxText();

        /// <summary>
        /// Checks if alert box is present
        /// </summary>
        /// <returns>A boolean value depends on the display of alert box.</returns>
        bool IsAlertBoxPresent();

        /// <summary>
        /// Accepts the alert. 
        /// </summary>
        void AcceptAlertBox();

        /// <summary>
        /// Dismisses the alert. 
        /// </summary>
        void DismissAlertBox();

        /// <summary>
        /// Switches to a frame.
        /// </summary>
        /// <param name="frameName">A frame name of the page.</param>
        void SwitchFrame(string frameName);

        /// <summary>
        /// Closes current page frame.
        /// </summary>
        /// <param name="frameName">A frame of the page.</param>
        void CloseFrame(string frameName);

        /// <summary>
        /// Refresh the current brower window.
        /// </summary>
        void Refresh();

        /// <summary>
        /// Move back a single entry in the browser's history.
        /// </summary>
        void Back();

        void Start(IBrowserOptions browserOptions);

        ISiteDriver SiteDriver { get; }
    }
}