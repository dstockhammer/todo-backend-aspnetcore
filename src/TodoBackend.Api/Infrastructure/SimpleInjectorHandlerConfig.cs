using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using paramore.brighter.commandprocessor;
using paramore.brighter.commandprocessor.logging.Handlers;
using paramore.brighter.commandprocessor.policy.Handlers;
using SimpleInjector;

namespace TodoBackend.Api.Infrastructure
{
    public class SimpleInjectorHandlerConfig
    {
        private readonly Container _container;
        private readonly HandlerFactory _handlerFactory;

        public IAmASubscriberRegistry Subscribers => _handlerFactory;
        public HandlerConfiguration HandlerConfiguration => new HandlerConfiguration(_handlerFactory, _handlerFactory);

        public SimpleInjectorHandlerConfig(Container container)
        {
            _container = container;
            _handlerFactory = new HandlerFactory(container);
        }

        public void RegisterDefaultHandlers()
        {
            _container.Register(typeof(RequestLoggingHandler<>));
            _container.Register(typeof(ExceptionPolicyHandler<>));
            _container.Register(typeof(RequestLoggingHandlerAsync<>));
            _container.Register(typeof(ExceptionPolicyHandlerAsync<>));
        }

        public void RegisterSubscribersFromAssembly(Assembly assembly)
        {
            var subscribers =
                from t in assembly.GetExportedTypes()
                let ti = t.GetTypeInfo()
                where ti.IsClass && !ti.IsAbstract && !ti.IsInterface
                from i in t.GetInterfaces()
                where i.GetTypeInfo().IsGenericType && (i.GetGenericTypeDefinition() == typeof(IHandleRequestsAsync<>))
                select new { Request = i.GetGenericArguments().First(), Handler = t };

            foreach (var subscriber in subscribers)
            {
                _handlerFactory.Register(subscriber.Request, subscriber.Handler);
            }
        }

        private sealed class HandlerFactory : IAmAHandlerFactoryAsync, IAmASubscriberRegistry
        {
            private readonly Container _container;
            private readonly SubscriberRegistry _registry;

            public HandlerFactory(Container container)
            {
                _container = container;
                _registry = new SubscriberRegistry();
            }

            IHandleRequestsAsync IAmAHandlerFactoryAsync.Create(Type handlerType)
            {
                return (IHandleRequestsAsync)_container.GetInstance(handlerType);
            }

            public void Release(IHandleRequestsAsync handler)
            {
                // todo not supported by all containers
            }

            public IEnumerable<Type> Get<T>() where T : class, IRequest
            {
                return _registry.Get<T>();
            }

            public void Register<TRequest, TImplementation>() where TRequest : class, IRequest where TImplementation : class, IHandleRequests<TRequest>
            {
                _container.Register<TImplementation>();
                _registry.Register<TRequest, TImplementation>();
            }

            public void Register(Type request, Type handler)
            {
                _container.Register(handler);
                _registry.Add(request, handler);
            }
        }
    }
}