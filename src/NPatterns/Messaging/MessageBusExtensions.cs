using System;

namespace NPatterns.Messaging
{
    public static class MessageBusExtensions
    {
        /// <summary>
        /// register a handler for a message type
        /// when the message published on bus, the handler will handle it
        /// </summary>
        /// <typeparam name="T">type of message</typeparam>
        /// <param name="bus">message bus</param>
        /// <param name="handler">handler</param>
        /// <param name="order">a sequence number that mark this callback would be invoked in sequence</param>
        /// <returns>disposer to remove the handler from bus</returns>
        public static IDisposable Subscribe<T>(this IMessageBus bus, IHandler<T> handler, int? order = null) where T : class
        {
            return bus.Subscribe<T>(handler.Handle, order);
        }
    }
}