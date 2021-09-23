using AspnetCoreBase.Contexts;
using AspnetCoreBase.Interceptors;
using AspnetCoreBase.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace AspnetCoreBase.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T> where T : BaseModel
    {
        protected readonly BaseContext _context;
        protected readonly ILogger<BaseRepository<T>> _logger;

        public BaseRepository(BaseContext context, ILogger<BaseRepository<T>> logger)
        {
            _context = context;
            _logger = logger;
        }

        public T Create(T model)
        {
            try
            {
                model.CreatedDateTime = DateTime.UtcNow;
                _context.Set<T>().Add(model);
                _context.SaveChanges();
                _context.Entry(model).State = EntityState.Detached;
                logInformation("Create", model.Id.ToString());
                return model;
            }
            catch (Exception ex)
            {
                logException("Create", ex.Message);
                throw new ApiException(ex.Message);
            }
        }

        public void Delete(Guid id)
        {
            try
            {
                var model = Get(id);
                if (model != null)
                {
                    _context.Set<T>().Remove(model);
                    _context.SaveChanges();
                    _context.Entry(model).State = EntityState.Detached;
                    logInformation("Delete", id.ToString());
                }
                throw new ApiException("Id Couldn't Be Found", 404);
            }
            catch (Exception ex)
            {
                logException("Delete", ex.Message);
                throw new ApiException(ex.Message);
            }
        }

        public T Get(Guid id)
        {
            try
            {
                return _context.Set<T>().AsNoTracking().FirstOrDefault(x => x.Id == id);
            }
            catch (Exception ex)
            {
                logException("Get", ex.Message);
                throw new ApiException(ex.Message);
            }
        }

        public IQueryable<T> GetQueryable()
        {
            try
            {
                return _context.Set<T>().AsNoTracking();
            }
            catch (Exception ex)
            {
                logException("GetQueryable", ex.Message);
                throw new ApiException(ex.Message);
            }
        }

        public T Update(T model)
        {
            try
            {
                var baseModel = Get(model.Id);
                model.CreatedDateTime = baseModel.CreatedDateTime;
                model.UpdatedDateTime = DateTime.UtcNow.ToLocalTime();
                _context.Set<T>().Update(model);
                _context.SaveChanges();
                _context.Entry(model).State = EntityState.Detached;
                logInformation("Edit", model.Id.ToString());
                return model;
            }
            catch (Exception ex)
            {
                logException("Edit", ex.Message);
                throw new ApiException(ex.Message);
            }
        }

        private void logException(string activity, string message)
        {
            _logger.LogError("{@Activity} {@TName}: {@Message}", activity, typeof(T).Name, message);
        }

        private void logInformation(string activity, string message)
        {
            _logger.LogInformation("{@Activity} {@TName}: {@Message}", activity, typeof(T).Name, message);
        }
    }
}