using System;
using System.Linq;
using System.Reflection;
using Application.Common.Behaviors;
using Application.Common.Events;
using Application.Common.Interceptors;
using Castle.Core.Internal;
using Castle.DynamicProxy;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Application
{
    public static class DependencyInjection
    {
        public static void AddProxiedScoped<TInterface, TImplementation>(this IServiceCollection services,
            params Type[] interceptorsType)
            where TInterface : class
            where TImplementation : class, TInterface
        {
            services.AddScoped<TImplementation>();
            services.AddScoped(typeof(TInterface), serviceProvider =>
            {
                var proxyGenerator = serviceProvider.GetRequiredService<ProxyGenerator>();
                var actual = serviceProvider.GetRequiredService<TImplementation>();
                var interceptors = serviceProvider
                    .GetServices<IInterceptor>()
                    .Where(interceptor =>
                        interceptorsType.IsNullOrEmpty() || interceptorsType.Contains(interceptor.GetType()))
                    .ToArray();
                return proxyGenerator.CreateInterfaceProxyWithTarget(typeof(TInterface), actual, interceptors);
            });
        }

        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddTransient<IDomainEventDispatcher, MediatrDomainEventDispatcher>();
            services.AddMediatR(typeof(MediatrDomainEventDispatcher).GetTypeInfo().Assembly);
            services.AddSingleton(new ProxyGenerator());
            services.AddScoped<IInterceptor, EventPublisherInterceptor>();
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(AuthenticationBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
            return services;
        }
    }
}