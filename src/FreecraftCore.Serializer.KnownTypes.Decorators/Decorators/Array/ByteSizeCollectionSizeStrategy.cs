using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace FreecraftCore.Serializer.KnownTypes
{
	/// <summary>
	/// Size strategy for fixed size collections.
	/// </summary>
	public class ByteSizeCollectionSizeStrategy : ICollectionSizeStrategy
	{
		/// <summary>
		/// Determines the size of the collection from the stream.
		/// </summary>
		public int Size(IWireMemberReaderStrategy reader)
		{
			if (reader == null) throw new ArgumentNullException(nameof(reader));

			//Read the byte size from the stream.
			return reader.ReadByte();
		}

		/// <summary>
		/// The size to consider the collection.
		/// </summary>
		public int Size<TCollectionType, TElementType>(TCollectionType collection, IWireMemberWriterStrategy writer)
			where TCollectionType : IEnumerable, IEnumerable<TElementType>
		{
			if (collection == null) throw new ArgumentNullException(nameof(collection));
			if (writer == null) throw new ArgumentNullException(nameof(writer));

			//Since the size is unknown it's critical that we write the size to the stream.
			writer.Write((byte)collection.Count());

			//We don't know the size so just provide the size of the collection
			return collection.Count();
		}
	}
}
