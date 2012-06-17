using System.Runtime.Serialization;

namespace NPatterns.ObjectRelational
{
    [DataContract]
    public class Criteria
    {
        [DataMember(Name = "field")]
        public string Field { get; set; }

        [DataMember(Name = "op")]
        public CriteriaOperator Operator { get; set; }

        /// <summary>
        /// string type of the property "Value"
        /// </summary>
        [DataMember(Name = "data")]
        public string Data
        {
            get { return Value.ToString(); }
            set { Value = value; }
        }

        public object Value { get; set; }
    }
}