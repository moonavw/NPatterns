using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NPatterns.Messaging
{
    /// <summary>
    /// implement MessageBus with list of callbacks and support handler factory to get all message handlers
    /// </summary>
    public class MessageBus : IMessageBus
    {
        private readonly List<Delegate> _subscriptions;
        private readonly Func<Type, IEnumerable<object>> _handlerFactory;

        public MessageBus()
        {
            _subscriptions = new List<Delegate>();
        }

        public MessageBus(Func<Type, IEnumerable<object>> handlerFactory)
            : this()
        {
            _handlerFactory = handlerFactory;
        }

        #region IMessageBus Members

        public IDisposable Subscribe<T>(Action<T> callback) where T : class
        {
            _subscriptions.Add(callback);
            return new Disposer(() => _subscriptions.Remove(callback));
        }

        public void Publish<T>(T message) where T : class
        {
            foreach (var handler in GetHandlers<T>())
                handler.Handle(message);

            foreach (var callback in GetCallbacks<T>())
                callback(message);
        }

        public Task PublishAsync<T>(T message) where T : class
        {
            var handlerTasks = from handler in GetHandlers<T>()
                               select handler.HandleAsync(message);

            var callbackTasks = from callback in GetCallbacks<T>()
                                select Task.Run(() => callback(message));

            return Task.WhenAll(handlerTasks.Concat(callbackTasks));
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            _subscriptions.Clear();
        }

        #endregion

        private IEnumerable<Action<T>> GetCallbacks<T>() where T : class
        {
            return from s in _subscriptions
                   where s is Action<T>
                   select (Action<T>) s;
        }

        private IEnumerable<IHandler<T>> GetHandlers<T>() where T : class
        {
            if (_handlerFactory == null)
                return Enumerable.Empty<IHandler<T>>();

            return from IHandler<T> handler in _handlerFactory(typeof(IHandler<T>))
                   select handler;
        }
    }
}