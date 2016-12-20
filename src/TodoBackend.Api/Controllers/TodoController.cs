using System;
using System.Linq;
using System.Threading.Tasks;
using Darker;
using Microsoft.AspNetCore.Mvc;
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

        public TodoController(IAmACommandProcessor commandProcessor, IQueryProcessor queryProcessor)
        {
            _commandProcessor = commandProcessor;
            _queryProcessor = queryProcessor;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _queryProcessor.ExecuteAsync(new GetAllTodos());

            var views = result.Todos.Select(t => new TodoView
            {
                Id = t.Id,
                Title = t.Title,
                Completed = t.Completed,
                Order = t.Order,
                Url = GetUri(t.Id)
            });

            return Ok(views);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _queryProcessor.ExecuteAsync(new GetTodo(id));
            if (result.Todo == null)
                return NotFound();

            var view = new TodoView
            {
                Id = result.Todo.Id,
                Title = result.Todo.Title,
                Completed = result.Todo.Completed,
                Order = result.Todo.Order,
                Url = GetUri(result.Todo.Id)
            };

            return Ok(view);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]TodoView view)
        {
            var id = Math.Abs(Guid.NewGuid().GetHashCode());
            await _commandProcessor.SendAsync(new CreateTodo(id, view.Title, view.Completed, view.Order));

            // todo: yeah, this is a hack
            view.Id = id;
            view.Url = GetUri(id);

            HttpContext.Response.Headers.Add("Location", view.Url);

            return Created(view.Url, view);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Patch(int id, [FromBody]TodoView view)
        {
            await _commandProcessor.SendAsync(new UpdateTodo(id, view.Title, view.Completed, view.Order));

            // todo: yeah, this is a hack
            view.Id = id;
            view.Url = GetUri(id);

            return Ok(view);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete()
        {
            await _commandProcessor.SendAsync(new DeleteAllTodos());

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _commandProcessor.SendAsync(new DeleteTodo(id));

            return Ok();
        }

        private string GetUri(int id)
        {
            var hostAndPort = HttpContext.Request.Host.Value.Split(':');

            var builder = new UriBuilder
            {
                Scheme = HttpContext.Request.Scheme,
                Host = hostAndPort.First(),
                Port = hostAndPort.Length == 1 ? 80 : int.Parse(hostAndPort.ElementAt(1)),
                Path = id.ToString()
            };

            return builder.Uri.ToString();
        }
    }
}