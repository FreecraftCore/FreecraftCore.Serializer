using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;


namespace FreecraftCore.Serializer.KnownTypes
{
	/// <summary>
	/// Size strategy for collections that send a <see cref="int"/> size.
	/// </summary>
	public class Int32SizeCollectionSizeStrategy : ICollectionSizeStrategy
	{
		private ITypeSerializerStrategy<int> intSerializer { get; }

		public Int32SizeCollectionSizeStrategy([NotNull] ITypeSerializerStrategy<int> serializer)
		{
			if (serializer == null) throw new ArgumentNullException(nameof(serializer));

			intSerializer = serializer;
		}


		/// <inheritdoc />
		public int Size(IWireStreamReaderStrategy reader)
		{
			if (reader == null) throw new ArgumentNullException(nameof(reader));

			//Reads a int from the stream.
			return intSerializer.Read(reader);
		}

		/// <inheritdoc />
		public int Size<TCollectionType, TElementType>(TCollectionType collection, IWireStreamWriterStrategy writer)
			where TCollectionType : IEnumerable, IEnumerable<TElementType>
		{
			if (collection == null) throw new ArgumentNullException(nameof(collection));
			if (writer == null) throw new ArgumentNullException(nameof(writer));

			//Write an int size to the stream
			intSerializer.Write(collection.Count(), writer);

			//We don't know the size so just provide the size of the collection
			return collection.Count();
		}
	}
}
