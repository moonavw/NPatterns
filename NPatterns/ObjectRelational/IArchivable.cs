using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

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

        public static void Active(this IArchivable entity)
        {
            entity.Deleted = null;
            entity.DeletedBy = null;
        }

        public static void Archive(this IArchivable entity)
        {
            entity.Deleted = DateTime.Now;
            entity.DeletedBy = Thread.CurrentPrincipal.Identity.Name;
        }
    }
}