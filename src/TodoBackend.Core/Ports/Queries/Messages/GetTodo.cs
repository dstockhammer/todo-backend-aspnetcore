using Darker;
using TodoBackend.Core.Domain;

namespace TodoBackend.Core.Ports.Queries.Messages
{
    public sealed class GetTodo : IQuery<Todo>
    {
        public int TodoId { get; }

        public GetTodo(int todoId)
        {
            TodoId = todoId;
        }
    }
}