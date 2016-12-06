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

		//TODO: Maybe pass in attribute
		public FixedSizeCollectionSizeStrategy(int size)
		{
			FixedSize = Math.Min(255, size); //WoW only sends byte sizes
		}

		/// <summary>
		/// The size to consider the collection.
		/// </summary>
		public int Size<TCollectionType, TElementType>(TCollectionType collection, IWireMemberWriterStrategy writer)
			where TCollectionType : IEnumerable, IEnumerable<TElementType>
		{
			//This is a fixed size stragey. Don't look at the collection
			//Don't write anything to the stream either. Consumers know the size too

			return FixedSize;
		}

		/// <summary>
		/// Determines the size of the collection from the stream.
		/// </summary>
		public int Size(IWireMemberReaderStrategy reader)
		{
			//It's always a fixed size. Just reutnr the size.
			return FixedSize;
		}
	}
}
