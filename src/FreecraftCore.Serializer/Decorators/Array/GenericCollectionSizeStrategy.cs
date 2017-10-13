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

		/// <summary>
		/// Additional size to add when reading the collection.
		/// Remove from writing the collection.
		/// </summary>
		public byte AddedSize { get; }

		public GenericCollectionSizeStrategy([NotNull] ITypeSerializerStrategy<TSizeType> sizeTypeSerializerStrategy, byte addedSize)
		{
			if (sizeTypeSerializerStrategy == null) throw new ArgumentNullException(nameof(sizeTypeSerializerStrategy));

			SizeTypeSerializerStrategy = sizeTypeSerializerStrategy;
			AddedSize = addedSize;
		}

		/// <summary>
		/// Determines the size of the collection from the stream.
		/// </summary>
		public int Size(IWireStreamReaderStrategy reader)
		{
			if (reader == null) throw new ArgumentNullException(nameof(reader));

			//Read the byte size from the stream.
			//Readd the addedsize so that we know how many items are really there.
			return GenericMath.Convert<TSizeType, int>(SizeTypeSerializerStrategy.Read(reader)) + AddedSize;
		}

		/// <summary>
		/// The size to consider the collection.
		/// </summary>
		public int Size<TCollectionType, TElementType>(TCollectionType collection, IWireStreamWriterStrategy writer)
			where TCollectionType : IEnumerable, IEnumerable<TElementType>
		{
			if (collection == null) throw new ArgumentNullException(nameof(collection));
			if (writer == null) throw new ArgumentNullException(nameof(writer));

			//When writing N items we want to write N - X. The otherside will X so it will still understand
			//This was added to suppose very rare scenarios where the encoded size was off from the actual size.
			int size = collection.Count() - AddedSize;

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

			//When writing N items we want to write N - X. The otherside will X so it will still understand
			//This was added to suppose very rare scenarios where the encoded size was off from the actual size.
			int size = collection.Count() - AddedSize;

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
			
			//Readd the addedsize so that we know how many items are really there.
			return GenericMath.Convert<TSizeType, int>(result) + AddedSize;
		}
	}
}
