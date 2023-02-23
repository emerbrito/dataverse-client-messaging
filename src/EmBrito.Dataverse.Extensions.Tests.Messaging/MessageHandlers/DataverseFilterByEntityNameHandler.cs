using EmBrito.Dataverse.Extensions.Messaging;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmBrito.Dataverse.Extensions.Tests.Messaging.MessageHandlers
{

    public class DataverseFilterByEntityNameHandler : DataverseMessageHandler
    {

        public static string? EntityNameFilter { get; set; }

        public override bool CanHandle(RemoteExecutionContext message)
        {
            return !string.IsNullOrWhiteSpace(message.PrimaryEntityName) 
                && message.PrimaryEntityName.Equals(EntityNameFilter, StringComparison.CurrentCultureIgnoreCase);
        }

        public override Task Handle(RemoteExecutionContext message, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

    }
}
