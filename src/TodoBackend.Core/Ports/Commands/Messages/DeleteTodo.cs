using System;
using paramore.brighter.commandprocessor;

namespace TodoBackend.Core.Ports.Commands.Messages
{
    public sealed class DeleteTodo : Command
    {
        public int TodoId { get; }

        public DeleteTodo(int todoId)
            : base(Guid.NewGuid())
        {
            TodoId = todoId;
        }
    }
}