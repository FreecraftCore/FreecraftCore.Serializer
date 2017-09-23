using FreecraftCore.Serializer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;


namespace FreecraftCore.Serializer.KnownTypes
{
	/// <summary>
	/// Known-type serializer for the <see cref="bool"/> value-type.
	/// </summary>
	[KnownTypeSerializer]
	public class BoolSerializerStrategy : SimpleTypeSerializerStrategy<bool>
	{
		//All primitive serializer stragies are contextless
		/// <inheritdoc />
		public override SerializationContextRequirement ContextRequirement { get; } = SerializationContextRequirement.Contextless;

		//Trinitycore Bytebuffer implementation
		/*ByteBuffer &operator>>(bool &value)
		{
			value = read<char>() > 0 ? true : false;
			return *this;
		}*/

		/// <inheritdoc />
		public override bool Read(IWireStreamReaderStrategy source)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));

			//Trinitycore could potentially send non-one bytes for a bool
			//See above
			return source.ReadByte() > 0;
		}

		/// <inheritdoc />
		public override void Write(bool value, IWireStreamWriterStrategy dest)
		{
			if (dest == null) throw new ArgumentNullException(nameof(dest));

			dest.Write(ConvertFromBool(value));
		}

		public override byte[] GetBytes(bool obj)
		{
			return new byte[] { ConvertFromBool(obj) };
		}

		public override bool FromBytes([NotNull] byte[] bytes)
		{
			if (bytes == null) throw new ArgumentNullException(nameof(bytes));

			return bytes.First() > 0;
		}

		/// <summary>
		/// Converts a byte to the boolean representation.
		/// (based on Trinitycore's ByteBuffer.h)
		/// </summary>
		/// <param name="b"></param>
		/// <returns></returns>
		private bool ConvertFromByte(byte b)
		{
			return b > 0;
		}

		/// <summary>
		/// Converts a boolean value to the byte representation.
		/// (based on Trinitycore's ByteBuffer.h)
		/// </summary>
		/// <param name="b"></param>
		/// <returns></returns>
		private byte ConvertFromBool(bool b)
		{
			return (byte) (b ? 1 : 0);
		}

		/// <inheritdoc />
		public override async Task WriteAsync(bool value, [NotNull] IWireStreamWriterStrategyAsync dest)
		{
			if (dest == null) throw new ArgumentNullException(nameof(dest));
			await dest.WriteAsync(ConvertFromBool(value));
		}

		/// <inheritdoc />
		public override async Task<bool> ReadAsync([NotNull] IWireStreamReaderStrategyAsync source)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));

			return ConvertFromByte(await source.ReadByteAsync());
		}
	}
}
