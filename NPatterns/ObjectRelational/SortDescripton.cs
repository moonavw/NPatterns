using System.Runtime.Serialization;

namespace NPatterns.ObjectRelational
{
    /// <summary>
    /// object to describe the sorting
    /// </summary>
    [DataContract]
    public class SortDescription
    {
        /// <summary>
        /// sort by field (order by this field)
        /// </summary>
        [DataMember(Name = "sidx")]
        public string Field { get; set; }

        /// <summary>
        /// asc or desc
        /// </summary>
        [DataMember(Name = "sord")]
        public SortDirection Direction { get; set; }

        /// <summary>
        /// has field or not
        /// </summary>
        public bool Valid
        {
            get { return !string.IsNullOrWhiteSpace(Field); }
        }
    }
}