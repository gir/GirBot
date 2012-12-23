//------------------------------------------------------------------------------
// <copyright file="PluginConfig.cs" company="non.io">
// © non.io
// </copyright>
//------------------------------------------------------------------------------

namespace GirBot.Config
{
    using System.Collections.Generic;
    using System.IO;
    using System.Xml.Serialization;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [XmlRoot("plugin")]
    public class PluginConfig
    {
        /// <summary>
        /// Gets or sets the author.
        /// </summary>
        /// <value>
        /// The author.
        /// </value>
        [XmlElement("author")]
        public string Author { get; set; }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        /// <value>
        /// The email.
        /// </value>
        [XmlElement("email")]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        [XmlElement("title")]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        [XmlElement("description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        /// <value>
        /// The version.
        /// </value>
        [XmlElement("version")]
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets the servers.
        /// </summary>
        /// <value>
        /// The servers.
        /// </value>
        [XmlArray("servers")]
        [XmlArrayItem(ElementName = "server")]
        public List<string> Servers { get; set; }

        /// <summary>
        /// Gets or sets the channels.
        /// </summary>
        /// <value>
        /// The channels.
        /// </value>
        [XmlArray("channels")]
        [XmlArrayItem(ElementName = "channel")]
        public List<string> Channels { get; set; }

        /// <summary>
        /// Deserializes the specified file name.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>Returns PluginConfig object.</returns>
        public static PluginConfig Deserialize(string fileName)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(PluginConfig));
            TextReader reader = new StreamReader(fileName);
            PluginConfig pluginConfig = (PluginConfig)serializer.Deserialize(reader);
            reader.Close();
            return pluginConfig;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0} v{1} by: {2}({3})", this.Title, this.Version, this.Author, this.Email);
        }
    }
}