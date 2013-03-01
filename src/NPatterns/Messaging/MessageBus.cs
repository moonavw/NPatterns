using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NPatterns.Messaging
{
    /// <summary>
    /// implement the MessageBus with concurrent collection
    /// </summary>
    public class MessageBus : IMessageBus
    {
        private readonly ConcurrentDictionary<int, Subscription> _subscriptions;

        public MessageBus()
        {
            _subscriptions = new ConcurrentDictionary<int, Subscription>();
        }

        #region IMessageBus Members

        public IDisposable Subscribe<T>(Action<T> callback, int? order = null) where T : class
        {
            var subscription = new Subscription(typeof(T), callback, order);
            int key = subscription.Key;
            _subscriptions.TryAdd(key, subscription);

            return new Disposer(() =>
                                    {
                                        Subscription item;
                                        _subscriptions.TryRemove(key, out item);
                                    });
        }

        public IDisposable Subscribe<T>(IHandler<T> handler) where T : class
        {
            return Subscribe((Action<T>)handler.Handle, handler.Order);
        }

        public virtual void Publish<T>(T message) where T : class
        {
            var subscribers = GetSubscribers<T>();

            foreach (var callback in subscribers)
                callback(message);
        }

        public virtual void PublishAsync<T>(T message) where T : class
        {
            var subscribers = GetSubscribers<T>();

            foreach (var callback in subscribers)
            {
                Action<T> action = callback;
                Task.Factory.StartNew(o => action((T)o), message);
            }
        }

        #endregion

        public void Dispose()
        {
            _subscriptions.Clear();
        }

        protected virtual IEnumerable<Action<T>> GetSubscribers<T>() where T : class
        {
            return (from s in _subscriptions.Values
                    where s.MessageType == typeof(T)
                    orderby s.Order ascending
                    select (Action<T>)s.OnMessagePublished).ToList();
        }

        #region Nested type: Subscription

        private class Subscription
        {
            public Subscription(Type messageType, Delegate onMessagePublished, int? order = null)
            {
                MessageType = messageType;
                OnMessagePublished = onMessagePublished;

                Order = order ?? DateTime.Now.Ticks;
            }

            public Type MessageType { get; private set; }
            public Delegate OnMessagePublished { get; private set; }
            public long Order { get; private set; }

            public int Key
            {
                get { return OnMessagePublished.Method.MetadataToken; }
            }
        }

        #endregion
    }
}