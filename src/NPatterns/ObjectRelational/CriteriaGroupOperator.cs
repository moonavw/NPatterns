using System.Runtime.Serialization;

namespace NPatterns.ObjectRelational
{
    /// <summary>
    /// Logical operator
    /// </summary>
    [DataContract]
    public enum CriteriaGroupOperator
    {
        [EnumMember(Value = "AND")]
        And,
        [EnumMember(Value = "OR")]
        Or
    }
}