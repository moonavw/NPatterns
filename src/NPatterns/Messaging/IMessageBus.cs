using System;

namespace NPatterns.Messaging
{
    public interface IMessageBus
    {
        IDisposable Subscribe<T>(Action<T> callback);
        void Publish<T>(T message);
    }
}
