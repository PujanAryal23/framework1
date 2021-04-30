//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using Nucleus.Service.Support.Common;
//using Nucleus.Service.Support.Common.Constants;
//using Nucleus.Service.Support.Environment;
//using UIAutomation.Framework.Core.Driver;

//namespace Nucleus.UIAutomation.TestSuites.Base
//{
//    public sealed class PaginationTests : NewAutomatedBase
//    {
//        private readonly PaginationService _pagination;

//        public PaginationTests( object pageObject)
//        {
//            _pagination = new PaginationService(pageObject,SiteDriver,);
//        }

//        //public void TestPagination()
//        {
//            //Test pagination
//            StringFormatter.PrintMessageTitle("Test pagination for Last Arrow");
        
//            int totalRecords = _pagination.TotalNoOfRecords;
//            _pagination.ClickOnLastArrow();
//            Console.Out.WriteLine("Verify clicking on last page should display End Record = Total No of Records");
//            int endRecord = _pagination.EndPage;
//            endRecord.ShouldBeEqual(totalRecords, "End Record");
//            StringFormatter.PrintMessageTitle("Test pagination for First Arrow");
//            _pagination.ClickOnFirstArrow();
//            Console.Out.WriteLine("Verify clicking on first page should display Start Record = 1");
//            int startRecord = _pagination.StartPage;
//            startRecord.ShouldBeEqual(1, "Start Record");


//            //if page numbers > 1 , check for next and previous arrow
//            if (_pagination.PageNumbers.Count > 1)
//            {
//                StringFormatter.PrintMessageTitle("Test pagination for Next Arrow");
//                int pageNoBeforeClick = _pagination.CurrentPage;
//                _pagination.ClickNextPage();
//                int pageNoAfterClickonNext = _pagination.CurrentPage;
//                pageNoAfterClickonNext.ShouldBeEqual( pageNoBeforeClick + 1, "Page number before and after click on Next Arrrow ");

//                StringFormatter.PrintMessageTitle("Test pagination for Previous Arrow");
//                pageNoBeforeClick = _pagination.CurrentPage;
//                _pagination.ClickPreviousPage();
//                int pageNoAfterClickonPrevious = _pagination.CurrentPage;
//                pageNoAfterClickonPrevious.ShouldBeEqual(pageNoBeforeClick - 1, "Page number before and after click on Previous Arrrow");

//                StringFormatter.PrintMessageTitle(" Verify can click through pages");
//                int selectedPageSize = _pagination.GetSelectedPageSize();
//                IList<int> numbers = _pagination.PageNumbers;
//                int lastPageNo = numbers.Last();
//                EnvironmentManager.GenereatetUniqueRndNumbers(1, numbers.Count);
//                foreach (int currentPageNo in numbers.Take(4))
//                {
//                    StringFormatter.PrintMessageTitle(string.Format("Current Page No. {0}", currentPageNo));
//                    int pageNum = EnvironmentManager.NewRandomNumber();
//                    _pagination.ClickOnPageNum(pageNum);
//                    int from = (pageNum - 1) * selectedPageSize + 1;
//                    int to = (lastPageNo == pageNum && (from - 1) + selectedPageSize >= totalRecords)
//                                 ? totalRecords
//                                 : selectedPageSize * pageNum;
//                    int startPage = _pagination.StartPage;
//                    int endPage = _pagination.EndPage;
//                    startPage.ShouldBeEqual(from,"Records From");
//                    endPage.ShouldBeEqual(to, "Records To ");
//                }
//            }
//            StringFormatter.PrintLineBreak();
//        }



//    }
//}
