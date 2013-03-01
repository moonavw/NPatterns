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
        public override void Publish<T>(T message)
        {
            var handlers = ServiceLocator.Current.GetAllInstances<IHandler<T>>().OrderBy(z => z.Order);

            foreach (var handler in handlers)
                using (handler)
                    handler.Handle(message);
        }
        public override void PublishAsync<T>(T message)
        {
            var handlers = ServiceLocator.Current.GetAllInstances<IHandler<T>>().OrderBy(z => z.Order);

            foreach (var handler in handlers)
            {
                IHandler<T> handler1 = handler;
                Task.Factory.StartNew(p =>
                                          {
                                              using (handler1)
                                                  handler1.Handle((T)p);
                                          }, message);
            }
        }
    }
}