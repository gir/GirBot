//------------------------------------------------------------------------------
// <copyright file="Plugin.cs" company="non.io">
// © non.io
// </copyright>
//------------------------------------------------------------------------------

namespace UrlTitle
{
    using System;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Text.RegularExpressions;
    using GirBot;
    using GirBot.Config;

    /// <summary>
    /// Plugin class. This class implements IPlugin and looks up the page title
    /// for URLs.
    /// </summary>
    public class Plugin : IPlugin
    {
        /// <summary>
        /// The config.
        /// </summary>
        private PluginConfig config;

        /// <summary>
        /// Initializes a new instance of the <see cref="Plugin" /> class.
        /// </summary>
        public Plugin()
        {
            // This will be called from the callee's directory so you still need
            // to add the plugins directory.
            this.config = PluginConfig.Deserialize(Path.Combine(Path.Combine(Directory.GetCurrentDirectory(), "plugins"), "urltitle.xml"));
        }

        /// <summary>
        /// Gets the page title.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <returns>Returns the page title</returns>
        public static string GetPageTitle(string s)
        {
            Uri uri;
            if (Uri.TryCreate(s, UriKind.Absolute, out uri))
            {
                try
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                    request.Timeout = 10000;
                    HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                    if (response.ContentType.Contains("html"))
                    {
                        Stream stream = response.GetResponseStream();
                        StreamReader reader = new StreamReader(stream);
                        string title = string.Empty;
                        char[] read = new char[1024];
                        int count = reader.Read(read, 0, read.Length);
                        StringBuilder content = new StringBuilder(string.Empty);
                        while (string.IsNullOrEmpty(title) && count > 0)
                        {
                            content.Append(new string(read, 0, count));
                            title = Regex.Match(content.ToString(), @"\<title\b[^>]*\>\s*(?<Title>[\s\S]*?)\</title\>", RegexOptions.IgnoreCase).Groups["Title"].Value;
                            count = reader.Read(read, 0, read.Length);
                        }

                        stream.Close();
                        response.Close();
                        reader.Close();
                        return title;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.StackTrace);
                }
            }

            return null;
        }

        #region IPlugin Members
        /// <summary>
        /// Called when [message].
        /// </summary>
        /// <param name="b">The b.</param>
        /// <param name="sender">The sender.</param>
        /// <param name="channel">The channel.</param>
        /// <param name="message">The message.</param>
        public void OnMessage(Bot b, string sender, string channel, string message)
        {
            if (this.config.Channels.Contains(channel))
            {
                if (message.ToLower().Equals("!help"))
                {
                    b.SendMessage(sender, this.OnHelp());
                }

                foreach (string s in message.Split(' '))
                {
                    Uri uri;
                    if (Uri.TryCreate(s, UriKind.Absolute, out uri))
                    {
                        try
                        {
                            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                            request.Timeout = 10000;
                            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                            if (response.ContentType.Contains("html"))
                            {
                                Stream stream = response.GetResponseStream();
                                StreamReader reader = new StreamReader(stream);
                                string title = string.Empty;
                                char[] read = new char[1024];
                                int count = reader.Read(read, 0, read.Length);
                                StringBuilder content = new StringBuilder(string.Empty);
                                while (string.IsNullOrEmpty(title) && count > 0)
                                {
                                    content.Append(new string(read, 0, count));
                                    title = Regex.Match(content.ToString(), @"\<title\b[^>]*\>\s*(?<Title>[\s\S]*?)\</title\>", RegexOptions.IgnoreCase).Groups["Title"].Value;
                                    count = reader.Read(read, 0, read.Length);
                                }

                                stream.Close();
                                response.Close();
                                reader.Close();
                                b.SendMessage(channel, string.Format("Title: {0}", title));
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.StackTrace);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Called when [private message].
        /// </summary>
        /// <param name="b">The b.</param>
        /// <param name="sender">The sender.</param>
        /// <param name="message">The message.</param>
        public void OnPrivateMessage(Bot b, string sender, string message)
        {
            // Do nothing.
        }

        /// <summary>
        /// Called when [help].
        /// </summary>
        /// <returns>
        /// Returns helpful information about the plugin.
        /// </returns>
        public string OnHelp()
        {
            return this.config.ToString();
        }
        #endregion
    }
}