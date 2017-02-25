using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Decorates a <see cref="IWireStreamReaderStrategy"/> with the ability to only peek the underlying reader.
	/// </summary>
	/// <typeparam name="TReaderType">The reader type.</typeparam>
	public class PeekOnlyWireStreamReaderStrategy<TReaderType> : WireMemberReaderStrategyDecorator<TReaderType>
		where TReaderType : IWireStreamReaderStrategy
	{
		/// <inheritdoc />
		public PeekOnlyWireStreamReaderStrategy([NotNull] TReaderType decoratedReader) 
			: base(decoratedReader)
		{

		}

		/// <inheritdoc />
		public override byte ReadByte()
		{
			//Force a peek
			return DecoratedReader.PeekByte();
		}

		/// <inheritdoc />
		public override byte PeekByte()
		{
			return DecoratedReader.PeekByte();
		}

		/// <inheritdoc />
		public override byte[] ReadAllBytes()
		{
			//TODO: Implement a peek method for all bytes.
			throw new NotImplementedException($"Cannot force a peek for all bytes.");
		}

		/// <inheritdoc />
		public override byte[] ReadBytes(int count)
		{
			return DecoratedReader.PeekBytes(count);
		}

		/// <inheritdoc />
		public override byte[] PeekBytes(int count)
		{
			return DecoratedReader.PeekBytes(count);
		}
	}
}
