using System;
using paramore.brighter.commandprocessor;

namespace TodoBackend.Core.Ports.Commands.Messages
{
    public sealed class UpdateTodo : Command
    {
        public int TodoId { get; }
        public string Title { get; }
        public bool Completed { get; }
        public int? Order { get; }

        public UpdateTodo(int todoId, string title, bool completed, int? order)
            : base(Guid.NewGuid())
        {
            TodoId = todoId;
            Title = title;
            Completed = completed;
            Order = order;
        }
    }
}
