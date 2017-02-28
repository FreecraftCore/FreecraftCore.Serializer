using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using JetBrains.Annotations;
using Reflect.Extent;


namespace FreecraftCore.Serializer
{
	//TODO: Refactor out of API project
	public class MemberInfoBasedSerializationContext : ISerializableTypeContext
	{
		/// <inheritdoc />
		public SerializationContextRequirement ContextRequirement { get; }

		/// <inheritdoc />
		public IEnumerable<Attribute> MemberMetadata { get; }

		/// <inheritdoc />
		public IEnumerable<Attribute> TypeMetadata { get; }

		/// <inheritdoc />
		public Type TargetType { get; }

		/// <inheritdoc />
		public ContextualSerializerLookupKey? BuiltContextKey { get; set; }

		public MemberInfoBasedSerializationContext([NotNull] MemberInfo memberInfoContext)
		{
			if (memberInfoContext == null)
				throw new ArgumentNullException(nameof(memberInfoContext), $"Provided member {nameof(memberInfoContext)} is null.");

			//From the member info we can grab the Type and the context related to it (if any)
			TargetType = memberInfoContext.Type();

			//we should search for known metadata; ignore stuff like WireMember. It doesn't affect serialization of the Type. Just order.
			MemberMetadata = memberInfoContext.GetCustomAttributes<Attribute>()
				.Where(IsContextualMemberAttribute);

			//Now we build the metadata for the Type itself. This could be some additional serialization information or it could be
			//Information about potential subtypes
			TypeMetadata = memberInfoContext.GetCustomAttributes<Attribute>()
				.Where(IsContextualTypeAttribute);

			//If there is any metadata in the member metdata then this serialization will require context.
			ContextRequirement = MemberMetadata.Any() ? SerializationContextRequirement.RequiresContext : SerializationContextRequirement.Contextless;
		}

		[Pure]
		private static bool IsContextualTypeAttribute(Attribute attri)
		{
			//We're interested in subtype metadata for Type contexts. Nothing else really.
			return attri.GetType() == typeof(WireDataContractBaseTypeAttribute) || attri.GetType() == typeof(WireDataContractBaseTypeByFlagsAttribute);
		}

		[Pure]
		private static bool IsContextualMemberAttribute(Attribute attri)
		{
			//TODO: Why does this class decide what is contextual? This is bad.
			//We don't check WireDataContractBaseType because that isn't part of the context of the member. That's context on the Type.
			//When registering a complex type a decorator may be available to handle that case but it's not relevant to the serialization context.
			return attri.GetType() == typeof(CompressAttribute) || attri.GetType() == typeof(KnownSizeAttribute) || attri.GetType() == typeof(EnumStringAttribute)
				|| attri.GetType() == typeof(SendSizeAttribute) || attri.GetType() == typeof(ReverseDataAttribute) //decided to add reverse data. It does require context because it decorates other serializers
				|| attri.GetType() == typeof(DontTerminateAttribute); //Dont terminate really shouldn't need to be contextual but at this point I'm abusing the system. We forgo purity for ease of implementation. We pay a slight memory penalty though.
		}
	}
}
