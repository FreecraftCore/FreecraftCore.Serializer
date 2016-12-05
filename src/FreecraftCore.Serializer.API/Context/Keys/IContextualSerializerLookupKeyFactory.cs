using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FreecraftCore.Serializer
{
	public interface IContextualSerializerLookupKeyFactory
	{
		/// <summary>
		/// Tries to create a <see cref="ContextualSerializerLookupKey"/> from the provided
		/// <see cref="MemberInfo"/>.
		/// </summary>
		/// <param name="memberInfo">Member info of the context.</param>
		/// <returns>A valid <see cref="ContextualSerializerLookupKey"/> or null.</returns>
		ContextualSerializerLookupKey Create(MemberInfo memberInfo);

		/// <summary>
		/// Tries to create a <see cref="ContextualSerializerLookupKey"/> from the provided
		/// <see cref="ISerializableTypeContext"/>.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <returns>A valid <see cref="ContextualSerializerLookupKey"/> or null.</returns>
		ContextualSerializerLookupKey Create(ISerializableTypeContext context);
	}
}
