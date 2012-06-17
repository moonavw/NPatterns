using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace NPatterns.ObjectRelational
{
    [DataContract]
    public class CriteriaGroup
    {
        public CriteriaGroup()
        {
            Criterias = new List<Criteria>();
        }

        [DataMember(Name = "rules")]
        public List<Criteria> Criterias { get; set; }

        [DataMember(Name = "groupop")]
        public CriteriaLogicalOperator LogicalOperator { get; set; }

        public bool Valid
        {
            get { return Criterias != null && Criterias.Count > 0; }
        }

        public void MoveTo(CriteriaGroup target, string prefix)
        {
            var rules = from rule in Criterias
                        where rule.Field.StartsWith(prefix)
                        select new Criteria
                                   {
                                       Field = rule.Field.Remove(0, prefix.Length),
                                       Data = rule.Data,
                                       Operator = rule.Operator
                                   };

            target.Criterias.AddRange(rules);

            Criterias.RemoveAll(rule => rule.Field.StartsWith(prefix));
        }
    }
}