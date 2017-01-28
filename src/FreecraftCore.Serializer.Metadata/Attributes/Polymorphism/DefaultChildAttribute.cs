using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Indicates the default child to deserialize if no flags were found.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
	public class DefaultChildAttribute : Attribute
	{
		/// <summary>
		/// Indicates the default child type to deserialize to.
		/// </summary>
		[NotNull]
		public Type ChildType { get; }

		public DefaultChildAttribute([NotNull] Type childType)
		{
			if (childType == null)
				throw new ArgumentNullException(nameof(childType), $"When marking a type with {nameof(DefaultChildAttribute)} you must provided a valid non-null Type.");

			ChildType = childType;
		}
	}
}
