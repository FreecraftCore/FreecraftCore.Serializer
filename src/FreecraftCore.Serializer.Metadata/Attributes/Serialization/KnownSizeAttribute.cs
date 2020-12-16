using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Meta-data attribute that can be used to mark wire serializable members that have known size (collections with static sizes) to prevent the serializer
	/// from writing their length or size into the message.
	/// Consult (Blizzard's JAM for information on the concept).
	/// </summary>
	[SerializationAttribute]
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)] //members can be fields or props
	public sealed class KnownSizeAttribute : Attribute
	{
		/// <summary>
		/// Indicates the known size/length of the member.
		/// </summary>
		public int KnownSize { get; }

		public KnownSizeAttribute(int size)
		{
			if (size < 0)
				throw new ArgumentException($"Provided argument {nameof(size)} must be positive.", nameof(size));

			KnownSize = size;
		}

		internal static int Parse(params string[] args)
		{
			if (args.Length != 1)
				throw new InvalidOperationException($"{nameof(KnownSizeAttribute)} handling must be updated.");

			return int.Parse(args[0]);
		}
	}
}
