using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace EmBrito.Dataverse.Extensions.Messaging
{
    public abstract class MessageDispatcherCore
    {

        protected ILogger _logger;
        protected List<Type> _handlerTypes = new();
        protected IServiceProvider _serviceProvider;        

        public MessageDispatcherCore(IServiceProvider serviceProvider, ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        internal void RegisterHandler(Type messageType, Type handlerType)
        {
            ArgumentNullException.ThrowIfNull(handlerType, nameof(handlerType));

            if (!handlerType.IsClass || handlerType.IsAbstract)
            {
                throw new InvalidOperationException($"Unable to register handler. Invalid or unexpected type: {handlerType.FullName}.");
            }

            if (!handlerType.GetInterfaces().Any(i => 
                i.IsGenericType && 
                i.GetGenericTypeDefinition() == typeof(IMessageHandler<>) && 
                i.GenericTypeArguments.Length > 0 &&
                i.GenericTypeArguments[0] == messageType))
            {
                throw new ArgumentException($"Unable to register handler. Message handler {handlerType.Name} does not implement {typeof(IMessageHandler<>).Name}<{messageType.Name}>.");
            }

            if (!_handlerTypes.Contains(handlerType))
            {
                _handlerTypes.Add(handlerType);
            }
        }

    }
}
