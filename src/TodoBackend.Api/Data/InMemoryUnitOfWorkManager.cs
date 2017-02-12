using TodoBackend.Core.Domain;

namespace TodoBackend.Api.Data
{
    internal sealed class InMemoryUnitOfWorkManager : IUnitOfWorkManager
    {
        public IUnitOfWork Start()
        {
            return new InMemoryUnitOfWork();
        }
    }
}