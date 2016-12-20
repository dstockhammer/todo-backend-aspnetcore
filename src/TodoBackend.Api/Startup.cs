using System.Reflection;
using Darker;
using Darker.Builder;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ViewComponents;
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
using SimpleInjector;
using SimpleInjector.Integration.AspNetCore;
using SimpleInjector.Integration.AspNetCore.Mvc;

namespace TodoBackend.Api
{
    public class Startup
    {
        private readonly Container _container;
        private readonly IConfigurationRoot _configuration;

        public Startup(IHostingEnvironment env)
        {
            _container = new Container();
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

            services.AddSingleton<IControllerActivator>(new SimpleInjectorControllerActivator(_container));
            services.AddSingleton<IViewComponentActivator>(new SimpleInjectorViewComponentActivator(_container));

            services.AddCors();
            services.AddMvcCore()
                .AddJsonFormatters(opt =>
                {
                    opt.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    opt.Converters.Add(new StringEnumConverter());
                    opt.Formatting = Formatting.Indented;
                    opt.NullValueHandling = NullValueHandling.Ignore;
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddSerilog(Log.Logger);

            InitializeContainer(app);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(opts => opts.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            app.UseMvc();
        }

        private void InitializeContainer(IApplicationBuilder app)
        {
            app.UseSimpleInjectorAspNetRequestScoping(_container);

            _container.Options.DefaultScopedLifestyle = new AspNetRequestLifestyle();

            _container.RegisterMvcControllers(app);

            _container.RegisterSingleton(Log.Logger);
            _container.RegisterSingleton<DummyRepository>();

            ConfigureBrighter();
            ConfigureDarker();

            _container.Verify();
        }

        private void ConfigureBrighter()
        {
            var config = new SimpleInjectorHandlerConfig(_container);
            config.RegisterSubscribersFromAssembly(typeof(CreateTodoHandler).GetTypeInfo().Assembly);
            config.RegisterDefaultHandlers();

            var commandProcessor = CommandProcessorBuilder.With()
                .Handlers(config.HandlerConfiguration)
                .DefaultPolicy()
                .NoTaskQueues()
                .RequestContextFactory(new paramore.brighter.commandprocessor.InMemoryRequestContextFactory())
                .Build();

            _container.RegisterSingleton<IAmACommandProcessor>(commandProcessor);
        }

        private void ConfigureDarker()
        {
            var handlerConfiguration = new SimpleInjectorHandlerConfigurationBuilder(_container)
                .WithQueriesAndHandlersFromAssembly(typeof(GetTodoHandler).GetTypeInfo().Assembly)
                .Build();

            var queryProcessor = QueryProcessorBuilder.With()
                .Handlers(handlerConfiguration)
                .DefaultPolicies()
                .InMemoryRequestContextFactory()
                .Build();

            _container.RegisterSingleton<IQueryProcessor>(queryProcessor);
        }
    }
}