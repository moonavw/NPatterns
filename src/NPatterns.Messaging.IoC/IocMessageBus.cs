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
    public class IocMessageBus : NPatterns.Messaging.MessageBus, IMessageBus
    {
        public override void Publish<T>(T message)
        {
            var handlers = ServiceLocator.Current.GetAllInstances<IHandler<T>>();

            foreach (var handler in handlers)
                handler.Handle(message);

            base.Publish<T>(message);
        }

        public override Task PublishAsync<T>(T message)
        {
            var handlers = ServiceLocator.Current.GetAllInstances<IHandler<T>>();

            var tasks = (from handler in handlers
                         select handler.HandleAsync(message)).ToList();
            tasks.Add(base.PublishAsync<T>(message));

            return Task.WhenAll(tasks);
        }
    }
}