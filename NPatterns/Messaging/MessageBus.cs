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
        private readonly List<Delegate> _subscriptions;

        public MessageBus()
        {
            _subscriptions = new List<Delegate>();
        }

        #region IMessageBus Members

        public IDisposable Subscribe<T>(Action<T> callback) where T : class
        {
            _subscriptions.Add(callback);
            return new Disposer(() => _subscriptions.Remove(callback));
        }

        public virtual void Publish<T>(T message) where T : class
        {
            foreach (var callback in GetCallbacks<T>())
                callback(message);
        }

        public virtual Task PublishAsync<T>(T message) where T : class
        {
            var tasks = from callback in GetCallbacks<T>()
                        select Task.Run(() => callback(message));

            return Task.WhenAll(tasks);
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
                   select (Action<T>)s;
        }
    }
}