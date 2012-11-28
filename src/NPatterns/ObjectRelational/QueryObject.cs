using System;
using System.Collections.Generic;
using System.Linq;

namespace NPatterns.ObjectRelational
{
    /// <summary>
    /// implement a "QueryObject Pattern", for
    /// more information about this pattern see http://martinfowler.com/eaaCatalog/queryObject.html
    /// </summary>
    public class QueryObject
    {
        private readonly List<Tuple<CriteriaGroup, CriteriaGroupOperator>> _criteriaGroups =
            new List<Tuple<CriteriaGroup, CriteriaGroupOperator>>();

        private readonly List<SortDescription> _sortDescriptions = new List<SortDescription>();

        /// <summary>
        /// appended criteria groups in this Query object.
        /// each group has a logical operator for concatenating groups
        /// </summary>
        public IEnumerable<Tuple<CriteriaGroup, CriteriaGroupOperator>> CriteriaGroups
        {
            get { return _criteriaGroups; }
        }

        /// <summary>
        /// sorting by a list of fields
        /// </summary>
        public IEnumerable<SortDescription> SortDescriptions
        {
            get { return _sortDescriptions; }
        }

        /// <summary>
        /// has criteria or not
        /// </summary>
        public bool HasCriteria
        {
            get { return _criteriaGroups.Count > 0 && _criteriaGroups.Any(z => z.Item1.Valid); }
        }

        /// <summary>
        /// has sort description or not
        /// </summary>
        public bool HasSortDescription
        {
            get { return _sortDescriptions.Count > 0 && _sortDescriptions.Any(z => z.Valid); }
        }

        /// <summary>
        /// append criteria with logical operator to this query object
        /// </summary>
        /// <param name="criteria">criteria to add</param>
        /// <param name="op">AND|OR the criteria</param>
        public void Add(Criteria criteria, CriteriaGroupOperator op = CriteriaGroupOperator.And)
        {
            _criteriaGroups.Add(
                new Tuple<CriteriaGroup, CriteriaGroupOperator>(
                    new CriteriaGroup
                        {
                            Criterias = new List<Criteria> { criteria }
                        },
                    op));
        }

        /// <summary>
        /// append criteria group with logical operator to this query object
        /// </summary>
        /// <param name="criteriaGroup">criteria group to add</param>
        /// <param name="op">AND|OR the criteria group</param>
        public void Add(CriteriaGroup criteriaGroup, CriteriaGroupOperator op = CriteriaGroupOperator.And)
        {
            _criteriaGroups.Add(
                new Tuple<CriteriaGroup, CriteriaGroupOperator>(
                    criteriaGroup,
                    op));
        }

        /// <summary>
        /// append sort description for this query
        /// </summary>
        /// <param name="sortDescription"></param>
        public void Add(SortDescription sortDescription)
        {
            _sortDescriptions.Add(sortDescription);
        }

        /// <summary>
        /// Execute the QueryObject on source
        /// </summary>
        /// <typeparam name="T">type of the element in source</typeparam>
        /// <param name="source">queryable source to query</param>
        /// <param name="executor">query object executor </param>
        /// <returns>queryable result</returns>
        public IQueryable<T> Execute<T>(IQueryable<T> source, IQueryObjectExecutor executor) where T : class
        {
            return executor.Execute(source, this);
        }
    }
}