using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Extensions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EmBrito.Dataverse.Extensions.Messaging
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddDataverseMessagetDispatcher(this IServiceCollection services, params Assembly[] assemblies) 
        {

            var handlerType = typeof(IMessageHandler<RemoteExecutionContext>);
            var concretions = ScanAssemblies(handlerType, assemblies);

            foreach (var concretion in concretions)
            {
                services.TryAddTransient(concretion);
            }

            services.AddSingleton<DataverseMessageDispatcher>(serviceProvider =>
            {
                var dispatcher = new DataverseMessageDispatcher(
                    serviceProvider,
                    serviceProvider.GetRequiredService<ILogger<DataverseMessageDispatcher>>());

                foreach (var concretion in concretions)
                {
                    dispatcher.RegisterHandler(concretion);
                }

                return dispatcher;
            });

            return services;
        }

        public static IServiceCollection AddMessagetDispatcher<TService>(this IServiceCollection services, Action<MessageDispatcherOptions> options)
            where TService : MessageDispatcherCore
        {

            ArgumentNullException.ThrowIfNull(nameof(options));
            //services.Configure(typeof(TService).FullName, options);

            var description = new ServiceDescriptor(typeof(TService), typeof(TService), ServiceLifetime.Singleton);
            var serviceType = typeof(TService);
            Type? messageType = null;

            if(serviceType.IsGenericType && serviceType.GetGenericTypeDefinition() == typeof(MessageDispatcher<>))
            {
                messageType = serviceType.GenericTypeArguments[0];
            }
            else 
            {

                var baseType = typeof(TService).BaseType;

                while (baseType != null)
                {
                    if (baseType.IsGenericType && baseType.GetGenericTypeDefinition() == typeof(MessageDispatcher<>))
                    {
                        messageType = baseType.GenericTypeArguments[0];
                        break;
                    }

                    baseType = baseType.BaseType;
                }

            }

            if (messageType == null)
            {
                throw new InvalidOperationException($"Unable to determine message type when attempting to register message dispatcher: {typeof(TService).FullName}.");
            }

            var registerOptions = new MessageDispatcherOptions();
            options.Invoke(registerOptions);

            var combinedTypes = registerOptions.HandlerTypes.Union(ScanAssemblies(registerOptions));

            foreach (var handler in combinedTypes)
            {
                services.TryAddTransient(handler);
            }

            services.AddSingleton<TService>(serviceProvider =>
            {
                //var svcOptions = serviceProvider
                //    .GetRequiredService<IOptionsSnapshot<MessageDispatcherOptions>>()?
                //    .Get(typeof(TService).FullName);

                var instance = (MessageDispatcherCore)ActivatorUtilities.CreateInstance(serviceProvider, typeof(TService));

                if(combinedTypes != null)
                {
                    foreach (var handler in combinedTypes)
                    {                        
                        instance.RegisterHandler(messageType, handler);
                    }
                }

                return (TService)instance;
            });

            return services;
        }

        private static List<Type> ScanAssemblies(MessageDispatcherOptions options)
        {
            if (options is null || options.ScanTypes is null || options.ScanTypes.Count == 0)
            {
                return new List<Type>();
            }

            List<Type> allTypes = new();

            foreach (var item in options.ScanTypes)
            {
                allTypes.AddRange(ScanAssemblies(item.Key, item.Value.ToArray()));
            }

            return allTypes;
        }

        private static List<Type> ScanAssemblies(Type handlerType, IEnumerable<Assembly> assemblies)
        {
            if(handlerType is null || assemblies is null)
            {
                return new List<Type>();
            }

            var concretions = assemblies
                            .SelectMany(a => a.GetTypes())
                            .Where(x => (handlerType.IsAssignableFrom(x) || handlerType.IsSubclassOf(typeof(DataverseMessageHandler))) && x.IsClass && !x.IsAbstract)
                            .ToList();

            return concretions;
        }

    }
}
