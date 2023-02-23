using EmBrito.Dataverse.Extensions.Messaging;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmBrito.Dataverse.Extensions.Tests.Messaging.MessageHandlers
{

    public class DataverseUpdateOrgNameHandler : DataverseMessageHandler
    {

        public static string? LastUpdatedOrgName { get; set; }

        public static string FormatOrgName(string name) => $"{name} - Changed";

        public override bool CanHandle(RemoteExecutionContext message)
        {
            return true;
        }

        public override Task Handle(RemoteExecutionContext message, CancellationToken cancellationToken)
        {
            LastUpdatedOrgName = FormatOrgName(message.OrganizationName);
            return Task.CompletedTask;
        }

    }
}
