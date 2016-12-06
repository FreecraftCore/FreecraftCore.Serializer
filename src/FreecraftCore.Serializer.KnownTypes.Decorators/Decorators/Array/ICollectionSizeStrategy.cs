using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreecraftCore.Serializer.KnownTypes
{
	/// <summary>
	/// Strategy for determining the size of an array.
	/// </summary>
	public interface ICollectionSizeStrategy
	{
		/// <summary>
		/// Determines the size of the collection.
		/// </summary>
		int Size<TCollectionType, TElementType>(TCollectionType collection, IWireMemberWriterStrategy writer)
			where TCollectionType : IEnumerable, IEnumerable<TElementType>;

		/// <summary>
		/// Determines the size of the collection from the stream.
		/// </summary>
		int Size(IWireMemberReaderStrategy reader);
	}
}
