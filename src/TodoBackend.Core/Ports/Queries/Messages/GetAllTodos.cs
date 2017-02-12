using System.Collections.Generic;
using Darker;
using TodoBackend.Core.Domain;

namespace TodoBackend.Core.Ports.Queries.Messages
{
    public sealed class GetAllTodos : IQuery<IEnumerable<Todo>>
    {
    }
}