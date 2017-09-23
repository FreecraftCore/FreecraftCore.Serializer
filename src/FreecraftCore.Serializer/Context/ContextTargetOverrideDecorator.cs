using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using JetBrains.Annotations;

namespace FreecraftCore.Serializer
{
	//Use to override target type
	/// <summary>
	/// Decorator used to override the <see cref="TargetType"/> of the provided
	/// <see cref="ISerializableTypeContext"/>.
	/// </summary>
	public class ContextTargetOverrideDecorator : ISerializableTypeContext
	{
		/// <inheritdoc />
		public ContextualSerializerLookupKey? BuiltContextKey
		{
			get
			{
				return managedContext.BuiltContextKey;
			}

			set
			{
				managedContext.BuiltContextKey = value;
			}
		}

		public SerializationContextRequirement ContextRequirement => managedContext.ContextRequirement;

		//Don't do null checks. Null check is enforced in ctor
		/// <inheritdoc />
		public IEnumerable<Attribute> MemberMetadata => managedContext.MemberMetadata;

		/// <inheritdoc />
		public Type TargetType { get; }

		/// <inheritdoc />
		public IEnumerable<Attribute> TypeMetadata => managedContext.TypeMetadata;

		[NotNull]
		private ISerializableTypeContext managedContext { get; }

		public ContextTargetOverrideDecorator([NotNull] ISerializableTypeContext context, [NotNull] Type targetType)
		{
			if (context == null) throw new ArgumentNullException(nameof(context));
			if (targetType == null) throw new ArgumentNullException(nameof(targetType));

			managedContext = context;
			TargetType = targetType;
		}
	}
}
