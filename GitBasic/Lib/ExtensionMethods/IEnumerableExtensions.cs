using System;
using System.Collections.Generic;

namespace GitBasic
{
    public static class IEnumerableExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> iEnumerable, Action<T> action)
        {
            foreach (T item in iEnumerable)
            {
                action(item);
            }
        }
    }
}
