using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace UIAutomation.Framework.Data
{
    /// <summary>
    /// Initializes a new instance of DataParser class and deserialize the XML documents into objects of the spcefied type.
    /// </summary>
    public class DataParser
    {
        /// <summary>
        /// Initializes  a new instance of DataParser class.
        /// </summary>
        private static readonly DataParser instance = new DataParser();

        /// <summary>
        /// Initializes a new instance of IDictionary for caching mechanism.
        /// </summary>
        private readonly IDictionary<string, object> _cache = new Dictionary<string, object>();

        /// <summary>
        /// Gets an Instance of DataParser
        /// </summary>
        public static DataParser Instance { get { return instance; } }

        /// <summary>
        /// Deserializes the XML document contained by the specified <see cref="System.Xml.XmlReader"/>.
        /// </summary>
        /// <typeparam name="T">A generic XML class.</typeparam>
        /// <param name="xmlPath">The path having specified file name.</param>
        /// <param name="reload">Checks whether it needs reload data or not.</param>
        /// <returns>A XML class.</returns>
        public T Deserialize<T>(string xmlPath, bool reload = false)
        {
            if (reload || !_cache.ContainsKey(xmlPath))
            {
                using (TextReader textReader = new StreamReader(xmlPath))
                {
                    var xmlReader = XmlReader.Create(textReader);
                    var xmlDeserializer = new XmlSerializer(typeof(T));
                    _cache[xmlPath] = xmlDeserializer.Deserialize(xmlReader);
                }
            }
            return (T)_cache[xmlPath];
        }
    }
}
