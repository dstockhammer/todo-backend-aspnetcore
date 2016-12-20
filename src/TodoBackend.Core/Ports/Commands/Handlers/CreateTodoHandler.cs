using System.Threading;
using System.Threading.Tasks;
using paramore.brighter.commandprocessor;
using paramore.brighter.commandprocessor.logging.Attributes;
using TodoBackend.Core.Domain;
using TodoBackend.Core.Ports.Commands.Messages;

namespace TodoBackend.Core.Ports.Commands.Handlers
{
    public sealed class CreateTodoHandler : RequestHandlerAsync<CreateTodo>
    {
        private readonly DummyRepository _repository;

        public CreateTodoHandler(DummyRepository repository)
        {
            _repository = repository;
        }

        [RequestLoggingAsync(1, HandlerTiming.Before)]
        public override async Task<CreateTodo> HandleAsync(CreateTodo command, CancellationToken? ct = null)
        {
            var todo = new Todo(command.TodoId, command.Title, command.Completed, command.Order);
            await _repository.AddAsync(todo);

            return await base.HandleAsync(command, ct);
        }
    }
}