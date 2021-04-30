using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Nucleus.Service.PageServices.Base.Default;
using Nucleus.Service.Support.Common.Constants;
using Nucleus.Service.Support.Environment;
using NUnit.Framework;

namespace Nucleus.UIAutomation.TestSuites.Base
{
    public static class TestExtensions
    {
        public static string TestName { get; set; }

        public static NewDefaultPage DefaultPage { get; set; }

        public static T ShouldBeNull<T>(this T obj)
        {
            Assert.IsNull(obj);
            return obj;
        }

        public static T ShouldBeNull<T>(this T obj, string message)
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
        
        public static T ShouldBeEqual<T>(this T actual, object expected)
        {
            Assert.AreEqual(expected, actual);
            return actual;
        }

        public static void ShouldNotBeEmpty(this string actual, string message, bool takeScreenshot = true)
        {
            try
            {
                Console.WriteLine("{0} - Is Empty or Null? <{1}> ", message, actual);
                Assert.That(actual, Is.Not.Null.And.Not.Empty);
                //Assert.IsNotNullOrEmpty(actual);
            }
            catch (AssertionException ex)
            {
                ThrowAssertionException(message, takeScreenshot, ex);
            }


        }

        public static void ShouldBeNullorEmpty(this string actual, string message, bool takeScreenshot = true)
        {
            try
            {
                Console.WriteLine("{0} - Is Empty or Null? <{1}> ", message, actual);
                Assert.That(actual, Is.Null.Or.Empty);
                //Assert.IsNullOrEmpty(actual);
            }
            catch (AssertionException ex)
            {
                ThrowAssertionException(message, takeScreenshot, ex);
            }


        }

        public static void ShouldBeGreater(this int actual, int expected,string message,bool takeScreenshot = false)
        {
            try
            {
                Console.WriteLine("{0} : <{1}> must be  greater than <{2}>", message, actual, expected);
                Assert.Greater(actual, expected);
            }
            catch (AssertionException ex)
            {
                ThrowAssertionException(message, takeScreenshot, ex);
            }
            
            
        }

        public static void ShouldBeGreater(this double actual, double expected, string message, bool takeScreenshot = false)
        {
            try
            {
                Console.WriteLine("{0} : <{1}> must be  greater than <{2}>", message, actual, expected);
                Assert.Greater(actual, expected);
            }
            catch (AssertionException ex)
            {
                ThrowAssertionException(message, takeScreenshot, ex);
            }


        }

        public static void ShouldBeLess(this int actual, int expected, string message, bool takeScreenshot = false)
        {
            try
            {
                Console.WriteLine("{0} : <{1}> must be  less than <{2}>", message, actual, expected);
                Assert.Less(actual, expected);
            }
            catch (AssertionException ex)
            {
                ThrowAssertionException(message, takeScreenshot, ex);
            }


        }

        public static void ShouldBeLessOrEqual(this string actual, string expected, string message, bool takeScreenshot = false)
        {
            try
            {
                Console.WriteLine("{0} : <{1}> must be  less than or equal<{2}>", message, actual, expected);
                Assert.LessOrEqual(actual, expected);
            }
            catch (AssertionException ex)
            {
                ThrowAssertionException(message, takeScreenshot, ex);
            }


        }
        public static void ShouldBeLessOrEqual(this int actual, int expected, string message, bool takeScreenshot = false)
        {
            try
            {
                Console.WriteLine("{0} : <{1}> must be  less than or equal<{2}>", message, actual, expected);
                Assert.LessOrEqual(actual, expected);
            }
            catch (AssertionException ex)
            {
                ThrowAssertionException(message, takeScreenshot, ex);
            }


        }

        public static void ShouldBeGreaterOrEqual(this int actual, int expected, string message, bool takeScreenshot = false)
        {
            try
            {
                Console.WriteLine("{0} : <{1}> must be  less than or greater<{2}>", message, actual, expected);
                Assert.GreaterOrEqual(actual, expected);
            }
            catch (AssertionException ex)
            {
                ThrowAssertionException(message, takeScreenshot, ex);
            }


        }

        public static void ShouldBeGreaterOrEqual(this string actual, string expected, string message, bool takeScreenshot = false)
        {
            try
            {
                Console.WriteLine("{0} : <{1}> must be  less than or greater<{2}>", message, actual, expected);
                Assert.GreaterOrEqual(actual, expected);
            }
            catch (AssertionException ex)
            {
                ThrowAssertionException(message, takeScreenshot, ex);
            }


        }
        public static void ShouldBeGreaterOrEqual(this double actual, double expected, string message, bool takeScreenshot = false)
        {
            try
            {
                Console.WriteLine("{0} : <{1}> must be  less than or greater<{2}>", message, actual, expected);
                Assert.GreaterOrEqual(actual, expected);
            }
            catch (AssertionException ex)
            {
                ThrowAssertionException(message, takeScreenshot, ex);
            }


        }

