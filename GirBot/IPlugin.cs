//------------------------------------------------------------------------------
// <copyright file="IPlugin.cs" company="non.io">
// © non.io
// </copyright>
//------------------------------------------------------------------------------

namespace GirBot
{
    /// <summary>
    /// IPlugin interface.
    /// </summary>
    public interface IPlugin
    {
        /// <summary>
        /// Called when [message].
        /// </summary>
        /// <param name="b">The b.</param>
        /// <param name="sender">The sender.</param>
        /// <param name="channel">The channel.</param>
        /// <param name="message">The message.</param>
        void OnMessage(Bot b, string sender, string channel, string message);

        /// <summary>
        /// Called when [private message].
        /// </summary>
        /// <param name="b">The b.</param>
        /// <param name="sender">The sender.</param>
        /// <param name="message">The message.</param>
        void OnPrivateMessage(Bot b, string sender, string message);

        /// <summary>
        /// Called when [help].
        /// </summary>
        /// <returns>Returns helpful information about the plugin.</returns>
        string OnHelp();
    }
}