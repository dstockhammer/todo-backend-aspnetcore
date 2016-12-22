using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using paramore.brighter.commandprocessor;
using paramore.brighter.commandprocessor.logging.Attributes;
using TodoBackend.Core.Domain;
using TodoBackend.Core.Ports.Commands.Messages;

namespace TodoBackend.Core.Ports.Commands.Handlers
{
    public sealed class DeleteAllTodosHandler : RequestHandlerAsync<DeleteAllTodos>
    {
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public DeleteAllTodosHandler(IUnitOfWorkManager unitOfWorkManager)
        {
            _unitOfWorkManager = unitOfWorkManager;
        }

        [RequestLoggingAsync(1, HandlerTiming.Before)]
        public override async Task<DeleteAllTodos> HandleAsync(DeleteAllTodos command, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var uow = _unitOfWorkManager.Start())
            using (var tx = await uow.BeginTransactionAsync(cancellationToken: cancellationToken).ConfigureAwait(ContinueOnCapturedContext))
            {
                var todos = await uow.AsQueryable<Todo>().ToListAsync(cancellationToken).ConfigureAwait(ContinueOnCapturedContext);

                foreach (var todo in todos)
                {
                    uow.Delete(todo);
                }

                await tx.CommitAsync(cancellationToken).ConfigureAwait(ContinueOnCapturedContext);
            }

            return await base.HandleAsync(command, cancellationToken).ConfigureAwait(ContinueOnCapturedContext);
        }
    }
}