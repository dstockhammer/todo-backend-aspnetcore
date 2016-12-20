using System.Threading;
using System.Threading.Tasks;
using paramore.brighter.commandprocessor;
using TodoBackend.Core.BrighterFix;
using TodoBackend.Core.Domain;
using TodoBackend.Core.Ports.Commands.Messages;

namespace TodoBackend.Core.Ports.Commands.Handlers
{
    public sealed class DeleteTodoHandler : RequestHandlerAsync<DeleteTodo>
    {
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public DeleteTodoHandler(IUnitOfWorkManager unitOfWorkManager)
        {
            _unitOfWorkManager = unitOfWorkManager;
        }

        [RequestLoggingAsync2(1, HandlerTiming.Before)]
        public override async Task<DeleteTodo> HandleAsync(DeleteTodo command, CancellationToken? ct = null)
        {
            using (var uow = _unitOfWorkManager.Start())
            using (var tx = await uow.BeginTransactionAsync(cancellationToken: ct ?? default(CancellationToken)))
            {
                var todo = await uow.GetAsync<Todo>(command.TodoId, ct ?? default(CancellationToken));
                uow.Delete(todo);

                await tx.CommitAsync(ct ?? default(CancellationToken));
            }

            return await base.HandleAsync(command, ct);
        }
    }
}