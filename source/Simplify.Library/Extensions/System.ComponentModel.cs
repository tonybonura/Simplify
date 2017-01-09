using System;
using System.Collections.Generic;
using System.Linq;

namespace System.ComponentModel
{
    public static class _ComponentModelExtensions
    {
        public static bool IsDefined<T>(this PropertyDescriptor descriptor) where T : Attribute
        {
            return descriptor.Attributes.Any(a => typeof(T).IsAssignableFrom(a.GetType()));
        }
        public static TAttribute GetCustomAttribute<TAttribute>(this PropertyDescriptor descriptor) where TAttribute : Attribute
        {
            return descriptor.Attributes.OfType<TAttribute>().SingleOrDefault();
        }
        public static TAttribute GetCustomAttribute<TAttribute>(this PropertyDescriptor descriptor, TAttribute attribute) where TAttribute : Attribute
        {
            return descriptor.Attributes.OfType<TAttribute>().SingleOrDefault(a => a.Match(attribute));
        }
        public static IEnumerable<TAttribute> GetCustomAttributes<TAttribute>(this PropertyDescriptor descriptor) where TAttribute : Attribute
        {
            return descriptor.Attributes.OfType<TAttribute>();
        }
    }
}
