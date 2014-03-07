using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NPatterns.Messaging
{
    /// <summary>
    /// implement MessageBus with Factory to get all Handlers
    /// </summary>
    public class MessageBusEx : MessageBus, IMessageBus
    {
        private readonly Func<Type, IEnumerable<object>> _handlerFactory;

        public MessageBusEx(Func<Type, IEnumerable<object>> handlerFactory)
        {
            _handlerFactory = handlerFactory;
        }

        public override void Publish<T>(T message)
        {
            var handlers = GetHandlers<T>();

            foreach (var handler in handlers)
                handler.Handle(message);

            base.Publish<T>(message);
        }

        public override Task PublishAsync<T>(T message)
        {
            var handlers = GetHandlers<T>();

            var tasks = (from handler in handlers
                         select handler.HandleAsync(message)).ToList();

            tasks.Add(base.PublishAsync<T>(message));

            return Task.WhenAll(tasks);
        }

        private IEnumerable<IHandler<T>> GetHandlers<T>() where T : class
        {
            return from IHandler<T> handler in _handlerFactory(typeof(IHandler<T>))
                   select handler;
        }
    }
}