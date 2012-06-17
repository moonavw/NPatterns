using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace NPatterns.ObjectRelational
{
    /// <summary>
    /// DynamicQuery
    /// using System.Linq.Dynamic;
    /// .Where(aQueryObject.Predicate,aQueryObject.Values);
    /// </summary>
    public class QueryObject
    {
        private readonly List<object> _values = new List<object>();
        private readonly StringBuilder _predicate = new StringBuilder();
        private readonly IEnumerable<PropertyInfo> _properties;

        public QueryObject(Type elementType)
            : this(elementType.GetProperties())
        {
        }

        public QueryObject(IEnumerable<PropertyInfo> elementProperties)
        {
            _properties = elementProperties;
        }

        public string Predicate
        {
            get { return _predicate.ToString(); }
        }

        public object[] Values
        {
            get { return _values.ToArray(); }
        }

        public void Add(CriteriaLogicalOperator logicalOperator, Criteria criteria)
        {
            Build(_predicate, logicalOperator, criteria);
        }

        public void Add(CriteriaLogicalOperator logicalOperator, CriteriaGroup criteriaGroup)
        {
            Build(_predicate, logicalOperator, criteriaGroup);
        }

        private void Build(StringBuilder builder, CriteriaLogicalOperator logicalOperator, CriteriaGroup criteriaGroup)
        {
            var sb = new StringBuilder();
            foreach (var criteria in criteriaGroup.Criterias)
                Build(sb, criteriaGroup.LogicalOperator, criteria);

            if (sb.Length == 0)
                return;

            if (builder.Length > 0)
                builder.Append(logicalOperator);

            builder.AppendFormat("({0})", sb);
        }

        private void Build(StringBuilder builder, CriteriaLogicalOperator logicalOperator, Criteria criteria)
        {
            var property = _properties.FirstOrDefault(z => z.Name == criteria.Field);
            if (property == null)
                return;

            if (criteria.Operator != CriteriaOperator.IsContainedIn &&
                criteria.Operator != CriteriaOperator.IsNotContainedIn)
            {
                criteria.Value = ConvertEx.ChangeType(criteria.Value, property.PropertyType);
                if (builder.Length > 0)
                    builder.Append(logicalOperator);
            }

            switch (criteria.Operator)
            {
                case CriteriaOperator.IsContainedIn:
                    {
                        var q =
                            from z in
                                criteria.Value.ToString().Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                            select
                                new Criteria { Field = criteria.Field, Operator = CriteriaOperator.IsEqualTo, Value = z.Trim() };

                        var group = new CriteriaGroup { LogicalOperator = CriteriaLogicalOperator.Or, Criterias = q.ToList() };

                        Build(builder, logicalOperator, group);
                    }
                    break;
                case CriteriaOperator.IsNotContainedIn:
                    {
                        var q =
                            from z in
                                criteria.Value.ToString().Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                            select
                                new Criteria { Field = criteria.Field, Operator = CriteriaOperator.IsNotEqualTo, Value = z.Trim() };

                        var group = new CriteriaGroup { LogicalOperator = CriteriaLogicalOperator.And, Criterias = q.ToList() };

                        Build(builder, logicalOperator, group);
                    }
                    break;
                case CriteriaOperator.IsEqualTo:
                    builder.AppendFormat("{0}==@{1}", criteria.Field, _values.Count);
                    _values.Add(criteria.Value);
                    break;
                case CriteriaOperator.IsNotEqualTo:
                    builder.AppendFormat("{0}!=@{1}", criteria.Field, _values.Count);
                    _values.Add(criteria.Value);
                    break;
                case CriteriaOperator.IsLessThan:
                    builder.AppendFormat("{0}<@{1}", criteria.Field, _values.Count);
                    _values.Add(criteria.Value);
                    break;
                case CriteriaOperator.IsLessThanOrEqualTo:
                    builder.AppendFormat("{0}<=@{1}", criteria.Field, _values.Count);
                    _values.Add(criteria.Value);
                    break;
                case CriteriaOperator.IsGreaterThan:
                    builder.AppendFormat("{0}>@{1}", criteria.Field, _values.Count);
                    _values.Add(criteria.Value);
                    break;
                case CriteriaOperator.IsGreaterThanOrEqualTo:
                    builder.AppendFormat("{0}>=@{1}", criteria.Field, _values.Count);
                    _values.Add(criteria.Value);
                    break;
                case CriteriaOperator.BeginsWith:
                    builder.AppendFormat("{0}.StartsWith(@{1})", criteria.Field, _values.Count);
                    _values.Add(criteria.Value);
                    break;
                case CriteriaOperator.DoesNotBeginWith:
                    builder.AppendFormat("{0}.StartsWith(@{1})==false", criteria.Field, _values.Count);
                    _values.Add(criteria.Value);
                    break;
                case CriteriaOperator.EndsWith:
                    builder.AppendFormat("{0}.EndsWith(@{1})", criteria.Field, _values.Count);
                    _values.Add(criteria.Value);
                    break;
                case CriteriaOperator.DoesNotEndWith:
                    builder.AppendFormat("{0}.EndsWith(@{1})==false", criteria.Field, _values.Count);
                    _values.Add(criteria.Value);
                    break;
                case CriteriaOperator.Contains:
                    builder.AppendFormat("{0}.Contains(@{1})", criteria.Field, _values.Count);
                    _values.Add(criteria.Value);
                    break;
                case CriteriaOperator.DoesNotContain:
                    builder.AppendFormat("{0}.Contains(@{1})==false", criteria.Field, _values.Count);
                    _values.Add(criteria.Value);
                    break;
                case CriteriaOperator.IsNull:
                    builder.AppendFormat("{0}==null", criteria.Field);
                    break;
                case CriteriaOperator.IsNotNull:
                    builder.AppendFormat("{0}!=null", criteria.Field);
                    break;
            }
        }
    }
}