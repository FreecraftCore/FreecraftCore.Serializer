using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;


namespace FreecraftCore.Serializer.KnownTypes
{
	/// <summary>
	/// Serializer for the <see cref="BitArray"/> that is used in the bitmask/updatemask from Trinitycore
	/// </summary>
	public class BitArraySerializerStrategyDecorator : SimpleTypeSerializerStrategy<BitArray>
	{
		//A BitArray is contextless since it only serializes a type not special semantics on a member.
		/// <inheritdoc />
		public override SerializationContextRequirement ContextRequirement { get; } = SerializationContextRequirement.Contextless;

		public BitArraySerializerStrategyDecorator()
		{

		}

		/// <inheritdoc />
		public override BitArray Read(IWireStreamReaderStrategy source)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));

			//TODO: We should handle multiple types of sizes
			//WoW sends a byte for the block count and an int array. We use a surrogate to deserialize it
			byte size = source.ReadByte();

			//Load the data for the bitmask
			//WoW sends the size as an int array but we can more efficiently read a byte array so we pretend
			//It is the same
			return new BitArray(source.ReadBytes(size * sizeof(int)));
		}

		/// <inheritdoc />
		public override void Write([NotNull] BitArray value, IWireStreamWriterStrategy dest)
		{
			if (value == null) throw new ArgumentNullException(nameof(value));
			if (dest == null) throw new ArgumentNullException(nameof(dest));

			//The size must be equal to the length divided by 8 bits (1 byte) but we do not include the
			//remainder from a modular division. The reason for this is it's always sent as 4 byte chunks from
			//Trinitycore and the size is always in terms of an int array
			byte[] bitmask = new byte[value.Length / 8];

			((ICollection) value).CopyTo(bitmask, 0);

			byte size = (byte)(bitmask.Length / sizeof(int));

			//Write the size as if it were an int array first
			dest.Write(size);

			dest.Write(bitmask);
		}

		/// <inheritdoc />
		public override async Task WriteAsync([NotNull] BitArray value, IWireStreamWriterStrategyAsync dest)
		{
			if (value == null) throw new ArgumentNullException(nameof(value));
			if (dest == null) throw new ArgumentNullException(nameof(dest));

			//The size must be equal to the length divided by 8 bits (1 byte) but we do not include the
			//remainder from a modular division. The reason for this is it's always sent as 4 byte chunks from
			//Trinitycore and the size is always in terms of an int array
			byte[] bitmask = new byte[(value.Length / 8)];

			((ICollection)value).CopyTo(bitmask, 0);

			//Write the size as if it were an int array first
			await dest.WriteAsync((byte)(bitmask.Length / 4));

			await dest.WriteAsync(bitmask);
		}

		/// <inheritdoc />
		public override async Task<BitArray> ReadAsync([NotNull] IWireStreamReaderStrategyAsync source)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));

			//TODO: We should handle multiple types of sizes
			//WoW sends a byte for the block count and an int array. We use a surrogate to deserialize it
			byte size = await source.ReadByteAsync();

			//Load the data for the bitmask
			//WoW sends the size as an int array but we can more efficiently read a byte array so we pretend
			//It is the same
			return new BitArray(await source.ReadBytesAsync(size * sizeof(int)));
		}
	}
}
