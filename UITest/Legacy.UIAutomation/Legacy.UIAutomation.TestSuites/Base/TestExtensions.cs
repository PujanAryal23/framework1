using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Legacy.Service.Support.Common.Constants;
using NUnit.Framework;

namespace Legacy.UIAutomation.TestSuites.Base
{
    public static class TestExtensions
    {
        public static string TestName { get; set; }

        public static T ShouldNotNull<T>(this T obj)
        {
            Assert.IsNull(obj);
            return obj;
        }

        public static T ShouldNotNull<T>(this T obj, string message)
        {
            Assert.IsNull(obj, message);
            return obj;
        }

        public static T ShouldNotBeNull<T>(this T obj)
        {
            Assert.IsNotNull(obj);
            return obj;
        }

        public static T ShouldNotBeNull<T>(this T obj, string message)
        {
            Assert.IsNotNull(obj, message);
            return obj;
        }

        public static T ShouldEqual<T>(this T actual, object expected,string verificationElement,string failureMessage)
        {
            Console.WriteLine("{0}: Expected <{1}> , Actual <{2}>", verificationElement, expected, actual);
            Assert.AreEqual(expected, actual);
            return actual;
        }

        public static void ShouldEqual(this object actual, object expected, string message, bool takeScreenshot = false)
        {
            Console.WriteLine("{0}: Expected <{1}> , Actual <{2}>", message, expected, actual);
            try
            {
                Assert.AreEqual(expected, actual);
            }
            catch (AssertionException ex)
            {
                ThrowAssertionException(message, takeScreenshot, ex);
            }
            Assert.AreEqual(expected, actual);
        }

        public static void ShouldCollectionEqual(this IList<string> actualList, IList<string> expectedList, string message, bool takeScreenshot = false, params object[] args)
        {
            try
            {
                CollectionAssert.AreEqual(expectedList, actualList.ToList(), message, args);
                StringFormatter.PrintMessageTitle(message);
                foreach (var expected in expectedList)
                {
                    string expectedValue = expected;
                    Console.Out.WriteLine("{0} {1} : Expected<{2}>, Actual<{3}> ", args.SingleOrDefault(), expectedList.IndexOf(expectedValue) + 1, expected, (actualList.Contains(expected)) ? actualList.Single(x => x == expectedValue) : string.Empty);
                }
                StringFormatter.PrintLineBreak();
            }
            catch (AssertionException ex)
            {
                ThrowAssertionException(message, takeScreenshot, ex);
            }
        }

        public static Exception ShouldBeThrownBy(this Type exceptionType, TestDelegate testDelegate)
        {
            return Assert.Throws(exceptionType, testDelegate);
        }

        public static void ShouldBe<T>(this object actual)
        {
            Assert.IsInstanceOf<T>(actual);
        }

        public static void ShouldBeNull(this object actual)
        {
            Assert.IsNull(actual);
        }

        public static void ShouldBeTheSameAs(this object actual, object expected)
        {
            Assert.AreSame(expected, actual);
        }

        public static void ShouldBeNotBeTheSameAs(this object actual, object expected)
        {
            Assert.AreNotSame(expected, actual);
        }

        public static T CastTo<T>(this object source)
        {
            return (T)source;
        }

        public static void ShouldBeTrue(this bool condition, string message, bool takeScreenshot = false)
        {
            try
            {
                Console.WriteLine("{0} - Is True ? {1}", message, condition);
                Assert.IsTrue(condition);
            }
            catch (AssertionException ex)
            {
                ThrowAssertionException(message, takeScreenshot, ex);
            }
        }

        public static void ShouldBeFalse(this bool condition, string message, bool takeScreenshot = false)
        {
            try
            {
                Console.WriteLine("{0} - Is False ? {1}", message, condition);
                Assert.IsFalse(condition);
            }
            catch (AssertionException ex)
            {
                ThrowAssertionException(message, takeScreenshot, ex);
            }
        }

        public static void AssertIsContained(this object actualValue, object expectedValue, string message)
        {
            Console.WriteLine("{0} Expected: <{1}> , Actual: <{2}>", message, expectedValue, actualValue);
            if (!(actualValue.ToString().Contains(expectedValue.ToString()) || expectedValue.ToString().Contains(actualValue.ToString())))
                Assert.Fail("Values do not match");
        }

        public static void AssertIsContainedInList(this string actualValue, IList<string> expectedList, string message, bool takeScreenshot = false)
        {
            try
            {
                Console.Out.WriteLine("{0} : Expected<{1}>, Actual<{2}> ", message, actualValue, (expectedList.Contains(actualValue)) ? expectedList.Single(x => x == actualValue) : string.Empty);
                Assert.Contains(actualValue, expectedList.ToList());
            }
            catch (AssertionException ex)
            {
                ThrowAssertionException(message, takeScreenshot, ex);
            }
        }

        public static void AssertDateRange(this DateTime actualDate, DateTime expectedDateBegin, DateTime expectedDateEnd, string message)
        {
            int compareStartDate = DateTime.Compare(actualDate, expectedDateBegin);
            Console.WriteLine("Expected {0} between {1} and {2} . Actual :{3}", message, expectedDateBegin.ToShortDateString(), expectedDateEnd.ToShortDateString(), actualDate.ToShortDateString());
            if (compareStartDate < 0)
                Assert.Fail("Grid Result do not match the search criteria");
            int compareEndDate = DateTime.Compare(expectedDateEnd, actualDate);
            if (compareEndDate < 0)
                Assert.Fail("Grid Result do not match the search criteria");
        }

        public static void AssertIsEqualsOrGreater(this Int16 actualValue, Int16 expectedValue, string message)
        {
            Console.WriteLine("{0} Expected: <{1}> , Actual: <{2}>", message, expectedValue, actualValue);
            if (actualValue < expectedValue)
                Assert.Fail("Values do not match");
        }

        /// <summary>
        /// Compares the two strings (case-insensitive).
        /// </summary>
        /// <param name="actual"></param>
        /// <param name="expected"></param>
        public static void AssertSameStringAs(this string actual, string expected)
        {
            if (!string.Equals(actual, expected, StringComparison.InvariantCultureIgnoreCase))
            {
                var message = string.Format("Expected {0} but was {1}", expected, actual);
                throw new AssertionException(message);
            }
        }

        public static void AssertFail(this object actual, string message)
        {
            Assert.Fail(message);
        }

        public static void ThrowAssertionException(string verificationElement, bool takeScreenshot, Exception ex)
        {
            if (takeScreenshot)
            {
                //EnvironmentManager.CaptureScreenShot(TestName);
               // throw new AssertionException(
                  //  String.Format("Test fail due to :\n{0}\n{1}\nTest failure screenshot location : \n{2}",
                    //              verificationElement, ex.Message, EnvironmentManager.ScreenshotFileName), ex);
            }
            throw new AssertionException(String.Format("Test fail due to :\n{0}\n{1}", verificationElement, ex.Message), ex);
        }
    }
}
