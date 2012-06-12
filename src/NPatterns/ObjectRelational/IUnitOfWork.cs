namespace NPatterns.ObjectRelational
{
    public interface IUnitOfWork
    {
        void Commit();

        IRepository<T> Repository<T>() where T : class;
        void MarkNew<T>(T entity) where T : class;
        void MarkModified<T>(T entity) where T : class;
        void MarkDeleted<T>(T entity) where T : class;
        void MarkUnchanged<T>(T entity) where T : class;
        void MarkDetached<T>(T entity) where T : class;
    }
}