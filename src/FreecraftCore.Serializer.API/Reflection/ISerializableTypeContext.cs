using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Context used to make serialization determiniations about a type.
	/// </summary>
	public interface ISerializableTypeContext
	{
		/// <summary>
		/// Indicates if this context is unique to a member.
		/// </summary>
		SerializationContextRequirement ContextRequirement { get; }

		/// <summary>
		/// The <see cref="FreecraftCore"/> attribute metadata associated with the member (if any).
		/// (If the context isn't unique then the Metadata is for the <see cref="Type"/> and not from a <see cref="MemberInfo"/>)
		/// </summary>
		IEnumerable<Attribute> MemberMetadata { get; }

		/// <summary>
		/// The <see cref="FreecraftCore"/> attribute metadata associated with the <see cref="Type"/>
		/// (Not all types have interesting metadata)
		/// </summary>
		IEnumerable<Attribute> TypeMetadata { get; }

		/// <summary>
		/// Represents the type.
		/// </summary>
		Type TargetType { get; }
	}
}
