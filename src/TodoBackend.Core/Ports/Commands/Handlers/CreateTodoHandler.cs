using System.Threading;
using System.Threading.Tasks;
using paramore.brighter.commandprocessor;
using TodoBackend.Core.BrighterFix;
using TodoBackend.Core.Domain;
using TodoBackend.Core.Ports.Commands.Messages;

namespace TodoBackend.Core.Ports.Commands.Handlers
{
    public sealed class CreateTodoHandler : RequestHandlerAsync<CreateTodo>
    {
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public CreateTodoHandler(IUnitOfWorkManager unitOfWorkManager)
        {
            _unitOfWorkManager = unitOfWorkManager;
        }

        [RequestLoggingAsync2(1, HandlerTiming.Before)]
        public override async Task<CreateTodo> HandleAsync(CreateTodo command, CancellationToken? ct = null)
        {
            using (var uow = _unitOfWorkManager.Start())
            using (var tx = await uow.BeginTransactionAsync(ct ?? default(CancellationToken)))
            {
                var todo = new Todo(command.TodoId, command.Title, command.Completed, command.Order);
                uow.Add(todo);

                await tx.CommitAsync(ct ?? default(CancellationToken));
            }

            return await base.HandleAsync(command, ct);
        }
    }
}