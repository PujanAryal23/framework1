using System.Collections.Generic;
using Legacy.Service.PageObjects;
using UIAutomation.Framework.Core.Navigation;

namespace Legacy.Service.PageActions
{
  public class AtsSearchPageAction : BasePageAction
  {
    private readonly AtsSearchPage _atsSearchPage;
    private const int StartIndex = 37;

    public AtsSearchPageAction(INavigator navigator, AtsSearchPage atsSearchPage)
      : base(navigator, atsSearchPage)
    {
      // Just for performance!
      _atsSearchPage = (AtsSearchPage)_pageObject;
    }

    public override IPageAction GoBack()
    {
      Navigator.Back();
      if (Navigator.CurrentUrl.StartsWith(_pageObject.PageUrl))
        return this;
      return new AtsPageAction(Navigator, new AtsPage());
    }

    public void SelectClient(string clientName)
    {
      _atsSearchPage.DdlClientCode.SelectByText(clientName);
    }

    public void SelectAppealStatus(string status)
    {
      _atsSearchPage.DdlStatus.SelectByText(status);
    }

    public AtsSearchPageAction StartSearch()
    {
      var atsSearchPage = Navigator.Navigate<AtsSearchPage>(_atsSearchPage.BtnSearch.Click);
      return new AtsSearchPageAction(Navigator, atsSearchPage);
    }

    public List<string> GetResultDataAt(int index)
    {
      return _atsSearchPage.TblResult.GetRow(StartIndex + index);
    }

    public Dictionary<int, List<string>> GetResultData()
    {
      return _atsSearchPage.TblResult.GetRows(StartIndex);
    }
  }
}
