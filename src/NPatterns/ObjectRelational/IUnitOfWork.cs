namespace NPatterns.ObjectRelational
{
    public interface IUnitOfWork
    {
        void Commit();

        IRepository<T> Repository<T>() where T : class;
    }
}