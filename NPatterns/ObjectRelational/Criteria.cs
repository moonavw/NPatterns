using System.Runtime.Serialization;

namespace NPatterns.ObjectRelational
{
    /// <summary>
    /// A Filter Descriptor for QueryObject
    /// </summary>
    [DataContract]
    public class Criteria
    {
        /// <summary>
        /// filter by field
        /// </summary>
        [DataMember(Name = "field")]
        public string Field { get; set; }

        /// <summary>
        /// filter operator
        /// </summary>
        [DataMember(Name = "op")]
        public CriteriaOperator Operator { get; set; }

        /// <summary>
        /// filter by what value, e.g. "field >= value"
        /// </summary>
        [DataMember(Name = "data")]
        public object Value { get; set; }

        /// <summary>
        /// has field or not
        /// </summary>
        public bool Valid
        {
            get { return !string.IsNullOrWhiteSpace(Field); }
        }
    }
}