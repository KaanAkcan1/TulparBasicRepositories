using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TulparResponses.v1.Responses;

namespace TulparBasicRepositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly DbContext _context;

        public Repository(DbContext context)
        {
            _context = context;
        }

        public DbSet<T> Table => _context.Set<T>();

        #region ReadRepository
        public IQueryable<T> FindAll(Expression<Func<T, bool>> method, bool tracking = true)
        {
            var query = Table.Where(method);
            if (!tracking)
                query = query.AsNoTracking();
            return query;
        }

        public IQueryable<T> GetAll(bool tracking = true)
        {
            var query = Table.AsQueryable();
            if (!tracking)
                query = query.AsNoTracking();
            return query;
        }

        public async Task<T> GetAsync(Expression<Func<T, bool>> method, bool tracking = true)
        {
            var query = Table.AsQueryable();

            try
            {
                var result = await query.FirstOrDefaultAsync(method);
            }
            catch (Exception ex)
            {
                var a = 2;
                throw;
            }

            if (!tracking)
                query = query.AsNoTracking();
            return await query.FirstOrDefaultAsync(method);
        }

        #endregion

        #region WriteRepository
        public async Task<DataResponse<T>> CreateAsync(T model)
        {
            EntityEntry<T> entityEntry = await Table.AddAsync(model);

            await _context.SaveChangesAsync();

            return new DataResponse<T>()
            {
                Success = entityEntry.State == EntityState.Added ? true : false,
                Message = entityEntry.State == EntityState.Added ? "Ok" : "Hata Alındı",
                Data = model
            };
        }

        public DataResponse<T> Update(T model)
        {
            var result = new DataResponse<T>()
            {
                Success = true,
                Message = "Ok",
                Data = model
            };

            try
            {
                Table.Update(model);
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
                result.Data = model;
            }

            _context.SaveChanges();

            return result;
        }

        public DataResponse<T> HardDelete(T model)
        {
            var result = new DataResponse<T>()
            {
                Success = true,
                Message = "Ok",
                Data = model
            };

            result.Message = "hard delete";

            EntityEntry<T> entityEntry = Table.Remove(model);

            if (entityEntry.State != EntityState.Deleted)
            {
                result.Success = false;
                result.Data = model;
            }

            _context.SaveChanges();

            return result;
        }

        #endregion
    }
}
