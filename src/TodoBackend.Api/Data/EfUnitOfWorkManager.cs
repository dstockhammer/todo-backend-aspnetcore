using Microsoft.EntityFrameworkCore;
using TodoBackend.Core.Domain;

namespace TodoBackend.Api.Data
{
    internal sealed class EfUnitOfWorkManager : IUnitOfWorkManager
    {
        private readonly DbContextOptions<TodoContext> _options;

        public EfUnitOfWorkManager(DbContextOptions<TodoContext> options)
        {
            _options = options;
        }

        public IUnitOfWork Start()
        {
            return new EfUnitOfWork(_options);
        }
    }
}