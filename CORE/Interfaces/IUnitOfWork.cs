using CORE.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CORE.Interfaces
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        Task<int> SaveChangesAsync();
        IGenericRepository<T> GenericRepo<T>() where T : class;
        public IUserRepository IUserRepository { get; }
    }
}
