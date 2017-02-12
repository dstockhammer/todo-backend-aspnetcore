using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Darker;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using paramore.brighter.commandprocessor;
using TodoBackend.Api.Views;
using TodoBackend.Core.Ports.Commands.Messages;
using TodoBackend.Core.Ports.Queries.Messages;

namespace TodoBackend.Api.Controllers
{
    [Route("")]
    public class TodoController : ControllerBase
    {
        private readonly IAmACommandProcessor _commandProcessor;
        private readonly IQueryProcessor _queryProcessor;
        private readonly IConfiguration _configuration;

        public TodoController(IAmACommandProcessor commandProcessor, IQueryProcessor queryProcessor, IConfiguration configuration)
        {
            _commandProcessor = commandProcessor;
            _queryProcessor = queryProcessor;
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> Get(CancellationToken cancellationToken = default(CancellationToken))
        {
            var todos = await _queryProcessor.ExecuteAsync(new GetAllTodos(), cancellationToken).ConfigureAwait(false);

            var views = todos.Select(t => new TodoView
            {
                Id = t.Id,
                Title = t.Title,
                Completed = t.Completed,
                Order = t.Order,
                Url = GetTodoUri(t.Id)
            });

            return Ok(views);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id, CancellationToken cancellationToken = default(CancellationToken))
        {
            var todo = await _queryProcessor.ExecuteAsync(new GetTodo(id), cancellationToken).ConfigureAwait(false);
            if (todo == null)
                return NotFound();

            var view = new TodoView
            {
                Id = todo.Id,
                Title = todo.Title,
                Completed = todo.Completed,
                Order = todo.Order,
                Url = GetTodoUri(todo.Id)
            };

            return Ok(view);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]TodoView view, CancellationToken cancellationToken = default(CancellationToken))
        {
            var id = Math.Abs(Guid.NewGuid().GetHashCode());
            await _commandProcessor.SendAsync(new CreateTodo(id, view.Title, view.Completed, view.Order), false, cancellationToken).ConfigureAwait(false);

            // todo: yeah, this is a hack
            view.Id = id;
            view.Url = GetTodoUri(id);

            HttpContext.Response.Headers.Add("Location", view.Url);

            return Created(view.Url, view);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Patch(int id, [FromBody]TodoView view, CancellationToken cancellationToken = default(CancellationToken))
        {
            await _commandProcessor.SendAsync(new UpdateTodo(id, view.Title, view.Completed, view.Order), false, cancellationToken).ConfigureAwait(false);

            // todo: yeah, this is a hack
            view.Id = id;
            view.Url = GetTodoUri(id);

            return Ok(view);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(CancellationToken cancellationToken = default(CancellationToken))
        {
            await _commandProcessor.SendAsync(new DeleteAllTodos(), false, cancellationToken).ConfigureAwait(false);

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default(CancellationToken))
        {
            await _commandProcessor.SendAsync(new DeleteTodo(id), false, cancellationToken).ConfigureAwait(false);

            return Ok();
        }

        private string GetTodoUri(int id) => $"{_configuration["Uris:Api"]}/{id}";
    }
}