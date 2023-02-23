using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmBrito.Dataverse.Extensions.Messaging
{
    public abstract class DataverseMessageHandler : IMessageHandler<RemoteExecutionContext>
    {
        public abstract bool CanHandle(RemoteExecutionContext message);

        public abstract Task Handle(RemoteExecutionContext message, CancellationToken cancellationToken);
    }
}
