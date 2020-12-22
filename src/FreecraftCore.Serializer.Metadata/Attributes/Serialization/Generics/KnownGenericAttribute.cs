using System;
using System.Collections.Generic;
using System.Text;
using FreecraftCore.Serializer.Internal;
using JetBrains.Annotations;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Attribute that marks a generic serializable type
	/// as having a specific known/forward declared closed generic Type.
	/// (Ex. Type{T} having a type T of TypeStub being declared with [KnownGeneric(typeof(TypeStub))]
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public sealed class KnownGenericAttribute : Attribute
	{
		public Type[] GenericTypeParameters { get; }

		/// <summary>
		/// The generic parameters that created 
		/// </summary>
		/// <param name="genericTypeParameters"></param>
		public KnownGenericAttribute([NotNull] params Type[] genericTypeParameters)
		{
			GenericTypeParameters = genericTypeParameters ?? throw new ArgumentNullException(nameof(genericTypeParameters));
			if(genericTypeParameters.Length == 0) throw new ArgumentException("Value cannot be an empty collection.", nameof(genericTypeParameters));
		}
	}
}
