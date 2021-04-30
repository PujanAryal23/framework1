using System;
using System.ComponentModel;

namespace UIAutomation.Framework.Common
{
    /// <summary>
    /// Marks the program element with methods by which to find a corresponding element on the page. This
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
    public sealed class FindsByAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the method used to look up the element
        /// </summary>
        [DefaultValue(How.Id)]
        public How How { get; set; }

        /// <summary>
        /// Gets or set the value to look up by (i.e for How.Name, the actual name to look up)
        /// </summary>
        public string Using { get; set; }

        /// <summary>
        /// Gets or sets the value indicating where this attribute should be evaluated relative to other instances
        /// of this attribute decorating the same class member
        /// </summary>
        [DefaultValue(0)]
        public int Priority { get; set; }
    }
}
