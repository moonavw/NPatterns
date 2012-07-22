using System.Linq;
using PagedList;

namespace NPatterns.ObjectRelational.PagingQuery
{
    public static class PagingQueryObjectExtensions
    {
        /// <summary>
        /// execute the PagingQueryObject on source to get paged filtered result
        /// </summary>
        /// <typeparam name="T">type of element in source</typeparam>
        /// <param name="source">queryable source to filter</param>
        /// <param name="query">PagingQueryObject to execute</param>
        /// <returns>paged queryable result</returns>
        public static IPagedList<T> Execute<T>(this IQueryable<T> source, PagingQueryObject query) where T : class
        {
            return query.Execute(source);
        }
    }
}