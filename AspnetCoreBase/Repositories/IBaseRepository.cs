using AspnetCoreBase.Models;
using System;
using System.Linq;

namespace AspnetCoreBase.Repositories
{
    public interface IBaseRepository<T> where T : BaseModel
    {
        T Create(T model);
        void Delete(Guid id);
        T Get(Guid id);
        IQueryable<T> GetQueryable();
        T Update(T model);
    }
}