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
            get { return Criterias != null && Criterias.Count > 0 && Criterias.Any(z => z.Valid); }
        }
    }
}