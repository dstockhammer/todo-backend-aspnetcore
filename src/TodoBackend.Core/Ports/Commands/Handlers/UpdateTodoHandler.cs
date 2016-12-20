using System.Threading;
using System.Threading.Tasks;
using paramore.brighter.commandprocessor;
using TodoBackend.Core.BrighterFix;
using TodoBackend.Core.Domain;
using TodoBackend.Core.Ports.Commands.Messages;

namespace TodoBackend.Core.Ports.Commands.Handlers
{
    public sealed class UpdateTodoHandler : RequestHandlerAsync<UpdateTodo>
    {
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public UpdateTodoHandler(IUnitOfWorkManager unitOfWorkManager)
        {
            _unitOfWorkManager = unitOfWorkManager;
        }

        [RequestLoggingAsync2(1, HandlerTiming.Before)]
        public override async Task<UpdateTodo> HandleAsync(UpdateTodo command, CancellationToken? ct = null)
        {
            using (var uow = _unitOfWorkManager.Start())
            using (var tx = await uow.BeginTransactionAsync(ct ?? default(CancellationToken)))
            {
                var todo = await uow.GetAsync<Todo>(command.TodoId, ct ?? default(CancellationToken));
                todo.Update(command.Title, command.Completed, command.Order);

                await tx.CommitAsync(ct ?? default(CancellationToken));
            }

            return await base.HandleAsync(command, ct);
        }
    }
}