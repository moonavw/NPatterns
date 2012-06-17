using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.ServiceLocation;

namespace NPatterns.Messaging.IoC
{
    public class MessageBusEx : MessageBus, IMessageBus
    {
        public override void Publish<T>(T message)
        {
            var handlers = ServiceLocator.Current.GetAllInstances<IHandler<T>>();
            foreach (var handler in handlers)
                handler.Handle(message);

            base.Publish<T>(message);
        }
    }
}
