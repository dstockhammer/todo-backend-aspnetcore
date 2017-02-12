using System.Threading;
using System.Threading.Tasks;
using TodoBackend.Core.Domain;

namespace TodoBackend.Api.Data
{
    internal sealed class NoOpTransaction : ITransaction
    {
        public Task CommitAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            // nothing to do
            return Task.FromResult(0);
        }

        public void Rollback()
        {
            // nothing to do
        }
        public void Dispose()
        {
            // nothing to do
        }
    }
}