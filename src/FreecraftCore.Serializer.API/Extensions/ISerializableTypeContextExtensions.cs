using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreecraftCore.Serializer
{
	public static class ISerializableTypeContextExtensions
	{
		/// <summary>
		/// Indicates if the context contains a specific type of metadata.
		/// </summary>
		/// <typeparam name="TAttributeType">The <see cref="Attribute"/> to look for.</typeparam>
		/// <param name="context">Context.</param>
		/// <returns>True if the context has the <typeparamref name="TAttributeType"/>.</returns>
		public static bool HasMemberAttribute<TAttributeType>(this ISerializableTypeContext context)
			where TAttributeType : Attribute
		{
			return context.MemberMetadata.Where(x => x.GetType() == typeof(TAttributeType)).Count() != 0;
		}

		/// <summary>
		/// Indicates if the context contains a specific type of metadata.
		/// </summary>
		/// <typeparam name="TAttributeType">The <see cref="Attribute"/> to look for.</typeparam>
		/// <param name="context">Context.</param>
		/// <returns>True if the context has the <typeparamref name="TAttributeType"/>.</returns>
		public static TAttributeType GetMemberAttribute<TAttributeType>(this ISerializableTypeContext context)
			where TAttributeType : Attribute
		{
			return context.MemberMetadata.FirstOrDefault(x => x.GetType() == typeof(TAttributeType)) as TAttributeType;
		}

		public static bool HasContextualMemberMetadata(this ISerializableTypeContext context)
		{
			//If there is no context requirement then there is probably no context metadata or shouldn't be
			if (context.ContextRequirement != SerializationContextRequirement.RequiresContext)
				return false;

			return context.MemberMetadata.Count() != 0;
		}

		public static bool HasContextualTypeMetadata(this ISerializableTypeContext context)
		{
			//If there is no context requirement then there is probably no context metadata or shouldn't be
			if (context.ContextRequirement != SerializationContextRequirement.RequiresContext)
				return false;

			return context.TypeMetadata.Count() != 0;
		}

		/// <summary>
		/// Indicates if the <see cref="ISerializableTypeContext"/> has a valid contextual lookup key.
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public static bool HasContextualKey(this ISerializableTypeContext context)
		{
			return context.BuiltContextKey.HasValue;
		}
	}
}
