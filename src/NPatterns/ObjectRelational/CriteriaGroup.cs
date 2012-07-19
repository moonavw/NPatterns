using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace NPatterns.ObjectRelational
{
    /// <summary>
    /// A collection of Criteria for QueryObject
    /// </summary>
    [DataContract]
    public class CriteriaGroup
    {
        public CriteriaGroup()
        {
            Criterias = new List<Criteria>();
        }

        /// <summary>
        /// Criteria in this Group
        /// </summary>
        [DataMember(Name = "rules")]
        public List<Criteria> Criterias { get; set; }

        /// <summary>
        /// logical operator (AND|OR) for concatenating the collection of criteria. 
        /// </summary>
        [DataMember(Name = "groupop")]
        public CriteriaGroupOperator Operator { get; set; }

        /// <summary>
        /// has criteria or not
        /// </summary>
        public bool Valid
        {
            get { return Criterias != null && Criterias.Count > 0; }
        }

        /// <summary>
        /// move the criteria which has the field name begin with prefix to another group
        /// </summary>
        /// <param name="dest">destination</param>
        /// <param name="prefix">field name begin with the string</param>
        public void MoveTo(CriteriaGroup dest, string prefix)
        {
            var rules = from rule in Criterias
                        where rule.Field.StartsWith(prefix)
                        select new Criteria
                                   {
                                       Field = rule.Field.Remove(0, prefix.Length),
                                       Value = rule.Value,
                                       Operator = rule.Operator
                                   };

            dest.Criterias.AddRange(rules);

            Criterias.RemoveAll(rule => rule.Field.StartsWith(prefix));
        }
    }
}