using System;
using System.Linq;
using System.Reflection;
using Darker;
using Darker.Decorators;
using Microsoft.Extensions.DependencyInjection;

namespace TodoBackend.Api.Infrastructure
{
    public sealed class AspNetDependencyInjectionHandlerConfigurationBuilder
    {
        private readonly IServiceCollection _services;
        private readonly IQueryHandlerRegistry _handlerRegistry;

        public AspNetDependencyInjectionHandlerConfigurationBuilder(IServiceCollection services)
        {
            _services = services;
            _handlerRegistry = new QueryHandlerRegistry();
        }

        public AspNetDependencyInjectionHandlerConfigurationBuilder WithQueriesAndHandlersFromAssembly(Assembly assembly)
        {
            var subscribers =
                from t in assembly.GetExportedTypes()
                let ti = t.GetTypeInfo()
                where ti.IsClass && !ti.IsAbstract && !ti.IsInterface
                from i in t.GetInterfaces()
                where i.GetTypeInfo().IsGenericType && (i.GetGenericTypeDefinition() == typeof(IQueryHandler<,>))
                select new { Request = i.GetGenericArguments().First(), ResponseType = i.GetGenericArguments().ElementAt(1), Handler = t };

            foreach (var subscriber in subscribers)
            {
                _handlerRegistry.Register(subscriber.Request, subscriber.ResponseType, subscriber.Handler);
                _services.AddTransient(subscriber.Handler);
            }

            return this;
        }

        public AspNetDependencyInjectionHandlerConfigurationBuilder WithDefaultDecorators()
        {
            _services.AddTransient(typeof(RequestLoggingDecorator<,>));
            _services.AddTransient(typeof(RetryableQueryDecorator<,>));
            _services.AddTransient(typeof(FallbackPolicyDecorator<,>));

            return this;
        }

        public IHandlerConfiguration Build()
        {
            var serviceProvider = _services.BuildServiceProvider();
            var factory = new AspNetDependencyInjectionHandlerFactory(serviceProvider);

            return new AspNetDependencyInjectionHandlerConfiguration(_handlerRegistry, factory);
        }

        private sealed class AspNetDependencyInjectionHandlerConfiguration : IHandlerConfiguration
        {
            public IQueryHandlerRegistry HandlerRegistry { get; }
            public IQueryHandlerFactory HandlerFactory { get; }
            public IQueryHandlerDecoratorFactory DecoratorFactory { get; }

            public AspNetDependencyInjectionHandlerConfiguration(IQueryHandlerRegistry handlerRegistry, AspNetDependencyInjectionHandlerFactory factory)
            {
                HandlerRegistry = handlerRegistry;
                HandlerFactory = factory;
                DecoratorFactory = factory;
            }
        }

        private sealed class AspNetDependencyInjectionHandlerFactory : IQueryHandlerFactory, IQueryHandlerDecoratorFactory
        {
            private readonly IServiceProvider _serviceProvider;

            public AspNetDependencyInjectionHandlerFactory(IServiceProvider serviceProvider)
            {
                _serviceProvider = serviceProvider;
            }

            T IQueryHandlerFactory.Create<T>(Type handlerType)
            {
                return (T)_serviceProvider.GetService(handlerType);
            }

            T IQueryHandlerDecoratorFactory.Create<T>(Type decoratorType)
            {
                return (T)_serviceProvider.GetService(decoratorType);
            }
        }
    }
}