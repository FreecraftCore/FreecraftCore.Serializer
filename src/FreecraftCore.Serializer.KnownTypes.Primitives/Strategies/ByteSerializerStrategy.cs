using System;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace FreecraftCore.Serializer.KnownTypes
{
	/// <summary>
	/// <see cref="ITypeSerializerStrategy"/> for Type <see cref="byte"/>.
	/// </summary>
	[KnownTypeSerializer]
	public class ByteSerializerStrategy : SimpleTypeSerializerStrategy<byte>
	{
		//All primitive serializer stragies are contextless
		/// <inheritdoc />
		public override SerializationContextRequirement ContextRequirement { get; } = SerializationContextRequirement.Contextless;

		/// <inheritdoc />
		public override void Write(byte value, IWireStreamWriterStrategy dest)
		{
			if (dest == null) throw new ArgumentNullException(nameof(dest));

			//Similar to read, this is simple; just write a byte
			dest.Write(value);
		}

		/// <inheritdoc />
		public override byte Read(IWireStreamReaderStrategy source)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));

			//This is a pretty simple request; just read a byte
			return source.ReadByte();
		}

		public override byte[] GetBytes(byte obj)
		{
			return new byte[] { obj };
		}

		public override byte FromBytes([NotNull] byte[] bytes)
		{
			if (bytes == null) throw new ArgumentNullException(nameof(bytes));

			return bytes.First();
		}

		/// <inheritdoc />
		public override async Task WriteAsync(byte value, [NotNull] IWireStreamWriterStrategyAsync dest)
		{
			if (dest == null) throw new ArgumentNullException(nameof(dest));

			await dest.WriteAsync(value);
		}

		/// <inheritdoc />
		public override async Task<byte> ReadAsync([NotNull] IWireStreamReaderStrategyAsync source)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));

			return await source.ReadByteAsync();
		}
	}
}
