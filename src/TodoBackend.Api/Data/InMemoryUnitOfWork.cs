using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TodoBackend.Core.Domain;

namespace TodoBackend.Api.Data
{
    internal sealed class InMemoryUnitOfWork : IUnitOfWork
    {
        private static readonly ConcurrentDictionary<int, Todo> _todos = new ConcurrentDictionary<int, Todo>();

        public Task<T> GetAsync<T>(int id, CancellationToken cancellationToken = default(CancellationToken))
            where T : class, IEntity
        {
            if (typeof(T) != typeof(Todo))
                throw new NotSupportedException();

            if (_todos.TryGetValue(id, out Todo todo))
            {
                return Task.FromResult((T)Convert.ChangeType(todo, typeof(T)));
            }

            return Task.FromResult(default(T));
        }

        public Task<IEnumerable<T>> GetAllAsync<T>(CancellationToken cancellationToken = new CancellationToken()) where T : class, IEntity
        {
            if (typeof(T) != typeof(Todo))
                throw new NotSupportedException();

            return Task.FromResult(_todos.Values.Cast<T>());
        }

        public void Add<T>(T entity) where T : class, IEntity
        {
            var todo = entity as Todo;
            if (todo == null)
                throw new NotSupportedException();

            _todos.AddOrUpdate(entity.Id, todo, (id, existing) => todo);
        }

        public void Delete<T>(T entity) where T : class, IEntity
        {
            if (typeof(T) != typeof(Todo))
                throw new NotSupportedException();

            _todos.TryRemove(entity.Id, out Todo existing);

            // just ignore errors
        }

        public Task<ITransaction> BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult<ITransaction>(new NoOpTransaction());
        }

        public void Dispose()
        {
            // nothing to do
        }
    }
}