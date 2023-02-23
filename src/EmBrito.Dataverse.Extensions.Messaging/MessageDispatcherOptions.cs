using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EmBrito.Dataverse.Extensions.Messaging
{
    public class MessageDispatcherOptions
    {

        internal Dictionary<Type, HashSet<Assembly>> ScanTypes { get; private set; } = new();

        internal HashSet<Type> HandlerTypes { get; private set; } = new HashSet<Type>();

        public void AddHandler<THandler>() where THandler : class
        {
            var handlerType = typeof(THandler);
            if(!HandlerTypes.Contains(handlerType))
            {
                HandlerTypes.Add(handlerType);
            }
        }

        public void ScanHandlers<THandler>(Assembly[] assemblies)
        {

            if (assemblies is null || assemblies.Length == 0)
            {
                return;
            }

            var handlerType = typeof(THandler);

            if(ScanTypes.Keys.Contains(handlerType))
            {
                foreach (var assembly in assemblies)
                {
                    ScanTypes[handlerType].TryAdd(assembly);
                }                
            }
            else
            {
                var values = new HashSet<Assembly>();

                foreach (var assembly in assemblies) 
                { 
                    values.TryAdd(assembly);
                }

                ScanTypes.Add(handlerType, values);
            }

        }

    }
}
