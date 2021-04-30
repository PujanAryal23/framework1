using System;

namespace UIAutomation.Framework.Utils
{
    /// <summary>
    /// This attribute is used to represent a string value
    /// for a value in an enum.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
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

        #region Properties

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

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new Instance of StringValue Attribute.
        /// </summary>
        /// <param name="value">A value that is assigned already for an enum.</param>
        public StringValueAttribute(string value)
        {
            _value = value;
        }

        #endregion

    }
}
