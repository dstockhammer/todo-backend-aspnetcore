using System;
using System.Linq;
using System.Reflection;
using Darker;
using Darker.Decorators;
using SimpleInjector;

namespace TodoBackend.Api.Infrastructure
{
    public sealed class SimpleInjectorHandlerConfigurationBuilder
    {
        private readonly Container _container;
        private readonly IQueryHandlerRegistry _handlerRegistry;

        public SimpleInjectorHandlerConfigurationBuilder(Container container)
        {
            _container = container;
            _handlerRegistry = new QueryHandlerRegistry();
        }

        public SimpleInjectorHandlerConfigurationBuilder WithQueriesAndHandlersFromAssembly(Assembly assembly)
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
                _container.Register(subscriber.Handler);
            }

            return this;
        }

        public SimpleInjectorHandlerConfigurationBuilder WithDefaultDecorators()
        {
            _container.Register(typeof(RequestLoggingDecorator<,>));
            _container.Register(typeof(RetryableQueryDecorator<,>));
            _container.Register(typeof(FallbackPolicyDecorator<,>));

            return this;
        }

        public IHandlerConfiguration Build()
        {
            var factory = new HandlerFactory(_container);
            return new HandlerConfiguration(_handlerRegistry, factory);
        }

        private sealed class HandlerConfiguration : IHandlerConfiguration
        {
            public IQueryHandlerRegistry HandlerRegistry { get; }
            public IQueryHandlerFactory HandlerFactory { get; }
            public IQueryHandlerDecoratorFactory DecoratorFactory { get; }

            public HandlerConfiguration(IQueryHandlerRegistry handlerRegistry, HandlerFactory factory)
            {
                HandlerRegistry = handlerRegistry;
                HandlerFactory = factory;
                DecoratorFactory = factory;
            }
        }

        private sealed class HandlerFactory : IQueryHandlerFactory, IQueryHandlerDecoratorFactory
        {
            private readonly Container _container;

            public HandlerFactory(Container container)
            {
                _container = container;
            }

            T IQueryHandlerFactory.Create<T>(Type handlerType)
            {
                return (T)_container.GetInstance(handlerType);
            }

            T IQueryHandlerDecoratorFactory.Create<T>(Type decoratorType)
            {
                return (T)_container.GetInstance(decoratorType);
            }
        }
    }
}