﻿using System;
using System.Threading.Tasks;

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
        /// <returns>disposer to remove the callback from bus</returns>
        IDisposable Subscribe<T>(Action<T> callback) where T : class;

        /// <summary>
        /// publish a message on bus.
        /// all matched registered/hanlder callback will handle this message
        /// </summary>
        /// <typeparam name="T">type of message</typeparam>
        /// <param name="message">message</param>
        void Publish<T>(T message) where T : class;

        /// <summary>
        /// Async publish a message on bus.
        /// all matched registered callback/hanlder will handle this message asynchronously
        /// </summary>
        /// <typeparam name="T">type of message</typeparam>
        /// <param name="message">message</param>
        Task PublishAsync<T>(T message) where T : class;
    }
}