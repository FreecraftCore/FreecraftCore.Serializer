using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Generic.Math;
using JetBrains.Annotations;


namespace FreecraftCore.Serializer.KnownTypes
{
	public class SizeIncludedStringSizeStrategy<TSizeType> : IStringSizeStrategy
		where TSizeType : struct
	{
		[NotNull]
		private ITypeSerializerStrategy<TSizeType> sizeSerializer { get; }

		private bool includeNullTerminatorInSizeCalculation { get; }

		public SizeIncludedStringSizeStrategy([NotNull] ITypeSerializerStrategy<TSizeType> serializer, bool shouldCountNullTerminator)
		{
			if (serializer == null)
				throw new ArgumentNullException(nameof(serializer), $"Provided argument {serializer} is null.");

			sizeSerializer = serializer;

			includeNullTerminatorInSizeCalculation = shouldCountNullTerminator;
		}

		/// <inheritdoc />
		public int Size(IWireStreamReaderStrategy reader)
		{
			if (reader == null) throw new ArgumentNullException(nameof(reader));

			TSizeType size = sizeSerializer.Read(reader);

			//Using JonSkeets MiscUtils we can convert objects efficently
			return GenericMath<TSizeType, int>.Convert(size);
		}

		/// <inheritdoc />
		public int Size(string stringValue, IWireStreamWriterStrategy writer)
		{
			if (stringValue == null) throw new ArgumentNullException(nameof(stringValue));
			if (writer == null) throw new ArgumentNullException(nameof(writer));

			int size = stringValue.Length;

			//add one for null terminator
			//Using JonSkeets MiscUtils we can convert objects efficently
			sizeSerializer.Write(GenericMath<int, TSizeType>.Convert(size + (includeNullTerminatorInSizeCalculation ? 1 : 0)), writer);

			return size;
		}

		/// <inheritdoc />
		public async Task<int> SizeAsync(string stringValue, IWireStreamWriterStrategyAsync writer)
		{
			if (stringValue == null) throw new ArgumentNullException(nameof(stringValue));
			if (writer == null) throw new ArgumentNullException(nameof(writer));

			int size = stringValue.Length;

			//add one for null terminator
			//Using JonSkeets MiscUtils we can convert objects efficently
			await sizeSerializer.WriteAsync(GenericMath<int, TSizeType>.Convert(size + (includeNullTerminatorInSizeCalculation ? 1 : 0)), writer);

			return size;
		}

		/// <inheritdoc />
		public async Task<int> SizeAsync(IWireStreamReaderStrategyAsync reader)
		{
			if (reader == null) throw new ArgumentNullException(nameof(reader));

			TSizeType size = await sizeSerializer.ReadAsync(reader);

			//Using JonSkeets MiscUtils we can convert objects efficently
			return GenericMath<TSizeType, int>.Convert(size);
		}
	}
}
