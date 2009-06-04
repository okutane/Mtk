using System;
using System.Collections.Generic;
using System.Text;

namespace Matveev.Common
{
    public delegate T2 Function<T1, T2>(T1 arg);

    public static class EnumerableHelpers
    {
        public static int IndexOf<T>(this IEnumerable<T> enumerable, T item)
        {
            int index = 0;
            foreach (T collectionItem in enumerable)
            {
                if (collectionItem.Equals(item))
                    return index;

                index++;
            }

            return -1;
        }

        public static int Count<T>(IEnumerable<T> enumerable)
        {
            int count = 0;
            for (IEnumerator<T> counter = enumerable.GetEnumerator(); counter.MoveNext();)
            {
                count++;
            }

            return count;
        }

        public static T2 Aggregate<T1, T2>(IEnumerable<T1> enumerable, Function<T1, T2> function,
            IAggregator<T2> aggregator)
        {
            aggregator.Init();

            foreach (T1 item in enumerable)
            {
                aggregator.Aggregate(function(item));
            }

            return aggregator.Result;
        }
    }
}
