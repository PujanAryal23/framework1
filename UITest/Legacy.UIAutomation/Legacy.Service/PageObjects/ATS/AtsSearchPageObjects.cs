using Legacy.Service.PageObjects.Default;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Base;
using UIAutomation.Framework.Elements;

namespace Legacy.Service.PageObjects.ATS
{
  public class AtsSearchPageObjects : DefaultPageObjects
  {
    [FindsBy(How = How.XPath, Using = "//img[contains(@src, '../_Images/Btn_Back.jpg')]")]
    public ImageButton BackBtn;

    [FindsBy(How = How.Id, Using = "ddlClientCode")]
    public SelectField DdlClientCode;
    
    [FindsBy(How = How.Id, Using = "ddlStatus")]
    public SelectField DdlStatus;

    [FindsBy(How = How.Id, Using = "btnSearch")]
    public ImageButton BtnSearch;

    [FindsBy(How = How.XPath, Using = "//img[contains(@src, '_Images/Btn_Clear.jpg')]")]
    public ImageButton BtnClear;

    [FindsBy(How = How.Id, Using = "dgResults")]
    public Table TblResult;

    public AtsSearchPageObjects()
      : base("ATS/ATSSearch.aspx")
    { }
  }
}
