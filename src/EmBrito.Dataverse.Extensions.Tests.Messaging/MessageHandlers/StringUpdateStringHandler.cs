using EmBrito.Dataverse.Extensions.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmBrito.Dataverse.Extensions.Tests.Messaging.MessageHandlers
{
    internal class StringUpdateStringHandler : IMessageHandler<string>
    {

        public static string? StringToMatch { get; set; }
        
        public static string? LastUpdatedString { get; set; }

        public static string FormatString(string value) => $"{value} - Changed";

        public bool CanHandle(string message)
        {
            return !string.IsNullOrWhiteSpace(message)
                && !string.IsNullOrWhiteSpace(StringToMatch)
                && StringToMatch.Equals(message, StringComparison.CurrentCultureIgnoreCase);
        }

        public Task Handle(string message, CancellationToken cancellationToken)
        {
            LastUpdatedString = FormatString(message);
            return Task.CompletedTask;
        }
    }
}
