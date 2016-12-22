using System.Threading;
using System.Threading.Tasks;
using Darker;
using Darker.Attributes;
using Microsoft.EntityFrameworkCore;
using TodoBackend.Core.Domain;
using TodoBackend.Core.Ports.Queries.Messages;

namespace TodoBackend.Core.Ports.Queries.Handlers
{
    public sealed class GetAllTodosHandler : AsyncQueryHandler<GetAllTodos, GetAllTodos.Response>
    {
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public GetAllTodosHandler(IUnitOfWorkManager unitOfWorkManager)
        {
            _unitOfWorkManager = unitOfWorkManager;
        }

        [RequestLogging(1)]
        public override async Task<GetAllTodos.Response> ExecuteAsync(GetAllTodos request, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var uow = _unitOfWorkManager.Start())
            {
                var todos = await uow.AsQueryable<Todo>().ToListAsync(cancellationToken).ConfigureAwait(false);
                return new GetAllTodos.Response(todos);
            }
        }
    }
}