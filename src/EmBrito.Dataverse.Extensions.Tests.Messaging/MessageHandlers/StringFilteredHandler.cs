using EmBrito.Dataverse.Extensions.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmBrito.Dataverse.Extensions.Tests.Messaging.MessageHandlers
{
    internal class StringFilteredHandler : IMessageHandler<string>
    {

        public static string? StringToMatch { get; set; }

        public bool CanHandle(string message)
        {
            return !string.IsNullOrWhiteSpace(message)
                && !string.IsNullOrWhiteSpace(StringToMatch)
                && StringToMatch.Equals(message, StringComparison.CurrentCultureIgnoreCase);
        }

        public Task Handle(string message, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }

}
