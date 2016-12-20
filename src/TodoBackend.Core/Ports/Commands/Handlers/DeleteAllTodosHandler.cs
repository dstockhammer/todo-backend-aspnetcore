using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using paramore.brighter.commandprocessor;
using TodoBackend.Core.BrighterFix;
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

        [RequestLoggingAsync2(1, HandlerTiming.Before)]
        public override async Task<DeleteAllTodos> HandleAsync(DeleteAllTodos command, CancellationToken? ct = null)
        {
            using (var uow = _unitOfWorkManager.Start())
            using (var tx = await uow.BeginTransactionAsync(ct ?? default(CancellationToken)))
            {
                foreach (var todo in await uow.AsQueryable<Todo>().ToListAsync(ct ?? default(CancellationToken)))
                {
                    uow.Delete(todo);
                }

                await tx.CommitAsync(ct ?? default(CancellationToken));
            }

            return await base.HandleAsync(command, ct);
        }
    }
}