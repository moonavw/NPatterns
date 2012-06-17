using System.Runtime.Serialization;

namespace NPatterns.ObjectRelational
{
    [DataContract]
    public enum CriteriaOperator
    {
        [EnumMember(Value = "eq")] IsEqualTo,
        [EnumMember(Value = "ne")] IsNotEqualTo,
        [EnumMember(Value = "lt")] IsLessThan,
        [EnumMember(Value = "le")] IsLessThanOrEqualTo,
        [EnumMember(Value = "gt")] IsGreaterThan,
        [EnumMember(Value = "ge")] IsGreaterThanOrEqualTo,
        [EnumMember(Value = "bw")] BeginsWith,
        [EnumMember(Value = "bn")] DoesNotBeginWith,
        [EnumMember(Value = "ew")] EndsWith,
        [EnumMember(Value = "en")] DoesNotEndWith,
        [EnumMember(Value = "cn")] Contains,
        [EnumMember(Value = "nc")] DoesNotContain,
        [EnumMember(Value = "nu")] IsNull,
        [EnumMember(Value = "nn")] IsNotNull,
        [EnumMember(Value = "in")] IsContainedIn,
        [EnumMember(Value = "ni")] IsNotContainedIn
    }
}