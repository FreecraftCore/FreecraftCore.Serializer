using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using JetBrains.Annotations;


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
		[Pure]
		public static bool HasMemberAttribute<TAttributeType>([NotNull] this ISerializableTypeContext context)
			where TAttributeType : Attribute
		{
			if (context == null) throw new ArgumentNullException(nameof(context));

			return context.MemberMetadata.Any(x => x.GetType() == typeof(TAttributeType));
		}

		/// <summary>
		/// Indicates if the context contains a specific type of metadata.
		/// </summary>
		/// <typeparam name="TAttributeType">The <see cref="Attribute"/> to look for.</typeparam>
		/// <param name="context">Context.</param>
		/// <exception cref="InvalidOperationException"></exception>
		/// <returns>True if the context has the <typeparamref name="TAttributeType"/>.</returns>
		[Pure]
		[NotNull]
		public static TAttributeType GetMemberAttribute<TAttributeType>([NotNull] this ISerializableTypeContext context)
			where TAttributeType : Attribute
		{
			if (context == null) throw new ArgumentNullException(nameof(context));

			TAttributeType attri = context.MemberMetadata.FirstOrDefault(x => x.GetType() == typeof(TAttributeType)) as TAttributeType;

			if (attri == null)
				throw new InvalidOperationException($"Requested Attribute Type {typeof(TAttributeType).FullName} was unavailable.");

			return attri;
		}

		//TODO: Doc
		[Pure]
		public static bool HasContextualMemberMetadata([NotNull] this ISerializableTypeContext context)
		{
			if (context == null) throw new ArgumentNullException(nameof(context));

			//If there is no context requirement then there is probably no context metadata or shouldn't be
			if (context.ContextRequirement != SerializationContextRequirement.RequiresContext)
				return false;

			return context.MemberMetadata.Any();
		}

		//TODO: Doc
		[Pure]
		public static bool HasContextualTypeMetadata([NotNull] this ISerializableTypeContext context)
		{
			if (context == null) throw new ArgumentNullException(nameof(context));

			//If there is no context requirement then there is probably no context metadata or shouldn't be
			if (context.ContextRequirement != SerializationContextRequirement.RequiresContext)
				return false;

			return context.TypeMetadata.Any();
		}

		/// <summary>
		/// Indicates if the <see cref="ISerializableTypeContext"/> has a valid contextual lookup key.
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		[Pure]
		public static bool HasContextualKey([NotNull] this ISerializableTypeContext context)
		{
			if (context == null) throw new ArgumentNullException(nameof(context));

			return context.BuiltContextKey.HasValue;
		}

		//TODO: Doc
		[Pure]
		[NotNull]
		public static ISerializableTypeContext Override([NotNull] this ISerializableTypeContext context, [NotNull] Type targetType)
		{
			if (context == null) throw new ArgumentNullException(nameof(context));
			if (targetType == null) throw new ArgumentNullException(nameof(targetType));

			//Decorate the context to override the target type
			return new ContextTargetOverrideDecorator(context, targetType);
		}

		//TODO: Doc
		[Pure]
		[NotNull]
		public static ISerializableTypeContext Override([NotNull] this ISerializableTypeContext context, ContextualSerializerLookupKey key)
		{
			if (context == null) throw new ArgumentNullException(nameof(context));

			//Deocrate the context to set the provided key
			return new ContextKeyOverrideDecorator(context, key);
		}

		//TODO: Doc
		[Pure]
		[NotNull]
		public static ISerializableTypeContext Override([NotNull] this ISerializableTypeContext context, SerializationContextRequirement requirement)
		{
			if (context == null) throw new ArgumentNullException(nameof(context));
			if (!Enum.IsDefined(typeof(SerializationContextRequirement), requirement))
				throw new ArgumentOutOfRangeException(nameof(requirement), "Value should be defined in the SerializationContextRequirement enum.");

			/*if (!Enum.IsDefined(typeof(SerializationContextRequirement), requirement))
				throw new InvalidEnumArgumentException(nameof(requirement), (int) requirement,
					typeof(SerializationContextRequirement));*/

			//Deocrate the context to set the provided key
			return new ContextRequirementOverrideDecorator(context, requirement);
		}
	}
}
