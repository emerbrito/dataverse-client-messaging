using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmBrito.Dataverse.Extensions.Messaging
{
    internal static class HashSetExtensions
    {

        internal static void TryAdd<TValue>(this HashSet<TValue> hashSet, TValue value)
        {
            if (hashSet is null) return;
            if (hashSet.Contains(value)) return;

            hashSet.Add(value); 
        }

    }
}
