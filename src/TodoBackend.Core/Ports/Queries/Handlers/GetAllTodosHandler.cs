using System.Threading.Tasks;
using Darker;
using Darker.Attributes;
using TodoBackend.Core.Ports.Queries.Messages;

namespace TodoBackend.Core.Ports.Queries.Handlers
{
    public sealed class GetAllTodosHandler : AsyncQueryHandler<GetAllTodos, GetAllTodos.Result>
    {
        private readonly DummyRepository _repository;

        public GetAllTodosHandler(DummyRepository repository)
        {
            _repository = repository;
        }

        [RequestLogging(1)]
        public override async Task<GetAllTodos.Result> ExecuteAsync(GetAllTodos request)
        {
            var todos = await _repository.GetAllAsync();
            return new GetAllTodos.Result(todos);
        }
    }
}