        public static void ShouldBeEqual(this object actual, object expected, string message,bool takeScreenshot = false)
        {
            try
            {
                Console.WriteLine("{0}: Expected <{1}> , Actual <{2}>", message, expected, actual);
                Assert.AreEqual(expected, actual);
            }
            catch (AssertionException ex)
            {
                ThrowAssertionException(message, takeScreenshot, ex);
            }
            Assert.AreEqual(expected, actual);
        }



        public static void ShouldNotBeEqual(this object actual, object expected, string message, bool takeScreenshot = false)
        {
            try
            {
                Console.WriteLine("{0}: Expected <{1}> , Actual <{2}>", message, expected, actual);
                Assert.AreNotEqual(expected, actual);
            }
            catch (AssertionException ex)
            {
                ThrowAssertionException(message, takeScreenshot, ex);
            }
            Assert.AreNotEqual(expected, actual);
        }

        public static void ShouldCollectionBeEqual(this IList<string> actualList, IList<string> expectedList, string message, bool takeScreenshot = false, params object[] args)
        {
            try
            {
                for (var i = 0; i < expectedList.Count; i++)
                    Console.Out.WriteLine("{0}{1} : Expected<{2}>, Actual<{3}> ",message, i + 1, expectedList[i], actualList[i]);

                CollectionAssert.AreEqual(expectedList, actualList.ToList(), message, args);
            }
            catch (AssertionException ex)
            {
                ThrowAssertionException(message, takeScreenshot, ex);
            }
        }

        public static void ShouldCollectionBeEqual(this IList<string> actualList, IList<string> expectedList, IList<string> header, string message, bool takeScreenshot = false, params object[] args)
        {
            try
            {
                for (var i = 0; i < expectedList.Count; i++)
                    Console.Out.WriteLine("{0} : Expected<{1}>, Actual<{2}> ", header[0], expectedList[i], actualList[i]);

                CollectionAssert.AreEqual(expectedList, actualList.ToList(), message, args);
            }
            catch (AssertionException ex)
            {
                ThrowAssertionException(message, takeScreenshot, ex);
            }
        }

        public static void ShouldCollectionBeEqual(this IEnumerable actualList, IEnumerable expectedList, string message, bool takeScreenshot = false, params object[] args)
        {
            try
            {
                CollectionAssert.AreEqual(expectedList, actualList, message, args);
            }
            catch (AssertionException ex)
            {
                ThrowAssertionException(message, takeScreenshot, ex);
            }
        }

        public static void CollectionShouldNotBeSubsetOf(this IList<string> list, IList<string> bigList, string message, bool takeScreenshot = false, params object[] args)
        {
            try
            {
                CollectionAssert.IsNotSubsetOf(list, bigList, message, args);
            }
            catch (AssertionException ex)
            {
                ThrowAssertionException(message, takeScreenshot, ex);
            }
        }

        public static void CollectionShouldBeSubsetOf(this IList<string> list, IList<string> bigList, string message, bool takeScreenshot = false, params object[] args)
        {
            try
            {
                CollectionAssert.IsSubsetOf(list, bigList, message, args);
            }
            catch (AssertionException ex)
            {
                ThrowAssertionException(message, takeScreenshot, ex);
            }
        }

        public static void CollectionShouldBeSupersetOf(this IList<string> bigList, IList<string> list, string message, bool takeScreenshot = false, params object[] args)
        {
            try
            {
                CollectionAssert.IsSupersetOf(bigList, list, message, args);
            }
            catch (AssertionException ex)
            {
                ThrowAssertionException(message, takeScreenshot, ex);
            }
        }

        public static void ShouldCollectionBeEquivalent(this IEnumerable actualList, IEnumerable expectedList, string message, bool takeScreenshot = false, params object[] args)
        {
            try
            {
                //foreach (var actualValue in actualList)
                //{
                //    Console.Out.WriteLine("{0} : Expected<{1}>, Actual<{2}> ", message, actualValue,
                //        (expectedList.Contains(actualValue)) ? expectedList.Single(x => x == actualValue) : string.Empty);
                //}
                foreach (var actualValue in actualList)
                {
                    Console.Out.WriteLine("{0} : Expected<{1}> ", message, actualValue);
                }
                CollectionAssert.AreEquivalent(expectedList, actualList, message, args);
            }
            catch (AssertionException ex)
            {
                ThrowAssertionException(message, takeScreenshot, ex);
            }
        }

        public static void ShouldCollectionBeNotEqual(this IList<string> actualList, IList<string> expectedList, string message, bool takeScreenshot = false, params object[] args)
        {
            try
            {
                CollectionAssert.AreNotEqual(expectedList, actualList.ToList(), message, args);
            }
            catch (AssertionException ex)
            {
                ThrowAssertionException(message, takeScreenshot, ex);
            }
        }

        public static void ShouldCollectionBeEmpty(this IList<string> expectedList, string message, bool takeScreenshot = false, params object[] args)
        {
            try
            {
                CollectionAssert.IsEmpty(expectedList, message, args);
                Console.WriteLine(message +" : Expected Count <0> and Actual <{0}>",expectedList.Count);

            }
            catch (AssertionException ex)
            {
                ThrowAssertionException(message, takeScreenshot, ex);
            }
        }

