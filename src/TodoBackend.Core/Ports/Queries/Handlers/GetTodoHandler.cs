using System.Threading;
using System.Threading.Tasks;
using Darker;
using Darker.RequestLogging;
using TodoBackend.Core.Domain;
using TodoBackend.Core.Ports.Queries.Messages;

namespace TodoBackend.Core.Ports.Queries.Handlers
{
    public sealed class GetTodoHandler : AsyncQueryHandler<GetTodo, GetTodo.Response>
    {
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public GetTodoHandler(IUnitOfWorkManager unitOfWorkManager)
        {
            _unitOfWorkManager = unitOfWorkManager;
        }

        [RequestLogging(1)]
        public override async Task<GetTodo.Response> ExecuteAsync(GetTodo request, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var uow = _unitOfWorkManager.Start())
            {
                var todo = await uow.GetAsync<Todo>(request.TodoId, cancellationToken).ConfigureAwait(false);
                return new GetTodo.Response(todo);
            }
        }
    }
}