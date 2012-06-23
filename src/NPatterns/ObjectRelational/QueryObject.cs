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
    public class QueryObject<T> where T : class
    {
        private readonly List<object> _values;
        private readonly StringBuilder _predicate;
        private readonly PropertyInfo[] _properties;

        public QueryObject()
        {
            _properties = typeof(T).GetProperties();
            _predicate = new StringBuilder();
            _values = new List<object>();
        }

        public string Predicate
        {
            get { return _predicate.ToString(); }
        }

        public object[] Values
        {
            get { return _values.ToArray(); }
        }

        public void Add(CriteriaGroupOperator op, Criteria criteria)
        {
            Build(_predicate, op, criteria);
        }

        public void Add(CriteriaGroupOperator op, CriteriaGroup criteriaGroup)
        {
            Build(_predicate, op, criteriaGroup);
        }

        #region predicate builder
        private void Build(StringBuilder builder, CriteriaGroupOperator op, CriteriaGroup criteriaGroup)
        {
            var sb = new StringBuilder();
            foreach (var criteria in criteriaGroup.Criterias)
                Build(sb, criteriaGroup.Operator, criteria);

            if (sb.Length == 0)
                return;

            if (builder.Length > 0)
                builder.AppendFormat(" {0} ", op);

            builder.AppendFormat("({0})", sb);
        }

        private void Build(StringBuilder builder, CriteriaGroupOperator op, Criteria criteria)
        {
            var property = _properties.FirstOrDefault(z => z.Name == criteria.Field);
            if (property == null)
                return;

            int valueIndex = _values.Count;
            if (criteria.Operator != CriteriaOperator.IsContainedIn &&
                criteria.Operator != CriteriaOperator.IsNotContainedIn)
            {
                if (builder.Length > 0)
                    builder.AppendFormat(" {0} ", op);

                if (criteria.Operator != CriteriaOperator.IsNull &&
                    criteria.Operator != CriteriaOperator.IsNotNull)
                {
                    criteria.Value = ConvertEx.ChangeType(criteria.Value, property.PropertyType);
                    _values.Add(criteria.Value);
                }
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

                        var group = new CriteriaGroup { Operator = CriteriaGroupOperator.Or, Criterias = q.ToList() };

                        Build(builder, op, group);
                    }
                    break;
                case CriteriaOperator.IsNotContainedIn:
                    {
                        var q =
                            from z in
                                criteria.Value.ToString().Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                            select
                                new Criteria { Field = criteria.Field, Operator = CriteriaOperator.IsNotEqualTo, Value = z.Trim() };

                        var group = new CriteriaGroup { Operator = CriteriaGroupOperator.And, Criterias = q.ToList() };

                        Build(builder, op, group);
                    }
                    break;
                default:
                    var format = GetFormat(criteria.Operator, property.PropertyType);
                    if (!string.IsNullOrEmpty(format))
                        builder.AppendFormat(format, criteria.Field, valueIndex);
                    break;
            }
        }
        #endregion

        #region predicate format provider
        private string GetFormat(CriteriaOperator op, Type fieldType)
        {
            switch (op)
            {
                case CriteriaOperator.IsEqualTo:
                    return IsEqualTo(fieldType);
                case CriteriaOperator.IsNotEqualTo:
                    return IsNotEqualTo(fieldType);
                case CriteriaOperator.IsLessThan:
                    return IsLessThan(fieldType);
                case CriteriaOperator.IsLessThanOrEqualTo:
                    return IsLessThanOrEqualTo(fieldType);
                case CriteriaOperator.IsGreaterThan:
                    return IsGreaterThan(fieldType);
                case CriteriaOperator.IsGreaterThanOrEqualTo:
                    return IsGreaterThanOrEqualTo(fieldType);
                case CriteriaOperator.BeginsWith:
                    return BeginsWith(fieldType);
                case CriteriaOperator.DoesNotBeginWith:
                    return DoesNotBeginWith(fieldType);
                case CriteriaOperator.EndsWith:
                    return EndsWith(fieldType);
                case CriteriaOperator.DoesNotEndWith:
                    return DoesNotEndWith(fieldType);
                case CriteriaOperator.Contains:
                    return Contains(fieldType);
                case CriteriaOperator.DoesNotContain:
                    return DoesNotContain(fieldType);
                case CriteriaOperator.IsNull:
                    return IsNull(fieldType);
                case CriteriaOperator.IsNotNull:
                    return IsNotNull(fieldType);
                default:
                    return null;
            }
        }

        protected virtual string IsNotNull(Type fieldType)
        {
            return "{0}!=null";
        }

        protected virtual string IsNull(Type fieldType)
        {
            return "{0}==null";
        }

        protected virtual string DoesNotContain(Type fieldType)
        {
            return "{0}.ToLower().Contains(@{1}.ToLower())==false";
        }

        protected virtual string Contains(Type fieldType)
        {
            return "{0}.ToLower().Contains(@{1}.ToLower())";
        }

        protected virtual string DoesNotEndWith(Type fieldType)
        {
            return "{0}.ToLower().EndsWith(@{1}.ToLower())==false";
        }

        protected virtual string EndsWith(Type fieldType)
        {
            return "{0}.ToLower().EndsWith(@{1}.ToLower())";
        }

        protected virtual string DoesNotBeginWith(Type fieldType)
        {
            return "{0}.ToLower().StartsWith(@{1}.ToLower())==false";
        }

        protected virtual string BeginsWith(Type fieldType)
        {
            return "{0}.ToLower().StartsWith(@{1}.ToLower())";
        }

        protected virtual string IsGreaterThanOrEqualTo(Type fieldType)
        {
            return "{0}>=@{1}";
        }

        protected virtual string IsGreaterThan(Type fieldType)
        {
            return "{0}>@{1}";
        }

        protected virtual string IsLessThanOrEqualTo(Type fieldType)
        {
            return "{0}<=@{1}";
        }

        protected virtual string IsLessThan(Type fieldType)
        {
            return "{0}<@{1}";
        }

        protected virtual string IsNotEqualTo(Type fieldType)
        {
            if (fieldType == typeof(string))
                return "{0}.ToLower()!=@{1}.ToLower()";
            return "{0}!=@{1}";
        }

        protected virtual string IsEqualTo(Type fieldType)
        {
            if (fieldType == typeof(string))
                return "{0}.ToLower()==@{1}.ToLower()";
            return "{0}==@{1}";
        }
        #endregion
    }
}