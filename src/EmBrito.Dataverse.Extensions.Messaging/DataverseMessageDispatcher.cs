using Microsoft.Extensions.Logging;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmBrito.Dataverse.Extensions.Messaging
{
    public class DataverseMessageDispatcher : MessageDispatcher<RemoteExecutionContext>
    {
        public DataverseMessageDispatcher(IServiceProvider serviceProvider, ILogger<DataverseMessageDispatcher> logger) 
            : base(serviceProvider,logger)
        {
        }
    }
}
