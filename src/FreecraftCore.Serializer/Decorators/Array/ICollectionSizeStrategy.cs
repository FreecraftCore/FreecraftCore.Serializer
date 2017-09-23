using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;


namespace FreecraftCore.Serializer.KnownTypes
{
	/// <summary>
	/// Strategy for determining the size of an array.
	/// </summary>
	public interface ICollectionSizeStrategy : ICollectionSizeStrategyAsync
	{
		/// <summary>
		/// Determines the size of the collection.
		/// <exception cref="ArgumentNullException">Throws if any of the provided parameters are null.</exception>
		/// </summary>
		int Size<TCollectionType, TElementType>([NotNull] TCollectionType collection, [NotNull] IWireStreamWriterStrategy writer)
			where TCollectionType : IEnumerable, IEnumerable<TElementType>;

		/// <summary>
		/// Determines the size of the collection from the stream.
		/// <exception cref="ArgumentNullException">Throws if any of the provided parameters are null.</exception>
		/// </summary>
		int Size([NotNull] IWireStreamReaderStrategy reader);
	}
}
