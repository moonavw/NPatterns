using System;

namespace NPatterns.ObjectRelational
{
    public interface IArchivable
    {
        DateTime? Deleted { get; set; }

        string DeletedBy { get; set; }
    }
}