using System.Threading;
using System.Threading.Tasks;
using paramore.brighter.commandprocessor;
using TodoBackend.Core.BrighterFix;
using TodoBackend.Core.Ports.Commands.Messages;

namespace TodoBackend.Core.Ports.Commands.Handlers
{
    public sealed class DeleteTodoHandler : RequestHandlerAsync<DeleteTodo>
    {
        private readonly DummyRepository _repository;

        public DeleteTodoHandler(DummyRepository repository)
        {
            _repository = repository;
        }

        [RequestLoggingAsync2(1, HandlerTiming.Before)]
        public override async Task<DeleteTodo> HandleAsync(DeleteTodo command, CancellationToken? ct = null)
        {
            await _repository.RemoveAsync(command.TodoId);

            return await base.HandleAsync(command, ct);
        }
    }
}