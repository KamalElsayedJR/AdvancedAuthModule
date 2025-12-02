using CORE.Interfaces;
using Microsoft.EntityFrameworkCore;
using REPOSITORY.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace REPOSITORY.Repsitories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly IdentityDbContext _dbContext;

        public GenericRepository(IdentityDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task AddAsync(T item)
        => await _dbContext.Set<T>().AddAsync(item);
        public void Delete(T item)
        => _dbContext.Remove(item);

        public void DeleteRange(IEnumerable<T> entities)
        => _dbContext.Set<T>().RemoveRange(entities);

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> expression)
        => await _dbContext.Set<T>().Where(expression).ToListAsync();

        public async Task<T?> GetByIdAsync(string id)
        => await _dbContext.FindAsync<T>(id);
        public void Update(T item)
        => _dbContext.Set<T>().Update(item);
    }
}
