//------------------------------------------------------------------------------
// <copyright file="Server.cs" company="non.io">
// © non.io
// </copyright>
//------------------------------------------------------------------------------

namespace GirBot.Config
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    /// <summary>
    /// Server class.
    /// </summary>
    [XmlRoot("server")]
    public class Server
    {
        /// <summary>
        /// Gets or sets the address.
        /// </summary>
        /// <value>
        /// The address.
        /// </value>
        [XmlElement("address")]
        public string Address { get; set; }

        /// <summary>
        /// Gets or sets the port.
        /// </summary>
        /// <value>
        /// The port.
        /// </value>
        [XmlElement("port")]
        public int Port { get; set; }

        /// <summary>
        /// Gets or sets the name of the nick.
        /// </summary>
        /// <value>
        /// The name of the nick.
        /// </value>
        [XmlElement("nickname")]
        public string NickName { get; set; }

        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        /// <value>
        /// The name of the user.
        /// </value>
        [XmlElement("username")]
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the name of the real.
        /// </summary>
        /// <value>
        /// The name of the real.
        /// </value>
        [XmlElement("realname")]
        public string RealName { get; set; }

        /// <summary>
        /// Gets or sets the channels.
        /// </summary>
        /// <value>
        /// The channels.
        /// </value>
        [XmlArray("channels")]
        [XmlArrayItem(ElementName = "channel")]
        public List<string> Channels { get; set; }
    }
}