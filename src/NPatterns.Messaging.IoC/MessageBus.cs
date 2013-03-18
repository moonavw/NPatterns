using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;

namespace NPatterns.Messaging.IoC
{
    /// <summary>
    /// implement MessageBus with ServiceLocator to get all Handlers
    /// </summary>
    public class MessageBus : Messaging.MessageBus, IMessageBus
    {
        public override bool Publish<T>(T message)
        {
            var handlers = ServiceLocator.Current.GetAllInstances<IHandler<T>>().OrderBy(z => z.Order).ToList();

            foreach (var handler in handlers)
                handler.Handle(message);

            return base.Publish(message) || handlers.Count > 0;
        }
        public override bool PublishAsync<T>(T message, Action callbackOnAllDone = null, Action callbackOnAnyDone = null)
        {
            var handlers = ServiceLocator.Current.GetAllInstances<IHandler<T>>().OrderBy(z => z.Order).ToList();

            StartHandlingCount(message.GetHashCode(), handlers.Count + GetSubscribers<T>().Count());

            foreach (var handler in handlers)
            {
                IHandler<T> handler1 = handler;
                Task.Factory.StartNew(p =>
                                          {
                                              handler1.Handle((T)p);
                                              //callback for this handler done
                                              UpdateHandlingCount(p.GetHashCode(), callbackOnAllDone, callbackOnAnyDone);
                                          }, message);
            }

            return base.PublishAsync(message, callbackOnAllDone, callbackOnAnyDone) || handlers.Count > 0;
        }
    }
}