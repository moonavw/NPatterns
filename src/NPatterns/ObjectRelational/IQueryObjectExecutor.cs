using System.Linq;

namespace NPatterns.ObjectRelational
{
    /// <summary>
    /// interface for executing an query object
    /// </summary>
    public interface IQueryObjectExecutor
    {
        /// <summary>
        /// Execute the QueryObject on source
        /// </summary>
        /// <typeparam name="T">type of the element in source</typeparam>
        /// <param name="source">queryable source to query</param>
        /// <param name="query">query object to execute</param>
        /// <returns>queryable result</returns>
        IQueryable<T> Execute<T>(IQueryable<T> source, QueryObject query) where T : class;
    }
}