        public static void ShouldCollectionBeAllNullOrEmpty(this IList<string> expectedList, string message, bool takeScreenshot = false, params object[] args)
        {
            try
            {
                Assert.That(expectedList, Is.All.Null.Or.Empty, message, args);
                Console.WriteLine("{0} - Is Empty or Null? <{1}> ", message, expectedList);

            }
            catch (AssertionException ex)
            {
                ThrowAssertionException(message, takeScreenshot, ex);
            }
        }

        public static void ShouldCollectionBeAllSame(this IList<string> expectedList,string value, string message, bool takeScreenshot = false, params object[] args)
        {
            try
            {
                Assert.That(expectedList, Is.All.EqualTo(value), message, args);
                Console.WriteLine("{0} - Is All Value of List is? <{1}> ", message, value);

            }
            catch (AssertionException ex)
            {
                ThrowAssertionException(message, takeScreenshot, ex);
            }
        }

        public static void ShouldCollectionBeNotEmpty(this IList<string> expectedList, string message, bool takeScreenshot = false, params object[] args)
        {
            try
            {
                CollectionAssert.IsNotEmpty(expectedList, message, args);
                Console.WriteLine(message + " : List Count <{0}> should be greater than 0", expectedList.Count);

            }
            catch (AssertionException ex)
            {
                ThrowAssertionException(message, takeScreenshot, ex);
            }
        }

        public static void ShouldCollectionBeSorted(this IEnumerable givenList, bool descending, string message, bool takeScreenshot = false, params object[] args)
        {
            try
            {
                var actualList = givenList as IList<object> ?? givenList.Cast<object>().ToList();
                var expectedList = descending
                    ? actualList.OrderByDescending(s => s).ToList()
                    : actualList.OrderBy(s => s).ToList();
                for (var i = 0; i < actualList.Count; i++)
                    Console.Out.WriteLine("{0}{1} : Expected<{2}>, Actual<{3}> ", message, i + 1, expectedList[i],
                        actualList[i]);

                CollectionAssert.AreEqual(expectedList, actualList, message, args);
            }
            catch (AssertionException ex)
            {
                ThrowAssertionException(message, takeScreenshot, ex);
            }
        }

        public static void ShouldNotContain(this IEnumerable expectedList,  object actualValue, string message, bool takeScreenshot = false, params object[] args)
        {
            try
            {
                CollectionAssert.DoesNotContain(expectedList,actualValue, message, args);
                Console.WriteLine(message + " : Expected List does not contains <{0}>", actualValue);

            }
            catch (AssertionException ex)
            {
                ThrowAssertionException(message, takeScreenshot, ex);
            }
        }

        public static void ShouldContain(this IEnumerable expectedList, object actualValue, string message, bool takeScreenshot = false, params object[] args)
        {
            try
            {
                CollectionAssert.Contains(expectedList, actualValue, message, args);
                Console.WriteLine(message + " : Expected List contains <{0}>", actualValue);

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

        public static void ShouldNotBeTheSameAs(this object actual, object expected, string message, bool takeScreenshot = false)
        {
            try
            {
                Console.WriteLine("{0} Unexpected: <{1}> , Actual: <{2}>", message, expected, actual);
                Assert.AreNotSame(expected, actual, message);

            }
            catch (AssertionException ex)
            {
                ThrowAssertionException(message, takeScreenshot, ex);
            }
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
                ThrowAssertionException(message,takeScreenshot, ex);
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


        public static void AssertIsNotContained(this object actualValue, object expectedValue, string message)
        {
            Console.WriteLine("{0} Expected: <{1}> , Actual: <{2}>", message, expectedValue, actualValue);
            if (actualValue != "")
            {
                if (actualValue.ToString().Contains(expectedValue.ToString()) ||
                    expectedValue.ToString().Contains(actualValue.ToString()))
                    Assert.Fail("Values match");
            }
        }

        public static void AssertIsContainedInList(this string actualValue, IList<string> expectedList, string message, bool takeScreenshot = false)
        {
            try
            {
               // Console.Out.WriteLine("{0} : Expected<{1}>, Actual<{2}> ", message, actualValue, (expectedList.Contains(actualValue)) ? expectedList.Single(x => x == actualValue) : string.Empty);
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
            Console.WriteLine("Expected {0} between {1} and {2} . Actual :{3}", message, expectedDateBegin, expectedDateEnd, Convert.ToString(actualDate));
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
                DefaultPage.CaptureScreenShot(TestName);
                throw new AssertionException(
                    String.Format("Test fail due to :\n{0}\n{1}\nTest failure screenshot location : \n{2}",
                                  verificationElement, ex.Message, DefaultPage.ScreenshotFileName), ex);
            }
            throw new AssertionException(String.Format("Test fail due to :\n{0}\n{1}", verificationElement, ex.Message), ex);
        }
       
    }
    


}
