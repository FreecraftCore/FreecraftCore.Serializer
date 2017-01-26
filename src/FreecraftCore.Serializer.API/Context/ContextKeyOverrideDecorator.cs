using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;

namespace FreecraftCore.Serializer
{
	//Used to override the key value
	/// <summary>
	/// Decorator used to override the key value of a <see cref="ISerializableTypeContext"/>.
	/// </summary>
	public class ContextKeyOverrideDecorator : ISerializableTypeContext
	{
		/// <summary>
		/// Intenrally managed context.
		/// </summary>
		[NotNull]
		private ISerializableTypeContext managedContext { get; }

		/// <inheritdoc />
		public SerializationContextRequirement ContextRequirement => managedContext.ContextRequirement;

		/// <inheritdoc />
		public IEnumerable<Attribute> MemberMetadata => managedContext.MemberMetadata;

		/// <inheritdoc />
		public IEnumerable<Attribute> TypeMetadata => managedContext.TypeMetadata;

		/// <inheritdoc />
		public Type TargetType => managedContext.TargetType;

		//TODO: Should we block set for this?
		/// <inheritdoc />
		public ContextualSerializerLookupKey? BuiltContextKey { get; set; }

		public ContextKeyOverrideDecorator([NotNull] ISerializableTypeContext context, ContextualSerializerLookupKey key)
		{
			if (context == null) throw new ArgumentNullException(nameof(context));

			managedContext = context;

			BuiltContextKey = key;
		}
	}
}
