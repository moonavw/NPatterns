using System;

namespace NPatterns.Messaging
{
    public interface IMessageBus
    {
        IDisposable Subscribe<T>(Action<T> callback) where T : class;
        void Publish<T>(T message) where T : class;
    }
}
