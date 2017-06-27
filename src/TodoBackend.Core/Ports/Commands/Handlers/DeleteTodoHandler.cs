using System.Threading;
using System.Threading.Tasks;
using Paramore.Brighter;
using Paramore.Brighter.Logging.Attributes;
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

        [RequestLoggingAsync(1, HandlerTiming.Before)]
        public override async Task<DeleteTodo> HandleAsync(DeleteTodo command, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var uow = _unitOfWorkManager.Start())
            using (var tx = await uow.BeginTransactionAsync(cancellationToken: cancellationToken).ConfigureAwait(ContinueOnCapturedContext))
            {
                var todo = await uow.GetAsync<Todo>(command.TodoId, cancellationToken).ConfigureAwait(ContinueOnCapturedContext);
                uow.Delete(todo);

                await tx.CommitAsync(cancellationToken).ConfigureAwait(ContinueOnCapturedContext);
            }

            return await base.HandleAsync(command, cancellationToken).ConfigureAwait(ContinueOnCapturedContext);
        }
    }
}