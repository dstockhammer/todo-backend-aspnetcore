using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Darker;
using Darker.RequestLogging;
using TodoBackend.Core.Domain;
using TodoBackend.Core.Ports.Queries.Messages;

namespace TodoBackend.Core.Ports.Queries.Handlers
{
    public sealed class GetAllTodosHandler : AsyncQueryHandler<GetAllTodos, IEnumerable<Todo>>
    {
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public GetAllTodosHandler(IUnitOfWorkManager unitOfWorkManager)
        {
            _unitOfWorkManager = unitOfWorkManager;
        }

        [RequestLogging(1)]
        public override async Task<IEnumerable<Todo>> ExecuteAsync(GetAllTodos request, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var uow = _unitOfWorkManager.Start())
            {
                return await uow.GetAllAsync<Todo>(cancellationToken).ConfigureAwait(false);
            }
        }
    }
}