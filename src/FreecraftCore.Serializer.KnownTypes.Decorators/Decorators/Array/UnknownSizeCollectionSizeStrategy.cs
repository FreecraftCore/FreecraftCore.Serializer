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
	public class UnknownSizeCollectionSizeStrategy : ICollectionSizeStrategy
	{
		//TODO: Maybe pass in attribute
		public UnknownSizeCollectionSizeStrategy()
		{

		}

		/// <summary>
		/// Determines the size of the collection from the stream.
		/// </summary>
		public int Size(IWireMemberReaderStrategy reader)
		{
			//In the future we may need to read more than a byte. As far as I can tell collection sizes are always uint8.
			return reader.ReadByte();
		}

		/// <summary>
		/// The size to consider the collection.
		/// </summary>
		public int Size<TCollectionType, TElementType>(TCollectionType collection, IWireMemberWriterStrategy writer)
			where TCollectionType : IEnumerable, IEnumerable<TElementType>
		{

			//Since the size is unknown it's critical that we write the size to the stream.
			writer.Write((byte)collection.Count());

			//We don't know the size so just provide the size of the collection
			return collection.Count();
		}
	}
}
