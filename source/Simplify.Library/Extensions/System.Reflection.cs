using System;
using System.Linq;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace System.Reflection
{
    #region [-- BindingFlagConstants --]

    internal static class BindingFlagConstants
    {
        public static readonly BindingFlags PublicInstance = BindingFlags.Public | BindingFlags.Instance;
        public static readonly BindingFlags NonPublicInstance = BindingFlags.NonPublic | BindingFlags.Instance;
        public static readonly BindingFlags PublicAndNonPublicInstance = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
        public static readonly BindingFlags PublicInstanceAndDeclaredOnly = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;
        public static readonly BindingFlags PublicStatic = BindingFlags.Public | BindingFlags.Static;
        public static readonly BindingFlags NonPublicStatic = BindingFlags.NonPublic | BindingFlags.Static;
        public static readonly BindingFlags PublicAndNonPublicStatic = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
        public static readonly BindingFlags PublicAndNonPublic = BindingFlags.Public | BindingFlags.NonPublic;
    }

    #endregion

    #region [-- MemberTypeConstants --}

    internal static class MemberTypeConstants
    {
        public static readonly MemberTypes PropertiesFieldsAndMethods = MemberTypes.Property | MemberTypes.Field | MemberTypes.Method;
        public static readonly MemberTypes PropertiesAndFields = MemberTypes.Property | MemberTypes.Field;
        public static readonly MemberTypes Properties = MemberTypes.Property;
        public static readonly MemberTypes Fields = MemberTypes.Field;
        public static readonly MemberTypes Methods = MemberTypes.Method;
        public static readonly MemberTypes Types = MemberTypes.TypeInfo | MemberTypes.NestedType;
    }

    #endregion

    #region [-- ReadWriteConstants --]

    internal enum ReadWriteFlags
    {
        Any = 0,
        CanRead = 1,
        CanWrite = 2,
        CanReadAndWrite = 3,
    }


    #endregion

    #region [-- MemberInfo Extensions --]

    internal static class _MemberInfoExtensions
    {
        public static bool IsDefined<T>(this MemberInfo member) where T : Attribute
        {
            return member.IsDefined(typeof(T));
        }
        public static bool IsDefined<T>(this MemberInfo member, bool inherit) where T : Attribute
        {
            return member.IsDefined(typeof(T), inherit);
        }
        public static string GetName(this MemberInfo member)
        {
            if(member is Type)
                return ((Type)member).GetName();

            return member?.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ?? member?.Name;
        }
        public static string GetFullName(this MemberInfo member)
        {
            if(member.IsType())
                return ((Type)member).GetFullName();

            var declaringTypeName = member.DeclaringType?.GetFullName();

            if(declaringTypeName != null)
                return $"{declaringTypeName}.{member.Name}";

            return member.Name;
        }


        public static Type GetValueType(this MemberInfo member)
        {
            if(member != null)
            {
                switch(member.MemberType)
                {
                    case MemberTypes.Property:
                        return ((PropertyInfo)member).PropertyType;

                    case MemberTypes.Field:
                        return ((FieldInfo)member).FieldType;

                    case MemberTypes.Method:
                        return ((MethodInfo)member).ReturnType;

                    case MemberTypes.TypeInfo:
                        return ((Type)member);
                }

            }

            return null;
        }
        public static object GetValue(this MemberInfo member, object instance)
        {
            var propInfo = member as PropertyInfo;
            if(propInfo != null)
                return propInfo.GetValue(instance);

            var fieldInfo = member as FieldInfo;
            if(fieldInfo != null)
                return fieldInfo.GetValue(instance);

            throw new ArgumentException(nameof(member));
        }
        public static void SetValue(this MemberInfo member, object instance, object value)
        {
            var propInfo = member as PropertyInfo;
            var fieldInfo = member as FieldInfo;

            if(propInfo != null)
                propInfo.SetValue(instance, Convert.ChangeType(value, propInfo.PropertyType));

            else if(fieldInfo != null)
                fieldInfo.SetValue(instance, Convert.ChangeType(value, fieldInfo.FieldType));

            else
                throw new Exception(nameof(member));
        }

        public static bool CanWrite(this MemberInfo member)
        {
            switch(member.MemberType)
            {
                case MemberTypes.Property:
                    return ((PropertyInfo)member).SetMethod != null;

                case MemberTypes.Field:
                    return !((FieldInfo)member).IsInitOnly;

                default:
                    return false;
            }
        }
        public static bool CanRead(this MemberInfo member)
        {
            switch(member.MemberType)
            {
                case MemberTypes.Property:
                    return ((PropertyInfo)member).GetMethod != null;

                default:
                    return true;
            }
        }
        public static bool IsPublic(this MemberInfo member)
        {
            if(member.IsField())
                return ((FieldInfo)member).IsPublic;

            else if(member.IsProperty())
                return (((PropertyInfo)member).GetMethod != null && ((PropertyInfo)member).GetMethod.IsPublic);

            else if(member.IsMethod())
                return ((MethodInfo)member).IsPublic;

            else if(member.IsNestedType())
                return ((Type)member).IsPublic;

            return false;
        }
        public static bool IsPrivate(this MemberInfo member)
        {
            if(member.IsField())
                return ((FieldInfo)member).IsPrivate;

            else if(member.IsProperty())
                return (((PropertyInfo)member).GetMethod != null && ((PropertyInfo)member).GetMethod.IsPrivate);

            else if(member.IsMethod())
                return ((MethodInfo)member).IsPrivate;

            return false;
        }
        public static bool IsType(this MemberInfo member)
        {
            return (MemberTypeConstants.Types & member.MemberType) == member.MemberType;
        }
        public static bool IsEvent(this MemberInfo member)
        {
            return (MemberTypes.Event & member.MemberType) == member.MemberType;
        }
        public static bool IsField(this MemberInfo member)
        {
            return (MemberTypes.Field & member.MemberType) == member.MemberType;
        }
        public static bool IsMethod(this MemberInfo member)
        {
            return (MemberTypes.Method & member.MemberType) == member.MemberType;
        }
        public static bool IsProperty(this MemberInfo member)
        {
            return (MemberTypes.Property & member.MemberType) == member.MemberType;
        }
        public static bool IsStatic(this MemberInfo member)
        {
            switch(member.MemberType)
            {
                case MemberTypes.Field:
                    return ((FieldInfo)member).IsStatic;

                case MemberTypes.Property:
                    return (((PropertyInfo)member).GetMethod != null && ((PropertyInfo)member).GetMethod.IsStatic) ||
                           (((PropertyInfo)member).SetMethod != null && ((PropertyInfo)member).SetMethod.IsStatic);

                case MemberTypes.Method:
                    return ((MethodInfo)member).IsStatic;

                case MemberTypes.TypeInfo:
                    return ((Type)member).IsAbstract && ((Type)member).IsSealed;
            }

            return false;
        }
        public static bool IsNestedType(this MemberInfo member)
        {
            return (MemberTypes.NestedType & member.MemberType) == member.MemberType;
        }
        public static bool IsPropertyOrField(this MemberInfo member)
        {
            return (MemberTypeConstants.PropertiesAndFields & member.MemberType) == member.MemberType;
        }
        public static bool IsPropertyFieldOrMethod(this MemberInfo member)
        {
            return (MemberTypeConstants.PropertiesFieldsAndMethods & member.MemberType) == member.MemberType;
        }

        public static Type GetEffectiveDeclaringType(this MethodInfo method)
        {
            if(method.IsStatic && method.IsDefined<ExtensionAttribute>(false) && method.GetParameters().Any())
                return method.GetParameters().First().ParameterType;

            return method.DeclaringType;
        }
        public static string GetParameterSignature(this MethodInfo method)
        {
            string genericArgs = string.Empty;

            if(method.ContainsGenericParameters)
            {
                foreach(var genArg in method.GetGenericArguments())
                {
                    if(!string.IsNullOrEmpty(genericArgs))
                        genericArgs += ", ";

                    genericArgs += genArg.Name;
                }

                genericArgs = string.Format("<{0}>", genericArgs);
            }

            string parameters = string.Empty;

            foreach(var param in method.GetParameters())
            {
                if(!string.IsNullOrEmpty(parameters))
                    parameters += ", ";

                var modifier = string.Empty;

                if(param.ParameterType.IsArray && param.GetCustomAttribute<ParamArrayAttribute>() != null)
                    modifier += "params ";

                if(param.IsOptional)
                    parameters += string.Format("[{0}{1} {2}]", modifier, param.ParameterType.GetName(), param.Name);
                else
                    parameters += string.Format("{0}{1} {2}", modifier, param.ParameterType.GetName(), param.Name);
            }

            return string.Format("{0}({1})", genericArgs, parameters);
        }
        public static string GetFullSignature(this MethodInfo method)
        {
            return string.Format("{0}{1} {2}{3}", method.IsStatic ? "static " : "", method.ReturnType.GetName(), method.Name, method.GetParameterSignature());
        }

        public static bool IsParamArray(this ParameterInfo parameter)
        {
            return
               parameter.ParameterType.IsArray &&
               parameter.GetCustomAttribute<ParamArrayAttribute>() != null;
        }
    }

    #endregion
}
