//------------------------------------------------------------------------------
// <copyright file="ConfigFile.cs" company="non.io">
// © non.io
// </copyright>
//------------------------------------------------------------------------------

namespace GirBot.Config
{
    using System.Collections.Generic;
    using System.IO;
    using System.Xml.Serialization;

    /// <summary>
    /// Config class.
    /// </summary>
    [XmlRoot("config")]
    public class ConfigFile
    {
        /// <summary>
        /// Gets or sets the servers.
        /// </summary>
        /// <value>
        /// The servers.
        /// </value>
        [XmlArray("servers")]
        [XmlArrayItem("server")]
        public List<Server> Servers { get; set; }

        /// <summary>
        /// Deserializes the specified file name.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>Returns ConfigFile object.</returns>
        public static ConfigFile Deserialize(string fileName)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ConfigFile));
            TextReader reader = new StreamReader(fileName);
            ConfigFile config = (ConfigFile)serializer.Deserialize(reader);
            reader.Close();
            return config;
        }
    }
}