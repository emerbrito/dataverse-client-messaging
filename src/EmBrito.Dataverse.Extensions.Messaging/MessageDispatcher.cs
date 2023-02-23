using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EmBrito.Dataverse.Extensions.Messaging
{
    public class MessageDispatcher<TMessage> : MessageDispatcherCore
    {

        public MessageDispatcher(IServiceProvider serviceProvider, ILogger<MessageDispatcher<TMessage>> logger)
            : base(serviceProvider, logger)
        {
        }

        public MessageDispatcher(IServiceProvider serviceProvider, ILogger logger)
            : base(serviceProvider, logger) 
        {
        }

        protected internal void RegisterHandler(Type handlerType)
        {
            RegisterHandler(typeof(TMessage), handlerType);
        }

        public async Task<int> Dispatch(TMessage message, CancellationToken cancellationToken)
        {

            int count = 0;

            foreach (var handlerType in _handlerTypes)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                var handler = _serviceProvider.GetService(handlerType);

                if (handler != null)
                {
                    var h = (IMessageHandler<TMessage>)handler;

                    if (h.CanHandle(message))
                    {
                        await h.Handle(message, cancellationToken);
                        count++;
                    }

                }
            }

            return count;
        }

    }
}
