namespace NPatterns.Messaging
{
    public interface IHandler<in T>
    {
        void Handle(T message);
    }
}