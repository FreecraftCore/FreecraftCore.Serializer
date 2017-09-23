using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using JetBrains.Annotations;


namespace FreecraftCore.Serializer
{
	//TODO: Refactor out of API project
	public class TypeBasedSerializationContext : ISerializableTypeContext
	{
		/// <inheritdoc />
		public SerializationContextRequirement ContextRequirement { get; }

		/// <inheritdoc />
		public IEnumerable<Attribute> MemberMetadata { get; }

		/// <inheritdoc />
		public IEnumerable<Attribute> TypeMetadata { get; }

		/// <inheritdoc />
		public Type TargetType { get; }

		public ContextualSerializerLookupKey? BuiltContextKey { get; set; }

		public TypeBasedSerializationContext([NotNull] Type type)
		{
			if (type == null)
				throw new ArgumentNullException(nameof(type), $"Provided member {nameof(type)} is null.");

			//Unlike with memberinfo serialization we know right away there is no context here. We ONLY know about the type so there can be no context
			ContextRequirement = SerializationContextRequirement.Contextless;
			TargetType = type;

			TypeMetadata = type.GetTypeInfo().GetCustomAttributes<Attribute>()
				.Where(IsContextualTypeAttribute);

			//Provide an empty member context. There is none to provide
			MemberMetadata = Enumerable.Empty<Attribute>();
		}

		[Pure]
		private static bool IsContextualTypeAttribute(Attribute attri)
		{
			//We're interested in subtype metadata for Type contexts. Nothing else really.
			return attri.GetType() == typeof(WireDataContractBaseTypeAttribute);
		}
	}
}
