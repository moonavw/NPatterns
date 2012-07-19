using System;
using System.Collections.Concurrent;
using System.Linq;

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

        public virtual IDisposable Subscribe<T>(Action<T> callback) where T : class
        {
            int key = callback.GetHashCode();
            _callbacks.TryAdd(key, callback);

            return new Disposer(() =>
            {
                Delegate item;
                _callbacks.TryRemove(key, out item);
            });
        }

        public virtual void Publish<T>(T message) where T : class
        {
            var callbacks = (from action in _callbacks.Values
                             let typedAction = action as Action<T>
                             where typedAction != null
                             select typedAction).ToList();

            foreach (var callback in callbacks)
                callback(message);
        }

        #endregion
    }
}