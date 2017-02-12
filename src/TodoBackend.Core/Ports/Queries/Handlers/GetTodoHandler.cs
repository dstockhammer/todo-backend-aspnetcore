using System.Threading;
using System.Threading.Tasks;
using Darker;
using Darker.RequestLogging;
using TodoBackend.Core.Domain;
using TodoBackend.Core.Ports.Queries.Messages;

namespace TodoBackend.Core.Ports.Queries.Handlers
{
    public sealed class GetTodoHandler : AsyncQueryHandler<GetTodo, Todo>
    {
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public GetTodoHandler(IUnitOfWorkManager unitOfWorkManager)
        {
            _unitOfWorkManager = unitOfWorkManager;
        }

        [RequestLogging(1)]
        public override async Task<Todo> ExecuteAsync(GetTodo request, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var uow = _unitOfWorkManager.Start())
            {
                return await uow.GetAsync<Todo>(request.TodoId, cancellationToken).ConfigureAwait(false);
            }
        }
    }
}