//------------------------------------------------------------------------------
// <copyright file="Plugin.cs" company="non.io">
// © non.io
// </copyright>
//------------------------------------------------------------------------------

namespace ImgurRoulette
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text.RegularExpressions;
    using GirBot;
    using GirBot.Config;

    /// <summary>
    /// Plugin class. This class implements IPlugin and generates an random
    /// imgur link when the keyword is seen.
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
            this.config = PluginConfig.Deserialize(Path.Combine(Path.Combine(Directory.GetCurrentDirectory(), "plugins"), "imgurroulette.xml"));
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
                string responseString = string.Empty;
                if (message.ToLower().Equals("!help"))
                {
                    b.SendMessage(sender, this.OnHelp());
                }
                else if (message.ToLower().Equals("!roulette"))
                {
                    while (string.IsNullOrEmpty(responseString))
                    {
                        try
                        {
                            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://api.imgur.com/2/image/" + this.RandomHash());
                            request.Timeout = 5000;
                            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                            Stream stream = response.GetResponseStream();
                            StreamReader reader = new StreamReader(stream);
                            responseString = Regex.Match(reader.ReadToEnd(), @"\<original\b[^>]*\>\s*(?<Original>[\s\S]*?)\</original\>", RegexOptions.IgnoreCase).Groups["Original"].Value;
                            b.SendMessage(channel, responseString);
                            response.Close();
                            stream.Close();
                            reader.Close();
                        }
                        catch
                        {
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
            return "Imgur Roulette - Use !roulette for a random image";
        }
        #endregion

        /// <summary>
        /// Gets a random hash.
        /// </summary>
        /// <returns>Returns a random hash.</returns>
        private string RandomHash()
        {
            string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Random r = new Random();
            return new string(Enumerable.Repeat(chars, 5).Select(s => s[r.Next(s.Length)]).ToArray());
        }
    }
}
