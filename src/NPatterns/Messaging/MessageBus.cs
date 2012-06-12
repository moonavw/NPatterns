using System;
using System.Collections.Concurrent;
using System.Linq;
using Microsoft.Practices.ServiceLocation;

namespace NPatterns.Messaging
{
    public static class MessageBus
    {
        internal static readonly ConcurrentDictionary<int, Delegate> Callbacks;

        static MessageBus()
        {
            Callbacks = new ConcurrentDictionary<int, Delegate>();
        }

        public static IDisposable Subscribe<T>(Action<T> callback)
        {
            int key = callback.GetHashCode();
            Callbacks.TryAdd(key, callback);

            return new SubscriptionRemover(() =>
                                               {
                                                   Delegate item;
                                                   Callbacks.TryRemove(key, out item);
                                               });
        }

        public static void Publish<T>(T message)
        {
            var handlers = ServiceLocator.Current.GetAllInstances<IHandler<T>>();
            foreach (var handler in handlers)
                handler.Handle(message);

            var callbacks = (from action in Callbacks.Values
                             let typedAction = action as Action<T>
                             where typedAction != null
                             select typedAction).ToList();

            foreach (var callback in callbacks)
                callback(message);
        }

        #region Nested type: SubscriptionRemover

        private sealed class SubscriptionRemover : IDisposable
        {
            private readonly Action _callOnDispose;

            public SubscriptionRemover(Action toCall)
            {
                _callOnDispose = toCall;
            }

            #region IDisposable Members

            public void Dispose()
            {
                _callOnDispose();
            }

            #endregion
        }

        #endregion
    }
}