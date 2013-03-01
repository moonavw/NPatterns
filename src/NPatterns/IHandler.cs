using System;

namespace NPatterns
{
    /// <summary>
    /// A handler could handle a specified message, like command, event, request etc
    /// </summary>
    /// <typeparam name="T">type of the message</typeparam>
    public interface IHandler<in T> : IDisposable
        where T : class
    {
        /// <summary>
        /// handlers for the same kind of message would handle it by the order
        /// </summary>
        int Order { get; }

        /// <summary>
        /// Handle the message
        /// </summary>
        /// <param name="message">message to be handled</param>
        void Handle(T message);
    }
}