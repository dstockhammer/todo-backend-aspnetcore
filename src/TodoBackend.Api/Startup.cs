using System.Reflection;
using Darker;
using Darker.Builder;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using paramore.brighter.commandprocessor;
using Serilog;
using TodoBackend.Api.Infrastructure;
using TodoBackend.Core;
using TodoBackend.Core.Ports.Commands.Handlers;
using TodoBackend.Core.Ports.Queries.Handlers;

namespace TodoBackend.Api
{
    public class Startup
    {
        private readonly IConfigurationRoot _configuration;

        public Startup(IHostingEnvironment env)
        {
            _configuration = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddEnvironmentVariables()
                .Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.LiterateConsole()
                .WriteTo.Seq("http://localhost:5341")
                .Enrich.WithMachineName()
                .CreateLogger();

            services.AddSingleton(Log.Logger);

            services.AddCors();
            services.AddMvcCore()
                .AddJsonFormatters(opt =>
                {
                    opt.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    opt.Converters.Add(new StringEnumConverter());
                    opt.Formatting = Formatting.Indented;
                    opt.NullValueHandling = NullValueHandling.Ignore;
                });

            services.AddSingleton<DummyRepository>();

            ConfigureBrighter(services);
            ConfigureDarker(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddSerilog(Log.Logger);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(opts => opts.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            app.UseMvc();
        }

        private static void ConfigureBrighter(IServiceCollection services)
        {
            var config = new AspNetDependencyInjectionHandlerConfig(services);
            config.RegisterSubscribersFromAssembly(typeof(CreateTodoHandler).GetTypeInfo().Assembly);
            config.RegisterDefaultHandlers();

            var commandProcessor = CommandProcessorBuilder.With()
                .Handlers(config.HandlerConfiguration)
                .DefaultPolicy()
                .NoTaskQueues()
                .RequestContextFactory(new paramore.brighter.commandprocessor.InMemoryRequestContextFactory())
                .Build();

            services.AddSingleton<IAmACommandProcessor>(commandProcessor);
        }

        private static void ConfigureDarker(IServiceCollection services)
        {
            var handlerConfiguration = new AspNetDependencyInjectionHandlerConfigurationBuilder(services)
                .WithQueriesAndHandlersFromAssembly(typeof(GetTodoHandler).GetTypeInfo().Assembly)
                .Build();

            var queryProcessor = QueryProcessorBuilder.With()
                .Handlers(handlerConfiguration)
                .DefaultPolicies()
                .InMemoryRequestContextFactory()
                .Build();

            services.AddSingleton<IQueryProcessor>(queryProcessor);
        }
    }
}