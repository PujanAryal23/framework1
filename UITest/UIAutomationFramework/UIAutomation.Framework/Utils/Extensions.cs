using System;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace UIAutomation.Framework.Utils
{
    /// <summary>
    /// Defines a class for extending any operation.
    /// </summary>
    public static class Extensions
    {


        /// <summary>
        /// Checks a instance of type is null or not.
        /// </summary>
        /// <typeparam name="T">A generic class.</typeparam>
        /// <param name="value">A member of T.</param>
        /// <returns>Equals or not.</returns>
        public static bool IsNullOrDefault<T>(this T? value) where T : struct
        {
            return default(T).Equals(value.GetValueOrDefault());
        }

        ///// <summary>
        ///// Will get the string value for a given enums value, this will
        ///// only work if you assign the StringValue attribute to
        ///// the items in your enum.
        ///// </summary>
        ///// <param name="value">An <see cref="Enum"/> value.</param>
        ///// <returns>An attribute value.</returns>
        //public static string GetStringValue(this Enum value)
        //{
        //    // Get the type
        //    var type = value.GetType();

        //    // Get fieldinfo for this type
        //    var fieldInfo = type.GetField(Convert.ToString(value));

        //    // Get the stringvalue attributes
        //    var attribs = fieldInfo.GetCustomAttributes(
        //        typeof(StringValueAttribute), false) as StringValueAttribute[];

        //    // Return the first if there was a match.
        //    return attribs != null && attribs.Length > 0 ? attribs[0].StringValue : null;
        //}

        public static string GetStringDisplayValue(this Enum value)
        {
            return GetEnumValue(value, false);
        }

        public static string GetStringValue(this Enum value)
        {
            return GetEnumValue(value);
        }

        public static string GetEnumValue(this Enum value, bool dataValue = true)
        {
            if (value == null)
            {
                return null;
            }

            string output = null;
            Type type = value.GetType();
            // look for our 'StringValueAttribute' in the field's custom attributes
            FieldInfo fi = type.GetField(value.ToString());

            StringValueAttribute[] attrs = fi.GetCustomAttributes(typeof(StringValueAttribute), false) as StringValueAttribute[];

            if (attrs != null && attrs.Length > 0)
            {
                output = dataValue ? attrs[0].Value : attrs[0].DisplayValue;
            }

            return output;
            
        }

        public static TEnum TryParse<TEnum>(this string value, bool ignoreCase = true) where TEnum : struct, IConvertible, IFormattable, IComparable
        {
            var parseType = typeof(TEnum);
            TEnum parsedValue = default(TEnum);
            if (!parseType.IsEnum)
            {
                var message = $"Type must be enum: {parseType}";
                throw new ArgumentException(message);
            }

            if (value == null)
            {
                return parsedValue;
            }

            foreach (var field in parseType.GetFields())
            {
                var attributes = field.GetCustomAttributes(typeof(StringValueAttribute), false) as StringValueAttribute[];
                if (attributes != null && attributes.Any())
                {
                    var firstAttribute = attributes.First();
                    var stringComparison = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.InvariantCulture;
                    Func<bool> matchesValueProperty = () => string.Equals(firstAttribute.Value, value, stringComparison);
                    Func<bool> matchesDisplayValueProperty = () => string.Equals(firstAttribute.DisplayValue, value, stringComparison);
                    if (matchesValueProperty() || matchesDisplayValueProperty())
                    {
                        return (TEnum)Enum.Parse(parseType, field.Name, ignoreCase);
                    }
                }
            }

            return parsedValue;
        }
    }
}
