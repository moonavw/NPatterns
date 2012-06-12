using System.Linq;

namespace NPatterns.ObjectRelational
{
    public interface IRepository<T> where T : class
    {
        IQueryable<T> AsQueryable();
        T Find(params object[] keyValues);
        void Add(T entity);
        void Attach(T entity);
        void Remove(T entity);
    }
}