using Fasterflect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;


namespace FreecraftCore.Serializer
{
	//TODO: Refactor out of API project
	public class TypeBasedSerializationContext : ISerializableTypeContext
	{
		/// <summary>
		/// Indicates if this context is unique to a member.
		/// </summary>
		public SerializationContextRequirement ContextRequirement { get; }

		/// <summary>
		/// The <see cref="FreecraftCore"/> attribute metadata associated with the type.
		/// (If the context isn't unique then the Metadata is for the <see cref="Type"/> and not from a <see cref="MemberInfo"/>)
		/// </summary>
		public IEnumerable<Attribute> MemberMetadata { get; }

		/// <summary>
		/// The <see cref="FreecraftCore"/> attribute metadata associated with the <see cref="Type"/>
		/// (Not all types have interesting metadata)
		/// </summary>
		public IEnumerable<Attribute> TypeMetadata { get; }

		/// <summary>
		/// Represents the type.
		/// </summary>
		public Type TargetType { get; }

		/// <summary>
		/// The conextual lookup key that should be associated with the serialization context.
		/// If null there is no context.
		/// </summary>
		public ContextualSerializerLookupKey? BuiltContextKey { get; set; }

		public TypeBasedSerializationContext(Type type)
		{
			if (type == null)
				throw new ArgumentNullException(nameof(type), $"Provided member {nameof(type)} is null.");

			//Unlike with memberinfo serialization we know right away there is no context here. We ONLY know about the type so there can be no context
			ContextRequirement = SerializationContextRequirement.Contextless;
			TargetType = type;

			TypeMetadata = type.Attributes<Attribute>()
				.Where(IsContextualTypeAttribute);

			//Provide an empty member context. There is none to provide
			MemberMetadata = Enumerable.Empty<Attribute>();
		}

		private bool IsContextualTypeAttribute(Attribute attri)
		{
			//We're interested in subtype metadata for Type contexts. Nothing else really.
			return attri.GetType() == typeof(WireMessageBaseTypeAttribute);
		}
	}
}
