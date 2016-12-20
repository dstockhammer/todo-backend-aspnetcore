using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using paramore.brighter.commandprocessor;
using paramore.brighter.commandprocessor.logging.Handlers;
using paramore.brighter.commandprocessor.policy.Handlers;

namespace TodoBackend.Api.Infrastructure
{
    public class AspNetDependencyInjectionHandlerConfig
    {
        private readonly IServiceCollection _services;
        private readonly HandlerFactory _handlerFactory;

        public IAmASubscriberRegistry Subscribers => _handlerFactory;
        public HandlerConfiguration HandlerConfiguration => new HandlerConfiguration(_handlerFactory, _handlerFactory);

        public AspNetDependencyInjectionHandlerConfig(IServiceCollection services)
        {
            _services = services;
            _handlerFactory = new HandlerFactory(services);
        }

        public void RegisterDefaultHandlers()
        {
            _services.AddTransient(typeof(RequestLoggingHandler<>));
            _services.AddTransient(typeof(ExceptionPolicyHandler<>));

            _services.AddTransient(typeof(RequestLoggingHandlerRequestHandlerAsync<>));
            _services.AddTransient(typeof(ExceptionPolicyHandlerAsync<>));
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
            private readonly IServiceCollection _services;
            private readonly SubscriberRegistry _registry;

            public HandlerFactory(IServiceCollection services)
            {
                _services = services;
                _registry = new SubscriberRegistry();
            }

            IHandleRequestsAsync IAmAHandlerFactoryAsync.Create(Type handlerType)
            {
                return (IHandleRequestsAsync)_services.BuildServiceProvider().GetService(handlerType);
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
                _services.AddTransient<TImplementation>();
                _registry.Register<TRequest, TImplementation>();
            }

            public void Register(Type request, Type handler)
            {
                _services.AddTransient(handler);
                _registry.Add(request, handler);
            }
        }
    }
}