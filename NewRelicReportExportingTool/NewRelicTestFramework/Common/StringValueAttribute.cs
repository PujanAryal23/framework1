using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewRelicTestFramework
{
    #region Class StringValueAttribute

    /// <summary>
    /// Simple attribute class for storing String Values
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    public class StringValueAttribute : Attribute
    {
        /// <summary>
        /// The _display value.
        /// </summary>
        private readonly string _displayValue;

        /// <summary>
        /// The _value.
        /// </summary>
        private readonly string _value;

        /// <summary>
        /// Initializes a new instance of the <see cref="StringValueAttribute"/> class. 
        /// Creates a new <see cref="StringValueAttribute"/> instance.
        /// </summary>
        /// <param name="value">
        /// Value parameter
        /// </param>
        public StringValueAttribute(string value)
        {
            _value = value;
            _displayValue = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringValueAttribute"/> class. 
        /// Creates a new StringValueAttribute instance
        /// </summary>
        /// <param name="value">
        /// Key Value parm
        /// </param>
        /// <param name="displayValue">
        /// Display Value parm
        /// </param>
        public StringValueAttribute(string value, string displayValue)
        {
            _value = value;
            _displayValue = displayValue;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value></value>
        public string Value
        {
            get { return _value; }
        }

        /// <summary>
        /// Gets the Display Value
        /// </summary>
        public string DisplayValue
        {
            get { return _displayValue; }
        }
    }

    #endregion
}
