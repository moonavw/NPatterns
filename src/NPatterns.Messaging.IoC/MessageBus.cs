using Microsoft.Practices.ServiceLocation;

namespace NPatterns.Messaging.IoC
{
    /// <summary>
    /// implement MessageBus with ServiceLocator to get all Handlers
    /// </summary>
    public class MessageBus : Messaging.MessageBus, IMessageBus
    {
        #region IMessageBus Members

        public override void Publish<T>(T message)
        {
            var handlers = ServiceLocator.Current.GetAllInstances<IHandler<T>>();
            foreach (var handler in handlers)
                handler.Handle(message);

            base.Publish(message);
        }

        #endregion
    }
}