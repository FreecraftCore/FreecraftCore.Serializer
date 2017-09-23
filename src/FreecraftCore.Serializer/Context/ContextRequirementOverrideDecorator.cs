using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using JetBrains.Annotations;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Decorator used to override the <see cref="ContextRequirement"/> value.
	/// </summary>
	public class ContextRequirementOverrideDecorator : ISerializableTypeContext
	{
		/// <summary>
		/// Intenrally managed context.
		/// </summary>
		[NotNull]
		private ISerializableTypeContext managedContext { get; }

		/// <inheritdoc />
		public SerializationContextRequirement ContextRequirement { get; }

		/// <inheritdoc />
		public IEnumerable<Attribute> MemberMetadata => managedContext.MemberMetadata;

		/// <inheritdoc />
		public IEnumerable<Attribute> TypeMetadata => managedContext.TypeMetadata;

		/// <inheritdoc />
		public Type TargetType => managedContext.TargetType;

		//TODO: Should we block set for this?
		/// <inheritdoc />
		public ContextualSerializerLookupKey? BuiltContextKey { get; set; }

		public ContextRequirementOverrideDecorator([NotNull] ISerializableTypeContext context,
			SerializationContextRequirement requirement)
		{
			if (context == null) throw new ArgumentNullException(nameof(context));

			if (!Enum.IsDefined(typeof(SerializationContextRequirement), requirement))
				throw new ArgumentOutOfRangeException(nameof(requirement), "Value should be defined in the SerializationContextRequirement enum.");

			/*if (!Enum.IsDefined(typeof(SerializationContextRequirement), requirement))
				throw new InvalidEnumArgumentException(nameof(requirement), (int) requirement,
					typeof(SerializationContextRequirement));*/

			managedContext = context;
			ContextRequirement = requirement;
		}
	}
}