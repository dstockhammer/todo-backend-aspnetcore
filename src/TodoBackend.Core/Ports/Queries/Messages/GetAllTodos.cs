using System.Collections.Generic;
using Darker;
using TodoBackend.Core.Domain;

namespace TodoBackend.Core.Ports.Queries.Messages
{
    public sealed class GetAllTodos : IQueryRequest<GetAllTodos.Response>
    {
        public sealed class Response : IQueryResponse
        {
            public IEnumerable<Todo> Todos { get; }

            public Response(IEnumerable<Todo> todos)
            {
                Todos = todos;
            }
        }
    }
}