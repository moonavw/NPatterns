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
        protected override IEnumerable<Action<T>> GetSubscribers<T>()
        {
            var handlers = ServiceLocator.Current.GetAllInstances<IHandler<T>>().ToList();

            var subscribers = handlers.Select(h => (Action<T>)h.Handle).ToList();
            subscribers.AddRange(base.GetSubscribers<T>());
            return subscribers;
        }
    }
}