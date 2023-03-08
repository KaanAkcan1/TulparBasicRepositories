using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TulparResponses.v1.Responses;

namespace TulparBasicRepositories
{

    public interface IRepository<T> where T : class
    {
        DbSet<T> Table { get; }

        #region IReadRepository
        IQueryable<T> GetAll(bool tracking = true);

        IQueryable<T> FindAll(Expression<Func<T, bool>> method, bool tracking = true);

        Task<T> GetAsync(Expression<Func<T, bool>> method, bool tracking = true);

        #endregion

        #region IWriteRepository
        

        DataResponse<T> Update(T model);

        Task<DataResponse<T>> CreateAsync(T model);

        DataResponse<T> HardDelete(T model);
        
        #endregion
    }

}
