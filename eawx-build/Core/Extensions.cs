using System;
using System.Collections.Generic;
using System.Linq;

namespace EawXBuild.Core
{
    internal static class Extensions
    {
        public static bool IsExceptionType<T>(this Exception error) where T : Exception
        {
            switch (error)
            {
                case T _:
                    return true;
                case AggregateException aggregateException:
                    return aggregateException.InnerExceptions.Any(p => p.IsExceptionType<T>());
                default:
                    return false;
            }
        }

        public static void AddRange<T>(this ICollection<T> source, IEnumerable<T> items)
        {
            if (source is null)
                throw new ArgumentNullException(nameof(source));
            if (items is null)
                throw new ArgumentNullException(nameof(items));
            foreach (var obj in items)
                source.Add(obj);
        }
    }
}
