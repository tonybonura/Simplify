using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Collections.Generic
{
    public static class _IEnumerableExtensions
    {
        /// <summary>
        /// Determines whether two sequences are equal by comparing the elements (without regard for sequence) by using the default equality comparer for their type.
        /// (Similar to IEnumerable&lt;T&gt;.SequenceEquals but compares the contents without regard for the order in which the items are contained)
        /// </summary>
        /// <typeparam name="T">The type of the elements of the input sequences.</typeparam>
        /// <param name="first">An System.Collections.Generic.IEnumerable&lt;T&gt; to compare to second.</param>
        /// <param name="second">An System.Collections.Generic.IEnumerable&lt;T&gt; to compare to the first sequence.</param>
        /// <returns>true if the two source sequences are of equal length and their corresponding elements are equal according to the default equality comparer for their type otherwise, false.</returns>
        public static bool CollectionEquals<T>(this IEnumerable<T> first, IEnumerable<T> second)
        {
            if(first == null)
                throw new ArgumentNullException("first is null.", nameof(first));

            if(second == null)
                throw new ArgumentNullException("second is null.", nameof(second));


            return
              first.Count() == second.Count() &&
              first.All(t => second.Contains(t)) &&
              second.All(t => first.Contains(t));
        }

        /// <summary>
        /// Determines whether two sequences are equal by comparing their elements by using a specified System.Collections.Generic.IEqualityComparer&lt;T&gt;
        /// (Similar to IEnumerable&lt;T&gt;.SequenceEquals but compares the contents without regard for the order in which the items are contained)
        /// </summary>
        /// <typeparam name="T">The type of the elements of the input sequences.</typeparam>
        /// <param name="first">An System.Collections.Generic.IEnumerable&lt;T&gt; to compare to second.</param>
        /// <param name="second">An System.Collections.Generic.IEnumerable&lt;T&gt; to compare to the first sequence.</param>
        /// <param name="comparer">An System.Collections.Generic.IEqualityComparer&lt;T&gt; to use to compare elements.</param>
        /// <returns>true if the two source sequences are of equal length and their corresponding elements are equal according to the default equality comparer for their type otherwise, false.</returns>
        public static bool CollectionEquals<T>(this IEnumerable<T> first, IEnumerable<T> second, IEqualityComparer<T> comparer)
        {
            if(first == null)
                throw new ArgumentNullException("first is null.", nameof(first));

            if(second == null)
                throw new ArgumentNullException("second is null.", nameof(second));

            if(comparer == null)
                throw new ArgumentNullException("comparer is null.", nameof(comparer));


            return
              first.Count() == second.Count() &&
              first.All(t => second.Contains(t, comparer)) &&
              second.All(t => first.Contains(t, comparer));
        }

        public static IEnumerable<T> Prepend<T>(this IEnumerable<T> source, params T[] elements)
        {
            foreach(var e in elements)
                yield return e;

            foreach(var e in source)
                yield return e;
        }
        public static IEnumerable<T> Append<T>(this IEnumerable<T> source, params T[] elements)
        {
            foreach(var e in source)
                yield return e;

            foreach(var e in elements)
                yield return e;
        }
        public static IEnumerable<T> Concat<T>(this IEnumerable<T> source, params T[] elements)
        {
            foreach(var e in source)
                yield return e;

            foreach(var e in elements)
                yield return e;
            //return source.Concat(elements.AsEnumerable());
        }
        public static IEnumerable<T> Except<T>(this IEnumerable<T> source, params T[] elements)
        {
            return source.Except(elements.AsEnumerable());
        }

        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach(var t in source)
                action(t);
        }
        public static void ForEach<T>(this IEnumerable<T> source, Action<T, int> action)
        {
            int index = 0;

            foreach(var t in source)
                action(t, index++);
        }

        public static string ConcatValues<T>(this IEnumerable<T> source)
        {
            var stringValues = source as IEnumerable<string>;

            if(stringValues == null)
                stringValues = source.Select(t => Concat(t, ""));

            return stringValues.ConcatValues();
        }
        public static string ConcatValues<T>(this IEnumerable<T> source, char separator)
        {
            var stringValues = source as IEnumerable<string>;

            if(stringValues == null)
                stringValues = source.Select(t => Concat(t, separator.ToString()));

            return stringValues.ConcatValues(separator);
        }
        public static string ConcatValues<T>(this IEnumerable<T> source, string separator)
        {
            var stringValues = source as IEnumerable<string>;

            if(stringValues == null)
                stringValues = source.Select(t => Concat(t, separator));

            return stringValues.ConcatValues(separator);
        }

        public static string ConcatValues<T>(this IEnumerable<T> source, Func<T, string> selector)
        {
            return source.Select(t => selector(t)).ConcatValues();
        }
        public static string ConcatValues<T>(this IEnumerable<T> source, Func<T, string> selector, char separator)
        {
            return source.Select(t => selector(t)).ConcatValues(separator);
        }
        public static string ConcatValues<T>(this IEnumerable<T> source, Func<T, string> selector, string separator)
        {
            return source.Select(t => selector(t)).ConcatValues(separator);
        }

        public static string ConcatValues(this IEnumerable<string> source)
        {
            return string.Join("", source);
        }
        public static string ConcatValues(this IEnumerable<string> source, char separator)
        {
            return string.Join(separator.ToString(), source);
        }
        public static string ConcatValues(this IEnumerable<string> source, string separator)
        {
            return string.Join(separator, source);
        }

        private static string Concat(object source, string separator)
        {
            if(source == null)
                return null;

            var type = source.GetType();

            if(type.IsString())
                return (string)source;

            else if(type.IsTypedEnumerable() || type.IsArray)
                return ((IEnumerable)source).Cast<object>().ConcatValues(separator);

            return source.ToString();
        }
    }
}
