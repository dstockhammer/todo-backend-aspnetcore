using Darker;
using TodoBackend.Core.Domain;

namespace TodoBackend.Core.Ports.Queries.Messages
{
    public sealed class GetTodo : IQueryRequest<GetTodo.Response>
    {
        public int TodoId { get; }

        public GetTodo(int todoId)
        {
            TodoId = todoId;
        }

        public sealed class Response : IQueryResponse
        {
            public Todo Todo { get; }

            public Response(Todo todo)
            {
                Todo = todo;
            }
        }
    }
}