using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UIAutomation.Framework.Common;

namespace UIAutomation.Framework.Core.Driver
{
    /// <summary>
    /// Provides the ability to produce Page Objects modeling a page.
    /// </summary>
    internal static class PageFactory
    {
        /// <summary>
        /// Initializes the elements in the Page Object.
        /// </summary>
        /// <param name="page">The Page Object to be populated with elements.</param>
        /// <returns>The Page Object populated with elements.</returns>
        internal static object InitElements(object page)
        {

            if (page == null)
            {
                throw new ArgumentNullException("page", "page cannot be null");
            }

            var type = page.GetType();
            var members = new List<MemberInfo>();
            const BindingFlags publicBindingOptions = BindingFlags.Instance | BindingFlags.Public;
            members.AddRange(type.GetFields(publicBindingOptions));
            members.AddRange(type.GetProperties(publicBindingOptions));
            while (type != null)
            {
                const BindingFlags nonPublicBindingOptions = BindingFlags.Instance | BindingFlags.NonPublic;
                members.AddRange(type.GetFields(nonPublicBindingOptions));
                members.AddRange(type.GetProperties(nonPublicBindingOptions));
                type = type.BaseType;
            }

            foreach (var member in members)
            {
                var attribute = (FindsByAttribute)Attribute.GetCustomAttribute(member, typeof(FindsByAttribute));
                if (attribute == null) continue;
                var fieldInfo = member as FieldInfo;
                var property = member as PropertyInfo;
                if (fieldInfo != null)
                {
                    fieldInfo.SetValue(page,
                        Activator.CreateInstance(fieldInfo.FieldType, false, attribute.How,
                                                                        attribute.Using));
                }
                else if (property != null)
                {
                    property.SetValue(page, Activator.CreateInstance(property.PropertyType, false, attribute.How, attribute.Using), null);
                }
            }
            return page;
        }
    }
}
