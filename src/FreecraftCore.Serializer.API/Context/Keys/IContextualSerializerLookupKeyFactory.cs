using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using JetBrains.Annotations;


namespace FreecraftCore.Serializer
{
	public interface IContextualSerializerLookupKeyFactory
	{
		//TODO: Since they returns can't be null we need a way to check if a key can be made.

		/// <summary>
		/// Tries to create a <see cref="ContextualSerializerLookupKey"/> from the provided
		/// <see cref="MemberInfo"/>.
		/// </summary>
		/// <param name="memberInfo">Member info of the context.</param>
		/// <returns>A valid <see cref="ContextualSerializerLookupKey"/> or null.</returns>
		ContextualSerializerLookupKey Create([NotNull] MemberInfo memberInfo);

		/// <summary>
		/// Tries to create a <see cref="ContextualSerializerLookupKey"/> from the provided
		/// <see cref="ISerializableTypeContext"/>.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <returns>A valid <see cref="ContextualSerializerLookupKey"/> or null.</returns>
		ContextualSerializerLookupKey Create([NotNull] ISerializableTypeContext context);
	}
}
