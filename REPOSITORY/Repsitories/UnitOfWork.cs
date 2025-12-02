using CORE.Interfaces;
using Microsoft.EntityFrameworkCore;
using REPOSITORY.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REPOSITORY.Repsitories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IdentityDbContext _dbContext;
        private Hashtable _repo = new Hashtable();

        public IUserRepository IUserRepository { get; }

        public UnitOfWork(IdentityDbContext dbContext, IUserRepository userRepository)
        {
            _dbContext = dbContext;
            IUserRepository = userRepository;
        }
        public async ValueTask DisposeAsync()
        => await _dbContext.DisposeAsync();
        
        public async Task<int> SaveChangesAsync()
        => await _dbContext.SaveChangesAsync();

        public IGenericRepository<T> GenericRepo<T>() where T : class
        {
            var type = typeof(T).Name;
            if (!_repo.ContainsKey(type))
            {
                var repo = new GenericRepository<T>(_dbContext);
                _repo.Add(type, repo);
            }
            return _repo[type] as IGenericRepository<T>;
        }

    }
}
