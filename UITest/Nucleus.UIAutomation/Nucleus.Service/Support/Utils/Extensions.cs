using System;
using System.Linq;

namespace Nucleus.Service.Support.Utils
{
    /// <summary>
    /// Defines a class for extending any operation.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Will get the string value for a given enums value, this will
        /// only work if you assign the StringValue attribute to
        /// the items in your enum.
        /// </summary>
        /// <param name="value">An <see cref="Enum"/> value.</param>
        /// <returns>An attribute value.</returns>
        public static string GetStringValue(this System.Enum value)
        {
            return UIAutomation.Framework.Utils.Extensions.GetStringValue(value);
        }

        public static string GetStringDisplayValue(this System.Enum value)
        {
            return UIAutomation.Framework.Utils.Extensions.GetStringDisplayValue(value);
        }

        public static string GetRandomString(string acceptedValues, int length)
        {
            var random = new Random();
            return new string(Enumerable.Repeat(acceptedValues, length).Select(s => s[random.Next(s.Length)])
                .ToArray());
        }

        public static string GetRandomPhoneNumber()
        {
            var random = new Random();
            string phoneNo = null;
            while (true)
            {
                phoneNo = new string(Enumerable.Repeat("0123456789", 10).Select(s => s[random.Next(s.Length)])
                    .ToArray());
                if(phoneNo[0]!=1 && phoneNo[0] != 0)
                    break;
            }
            Console.WriteLine(phoneNo);
            return phoneNo;

        }
    }
}
