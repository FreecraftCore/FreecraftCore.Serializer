using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace FreecraftCore.Serializer.KnownTypes
{
	/// <summary>
	/// Size strategy for fixed size collections.
	/// </summary>
	public class FixedSizeCollectionSizeStrategy : ICollectionSizeStrategy
	{
		/// <summary>
		/// Fixed size of the array.
		/// </summary>
		public int FixedSize { get; }

		/// <summary>
		/// 
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">Throws if the provided <see cref="size"/> is 0, less than or greater than the value of a byte.</exception>
		/// <param name="size"></param>
		public FixedSizeCollectionSizeStrategy(int size)
		{
			if (size <= 0 || size > 255)
				throw new ArgumentOutOfRangeException(nameof(size),
					$"Provided {nameof(size)} must be bounded between 1 and 255. Was {size}.");

			FixedSize = size;
		}


		/// <inheritdoc />
		public int Size<TCollectionType, TElementType>(TCollectionType collection, IWireStreamWriterStrategy writer)
			where TCollectionType : IEnumerable, IEnumerable<TElementType>
		{
			if (collection == null) throw new ArgumentNullException(nameof(collection));
			if (writer == null) throw new ArgumentNullException(nameof(writer));

			//This is a fixed size stragey. Don't look at the collection
			//Don't write anything to the stream either. Consumers know the size too

			return FixedSize;
		}

		/// <inheritdoc />
		public int Size(IWireStreamReaderStrategy reader)
		{
			if (reader == null) throw new ArgumentNullException(nameof(reader));

			//It's always a fixed size. Just reutnr the size.
			return FixedSize;
		}

		/// <inheritdoc />
		public Task<int> SizeAsync<TCollectionType, TElementType>(TCollectionType collection, IWireStreamWriterStrategyAsync writer) where TCollectionType : IEnumerable, IEnumerable<TElementType>
		{
			if (collection == null) throw new ArgumentNullException(nameof(collection));
			if (writer == null) throw new ArgumentNullException(nameof(writer));

			//This is a fixed size stragey. Don't look at the collection
			//Don't write anything to the stream either. Consumers know the size too

			return Task.FromResult(FixedSize);
		}

		/// <inheritdoc />
		public Task<int> SizeAsync(IWireStreamReaderStrategyAsync reader)
		{
			if (reader == null) throw new ArgumentNullException(nameof(reader));

			//It's always a fixed size. Just reutnr the size.
			return Task.FromResult(FixedSize);
		}
	}
}
