using System;

namespace Legacy.Service.Support.Utils
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
    }
}
