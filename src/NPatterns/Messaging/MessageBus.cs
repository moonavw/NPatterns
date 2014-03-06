using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NPatterns.Messaging
{
    /// <summary>
    /// A basic implement of the MessageBus
    /// </summary>
    public class MessageBus : IMessageBus
    {
        private readonly List<Delegate> _subscriptions = new List<Delegate>();

        #region IMessageBus Members

        public IDisposable Subscribe<T>(Action<T> callback) where T : class
        {
            _subscriptions.Add(callback);
            return new Disposer(() => _subscriptions.Remove(callback));
        }

        public virtual void Publish<T>(T message) where T : class
        {
            var subscribers = GetSubscribers<T>().ToList();

            foreach (var callback in subscribers)
                callback(message);
        }

        public virtual Task PublishAsync<T>(T message) where T : class
        {
            var subscribers = GetSubscribers<T>().ToList();

            var tasks = (from callback in subscribers
                         select Task.Run(() => callback(message))).ToArray();
            return Task.WhenAll(tasks);
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            _subscriptions.Clear();
        }

        #endregion

        protected virtual IEnumerable<Action<T>> GetSubscribers<T>() where T : class
        {
            return from s in _subscriptions
                   where s is Action<T>
                   select (Action<T>)s;
        }
    }
}