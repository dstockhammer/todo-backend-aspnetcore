using Darker;
using TodoBackend.Core.Domain;

namespace TodoBackend.Core.Ports.Queries.Messages
{
    public sealed class GetTodo : IQueryRequest<GetTodo.Result>
    {
        public int TodoId { get; }

        public GetTodo(int todoId)
        {
            TodoId = todoId;
        }

        public sealed class Result : IQueryResponse
        {
            public Todo Todo { get; }

            public Result(Todo todo)
            {
                Todo = todo;
            }
        }
    }
}