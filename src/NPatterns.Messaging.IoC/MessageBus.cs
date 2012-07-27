using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.ServiceLocation;

namespace NPatterns.Messaging.IoC
{
    /// <summary>
    /// implement MessageBus with ServiceLocator to get all Handlers
    /// </summary>
    public class MessageBus : Messaging.MessageBus, IMessageBus
    {
        protected override IEnumerable<Action<T>> GetSubscriber<T>()
        {
            var handlers = ServiceLocator.Current.GetAllInstances<IHandler<T>>();
            var callbacks = new List<Action<T>>(handlers.Select(handler => (Action<T>)handler.Handle));
            callbacks.AddRange(base.GetSubscriber<T>());

            return callbacks;
        }
    }
}