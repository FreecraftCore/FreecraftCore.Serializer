using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace FreecraftCore.Serializer.KnownTypes
{
	public class GenericChildKeyStrategy<TSizeType> : IChildKeyStrategy
		where TSizeType : struct
	{
		/// <summary>
		/// Indicates if the key should be consumed from the stream.
		/// </summary>
		private InformationHandlingFlags typeHandlingFlags { get; }

		[NotNull]
		private ITypeSerializerStrategy<TSizeType> SizTypeSerializerStrategy { get; }

		public GenericChildKeyStrategy(InformationHandlingFlags typeHandling, [NotNull] ITypeSerializerStrategy<TSizeType> sizTypeSerializerStrategy)
		{
			if (sizTypeSerializerStrategy == null) throw new ArgumentNullException(nameof(sizTypeSerializerStrategy));

			int i;

			if (!Enum.IsDefined(typeof(InformationHandlingFlags), typeHandling) && Int32.TryParse(typeHandling.ToString(), out i))
				throw new ArgumentOutOfRangeException(nameof(typeHandling), "Value should be defined in the InformationHandlingFlags enum.");
			/*int i;

			if (!Enum.IsDefined(typeof(InformationHandlingFlags), typeHandling) && Int32.TryParse(typeHandling.ToString(), out i))
				throw new InvalidEnumArgumentException(nameof(typeHandling), (int)typeHandling,
					typeof(InformationHandlingFlags));*/

			typeHandlingFlags = typeHandling;
			SizTypeSerializerStrategy = sizTypeSerializerStrategy;
		}

		/// <inheritdoc />
		public int Read(IWireStreamReaderStrategy source)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));

			//Read a byte from the stream; should be the byte sized child key
			return typeHandlingFlags.HasFlag(InformationHandlingFlags.DontConsumeRead)
				? source.PeekByte()
				: source.ReadByte();
		}

		/// <inheritdoc />
		public void Write(int value, IWireStreamWriterStrategy dest)
		{
			if (dest == null) throw new ArgumentNullException(nameof(dest));

			//If the key should be consumed then we should write one, to be consumed.
			//Otherwise if it's not then something in the stream will be read and then left in
			//meaning we need to write nothing
			if (!typeHandlingFlags.HasFlag(InformationHandlingFlags.DontWrite))
				dest.Write((byte)value); //Write the byte sized key to the stream.
		}

		/// <inheritdoc />
		public Task<int> ReadAsync(IWireStreamReaderStrategyAsync source)
		{
			throw new NotImplementedException();
		}

		/// <inheritdoc />
		public Task WriteAsync(int value, IWireStreamWriterStrategyAsync dest)
		{
			throw new NotImplementedException();
		}
	}
}
