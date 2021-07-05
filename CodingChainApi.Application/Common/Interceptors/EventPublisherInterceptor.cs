using System;
using System.Linq;
using System.Reflection;
using Application.Common.Events;
using Application.Contracts;
using Castle.DynamicProxy;
using Domain.Contracts;
using ApplicationException = Application.Common.Exceptions.ApplicationException;

namespace Application.Common.Interceptors
{
    public class EventPublisherInterceptor : IInterceptor
    {
        private readonly IDomainEventDispatcher _dispatcher;


        public EventPublisherInterceptor(IDomainEventDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        public void Intercept(IInvocation invocation)
        {
            if (!IsAggregateRepository(invocation.TargetType))
                throw new ApplicationException(
                    $"{invocation.GetType()} doesn't implement IAggregateRepository<,> interface");

            invocation.Proceed();
            if (!IsSetAsyncMethod(invocation.Method))
                return;
            var aggregate = invocation.Arguments.First() as IAggregate;
            foreach (var aggregateEvent in aggregate.Events.ToList()) _dispatcher.Dispatch(aggregateEvent);
            aggregate.ClearEvents();
        }

        private bool IsAggregateRepository(Type invocationType)
        {
            return invocationType.GetInterfaces().Any(interfaceType =>
                interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition()
                    .IsAssignableFrom(typeof(IAggregateRepository<,>))
            );
        }

        private bool IsSetAsyncMethod(MethodInfo methodInfo)
        {
            return methodInfo.Name == nameof(IAggregateRepository<IEntityId, Aggregate<IEntityId>>.SetAsync);
        }
    }
}