using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Collections.Generic
{
    public static class _IEnumerableExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach(var t in source)
                action(t);
        }
        public static void ForEach<T>(this IEnumerable<T> source, Action<T, int> action)
        {
            var index = 0;
            foreach(var t in source)
                action(t, index++);
        }

        public static string Join<T>(this IEnumerable<T> source, char separator)
        {
            return string.Join(separator, source.Select(t => t?.ToString()));
        }
        public static string Join<T>(this IEnumerable<T> source, string separator)
        {
            return string.Join(separator, source.Select(t => t?.ToString()));
        }

        public static string Join(this IEnumerable<string> source, char separator)
        {
            return string.Join(separator, source);
        }
        public static string Join(this IEnumerable<string> source, string separator)
        {
            return string.Join(separator, source);
        }
    }
}
