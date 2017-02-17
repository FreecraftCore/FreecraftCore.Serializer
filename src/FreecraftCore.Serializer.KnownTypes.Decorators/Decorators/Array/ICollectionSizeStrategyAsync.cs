using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;


namespace FreecraftCore.Serializer.KnownTypes
{
	//This may seem ridiculous but if the size is coming over the network we don't want people
	//stalling a server or blocking a thread that could do something else because of a malicious client.
	/// <summary>
	/// Strategy for determining the size of an array in an async fashion.
	/// </summary>
	public interface ICollectionSizeStrategyAsync
	{
		/// <summary>
		/// Determines the size of the collection.
		/// <exception cref="ArgumentNullException">Throws if any of the provided parameters are null.</exception>
		/// </summary>
		Task<int> SizeAsync<TCollectionType, TElementType>([NotNull] TCollectionType collection, [NotNull] IWireStreamWriterStrategyAsync writer)
			where TCollectionType : IEnumerable, IEnumerable<TElementType>;

		/// <summary>
		/// Determines the size of the collection from the stream.
		/// <exception cref="ArgumentNullException">Throws if any of the provided parameters are null.</exception>
		/// </summary>
		Task<int> SizeAsync([NotNull] IWireStreamReaderStrategyAsync reader);
	}
}
