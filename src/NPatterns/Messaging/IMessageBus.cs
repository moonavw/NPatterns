using System;

namespace NPatterns.Messaging
{
    /// <summary>
    /// Contract for "Message bus a.k.a Event bus pattern".
    /// </summary>
    public interface IMessageBus
    {
        /// <summary>
        /// register a callback for a specific message type.
        /// the callback will be invoked when the message published on bus
        /// </summary>
        /// <typeparam name="T">type of message</typeparam>
        /// <param name="callback">callback action</param>
        /// <returns>a disposable handle to release the callback from bus</returns>
        IDisposable Subscribe<T>(Action<T> callback) where T : class;

        /// <summary>
        /// publish a message on bus.
        /// all matched registered callback will be invoked to handle this message
        /// </summary>
        /// <typeparam name="T">type of message</typeparam>
        /// <param name="message">message</param>
        void Publish<T>(T message) where T : class;
    }
}
