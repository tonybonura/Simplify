using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace System
{
    #region [-- string Extensions --]

    internal static class _StringExtensions
    {
    }

    #endregion

    #region [-- Enum Extensions --]

    internal static class _EnumExtensions
    {
        public static TEnum ToEnum<TEnum>(this string value) where TEnum : struct, IConvertible, IComparable, IFormattable
        {
            if(!typeof(TEnum).IsEnum)
                throw new ArgumentException($"Type argument must be an enumeration type.", nameof(TEnum));

            return (TEnum)Enum.Parse(typeof(TEnum), value, true);
        }

        public static TEnum ToEnum<TEnum>(this string value, bool ignoreCase) where TEnum : struct, IConvertible, IComparable, IFormattable
        {
            if(!typeof(TEnum).IsEnum)
                throw new ArgumentException($"Type argument must be an enumeration type.", nameof(TEnum));

            return (TEnum)Enum.Parse(typeof(TEnum), value, ignoreCase);
        }

        public static TEnum ToEnum<TEnum>(object value) where TEnum : struct, IConvertible, IComparable, IFormattable
        {
            if(value == null)
                throw new ArgumentNullException(nameof(value));

            else if(value.GetType() == typeof(TEnum))
                return (TEnum)value;

            else if(!typeof(TEnum).IsEnum)
                throw new ArgumentException($"Type argument must be an enumeration type.");

            else if(Enum.IsDefined(typeof(TEnum), value))
                return (TEnum)value;

            return (TEnum)Enum.Parse(typeof(TEnum), value.ToString(), true);
        }
    }

    #endregion

    #region [-- Type Extensions --]

    internal static class _TypeExtensions
    {
        public static bool IsDefined<T>(this Type type) where T : Attribute
        {
            return type.IsDefined(typeof(T));
        }
        public static bool IsDefined<T>(this Type type, bool inherit) where T : Attribute
        {
            return type.IsDefined(typeof(T), inherit);
        }


        public static TypeCode GetTypeCode(this Type type)
        {
            return Type.GetTypeCode(type);
        }

        public static string GetName(this Type type)
        {
            var typeName = type.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ?? type.Name;

            if(type.IsGenericType)
            {
                typeName = typeName.Replace("`", "");

                var typeArgNames = type.GetGenericArguments()?.Select(a => a.GetName());

                typeName = $"{typeName}<{string.Join(", ", typeArgNames)}>";
            }

            return typeName;
        }

        public static string GetFullName(this Type type)
        {
            if(type.DeclaringType != null)
                return $"{type.DeclaringType.GetFullName()}.{type.GetName()}";

            else if(type.Namespace != null)
                return $"{type.Namespace}.{type.GetName()}";

            return type.GetName();
        }

        public static bool IsTypeOfType(this Type type)
        {
            return type.IsCompatible(typeof(Type));
        }

        public static bool IsNative(this Type type)
        {
            switch(Type.GetTypeCode(type.GetNullableType()))
            {
                case TypeCode.Boolean:
                case TypeCode.Char:
                case TypeCode.SByte:
                case TypeCode.Byte:
                case TypeCode.DateTime:
                case TypeCode.String:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                    return true;
            }

            if(type.Equals(typeof(object)))
                return true;

            if(type.IsEnum)
                throw new NotImplementedException();

            return false;
        }

        public static bool IsNumberType(this Type type)
        {
            return Type.GetTypeCode(type.GetNullableType()).IsNumberType();
        }
        public static bool IsInteger(this Type type)
        {
            return Type.GetTypeCode(type.GetNullableType()).IsInteger();
        }
        public static bool IsInt8(this Type type)
        {
            return Type.GetTypeCode(type.GetNullableType()).IsInt8();
        }
        public static bool IsInt16(this Type type)
        {
            return Type.GetTypeCode(type.GetNullableType()).IsInt16();
        }
        public static bool IsInt32(this Type type)
        {
            return Type.GetTypeCode(type.GetNullableType()).IsInt32();
        }
        public static bool IsInt64(this Type type)
        {
            return Type.GetTypeCode(type.GetNullableType()).IsInt64();
        }
        public static bool IsSignedInteger(this Type type)
        {
            return Type.GetTypeCode(type.GetNullableType()).IsSignedInteger();
        }
        public static bool IsSignedInt8(this Type type)
        {
            return Type.GetTypeCode(type.GetNullableType()).IsSignedInt8();
        }
        public static bool IsSignedInt16(this Type type)
        {
            return Type.GetTypeCode(type.GetNullableType()).IsSignedInt16();
        }
        public static bool IsSignedInt32(this Type type)
        {
            return Type.GetTypeCode(type.GetNullableType()).IsSignedInt32();
        }
        public static bool IsSignedInt64(this Type type)
        {
            return Type.GetTypeCode(type.GetNullableType()).IsSignedInt64();
        }
        public static bool IsUnsignedInteger(this Type type)
        {
            return Type.GetTypeCode(type.GetNullableType()).IsUnsignedInteger();
        }
        public static bool IsUnsignedInt8(this Type type)
        {
            return Type.GetTypeCode(type.GetNullableType()).IsUnsignedInt8();
        }
        public static bool IsUnsignedInt16(this Type type)
        {
            return Type.GetTypeCode(type.GetNullableType()).IsUnsignedInt16();
        }
        public static bool IsUnsignedInt32(this Type type)
        {
            return Type.GetTypeCode(type.GetNullableType()).IsUnsignedInt32();
        }
        public static bool IsUnsignedInt64(this Type type)
        {
            return Type.GetTypeCode(type.GetNullableType()).IsUnsignedInt64();
        }
        public static bool IsNumericType(this Type type)
        {
            return Type.GetTypeCode(type.GetNullableType()).IsNumericType();
        }
        public static bool IsDecimal(this Type type)
        {
            return Type.GetTypeCode(type.GetNullableType()).IsDecimal();
        }
        public static bool IsFloat(this Type type)
        {
            return Type.GetTypeCode(type.GetNullableType()).IsFloat();
        }

        public static bool IsDateTime(this Type type)
        {
            return Type.GetTypeCode(type.GetNullableType()).IsDateTime();
        }
        public static bool IsBool(this Type type)
        {
            return Type.GetTypeCode(type.GetNullableType()).IsBool();
        }
        public static bool IsString(this Type type)
        {
            return Type.GetTypeCode(type.GetNullableType()).IsString();
        }

        public static bool IsTimeSpan(this Type type)
        {
            return type.Equals(typeof(TimeSpan));
        }

        public static bool IsAnonymousType(this Type type)
        {
            return
                type.IsGenericType &&
                type.Namespace == null &&
                type.GetCustomAttributes().Contains(new System.Runtime.CompilerServices.CompilerGeneratedAttribute());
        }
        public static bool IsOneOf(this Type type, IEnumerable<Type> types)
        {
            return type.IsOneOf(types.ToArray());
        }
        public static bool IsOneOf(this Type type, params Type[] types)
        {
            foreach(var t in types)
            {
                if(t.Equals(type))
                    return true;
            }

            return false;
        }
        public static bool IsNullable(this Type type)
        {
            return !type.IsValueType || Nullable.GetUnderlyingType(type) != null;
        }

        private static Type GetGenericInterface(this Type type, Type genericTypeDef)
        {
            if(type.IsInterface && type.IsGenericType && type.GetGenericTypeDefinition().Equals(genericTypeDef))
                return type;

            return type.GetInterfaces().FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition().Equals(genericTypeDef));
        }

        public static bool IsEnumerable(this Type type)
        {
            if(type.IsString())
                return false;

            return typeof(IEnumerable).IsAssignableFrom(type);
        }
        public static bool IsTypedEnumerable(this Type type)
        {
            if(type.IsString())
                return false;

            return !(type.GetGenericInterface(typeof(IEnumerable<>)) == null);
        }
        public static Type GetEnumerableType(this Type type)
        {
            if(type.HasElementType)
                return type.GetElementType();

            else if(type.IsTypedEnumerable())
                return type.GetGenericInterface(typeof(IEnumerable<>)).GetGenericArguments().First();

            else if(type.IsEnumerable())
                return typeof(object);

            throw new ArgumentException("type '{0}' does not implement IEnumerable", nameof(type));
        }

        public static bool IsTypedEnumerable<T>(this Type type)
        {
            return
                type.IsTypedEnumerable() &&
                type.GetEnumerableType().Equals(typeof(T));
        }
        public static bool IsTypedEnumerable(this Type type, Type elementType)
        {
            return
                type.IsTypedEnumerable() &&
                type.GetEnumerableType().Equals(elementType);
        }

        public static bool IsTypedCollection(this Type type)
        {
            if(type.IsString())
                return false;

            return !(type.GetGenericInterface(typeof(ICollection<>)) == null);
        }
        public static bool IsTypedCollection(this Type type, Type elementType)
        {
            if(type.IsString())
                return false;

            return type.IsTypedCollection() && type.GetCollectionType().Equals(elementType);
        }
        public static Type GetCollectionType(this Type type)
        {
            return type.GetGenericInterface(typeof(ICollection<>)).GetGenericArguments().First();
        }

        public static bool IsTypedNullable(this Type type)
        {
            return Nullable.GetUnderlyingType(type) != null;
        }
        public static Type GetNullableType(this Type type)
        {
            var nullableType = Nullable.GetUnderlyingType(type);

            if(nullableType != null)
                return nullableType;

            return type;
        }

        public static Type GetRootEnumerableType(this Type type)
        {
            while(type.IsTypedEnumerable())
                type = type.GetEnumerableType();

            return type;
        }

        public static bool IsAssignableTo(this Type type, Type otherType)
        {
            return otherType.IsAssignableFrom(type);
        }

        public static bool IsCompatible(this Type type, Type otherType)
        {
            return
                type.IsAssignableFrom(otherType) ||
                otherType.IsAssignableFrom(type);
        }

        public static MemberInfo[] GetMembers(this Type type, MemberTypes memberTypes, BindingFlags bindingAttr, bool includePrivate = false)
        {
            return type.GetMembers(memberTypes, bindingAttr, ReadWriteFlags.CanReadAndWrite, includePrivate);
        }
        public static MemberInfo[] GetMembers(this Type type, MemberTypes memberTypes, BindingFlags bindingAttr, ReadWriteFlags readWriteFlags, bool includePrivate = false)
        {
            var members = type.GetMembers(bindingAttr);

            members = members.Where(m => (m.MemberType & memberTypes) == m.MemberType).ToArray();

            members = members.Where(m => !m.IsPrivate() || includePrivate).ToArray();

            if(readWriteFlags != ReadWriteFlags.Any)
                members = members.Where(m => m.CanRead() == readWriteFlags.HasFlag(ReadWriteFlags.CanRead) || m.CanWrite() == readWriteFlags.HasFlag(ReadWriteFlags.CanWrite)).ToArray();

            return members;
        }
    }

    #endregion

    #region [-- TypeCode Extensions --]

    internal static class _TypeCodeExtensions
    {
        public static bool IsNumberType(this TypeCode typeCode)
        {
            switch(typeCode)
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                    return true;
            }

            return false;
        }

        public static bool IsInteger(this TypeCode typeCode)
        {
            switch(typeCode)
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                    return true;
            }

            return false;
        }

        public static bool IsInt8(this TypeCode typeCode)
        {
            switch(typeCode)
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                    return true;
            }

            return false;
        }
        public static bool IsInt16(this TypeCode typeCode)
        {
            switch(typeCode)
            {
                case TypeCode.Int16:
                case TypeCode.UInt16:
                    return true;
            }

            return false;
        }
        public static bool IsInt32(this TypeCode typeCode)
        {
            switch(typeCode)
            {
                case TypeCode.Int32:
                case TypeCode.UInt32:
                    return true;
            }

            return false;
        }
        public static bool IsInt64(this TypeCode typeCode)
        {
            switch(typeCode)
            {
                case TypeCode.Int64:
                case TypeCode.UInt64:
                    return true;
            }

            return false;
        }

        public static bool IsSignedInteger(this TypeCode typeCode)
        {
            switch(typeCode)
            {
                case TypeCode.SByte:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                    return true;
            }

            return false;
        }

        public static bool IsSignedInt8(this TypeCode typeCode)
        {
            return typeCode == TypeCode.SByte;
        }
        public static bool IsSignedInt16(this TypeCode typeCode)
        {
            return typeCode == TypeCode.Int16;
        }
        public static bool IsSignedInt32(this TypeCode typeCode)
        {
            return typeCode == TypeCode.Int32;
        }
        public static bool IsSignedInt64(this TypeCode typeCode)
        {
            return typeCode == TypeCode.Int64;
        }

        public static bool IsUnsignedInteger(this TypeCode typeCode)
        {
            switch(typeCode)
            {
                case TypeCode.Byte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return true;
            }

            return false;
        }

        public static bool IsUnsignedInt8(this TypeCode typeCode)
        {
            return typeCode == TypeCode.Byte;
        }
        public static bool IsUnsignedInt16(this TypeCode typeCode)
        {
            return typeCode == TypeCode.UInt16;
        }
        public static bool IsUnsignedInt32(this TypeCode typeCode)
        {
            return typeCode == TypeCode.UInt32;
        }
        public static bool IsUnsignedInt64(this TypeCode typeCode)
        {
            return typeCode == TypeCode.UInt64;
        }

        public static bool IsNumericType(this TypeCode typeCode)
        {
            switch(typeCode)
            {
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                    return true;
            }

            return false;
        }
        public static bool IsDecimal(this TypeCode typeCode)
        {
            switch(typeCode)
            {
                case TypeCode.Decimal:
                    return true;
            }

            return false;
        }
        public static bool IsFloat(this TypeCode typeCode)
        {
            switch(typeCode)
            {
                case TypeCode.Single:
                case TypeCode.Double:
                    return true;
            }

            return false;
        }

        public static bool IsDateTime(this TypeCode typeCode)
        {
            return typeCode == TypeCode.DateTime;
        }
        public static bool IsBool(this TypeCode typeCode)
        {
            return typeCode == TypeCode.Boolean;
        }
        public static bool IsString(this TypeCode typeCode)
        {
            return typeCode == TypeCode.String;
        }
    }

    #endregion

    #region [-- Attribute Extensions --]

    internal static class _AttributeExtensions
    {
        public static bool AllowMultiple(this Attribute attribute)
        {
            return attribute.GetType().GetCustomAttributes().OfType<AttributeUsageAttribute>().Any(a => a.AllowMultiple);
        }
        public static AttributeCollection AsAttributeCollection(this IEnumerable<Attribute> source)
        {
            if(source is AttributeCollection)
                return (AttributeCollection)source;

            return new AttributeCollection(source.ToArray());
        }
    }

    #endregion

    #region [-- Attribute Collection Extensions --]

    internal static class _AttributeCollectionExtensions
    {
        public static bool Any(this AttributeCollection collection)
        {
            return collection.Count > 0;
        }
        public static bool Any(this AttributeCollection collection, Func<Attribute, bool> predicate)
        {
            return collection.Cast<Attribute>().Any(predicate);
        }

        public static bool Any<TAttribute>(this AttributeCollection collection) where TAttribute : Attribute
        {
            return collection.OfType<TAttribute>().Any();
        }
        public static bool Any<TAttribute>(this AttributeCollection collection, Func<Attribute, bool> predicate) where TAttribute : Attribute
        {
            return collection.OfType<TAttribute>().Any(predicate);
        }

        public static AttributeCollection Concat(this AttributeCollection collection, IEnumerable<Attribute> second)
        {
            return new AttributeCollection(collection.Cast<Attribute>().Concat(second).ToArray());
        }

        public static bool Contains<TAttribute>(this AttributeCollection collection) where TAttribute : Attribute
        {
            return collection.OfType<TAttribute>().Any();
        }
        public static bool Contains<TAttribute>(this AttributeCollection collection, TAttribute attribute) where TAttribute : Attribute
        {
            return collection.OfType<TAttribute>().Contains(attribute);
        }

        public static Attribute First(this AttributeCollection collection)
        {
            return collection.Cast<Attribute>().First();
        }
        public static Attribute First(this AttributeCollection collection, Func<Attribute, bool> predicate)
        {
            return collection.Cast<Attribute>().First(predicate);
        }

        public static TAttribute First<TAttribute>(this AttributeCollection collection) where TAttribute : Attribute
        {
            return collection.OfType<TAttribute>().First();
        }
        public static TAttribute First<TAttribute>(this AttributeCollection collection, Func<TAttribute, bool> predicate) where TAttribute : Attribute
        {
            return collection.OfType<TAttribute>().First(predicate);
        }

        public static Attribute FirstOrDefault(this AttributeCollection collection)
        {
            return collection.Cast<Attribute>().FirstOrDefault();
        }
        public static Attribute FirstOrDefault(this AttributeCollection collection, Func<Attribute, bool> predicate)
        {
            return collection.Cast<Attribute>().FirstOrDefault(predicate);
        }

        public static TAttribute FirstOrDefault<TAttribute>(this AttributeCollection collection) where TAttribute : Attribute
        {
            return collection.OfType<TAttribute>().FirstOrDefault();
        }
        public static TAttribute FirstOrDefault<TAttribute>(this AttributeCollection collection, Func<TAttribute, bool> predicate) where TAttribute : Attribute
        {
            return collection.OfType<TAttribute>().FirstOrDefault(predicate);
        }

        public static Attribute Last(this AttributeCollection collection)
        {
            return collection.Cast<Attribute>().Last();
        }
        public static Attribute Last(this AttributeCollection collection, Func<Attribute, bool> predicate)
        {
            return collection.Cast<Attribute>().Last(predicate);
        }

        public static TAttribute Last<TAttribute>(this AttributeCollection collection) where TAttribute : Attribute
        {
            return collection.OfType<TAttribute>().Last();
        }
        public static TAttribute Last<TAttribute>(this AttributeCollection collection, Func<TAttribute, bool> predicate) where TAttribute : Attribute
        {
            return collection.OfType<TAttribute>().Last(predicate);
        }

        public static Attribute LastOrDefault(this AttributeCollection collection)
        {
            return collection.Cast<Attribute>().LastOrDefault();
        }
        public static Attribute LastOrDefault(this AttributeCollection collection, Func<Attribute, bool> predicate)
        {
            return collection.Cast<Attribute>().LastOrDefault(predicate);
        }

        public static TAttribute LastOrDefault<TAttribute>(this AttributeCollection collection) where TAttribute : Attribute
        {
            return collection.OfType<TAttribute>().LastOrDefault();
        }
        public static TAttribute LastOrDefault<TAttribute>(this AttributeCollection collection, Func<TAttribute, bool> predicate) where TAttribute : Attribute
        {
            return collection.OfType<TAttribute>().LastOrDefault(predicate);
        }

        public static AttributeCollection OrderBy<TKey>(this AttributeCollection collection, Func<Attribute, TKey> keySelector)
        {
            return new AttributeCollection(collection.Cast<Attribute>().OrderBy(keySelector).ToArray());
        }

        public static Attribute Single(this AttributeCollection collection)
        {
            return collection.Cast<Attribute>().SingleOrDefault();
        }
        public static Attribute Single(this AttributeCollection collection, Func<Attribute, bool> predicate)
        {
            return collection.Cast<Attribute>().SingleOrDefault(predicate);
        }

        public static TAttribute Single<TAttribute>(this AttributeCollection collection) where TAttribute : Attribute
        {
            return collection.OfType<TAttribute>().SingleOrDefault();
        }
        public static TAttribute Single<TAttribute>(this AttributeCollection collection, Func<TAttribute, bool> predicate) where TAttribute : Attribute
        {
            return collection.OfType<TAttribute>().SingleOrDefault(predicate);
        }

        public static Attribute SingleOrDefault(this AttributeCollection collection)
        {
            return collection.Cast<Attribute>().SingleOrDefault();
        }
        public static Attribute SingleOrDefault(this AttributeCollection collection, Func<Attribute, bool> predicate)
        {
            return collection.Cast<Attribute>().SingleOrDefault(predicate);
        }

        public static TAttribute SingleOrDefault<TAttribute>(this AttributeCollection collection) where TAttribute : Attribute
        {
            return collection.OfType<TAttribute>().SingleOrDefault();
        }
        public static TAttribute SingleOrDefault<TAttribute>(this AttributeCollection collection, Func<TAttribute, bool> predicate) where TAttribute : Attribute
        {
            return collection.OfType<TAttribute>().SingleOrDefault(predicate);
        }

        public static IEnumerable<TResult> Select<TResult>(this AttributeCollection collection, Func<Attribute, TResult> selector)
        {
            return collection.Cast<Attribute>().Select(selector);
        }
        public static IEnumerable<TResult> Select<TAttribute, TResult>(this AttributeCollection collection, Func<Attribute, TResult> selector) where TAttribute : Attribute
        {
            return collection.Cast<TAttribute>().Select(selector);
        }

        public static IEnumerable<TResult> SelectMany<TResult>(this AttributeCollection collection, Func<Attribute, IEnumerable<TResult>> selector)
        {
            return collection.Cast<Attribute>().SelectMany(selector);
        }
        public static IEnumerable<TResult> SelectMany<TAttribute, TResult>(this AttributeCollection collection, Func<Attribute, IEnumerable<TResult>> selector) where TAttribute : Attribute
        {
            return collection.OfType<TAttribute>().SelectMany(selector);
        }

        public static Attribute[] ToArray(this AttributeCollection collection)
        {
            return collection.Cast<Attribute>().ToArray();
        }
        public static TAttribute[] ToArray<TAttribute>(this AttributeCollection collection) where TAttribute : Attribute
        {
            return collection.OfType<TAttribute>().ToArray();
        }

        public static AttributeCollection Where(this AttributeCollection collection, Func<Attribute, bool> predicate)
        {
            return new AttributeCollection(collection.Cast<Attribute>().Where(predicate).ToArray());
        }
        public static AttributeCollection Where<TAttribute>(this AttributeCollection collection, Func<Attribute, bool> predicate) where TAttribute : Attribute
        {
            return new AttributeCollection(collection.OfType<TAttribute>().Where(predicate).ToArray());
        }

        public static AttributeCollection Merge(this AttributeCollection a, AttributeCollection b)
        {
            return new AttributeCollection(MergeAsArray(a.Cast<Attribute>(), b.Cast<Attribute>()));
        }
        public static AttributeCollection Merge(this AttributeCollection a, IEnumerable<Attribute> b)
        {
            return new AttributeCollection(MergeAsArray(a.Cast<Attribute>(), b));
        }
        public static AttributeCollection Merge(this AttributeCollection a, params Attribute[] b)
        {
            return new AttributeCollection(MergeAsArray(a.Cast<Attribute>(), b));
        }

        private static Attribute[] MergeAsArray(this IEnumerable<Attribute> a, IEnumerable<Attribute> b)
        {
            // untested

            if(a == null)
                throw new ArgumentNullException(nameof(a));

            if(b == null)
                throw new ArgumentNullException(nameof(b));

            var result = new HashSet<Attribute>(a.Distinct());

            foreach(var attrB in b)
            {
                var existingOfSameType = result.Where(attrA => attrA.GetType().Equals(attrB.GetType()));

                if(!existingOfSameType.Any())
                    result.Add(attrB);
                else if(attrB.AllowMultiple())
                {
                    var matchExisting = false;

                    foreach(var existing in existingOfSameType)
                    {
                        if(existing.Match(attrB))
                        {
                            matchExisting = true;
                            break;
                        }
                    }

                    if(!matchExisting)
                        result.Add(attrB);
                }
            }

            return result.ToArray();
        }
    }

    #endregion
}
