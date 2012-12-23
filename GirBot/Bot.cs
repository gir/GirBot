//------------------------------------------------------------------------------
// <copyright file="Bot.cs" company="non.io">
// © non.io
// </copyright>
//------------------------------------------------------------------------------

namespace GirBot
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net.Sockets;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using GirBot.Config;

    /// <summary>
    /// Bot class.
    /// </summary>
    public class Bot
    {
        /// <summary>
        /// The TCP client.
        /// </summary>
        private TcpClient tcpClient;

        /// <summary>
        /// The address.
        /// </summary>
        private string address;

        /// <summary>
        /// The port.
        /// </summary>
        private int port;

        /// <summary>
        /// The nick name.
        /// </summary>
        private string nickName;

        /// <summary>
        /// The user name.
        /// </summary>
        private string userName;

        /// <summary>
        /// The real name.
        /// </summary>
        private string realName;

        /// <summary>
        /// The channels.
        /// </summary>
        private List<string> channels;

        /// <summary>
        /// The stream.
        /// </summary>
        private NetworkStream stream = null;

        /// <summary>
        /// The reader.
        /// </summary>
        private StreamReader reader = null;

        /// <summary>
        /// The writer.
        /// </summary>
        private StreamWriter writer = null;

        /// <summary>
        /// The is running.
        /// </summary>
        private bool isRunning;

        /// <summary>
        /// The plugins.
        /// </summary>
        private List<IPlugin> plugins;

        /// <summary>
        /// Initializes a new instance of the <see cref="Bot" /> class.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="port">The port.</param>
        /// <param name="nickName">Name of the nick.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="realName">Name of the real.</param>
        /// <param name="channels">The channels.</param>
        public Bot(string address, int port, string nickName, string userName, string realName, List<string> channels)
        {
            this.tcpClient = null;
            this.address = address;
            this.port = port;
            this.nickName = nickName;
            this.userName = userName;
            this.realName = realName;
            this.channels = channels;
            this.isRunning = true;
            this.plugins = new List<IPlugin>();
            this.LoadPlugins();
            if (this.plugins.Count > 0)
            {
                this.RegisterPluginCallbacks();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Bot" /> class.
        /// </summary>
        /// <param name="server">The server.</param>
        public Bot(Server server)
            : this(server.Address, server.Port, server.NickName, server.UserName, server.RealName, server.Channels)
        {
        }

        /// <summary>
        /// Message handler delegate.
        /// </summary>
        /// <param name="b">The b.</param>
        /// <param name="sender">The sender.</param>
        /// <param name="channel">The channel.</param>
        /// <param name="message">The message.</param>
        protected delegate void MessageHandler(Bot b, string sender, string channel, string message);

        /// <summary>
        /// Private message handler delegate.
        /// </summary>
        /// <param name="b">The b.</param>
        /// <param name="sender">The sender.</param>
        /// <param name="message">The message.</param>
        protected delegate void PrivateMessageHandler(Bot b, string sender, string message);

        /// <summary>
        /// Occurs when [message event].
        /// </summary>
        protected event MessageHandler MessageEvent;

        /// <summary>
        /// Occurs when [private message event].
        /// </summary>
        protected event PrivateMessageHandler PrivateMessageEvent;

        /// <summary>
        /// Sends the message.
        /// </summary>
        /// <param name="to">User to send to.</param>
        /// <param name="message">The message.</param>
        public void SendMessage(string to, string message)
        {
            this.SendData("PRIVMSG", to, string.Format(":{0}", message));
        }

        /// <summary>
        /// Runs this instance.
        /// </summary>
        public void Run()
        {
            try
            {
                this.tcpClient = new TcpClient(this.address, this.port);
            }
            catch
            {
                Console.WriteLine("Connection Error");
            }

            try
            {
                this.stream = this.tcpClient.GetStream();
                this.reader = new StreamReader(this.stream);
                this.writer = new StreamWriter(this.stream);

                this.SendData("USER", this.userName, "Ircen.armada", "Ircen.armada", this.realName);
                this.SendData("NICK", this.nickName);

                this.IRCWork();
            }
            catch (Exception e)
            {
                Console.WriteLine("Communication Error");
                Console.WriteLine(e);
                Console.WriteLine(e.StackTrace);
            }
            finally
            {
                if (this.reader != null)
                {
                    this.reader.Close();
                }

                if (this.writer != null)
                {
                    this.writer.Close();
                }

                if (this.stream != null)
                {
                    this.stream.Close();
                }

                if (this.tcpClient != null)
                {
                    this.tcpClient.Close();
                }
            }
        }

        /// <summary>
        /// Sends the data.
        /// </summary>
        /// <param name="cmd">The CMD.</param>
        /// <param name="parameters">The parameters.</param>
        protected void SendData(string cmd, params string[] parameters)
        {
            string fullCmd = string.Format("{0} {1}", cmd, string.Join(" ", parameters));
            Console.WriteLine(">>>>{0}", fullCmd);
            this.writer.WriteLine(fullCmd);
            this.writer.Flush();
        }

        /// <summary>
        /// Sends the pong.
        /// </summary>
        /// <param name="origin">The origin.</param>
        protected void SendPong(string origin)
        {
            this.SendData("PONG", origin);
        }

        /// <summary>
        /// Joins the channel.
        /// </summary>
        /// <param name="channel">The channel.</param>
        protected void JoinChannel(string channel)
        {
            this.SendData("JOIN", channel);
        }

        /// <summary>
        /// Joins the channel.
        /// </summary>
        /// <param name="channels">The channels.</param>
        protected void JoinChannel(List<string> channels)
        {
            string channelsList = string.Join(",", channels.ToArray());
            this.JoinChannel(channelsList);
        }

        /// <summary>
        /// Parts the channel.
        /// </summary>
        /// <param name="channel">The channel.</param>
        protected void PartChannel(string channel)
        {
            this.SendData("PART", channel);
        }

        /// <summary>
        /// Parts the channel.
        /// </summary>
        /// <param name="channels">The channels.</param>
        protected void PartChannel(List<string> channels)
        {
            string channelsList = string.Join(",", channels.ToArray());
            this.PartChannel(channelsList);
        }

        /// <summary>
        /// Parses the message.
        /// </summary>
        /// <param name="message">The message.</param>
        protected virtual void ParseMessage(string message)
        {
            string[] tokens;
            Console.WriteLine(message);
            tokens = message.Split(' ');

            if (tokens[0].Equals("PING"))
            {
                // Respond to ping.
                this.SendPong(tokens[1]);
            }
            else if (tokens.Length > 1 && tokens[1].Equals("376"))
            {
                // End of MOTD, join channels.
                this.JoinChannel(this.channels);
            }
            else if (tokens.Length > 3 && tokens[1].Equals("PRIVMSG"))
            {
                // Get the sender.
                Regex r = new Regex(":(.+)!.*");
                string sender = string.Empty;
                GroupCollection gc = r.Match(tokens[0]).Groups;
                if (gc.Count == 2)
                {
                    sender = gc[1].ToString();
                }

                // Get the message.
                string m = string.Join(" ", tokens, 3, tokens.Length - 3).Substring(1);

                // Got a message.
                if (tokens[2].Contains("#"))
                {
                    // Channel message.
                    this.MessageEvent(this, sender, tokens[2], m);
                }
                else
                {
                    // Private message.
                    this.PrivateMessageEvent(this, sender, m);
                }
            }
        }

        /// <summary>
        /// Loads the plugins.
        /// </summary>
        protected void LoadPlugins()
        {
            foreach (string fileName in Directory.GetFiles(Path.Combine(Directory.GetCurrentDirectory(), "plugins"), "*.dll"))
            {
                Assembly assembly = Assembly.LoadFile(fileName);
                foreach (Type type in assembly.GetTypes())
                {
                    if (type.GetInterface("IPlugin") != null)
                    {
                        IPlugin plugin = (IPlugin)Activator.CreateInstance(type);
                        this.plugins.Add(plugin);
                    }
                }
            }
        }

        /// <summary>
        /// Registers the plugin callbacks.
        /// </summary>
        protected void RegisterPluginCallbacks()
        {
            foreach (IPlugin plugin in this.plugins)
            {
                this.MessageEvent += plugin.OnMessage;
                this.PrivateMessageEvent += plugin.OnPrivateMessage;
            }
        }

        /// <summary>
        /// IRC work.
        /// </summary>
        private void IRCWork()
        {
            string message;
            while (this.isRunning)
            {
                message = this.reader.ReadLine();
                this.ParseMessage(message);
                ////Thread.Sleep(100);
            }
        }
    }
}