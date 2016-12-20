using System;
using paramore.brighter.commandprocessor;

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