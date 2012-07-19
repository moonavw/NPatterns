using System.Linq;
using System.Linq.Dynamic;

namespace NPatterns.ObjectRelational.DynamicQuery
{
    /// <summary>
    /// implement the QueryObject with DynamicQuery (System.Linq.Dynamic)
    /// </summary>
    public class QueryObject : NPatterns.ObjectRelational.QueryObject
    {
        public override IQueryable<T> Execute<T>(IQueryable<T> source)
        {
            if (!Valid)
                return source;

            var builder = new PredicateBuilder(typeof(T));
            foreach (var criteriaGroup in CriteriaGroups)
                builder.Add(criteriaGroup.Item1, criteriaGroup.Item2);

            return source.Where(builder.Predicate, builder.Values);
        }
    }
}