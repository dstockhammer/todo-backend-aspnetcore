using System.Threading;
using System.Threading.Tasks;
using paramore.brighter.commandprocessor;
using paramore.brighter.commandprocessor.logging.Attributes;
using TodoBackend.Core.Ports.Commands.Messages;

namespace TodoBackend.Core.Ports.Commands.Handlers
{
    public sealed class UpdateTodoHandler : RequestHandlerAsync<UpdateTodo>
    {
        private readonly DummyRepository _repository;

        public UpdateTodoHandler(DummyRepository repository)
        {
            _repository = repository;
        }

        [RequestLoggingAsync(1, HandlerTiming.Before)]
        public override async Task<UpdateTodo> HandleAsync(UpdateTodo command, CancellationToken? ct = null)
        {
            var todo = await _repository.GetAsync(command.TodoId);
            todo.Update(command.Title, command.Completed, command.Order);

            return await base.HandleAsync(command, ct);
        }
    }
}