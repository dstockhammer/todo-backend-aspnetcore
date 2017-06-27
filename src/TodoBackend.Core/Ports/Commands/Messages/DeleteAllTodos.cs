using System;
using Paramore.Brighter;

namespace TodoBackend.Core.Ports.Commands.Messages
{
    public sealed class DeleteAllTodos : Command
    {
        public DeleteAllTodos()
            : base(Guid.NewGuid())
        {
        }
    }
}