using System;
using System.Threading;
using System.Threading.Tasks;

namespace TodoBackend.Core.Domain
{
    public interface ITransaction : IDisposable
    {
        Task CommitAsync(CancellationToken cancellationToken = default(CancellationToken));
        void Rollback();
    }
}