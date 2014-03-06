using System.Threading.Tasks;

namespace NPatterns
{
    /// <summary>
    /// A handler could handle a specified message, like command, event, request etc
    /// </summary>
    /// <typeparam name="T">type of the message</typeparam>
    public interface IHandler<in T>
        where T : class
    {
        /// <summary>
        /// Handle the message
        /// </summary>
        /// <param name="message">message to be handled</param>
        void Handle(T message);

        /// <summary>
        /// Async Handle the message
        /// </summary>
        /// <param name="message">message to be handled</param>
        Task HandleAsync(T message);
    }
}