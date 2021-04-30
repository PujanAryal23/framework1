using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Legacy.Service.PageObjects;
using Legacy.Service.PageObjects.Common;
using Legacy.Service.PageObjects.Default;
using Legacy.Service.PageObjects.Product;
using Legacy.Service.PageObjects.Welcome;
using Legacy.Service.PageServices.Base;
using Legacy.Service.PageServices.Common;
using Legacy.Service.PageServices.Product;
using Legacy.Service.PageServices.Welcome;
using Legacy.Service.Support.Enum;
using UIAutomation.Framework.Common;
using UIAutomation.Framework.Core.Base;
using UIAutomation.Framework.Core.Navigation;
using UIAutomation.Framework.Core.Driver;
using UIAutomation.Framework.Elements;
using UIAutomation.Framework.Utils;

namespace Legacy.Service.PageServices.Default
{
    public class DefaultPage : BasePageService
    {
        #region PRIVATE FIELDS

        private readonly DefaultPageObjects _defaultPage;

        #endregion

        #region CONSTRUCTOR

        public DefaultPage(INavigator navigator, PageBase pageObject)
            : base(navigator, pageObject)
        {
            _defaultPage = (DefaultPageObjects)pageObject;
        }

        #endregion

        public override int GetHashCode()
        {
            var hashCode = 0;
            if (_defaultPage != null)
                hashCode = _defaultPage.GetHashCode();
            return hashCode;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            return string.Equals(this.ToString(), obj.ToString());
        }

        public ProductPage ClickOnBack(ProductEnum productEnum)
        {
            switch (productEnum)
            {
                case ProductEnum.DCI:
                    return NavigateToDentalClaimInsightPage();
                case ProductEnum.FCI:
                    return NavigateToFacilityClaimInsightPage();
                default:
                    return NavigateToPhysicianClaimInsightPage();
            }
        }

        #region NAVIGATION

        public PhysicianClaimInsightPage NavigateToPhysicianClaimInsightPage()
        {
            var physicianClaim = Navigator.Navigate<PhysicianClaimInsightPageObjects>(() =>
                                                                                   {
                                                                                       SiteDriver.FindElement<Link>(DefaultPageObjects.BackButtonXPath, How.XPath).Click();
                                                                                       Console.Out.WriteLine(
                                                                                           "Click on Back Button.");
                                                                                       
                                                                                   });
            return new PhysicianClaimInsightPage(Navigator, physicianClaim);
        }

        public DentalClaimInsightPage NavigateToDentalClaimInsightPage()
        {
            var dentalClaim = Navigator.Navigate<DentalClaimInsightPageObjects>(() =>
                                                                                    {
                                                                                        SiteDriver.FindElement<Link>(DefaultPageObjects.BackButtonXPath,How.XPath).Click();
                                                                                        Console.Out.WriteLine(
                                                                                            "Click on Back Button.");
                                                                                        
                                                                                    });
            return new DentalClaimInsightPage(Navigator, dentalClaim);
        }

        public FacilityClaimInsightPage NavigateToFacilityClaimInsightPage()
        {
            var facilityClaim = Navigator.Navigate<FacilityClaimInsightPageObjects>(() =>
                                                                                        {
                                                                                            SiteDriver.FindElement<Link>(DefaultPageObjects.BackButtonXPath, How.XPath).Click();
                                                                                            Console.Out.WriteLine(
                                                                                                "Click on Back Button.");
                                                                                            
                                                                                        });
            return new FacilityClaimInsightPage(Navigator, facilityClaim);
        }

        public WelcomePage NavigateToWelcomePage()
        {
            while (!SiteDriver.IsElementPresent(DefaultPageObjects.WelcomePageButtonXPath, How.XPath))
            {
                NavigateToBackPage();
            }
            var welcome = Navigator.Navigate<WelcomePageObjects>(() =>
            {
                SiteDriver.FindElement<Link>(DefaultPageObjects.WelcomePageButtonXPath, How.XPath).Click();
                Console.Out.WriteLine(
                    "Click on Welcome Button.");
                
            });
            return new WelcomePage(Navigator, welcome);
        }

        public void Logout()
        {
            SiteDriver.FindElement<Link>(DefaultPageObjects.LogoutButtonXPath, How.XPath).Click();
            Console.Out.WriteLine(
                "Click on Logout Button.");
            if(SiteDriver.IsAlertBoxPresent())
                SiteDriver.AcceptAlertBox();
            
        }

        public int MonthStringToNumeric(string month)
        {
            int month_numeric = 1;
            switch (month)
            {
                case "January":
                    month_numeric = 1;
                    break;
                case "February":
                    month_numeric = 2;
                    break;
                case "March":
                    month_numeric = 3;
                    break;
                case "April":
                    month_numeric = 4;
                    break;
                case "May":
                    month_numeric = 5;
                    break;
                case "June":
                    month_numeric = 6;
                    break;
                case "July":
                    month_numeric = 7;
                    break;
                case "August":
                    month_numeric = 8;
                    break;
                case "September":
                    month_numeric = 9;
                    break;
                case "October":
                    month_numeric = 10;
                    break;
                case "November":
                    month_numeric = 11;
                    break;
                case "December":
                    month_numeric = 12;
                    break;
            }
            return month_numeric;
        }

        public void NavigateToBackPage()
        {
            _defaultPage.BackButton.Click();
        }

        public void ClickOnBackButton()
        {
            _defaultPage.BackButton.Click();
             
            //var target = typeof (T);
            //return (T) target.GetMethod("ClickOnBackButton").Invoke(PageObject,null);
        }

        public void ClickToOpenACalendar(Link calendarLink)
        {
            
            calendarLink.Click();
            Console.WriteLine("Calender Icon Clicked");
            
        }
     
        public void ClickOnDate(DateTime date)
        {
            var month = SiteDriver.FindElement<TextLabel>(DefaultPageObjects.MonthCssLocator, How.CssSelector).Text;
            var year = SiteDriver.FindElement<TextLabel>(DefaultPageObjects.YearCssLocator, How.CssSelector).Text;
            var timesToClick = (int.Parse(year) - date.Year)*12 + MonthStringToNumeric(month) - date.Month;
            if(timesToClick>0)
                for (int i = 1; i <= timesToClick; i++)
                {
                    JavaScriptExecutor.ExecuteClick(DefaultPageObjects.PreviousMonthCssLocator,How.CssSelector);
                    //SiteDriver.WaitToLoad(50);
                }
            else
            for (int i = 1; i <=Math.Abs( timesToClick); i++)
                {
                    JavaScriptExecutor.ExecuteClick(DefaultPageObjects.NextMonthCssLocator,How.CssSelector);
                   // SiteDriver.WaitToLoad(50);
                }
            JavaScriptExecutor.ExecuteClick(string.Format( DefaultPageObjects.DateXPathTemplate,date.Day),How.XPath);
        }





        public T GetDate<T>(object pageObject, TextField dateField, out DateTime dateValue)
        {
            DateTime.TryParse(dateField.GetAttribute("value"), out dateValue);
            return (T)Activator.CreateInstance(typeof(T), Navigator, pageObject);
        }



        #endregion

        #region PRIVATE METHODS

        #endregion
    }
}
