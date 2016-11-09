using System;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace TodoBackend.Api.Controllers
{
    [Route("hello")]
    public class HelloController : ControllerBase
    {
        private readonly ILogger _logger;

        public HelloController(ILogger logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Get()
        {
            _logger.Information("Hello there, this is an example log with a random number: {number}", new Random().Next());

            var hostName = Environment.GetEnvironmentVariable("DYNO")
                ?? Environment.GetEnvironmentVariable("COMPUTERNAME")
                ?? Environment.GetEnvironmentVariable("HOSTNAME");

            return Ok($"Hello from {hostName}");
        }
    }
}