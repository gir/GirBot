//------------------------------------------------------------------------------
// <copyright file="Program.cs" company="non.io">
// © non.io
// </copyright>
//------------------------------------------------------------------------------

namespace GirBot
{
    using GirBot.Config;

    /// <summary>
    /// Program class.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Main entry point.
        /// </summary>
        /// <param name="args">The args.</param>
        public static void Main(string[] args)
        {
            ConfigFile config = ConfigFile.Deserialize(@"config.xml");
            Bot bot = new Bot(config.Servers[0]);
            bot.Run();
        }
    }
}