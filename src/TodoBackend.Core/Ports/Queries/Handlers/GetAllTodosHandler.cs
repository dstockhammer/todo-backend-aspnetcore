using System.Threading;
using System.Threading.Tasks;
using Darker;
using Darker.Attributes;
using Microsoft.EntityFrameworkCore;
using TodoBackend.Core.Domain;
using TodoBackend.Core.Ports.Queries.Messages;

namespace TodoBackend.Core.Ports.Queries.Handlers
{
    public sealed class GetAllTodosHandler : AsyncQueryHandler<GetAllTodos, GetAllTodos.Result>
    {
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public GetAllTodosHandler(IUnitOfWorkManager unitOfWorkManager)
        {
            _unitOfWorkManager = unitOfWorkManager;
        }

        [RequestLogging(1)]
        public override async Task<GetAllTodos.Result> ExecuteAsync(GetAllTodos request, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var uow = _unitOfWorkManager.Start())
            {
                //await Task.Delay(5000, cancellationToken);

                var todos = await uow.AsQueryable<Todo>().ToListAsync(cancellationToken);
                return new GetAllTodos.Result(todos);
            }
        }
    }
}