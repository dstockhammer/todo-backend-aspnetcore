using System.Data;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TodoBackend.Core.Domain
{
    public interface IUnitOfWork : IDisposable
    {
        Task<T> GetAsync<T>(int id, CancellationToken cancellationToken = default(CancellationToken)) where T : class, IEntity;
        Task<IEnumerable<T>> GetAllAsync<T>(CancellationToken cancellationToken = default(CancellationToken)) where T : class, IEntity;
        void Add<T>(T entity) where T : class, IEntity;
        void Delete<T>(T entity) where T : class, IEntity;

        Task<ITransaction> BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, CancellationToken cancellationToken = default(CancellationToken));
    }
}