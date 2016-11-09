using System;
using System.Collections.Concurrent;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using TodoBackend.Api.Views;

namespace TodoBackend.Api.Controllers
{
    [Route("")]
    public class TodoController : ControllerBase
    {
        private static readonly ConcurrentDictionary<int, TodoView> _todos = new ConcurrentDictionary<int, TodoView>();

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_todos.Values);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            if (!_todos.ContainsKey(id))
                return NotFound();

            return Ok(_todos[id]);
        }

        [HttpPost]
        public IActionResult Post([FromBody]TodoView view)
        {
            view.Id = Math.Abs(Guid.NewGuid().GetHashCode());
            view.Url = GetUri(view.Id);

            _todos.TryAdd(view.Id, view);

            HttpContext.Response.Headers.Add("Location", view.Url);

            return Created(view.Url, view);
        }

        [HttpPatch("{id}")]
        public IActionResult Patch(int id, [FromBody]TodoView view)
        {
            if (!_todos.ContainsKey(id))
                return NotFound();

            _todos[id].Title = view.Title;
            _todos[id].Completed = view.Completed;
            _todos[id].Order = view.Order;

            return Ok(_todos[id]);
        }

        [HttpDelete]
        public IActionResult Delete()
        {
            _todos.Clear();

            return Ok();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (!_todos.ContainsKey(id))
                return NotFound();

            TodoView todo;
            _todos.TryRemove(id, out todo);

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