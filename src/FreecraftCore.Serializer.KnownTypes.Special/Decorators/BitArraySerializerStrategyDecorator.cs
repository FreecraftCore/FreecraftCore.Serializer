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
	/// Serializer for the packed <see cref="DateTime"/> that is based on the packing implemented on Trinitycore.
	/// </summary>
	public class BitArraySerializerStrategyDecorator : SimpleTypeSerializerStrategy<BitArray>
	{
		//A BitArray is contextless since it only serializes a type not special semantics on a member.
		/// <inheritdoc />
		public override SerializationContextRequirement ContextRequirement { get; } = SerializationContextRequirement.Contextless;

		[NotNull]
		private ITypeSerializerStrategy<int[]> decoratedSerializer { get; }

		public BitArraySerializerStrategyDecorator([NotNull] ITypeSerializerStrategy<int[]> intArraySerializer)
		{
			if (intArraySerializer == null)
				throw new ArgumentNullException(nameof(intArraySerializer), $"Provided arg {intArraySerializer} is null.");

			decoratedSerializer = intArraySerializer;
		}

		/// <inheritdoc />
		public override BitArray Read(IWireStreamReaderStrategy source)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));
			
			//Load the data for the bitmask
			return new BitArray(decoratedSerializer.Read(source));
		}

		/// <inheritdoc />
		public override void Write(ref BitArray value, IWireStreamWriterStrategy dest)
		{
			if (dest == null) throw new ArgumentNullException(nameof(dest));
		}

		/// <inheritdoc />
		public override async Task WriteAsync(BitArray value, IWireStreamWriterStrategyAsync dest)
		{
			//pass to decorated serializer
			await Task.CompletedTask;
		}

		/// <inheritdoc />
		public override async Task<BitArray> ReadAsync(IWireStreamReaderStrategyAsync source)
		{
			//reads the packed int value from the stream
			return null;
		}
	}
}
