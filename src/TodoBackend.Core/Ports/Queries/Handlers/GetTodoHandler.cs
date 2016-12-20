using System.Threading.Tasks;
using Darker;
using Darker.Attributes;
using TodoBackend.Core.Domain;
using TodoBackend.Core.Ports.Queries.Messages;

namespace TodoBackend.Core.Ports.Queries.Handlers
{
    public sealed class GetTodoHandler : AsyncQueryHandler<GetTodo, GetTodo.Result>
    {
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public GetTodoHandler(IUnitOfWorkManager unitOfWorkManager)
        {
            _unitOfWorkManager = unitOfWorkManager;
        }

        [RequestLogging(1)]
        public override async Task<GetTodo.Result> ExecuteAsync(GetTodo request)
        {
            using (var uow = _unitOfWorkManager.Start())
            {
                var todo = await uow.GetAsync<Todo>(request.TodoId);
                return new GetTodo.Result(todo);
            }
        }
    }
}