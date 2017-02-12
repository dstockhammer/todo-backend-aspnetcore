using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using TodoBackend.Core.Domain;

namespace TodoBackend.Api.Data
{
    internal sealed class EfTransaction : ITransaction
    {
        private readonly TodoContext _context;
        private readonly IDbContextTransaction _transaction;

        public EfTransaction(TodoContext context, IDbContextTransaction transaction)
        {
            _context = context;
            _transaction = transaction;
        }

        public async Task CommitAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            await _context.SaveChangesAsync(cancellationToken);

            _transaction.Commit();
        }

        public void Rollback()
        {
            _transaction.Rollback();
        }

        public void Dispose()
        {
            _transaction.Dispose();
        }
    }
}