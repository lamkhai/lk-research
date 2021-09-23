using AspnetCoreBase.Interceptors;
using AspnetCoreBase.Models;
using AspnetCoreBase.Repositories;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AspnetCoreBase.Controllers
{
    [ApiController]
    public class BaseController<V, X> : Controller where V : IBaseRepository<X> where X : BaseModel
    {
        protected readonly V _repository;

        public BaseController(V repository)
        {
            _repository = repository;
        }

        [HttpPost]
        public virtual X Create(X record)
        {
            return _repository.Create(record);
        }

        [HttpDelete("{id}")]
        public virtual void Delete(string id)
        {
            _repository.Delete(Guid.Parse(id));
        }

        [HttpGet("{id}")]
        public virtual X Get(string id)
        {
            return _repository.Get(Guid.Parse(id));
        }

        [HttpGet]
        public virtual List<X> GetAll()
        {
            return _repository.GetQueryable().ToList();
        }

        [HttpPost("{id}")]
        public virtual X Update(string id, X record)
        {
            if (record.Id != Guid.Parse(id))
            {
                throw new ApiException("Invalid Request");
            }
            return _repository.Update(record);
        }
    }
}