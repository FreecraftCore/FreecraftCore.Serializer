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
		public static bool HasAttribute<TAttributeType>(this ISerializableTypeContext context)
			where TAttributeType : Attribute
		{
			return context.Metadata.Where(x => x.GetType() == typeof(TAttributeType)).Count() != 0;
		}

		/// <summary>
		/// Indicates if the context contains a specific type of metadata.
		/// </summary>
		/// <typeparam name="TAttributeType">The <see cref="Attribute"/> to look for.</typeparam>
		/// <param name="context">Context.</param>
		/// <returns>True if the context has the <typeparamref name="TAttributeType"/>.</returns>
		public static TAttributeType GetAttribute<TAttributeType>(this ISerializableTypeContext context)
			where TAttributeType : Attribute
		{
			return context.Metadata.FirstOrDefault(x => x.GetType() == typeof(TAttributeType)) as TAttributeType;
		}
	}
}
