using System;
using System.Collections.Generic;
using System.Linq;

namespace ggj2018.Core.Util
{
    public static class RandomExtensions
    {
        public static T RandomEntry<T>(this Random random, IReadOnlyCollection<T> collection)
        {
            int idx = random.Next(collection.Count);
            return collection.ElementAt(idx);
        }
    }
}
