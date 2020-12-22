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
	/// Unlike <see cref="KnownGenericAttribute"/> this represents a complete generic type.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public sealed class ClosedGenericAttribute : Attribute
	{
		public Type GenericTypeParameter { get; }

		public ClosedGenericAttribute([NotNull] Type genericTypeParameter)
		{
			GenericTypeParameter = genericTypeParameter ?? throw new ArgumentNullException(nameof(genericTypeParameter));
		}
	}
}
