using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NPatterns.Messaging
{
    /// <summary>
    /// implement the MessageBus with concurrent collection for callbacks
    /// </summary>
    public class MessageBus : IMessageBus
    {
        private readonly ConcurrentDictionary<int, Delegate> _callbacks;

        public MessageBus()
        {
            _callbacks = new ConcurrentDictionary<int, Delegate>();
        }

        #region IMessageBus Members

        public IDisposable Subscribe<T>(Action<T> callback) where T : class
        {
            int key = callback.Method.MetadataToken;
            _callbacks.TryAdd(key, callback);

            return new Disposer(() =>
                                    {
                                        Delegate item;
                                        _callbacks.TryRemove(key, out item);
                                    });
        }

        public void Publish<T>(T message) where T : class
        {
            var callbacks = GetSubscriber<T>();

            foreach (var callback in callbacks)
                callback(message);
        }

        public void PublishAsync<T>(T message) where T : class
        {
            var callbacks = GetSubscriber<T>();

            foreach (var callback in callbacks)
            {
                Action<T> callback1 = callback;
                Task.Factory.StartNew(o => callback1((T)o), message); // callback(message);
            }
        }

        #endregion

        protected virtual IEnumerable<Action<T>> GetSubscriber<T>() where T : class
        {
            var callbacks = (from action in _callbacks.Values
                             let typedAction = action as Action<T>
                             where typedAction != null
                             select typedAction).ToList();
            return callbacks;
        }
    }
}