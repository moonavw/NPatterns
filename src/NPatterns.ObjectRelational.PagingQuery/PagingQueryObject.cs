using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using PagedList;

namespace NPatterns.ObjectRelational.PagingQuery
{
    /// <summary>
    /// a wrapper of QueryObject to support paging
    /// </summary>
    public class PagingQueryObject
    {
        public PagingQueryObject()
        {
            SortObjects = new List<SortObject>();
        }

        /// <summary>
        /// number of current page
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        /// how many items in a page
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// sorting by a list of fields
        /// </summary>
        public List<SortObject> SortObjects { get; set; }

        /// <summary>
        /// the query object for filtering
        /// </summary>
        public QueryObject QueryObject { get; set; }

        /// <summary>
        /// execute the PagingQueryObject on source to get paged filtered result
        /// </summary>
        /// <typeparam name="T">type of element in source</typeparam>
        /// <param name="source">queryable source to filter</param>
        /// <returns>paged queryable result</returns>
        public IPagedList<T> Execute<T>(IQueryable<T> source) where T : class
        {
            var filteredSource = source;
            if (QueryObject != null && QueryObject.Valid)
                filteredSource = filteredSource.Execute(QueryObject);

            if (SortObjects == null || SortObjects.Count == 0)
                SortObjects = new List<SortObject> { new SortObject { Field = "Id", Direction = "desc" } };

            filteredSource = SortObjects.Aggregate(filteredSource, (current, sort) => current.OrderBy(sort.ToString()));
            return filteredSource.ToPagedList(PageNumber, PageSize);
        }

        #region Nested type: SortObject

        /// <summary>
        /// object to describe the sorting
        /// </summary>
        public class SortObject
        {
            /// <summary>
            /// sort by field (order by this field)
            /// </summary>
            public string Field { get; set; }

            /// <summary>
            /// asc or desc
            /// </summary>
            public string Direction { get; set; }

            public override string ToString()
            {
                return (Field + " " + Direction).Trim();
            }
        }

        #endregion
    }
}