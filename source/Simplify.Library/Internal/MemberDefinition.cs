using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Simplify.Internal
{
    /// <summary>
    /// Describes a property or field of type.
    /// </summary>
    internal class MemberDefinition
    {
        #region [-- MemberSetter/MemberGetter --]

        #region [-- MemberSetter --]

        private abstract class MemberSetter
        {
            public abstract void Set(object target, object value);
        }
        private abstract class MemberSetter<TTarget> : MemberSetter where TTarget : class
        {
            public abstract void Set(TTarget target, object value);
        }
        private class MemberSetter<TTarget, TValue> : MemberSetter<TTarget> where TTarget : class
        {
            private Action<TTarget, TValue> setterDelegate;

            public MemberSetter(MemberInfo member)
            {
                if(member == null)
                    throw new ArgumentNullException(nameof(member));

                if(member.MemberType == MemberTypes.Property)
                {
                    var propInfo = member as PropertyInfo;

                    var targetExpr = Expression.Parameter(typeof(TTarget));
                    var valueExpr = Expression.Parameter(typeof(TValue));
                    var expr = Expression.Call(targetExpr, propInfo.SetMethod, valueExpr);

                    this.setterDelegate = Expression.Lambda<Action<TTarget, TValue>>(expr, targetExpr, valueExpr).Compile();
                }
                else if(member.MemberType == MemberTypes.Field)
                {
                    var fieldInfo = member as FieldInfo;

                    var targetExpr = Expression.Parameter(typeof(TTarget), "target");
                    var valueExpr = Expression.Parameter(typeof(TValue), "value");
                    var fieldExpr = Expression.Field(targetExpr, fieldInfo);

                    var expr = Expression.Assign(fieldExpr, valueExpr);

                    this.setterDelegate = Expression.Lambda<Action<TTarget, TValue>>(expr, targetExpr, valueExpr).Compile();
                }
                else
                    throw new ArgumentException(nameof(member), $"{member.MemberType} cannot be mapped.");
            }

            public void SetValue(TTarget target, TValue value)
            {
                this.setterDelegate(target, value);
            }

            public override void Set(TTarget target, object value)
            {
                this.setterDelegate(target, (TValue)value);
            }

            public override void Set(object target, object value)
            {
                this.setterDelegate((TTarget)target, (TValue)value);
            }
        }

        #endregion

        #region [-- MemberGetter --]

        private abstract class MemberGetter
        {
            public abstract object Get(object target);
        }
        private abstract class MemberGetter<TTarget> : MemberGetter where TTarget : class
        {
            public abstract object Get(TTarget target);
        }
        private class MemberGetter<TTarget, TValue> : MemberGetter<TTarget> where TTarget : class
        {
            private Func<TTarget, TValue> getterDelegate;

            public MemberGetter(MemberInfo member)
            {
                if(member == null)
                    throw new ArgumentNullException(nameof(member));

                if(member.MemberType == MemberTypes.Property)
                {
                    var propInfo = member as PropertyInfo;

                    var targetExpr = Expression.Parameter(typeof(TTarget));
                    var expr = Expression.Call(targetExpr, propInfo.GetMethod);

                    this.getterDelegate = Expression.Lambda<Func<TTarget, TValue>>(expr, targetExpr).Compile();
                }
                else if(member.MemberType == MemberTypes.Field)
                {
                    var fieldInfo = member as FieldInfo;

                    var targetExpr = Expression.Parameter(typeof(TTarget));

                    var expr = Expression.Field(targetExpr, fieldInfo);

                    this.getterDelegate = Expression.Lambda<Func<TTarget, TValue>>(expr, targetExpr).Compile();
                }
                else
                    throw new ArgumentException(nameof(member), $"{member.MemberType} cannot be mapped.");
            }

            public TValue GetValue(TTarget target)
            {
                return this.getterDelegate(target);
            }

            public override object Get(TTarget target)
            {
                return (TValue)this.getterDelegate(target);
            }

            public override object Get(object target)
            {
                return (TValue)this.getterDelegate((TTarget)target);
            }
        }

        #endregion

        #endregion

        private MemberSetter setter;
        private MemberGetter getter;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemberDefinition" /> class.
        /// </summary>
        /// <param name="objectType">Type of the object for which <paramref name="memberInfo" /> is a member.</param>
        /// <param name="memberInfo">The member information.</param>
        /// <exception cref="ArgumentNullException">
        /// objectType is null.
        /// -or-
        /// memberInfo is null.</exception>
        public MemberDefinition(Type objectType, MemberInfo memberInfo)
        {
            if(objectType == null)
                throw new ArgumentNullException(nameof(objectType));

            else if(memberInfo == null)
                throw new ArgumentNullException(nameof(memberInfo));

            this.Type = memberInfo.GetValueType();
            this.ObjectType = objectType;
            this.DeclaringType = memberInfo.DeclaringType;
            this.IsProperty = memberInfo.IsProperty();
            this.IsField = memberInfo.IsField();
            this.IsPublic = memberInfo.IsPublic();
            this.IsPrivate = memberInfo.IsPrivate();
            this.IsProtected = !this.IsPublic && !this.IsPrivate;
            this.IsReadOnly = !memberInfo.CanWrite();
            this.IsWriteOnly = !memberInfo.CanRead();

            this.InitializeGetterAndSetter(memberInfo);
        }

        /// <summary>
        /// Gets the type of the member.
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        /// Gets the type of the object for which this is a member.
        /// </summary>
        public Type ObjectType { get; private set; }

        /// <summary>
        /// Gets the type in which this member is declared.
        /// </summary>
        /// <value>
        /// The type of the declaring.
        /// </value>
        public Type DeclaringType { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this member is a property.
        /// </summary>
        /// <value>
        /// <c>true</c> if this member is a property; otherwise, <c>false</c>.
        /// </value>
        public bool IsProperty { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this member is a field.
        /// </summary>
        /// <value>
        /// <c>true</c> if this member is a field; otherwise, <c>false</c>.
        /// </value>
        public bool IsField { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this member is private.
        /// </summary>
        /// <value>
        /// <c>true</c> if this member is private; otherwise, <c>false</c>.
        /// </value>
        public bool IsPrivate { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this member is potected.
        /// </summary>
        /// <value>
        /// <c>true</c> if this member is protected; otherwise, <c>false</c>.
        /// </value>
        public bool IsProtected { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this member is public.
        /// </summary>
        /// <value>
        /// <c>true</c> if this member is public; otherwise, <c>false</c>.
        /// </value>
        public bool IsPublic { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this member is read-only.
        /// </summary>
        /// <value>
        /// <c>true</c> if this member is read-only; otherwise, <c>false</c>.
        /// </value>
        public bool IsReadOnly { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this member is write-only.
        /// </summary>
        /// <value>
        /// <c>true</c> if this member is write-only; otherwise, <c>false</c>.
        /// </value>
        public bool IsWriteOnly { get; private set; }

        /// <summary>
        /// Initializes the getter and setter delegates for this method.
        /// </summary>
        /// <param name="memberInfo">The <see cref="MemberInfo"/> associated with this <see cref="MemberMappingInfo"/> instance.</param>
        private void InitializeGetterAndSetter(MemberInfo memberInfo)
        {
            if(!this.IsReadOnly)
            {
                var setterType = typeof(MemberSetter<,>).MakeGenericType(memberInfo.DeclaringType, this.Type);
                this.setter = (MemberSetter)Activator.CreateInstance(setterType, new object[] { memberInfo });
            }

            if(!this.IsWriteOnly)
            {
                var getterType = typeof(MemberGetter<,>).MakeGenericType(memberInfo.DeclaringType, this.Type);
                this.getter = (MemberGetter)Activator.CreateInstance(getterType, new object[] { memberInfo });
            }
        }
    }
}
