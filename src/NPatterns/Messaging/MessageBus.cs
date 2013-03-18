using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
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
        private readonly ConcurrentDictionary<int, HandlingCounter> _handlingCounters;

        public MessageBus()
        {
            _subscriptions = new ConcurrentDictionary<int, Subscription>();
            _handlingCounters = new ConcurrentDictionary<int, HandlingCounter>();
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

        public virtual bool Publish<T>(T message) where T : class
        {
            var subscribers = GetSubscribers<T>().ToList();

            foreach (var callback in subscribers)
                callback(message);

            return subscribers.Count > 0;
        }

        public virtual bool PublishAsync<T>(T message, Action callbackOnAllDone = null, Action callbackOnAnyDone = null) where T : class
        {
            var subscribers = GetSubscribers<T>().ToList();

            StartHandlingCount(message.GetHashCode(), subscribers.Count);

            foreach (var callback in subscribers)
            {
                Action<T> action = callback;
                Task.Factory.StartNew(p =>
                                          {
                                              action((T)p);
                                              //callback for this handler done
                                              UpdateHandlingCount(p.GetHashCode(), callbackOnAllDone, callbackOnAnyDone);
                                          }, message);
            }

            return subscribers.Count > 0;
        }

        #endregion

        protected virtual void StartHandlingCount(int key, int maxCount)
        {
            _handlingCounters.TryAdd(key, new HandlingCounter(maxCount));
        }
        protected virtual void UpdateHandlingCount(int key, Action callbackOnAllDone, Action callbackOnAnyDone)
        {
            if (callbackOnAnyDone != null)
                callbackOnAnyDone();

            HandlingCounter counter;
            if (_handlingCounters.TryGetValue(key, out counter))
            {
                counter.Increment();
                Debug.WriteLine("progress: {0}/{1}", counter.CurrentCount, counter.MaxCount);
                if (counter.IsCompleted)
                {
                    _handlingCounters.TryRemove(key, out counter);

                    if (callbackOnAllDone != null)
                        callbackOnAllDone();
                }
            }
        }

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

        private class HandlingCounter
        {
            private readonly int _expectedValue;
            private int _actualValue;

            public HandlingCounter(int maxCount)
            {
                _expectedValue = maxCount;
            }

            public int MaxCount
            {
                get { return _expectedValue; }
            }

            public int CurrentCount
            {
                get { return _actualValue; }
            }

            public bool IsCompleted
            {
                get { return _actualValue >= _expectedValue; }
            }

            public void Increment()
            {
                System.Threading.Interlocked.Increment(ref _actualValue);
            }

            public override string ToString()
            {
                return string.Format("progress: {0}/{1}", _actualValue, _expectedValue);
            }
        }
    }
}