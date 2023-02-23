using EmBrito.Dataverse.Extensions.Messaging;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmBrito.Dataverse.Extensions.Tests.Messaging.Dispatchers
{
    public class StringMessageDispatcher : MessageDispatcher<string>
    {
        public StringMessageDispatcher(IServiceProvider serviceProvider, ILogger<StringMessageDispatcher> logger)
            : base(serviceProvider, logger)
        {
        }
    }
}
