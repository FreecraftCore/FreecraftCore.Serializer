using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace FreecraftCore.Serializer
{
	public class EndianReverseDecorator<TType> : SimpleTypeSerializerStrategy<TType>
	{
		public override SerializationContextRequirement ContextRequirement { get; } = SerializationContextRequirement.RequiresContext;

		/// <summary>
		/// Decorated serializer. For example. An enum Type : byte would have a ITypeSerializerStrategy{byte}.
		/// </summary>
		[NotNull]
		private ITypeSerializerStrategy<TType> SerializerStrategy { get; }

		public EndianReverseDecorator([NotNull] ITypeSerializerStrategy<TType> serializerStrategy)
		{
			if (serializerStrategy == null) throw new ArgumentNullException(nameof(serializerStrategy));

			SerializerStrategy = serializerStrategy;
		}

		/// <inheritdoc />
		public override TType Read(IWireStreamReaderStrategy source)
		{
			//To read a reversed chunk and interpret it as TType
			//We must decorate the stream with the semantics required to do this
			//We do not have access to the internals of the serializer to tell it how to treat the bytes it reads otherwise
			//Additionally we can do FromBytes because we don't know how many bytes the serializer wants
			return SerializerStrategy.Read(source.WithOneTimeReading().WithByteReversal());
		}

		/// <inheritdoc />
		public override void Write(TType value, IWireStreamWriterStrategy dest)
		{
			byte[] bytes = SerializerStrategy.GetBytes(value);
			Array.Reverse(bytes);
			dest.Write(bytes);
		}

		/// <inheritdoc />
		public override async Task WriteAsync(TType value, IWireStreamWriterStrategyAsync dest)
		{
			byte[] bytes = SerializerStrategy.GetBytes(value);
			Array.Reverse(bytes);

			await dest.WriteAsync(bytes);
		}

		/// <inheritdoc />
		public override async Task<TType> ReadAsync(IWireStreamReaderStrategyAsync source)
		{
			return await SerializerStrategy.ReadAsync(source.WithOneTimeReadingAsync().WithByteReversalAsync());
		}
	}
}
