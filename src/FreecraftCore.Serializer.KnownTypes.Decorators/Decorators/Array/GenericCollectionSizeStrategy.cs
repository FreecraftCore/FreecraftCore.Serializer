using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Generic.Math;
using JetBrains.Annotations;


namespace FreecraftCore.Serializer.KnownTypes
{
	/// <summary>
	/// Generic size strategy for dynamically sized collections.
	/// </summary>
	public class GenericCollectionSizeStrategy<TSizeType> : ICollectionSizeStrategy
		where TSizeType : struct
	{
		/// <summary>
		/// The managed serializer that reads and writes the size.
		/// </summary>
		[NotNull]
		private ITypeSerializerStrategy<TSizeType> SizeTypeSerializerStrategy { get; }

		public GenericCollectionSizeStrategy([NotNull] ITypeSerializerStrategy<TSizeType> sizeTypeSerializerStrategy)
		{
			if (sizeTypeSerializerStrategy == null) throw new ArgumentNullException(nameof(sizeTypeSerializerStrategy));

			SizeTypeSerializerStrategy = sizeTypeSerializerStrategy;
		}

		/// <summary>
		/// Determines the size of the collection from the stream.
		/// </summary>
		public int Size(IWireStreamReaderStrategy reader)
		{
			if (reader == null) throw new ArgumentNullException(nameof(reader));

			//Read the byte size from the stream.
			return GenericMath.Convert<TSizeType, int>(SizeTypeSerializerStrategy.Read(reader));
		}

		/// <summary>
		/// The size to consider the collection.
		/// </summary>
		public int Size<TCollectionType, TElementType>(TCollectionType collection, IWireStreamWriterStrategy writer)
			where TCollectionType : IEnumerable, IEnumerable<TElementType>
		{
			if (collection == null) throw new ArgumentNullException(nameof(collection));
			if (writer == null) throw new ArgumentNullException(nameof(writer));

			int size = collection.Count();

			//Since the size is unknown it's critical that we write the size to the stream.
			SizeTypeSerializerStrategy.Write(GenericMath.Convert<int, TSizeType>(size), writer);

			//We don't know the size so just provide the size of the collection
			return size;
		}

		/// <inheritdoc />
		public async Task<int> SizeAsync<TCollectionType, TElementType>(TCollectionType collection, IWireStreamWriterStrategyAsync writer) 
			where TCollectionType : IEnumerable, IEnumerable<TElementType>
		{
			if (collection == null) throw new ArgumentNullException(nameof(collection));
			if (writer == null) throw new ArgumentNullException(nameof(writer));

			int size = collection.Count();

			//yield until we write (shouldn't take long and maybe syncronous is more efficient and performant but async API should be fully async) 
			await SizeTypeSerializerStrategy.WriteAsync(GenericMath.Convert<int, TSizeType>(size), writer);

			//We don't know the size so just provide the size of the collection
			return size;
		}

		/// <inheritdoc />
		public async Task<int> SizeAsync(IWireStreamReaderStrategyAsync reader)
		{
			if (reader == null) throw new ArgumentNullException(nameof(reader));

			//Read the byte size from the stream.
			TSizeType result = await SizeTypeSerializerStrategy.ReadAsync(reader);

			return GenericMath.Convert<TSizeType, int>(result);
		}
	}
}
