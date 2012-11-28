using System.Runtime.Serialization;

namespace NPatterns.ObjectRelational
{
    /// <summary>
    /// sort direction for sort description
    /// </summary>
    [DataContract]
    public enum SortDirection
    {
        [EnumMember(Value = "asc")]
        Ascending,
        [EnumMember(Value = "desc")]
        Descending
    }
}