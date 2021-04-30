using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NewRelicTestFramework.Common
{
    public static class StringEnumerator
    {

       
        public static string GetStringValue(this Enum value)
        {
            return GetEnumValue(value, true);
        }

    
        
        private static string GetEnumValue(Enum value, bool dataValue)
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

      
      
    }
}
