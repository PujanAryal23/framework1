using System.Collections.Generic;
using Legacy.Service.PageObjects;
using Legacy.Service.PageObjects.ATS;
using Legacy.Service.PageServices.Base;
using Legacy.Service.PageServices.Default;
using UIAutomation.Framework.Core.Navigation;

namespace Legacy.Service.PageServices.ATS
{
  public class AtsSearchPage : DefaultPage
  {
    private readonly AtsSearchPageObjects _atsSearchPage;
    private const int StartIndex = 37;

    public AtsSearchPage(INavigator navigator, AtsSearchPageObjects atsSearchPage)
      : base(navigator, atsSearchPage)
    {
      // Just for performance!
      _atsSearchPage = (AtsSearchPageObjects)PageObject;
    }

    public override IPageService GoBack()
    {
      Navigator.Back();
      if (Navigator.CurrentUrl.StartsWith(PageObject.PageUrl))
        return this;
      return new AtsPage(Navigator, new AtsPageObjects());
    }

    public void SelectClient(string clientName)
    {
      _atsSearchPage.DdlClientCode.SelectByText(clientName);
    }

    public void SelectAppealStatus(string status)
    {
      _atsSearchPage.DdlStatus.SelectByText(status);
    }

    public AtsSearchPage StartSearch()
    {
      var atsSearchPage = Navigator.Navigate<AtsSearchPageObjects>(_atsSearchPage.BtnSearch.Click);
      return new AtsSearchPage(Navigator, atsSearchPage);
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
