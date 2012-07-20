using System.Linq;

namespace NPatterns.ObjectRelational
{
    public static class QueryObjectExtensions
    {
        /// <summary>
        /// Execute the QueryObject on source to get filtered result
        /// </summary>
        /// <typeparam name="T">type of element in source</typeparam>
        /// <param name="source">queryable source</param>
        /// <param name="query">QueryObject to execute</param>
        /// <returns>filtered queryable result</returns>
        public static IQueryable<T> Execute<T>(this IQueryable<T> source, QueryObject query) where T : class
        {
            return query.Execute(source);
        }
    }
}