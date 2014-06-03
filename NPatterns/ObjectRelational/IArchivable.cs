using System;
using System.Collections.Generic;
using System.Linq;

namespace NPatterns.ObjectRelational
{
    public interface IArchivable
    {
        DateTime? Deleted { get; set; }

        string DeletedBy { get; set; }

        //TODO: ArchiveKey for recovery this when creating same one
    }

    public static class ArchivableEx
    {
        public static IEnumerable<T> Actives<T>(this IEnumerable<T> query) where T : IArchivable
        {
            return query.Where(z => z.Deleted == null);
        }

        public static IEnumerable<T> Archives<T>(this IEnumerable<T> query) where T : IArchivable
        {
            return query.Where(z => z.Deleted != null);
        }
    }
}