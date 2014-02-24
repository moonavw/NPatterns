using System;

namespace NPatterns.Messaging
{
    /// <summary>
    /// Contract for "Message bus a.k.a Event bus pattern".
    /// </summary>
    public interface IMessageBus : IDisposable
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
        /// register a handler for a message type.
        /// the hanlder will be invoked when the message published on bus
        /// </summary>
        /// <typeparam name="T">type of message</typeparam>
        /// <param name="handler">handler that handle message</param>
        /// <returns>disposer to remove the callback from bus</returns>
        IDisposable Subscribe<T>(IHandler<T> handler) where T : class;

        /// <summary>
        /// publish a message on bus.
        /// all matched registered/hanlder callback will handle this message
        /// </summary>
        /// <typeparam name="T">type of message</typeparam>
        /// <param name="message">message</param>
        /// <returns>true if any subscriber process the message, false if no subscriber process it</returns>
        bool Publish<T>(T message) where T : class;

        /// <summary>
        /// publish a message on bus.
        /// all matched registered callback/hanlder will handle this message asynchronously
        /// </summary>
        /// <typeparam name="T">type of message</typeparam>
        /// <param name="message">message</param>
        /// <param name="callbackOnAllDone">a callback invoked when the message handled by all registered callback/handler</param>
        /// <param name="callbackOnAnyDone">a callback invoked when the message handled by any registered callback/handler, so it would be invoked when each callback/handler handling the message</param>
        /// <returns>true if any subscriber process the message, false if no subscriber process it</returns>
        bool PublishAsync<T>(T message, Action callbackOnAllDone = null, Action callbackOnAnyDone = null) where T : class;
    }
}