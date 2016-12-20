using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using TodoBackend.Core.Domain;

namespace TodoBackend.Core
{
    public sealed class DummyRepository
    {
        private static readonly ConcurrentDictionary<int, Todo> _db = new ConcurrentDictionary<int, Todo>();

        public Task AddAsync(Todo todo)
        {
            _db.TryAdd(todo.Id, todo);

            return Task.FromResult(0);
        }

        public Task<Todo> GetAsync(int id)
        {
            if (!_db.ContainsKey(id))
                return Task.FromResult<Todo>(null);

            return Task.FromResult(_db[id]);
        }

        public Task<IEnumerable<Todo>> GetAllAsync()
        {
            return Task.FromResult<IEnumerable<Todo>>(_db.Values);
        }

        public Task RemoveAsync(int id)
        {
            if (!_db.ContainsKey(id))
                return Task.FromResult(0);

            Todo x;
            _db.TryRemove(id, out x);

            return Task.FromResult(0);
        }

        public Task ClearAsync()
        {
            _db.Clear();

            return Task.FromResult(0);
        }
    }
}