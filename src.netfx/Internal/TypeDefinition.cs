using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Simplify.Internal
{
    /// <summary>
    /// Describes a type and its members.
    /// </summary>
    internal class TypeDefinition
    {
        private static readonly ConcurrentDictionary<Type, TypeDefinition> typeDefinitionCache = new ConcurrentDictionary<Type, TypeDefinition>();

        /// <summary>
        /// Gets the type definition that describes the specified <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The <see cref="TypeDefinition"/> instance that describes the specified <paramref name="type"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="type"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="type"/> is a static type.</exception>
        public static TypeDefinition GetTypeDefinition(Type type)
        {
            if(type == null)
                throw new ArgumentNullException(nameof(type));

            else if(type.IsStatic())
                throw new ArgumentException("Type definitions cannot be created for static types.");

            return typeDefinitionCache.GetOrAdd(type, (t) => new TypeDefinition(t));
        }

        /// <summary>
        /// Gets the members of the specified type that eligible to be bound for mapping.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        /// An array of <see cref="MemberInfo" /> objects that describe the members of the specified <paramref name="type" /> that are eligible for binding.
        /// </returns>
        /// <remarks>
        /// <para>This methods returns <see cref="MethodInfo"/> objects that are framed in the context of the type in which they are declared.</para>
        /// <para>So, for example, if member M is declared in type A as <code>public property M { get; private set }</code> and <paramref name="type"/> is of 
        /// type B (which is a subclass of type A), the MemberInfo returned for member M will be the context of type A.</para>  <para>This is to ensure that the setter method 
        /// for member M is visible via reflection.</para>
        /// </remarks>
        public static MemberInfo[] GetEligibleMembers(Type type)
        {
            var memberTypes = MemberTypeConstants.PropertiesAndFields;

            var bindingFlags = BindingFlagConstants.PublicAndNonPublicInstance;

            var predicate = new Func<MemberInfo, bool>((m) => !m.IsDefined<CompilerGeneratedAttribute>());

            var members = type.GetMembers(memberTypes, bindingFlags, true).Where(predicate);

            members = members.Select(m => m.DeclaringType == type ? m : m.DeclaringType.GetMember(m.Name, memberTypes, bindingFlags).Single());

            return members.ToArray();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeDefinition"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <exception cref="ArgumentNullException">type is null.</exception>
        public TypeDefinition(Type type)
        {
            if(type == null)
                throw new ArgumentNullException(nameof(type));

            this.Type = type;

            var eligibleMembers = GetEligibleMembers(type);

            var memberDefinitionList = eligibleMembers.Select(m => new MemberDefinition(type, m)).ToList();

            this.MemberDefinitions = new ReadOnlyCollection<MemberDefinition>(memberDefinitionList);
        }

        /// <summary>
        /// Gets the type described by this instance.
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        /// Gets the member definitions declared or inherited by this type.
        /// </summary>
        public ReadOnlyCollection<MemberDefinition> MemberDefinitions { get; private set; }
    }
}