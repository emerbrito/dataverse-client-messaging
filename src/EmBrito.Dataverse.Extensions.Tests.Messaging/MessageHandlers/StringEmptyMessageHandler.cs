using EmBrito.Dataverse.Extensions.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmBrito.Dataverse.Extensions.Tests.Messaging.MessageHandlers
{
    public class StringEmptyMessageHandler : IMessageHandler<string>
    {
        public bool CanHandle(string message)
        {
            return true;
        }

        public Task Handle(string message, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
