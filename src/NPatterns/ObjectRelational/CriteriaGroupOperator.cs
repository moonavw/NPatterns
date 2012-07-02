using System.Runtime.Serialization;

namespace NPatterns.ObjectRelational
{
    [DataContract]
    public enum CriteriaGroupOperator
    {
        [EnumMember(Value = "AND")] All,
        [EnumMember(Value = "OR")] Any
    }
}