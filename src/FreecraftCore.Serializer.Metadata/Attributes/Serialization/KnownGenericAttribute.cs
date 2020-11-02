using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Attribute that marks a generic serializable type
	/// as having a specific known/forward declared closed generic Type.
	/// (Ex. Type{T} having a type T of TypeStub being declared with [KnownGeneric(typeof(TypeStub))]
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class KnownGenericAttribute : Attribute
	{
		public Type[] GenericTypeParameters { get; }

		public KnownGenericAttribute([NotNull] params Type[] genericTypeParameters)
		{
			GenericTypeParameters = genericTypeParameters ?? throw new ArgumentNullException(nameof(genericTypeParameters));
			if(genericTypeParameters.Length == 0) throw new ArgumentException("Value cannot be an empty collection.", nameof(genericTypeParameters));
		}
	}
}
