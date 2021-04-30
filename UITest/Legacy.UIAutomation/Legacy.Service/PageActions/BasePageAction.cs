using System;
using UIAutomation.Framework.Core.Base;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Core.Navigation;

namespace Legacy.Service.PageActions
{
  public abstract class BasePageAction : IPageAction
  {
    protected PageBase _pageObject;
    private readonly INavigator _navigator;

    protected BasePageAction(INavigator navigator, PageBase page)
    {
      _navigator = navigator;
      _pageObject = (PageBase) SiteDriver.InitializePageElement(page);
    }

    protected INavigator Navigator
    {
      get { return _navigator; }
    }

    public virtual IPageAction GoBack()
    {
      throw new InvalidOperationException("Method 'GoBack' should have been called on this page!");
    }
  }
}