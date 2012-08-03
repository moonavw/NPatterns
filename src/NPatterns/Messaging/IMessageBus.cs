using System;

namespace NPatterns.Messaging
{
    /// <summary>
    /// Contract for "Message bus a.k.a Event bus pattern".
    /// </summary>
    public interface IMessageBus
    {
        /// <summary>
        /// register a callback for a message type.
        /// the callback will be invoked when the message published on bus
        /// </summary>
        /// <typeparam name="T">type of message</typeparam>
        /// <param name="callback">callback action</param>
        /// <param name="order">a sequence number that mark this callback would be invoked in sequence</param>
        /// <returns>disposer to remove the callback from bus</returns>
        IDisposable Subscribe<T>(Action<T> callback, int? order = null) where T : class;

        /// <summary>
        /// publish a message on bus.
        /// all matched registered callback will handle this message
        /// </summary>
        /// <typeparam name="T">type of message</typeparam>
        /// <param name="message">message</param>
        void Publish<T>(T message) where T : class;

        /// <summary>
        /// publish a message on bus.
        /// all matched registered callback will handle this message asynchronously
        /// </summary>
        /// <typeparam name="T">type of message</typeparam>
        /// <param name="message">message</param>
        void PublishAsync<T>(T message) where T : class;
    }
}
