﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Indicates the default child to deserialize if no flags were found.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
	public class DefaultNoFlagsAttribute : Attribute
	{
		/// <summary>
		/// Indicates the default child type to deserialize to.
		/// </summary>
		public Type ChildType { get; }

		public DefaultNoFlagsAttribute(Type childType)
		{
			if (childType == null)
				throw new ArgumentNullException(nameof(childType), $"When marking a type with {nameof(DefaultNoFlagsAttribute)} you must provided a valid non-null Type.");

			ChildType = childType;
		}
	}
}
