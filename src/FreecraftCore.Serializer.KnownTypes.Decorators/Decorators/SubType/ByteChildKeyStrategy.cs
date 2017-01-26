using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace FreecraftCore.Serializer.KnownTypes
{
	/// <summary>
	/// Strategy for reading and writing byte sized child keys from the stream.
	/// </summary>
	public class ByteChildKeyStrategy : IChildKeyStrategy
	{
		/// <summary>
		/// Indicates if the key should be consumed from the stream.
		/// </summary>
		private bool shouldConsumeKey { get; }

		public ByteChildKeyStrategy(bool consumeKey)
		{
			shouldConsumeKey = consumeKey;
		}

		/// <inheritdoc />
		public int Read(IWireMemberReaderStrategy source)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));

			//Read a byte from the stream; should be the byte sized child key
			return shouldConsumeKey ? source.ReadByte() : source.PeekByte();
		}

		/// <inheritdoc />
		public void Write(int value, IWireMemberWriterStrategy dest)
		{
			if (dest == null) throw new ArgumentNullException(nameof(dest));

			//If the key should be consumed then we should write one, to be consumed.
			//Otherwise if it's not then something in the stream will be read and then left in
			//meaning we need to write nothing
			if(shouldConsumeKey)
				dest.Write((byte)value); //Write the byte sized key to the stream.
		}
	}
}
