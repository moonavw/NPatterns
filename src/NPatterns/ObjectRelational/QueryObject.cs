using System;
using System.Collections.Generic;
using System.Linq;

namespace NPatterns.ObjectRelational
{
    /// <summary>
    /// Base class for implement a "QueryObject Pattern", for
    /// more information about this pattern see http://martinfowler.com/eaaCatalog/queryObject.html
    /// </summary>
    public abstract class QueryObject
    {
        protected QueryObject()
        {
            CriteriaGroups = new List<Tuple<CriteriaGroup, CriteriaGroupOperator>>();
        }

        /// <summary>
        /// appended criteria groups in this Query object.
        /// each group has a logical operator for concatenating groups
        /// </summary>
        protected List<Tuple<CriteriaGroup, CriteriaGroupOperator>> CriteriaGroups { get; set; }

        /// <summary>
        /// has criteria or not
        /// </summary>
        public bool Valid
        {
            get { return CriteriaGroups != null && CriteriaGroups.Count > 0; }
        }

        /// <summary>
        /// append criteria with logical operator to this query object
        /// </summary>
        /// <param name="criteria">criteria to add</param>
        /// <param name="op">AND|OR the criteria</param>
        public void Add(Criteria criteria, CriteriaGroupOperator op = CriteriaGroupOperator.And)
        {
            CriteriaGroups.Add(
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
            CriteriaGroups.Add(
                new Tuple<CriteriaGroup, CriteriaGroupOperator>(
                    criteriaGroup,
                    op));
        }

        /// <summary>
        /// Execute the QueryObject on source to get filtered result
        /// </summary>
        /// <typeparam name="T">type of the element in source</typeparam>
        /// <param name="source">queryable source to filter</param>
        /// <returns>filtered queryable result</returns>
        public abstract IQueryable<T> Execute<T>(IQueryable<T> source) where T : class;
    }
}