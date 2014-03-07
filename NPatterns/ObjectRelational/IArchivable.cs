using System;

namespace NPatterns.ObjectRelational
{
    public interface IArchivable
    {
        DateTime? Deleted { get; set; }

        string DeletedBy { get; set; }

        //TODO: ArchiveKey for recovery this when creating same one
    }
}