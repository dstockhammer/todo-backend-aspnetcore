using System.Collections.Generic;
using Darker;
using TodoBackend.Core.Domain;

namespace TodoBackend.Core.Ports.Queries.Messages
{
    public sealed class GetAllTodos : IQueryRequest<GetAllTodos.Result>
    {
        public sealed class Result : IQueryResponse
        {
            public IEnumerable<Todo> Todos { get; }

            public Result(IEnumerable<Todo> todos)
            {
                Todos = todos;
            }
        }
    }
}