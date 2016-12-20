using System.Threading.Tasks;
using Darker;
using TodoBackend.Core.Ports.Queries.Messages;

namespace TodoBackend.Core.Ports.Queries.Handlers
{
    public sealed class GetTodoHandler : AsyncQueryHandler<GetTodo, GetTodo.Result>
    {
        private readonly DummyRepository _repository;

        public GetTodoHandler(DummyRepository repository)
        {
            _repository = repository;
        }

        //[RequestLogging(1)]
        public override async Task<GetTodo.Result> ExecuteAsync(GetTodo request)
        {
            var todo = await _repository.GetAsync(request.TodoId);
            return new GetTodo.Result(todo);
        }
    }
}