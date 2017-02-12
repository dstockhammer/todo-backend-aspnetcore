using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TodoBackend.Core.Domain;

namespace TodoBackend.Api.Data
{
    internal sealed class EfUnitOfWork : IUnitOfWork
    {
        private readonly TodoContext _context;

        public EfUnitOfWork(DbContextOptions<TodoContext> options)
        {
            _context = new TodoContext(options);
        }

        public async Task<T> GetAsync<T>(int id, CancellationToken cancellationToken = default(CancellationToken))
            where T : class, IEntity
        {
            return await _context.Set<T>().SingleOrDefaultAsync(e => e.Id == id, cancellationToken).ConfigureAwait(false);
        }

        public async Task<IEnumerable<T>> GetAllAsync<T>(CancellationToken cancellationToken = new CancellationToken()) where T : class, IEntity
        {
            return await _context.Set<T>().ToListAsync(cancellationToken).ConfigureAwait(false);
        }

        public void Add<T>(T entity) where T : class, IEntity
        {
            _context.Set<T>().Add(entity);
        }

        public void Delete<T>(T entity) where T : class, IEntity
        {
            _context.Set<T>().Remove(entity);
        }

        public IQueryable<T> AsQueryable<T>() where T : class, IEntity
        {
            return _context.Set<T>();
        }

        public async Task<ITransaction> BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, CancellationToken cancellationToken = default(CancellationToken))
        {
            var tx = await _context.Database.BeginTransactionAsync(isolationLevel, cancellationToken).ConfigureAwait(false);
            return new EfTransaction(_context, tx);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
