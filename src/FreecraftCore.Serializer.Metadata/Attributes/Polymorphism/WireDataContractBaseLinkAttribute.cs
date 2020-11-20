using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Metadata marker for Types that are linked to their base type but allow for
	/// registration to be done at runtime.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false, Inherited = false)] //classes or structs can be WireDataContracts
	public class WireDataContractBaseLinkAttribute : Attribute
	{
		/// <summary>
		/// Unique index/key for the interpter to know what the base type is in the stream.
		/// </summary>
		public int Index { get; }

		/// <summary>
		/// Links to the basetype with the provided index at runtime.
		/// Will requite you to manually link with the serializer before compiling.
		/// </summary>
		/// <param name="index">Unique per Type index.</param>
		public WireDataContractBaseLinkAttribute(int index)
		{
			if (index < 0)
				throw new ArgumentException($"Provided wire child index is less than 0. Was: {index}.");

			Index = index;
		}
	}
}
