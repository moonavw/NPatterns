using System.Linq;
using System.Linq.Dynamic;

namespace NPatterns.ObjectRelational.DynamicQuery
{
    /// <summary>
    /// implement the QueryObjectExecutor with DynamicQuery (System.Linq.Dynamic)
    /// </summary>
    public class DynamicQueryObjectExecutor : IQueryObjectExecutor
    {
        private static string OrderBy(SortDescription sort)
        {
            return sort.Field + " " + (sort.Direction == SortDirection.Descending ? "desc" : "asc");
        }

        public IQueryable<T> Execute<T>(IQueryable<T> source, QueryObject query) where T : class
        {
            var res = source;

            if (query.HasCriteria)
            {
                var builder = new PredicateBuilder<T>();
                foreach (var criteriaGroup in query.CriteriaGroups)
                    builder.Add(criteriaGroup.Item1, criteriaGroup.Item2);

                if (!string.IsNullOrWhiteSpace(builder.Predicate))
                    res = source.Where(builder.Predicate, builder.Values);
            }

            if (query.HasSortDescription)
                res = query.SortDescriptions.Where(z => z.Valid)
                    .Aggregate(res, (current, sort) => current.OrderBy(OrderBy(sort)));

            return res;
        }
    }
}