using System.Runtime.Serialization;

namespace NPatterns.ObjectRelational
{
    /// <summary>
    /// A Filter Descriptor for QueryObject
    /// </summary>
    [DataContract]
    public class Criteria
    {
        [DataMember(Name = "field")]
        public string Field { get; set; }

        [DataMember(Name = "op")]
        public CriteriaOperator Operator { get; set; }

        [DataMember(Name = "data")]
        public object Value { get; set; }
    }
}