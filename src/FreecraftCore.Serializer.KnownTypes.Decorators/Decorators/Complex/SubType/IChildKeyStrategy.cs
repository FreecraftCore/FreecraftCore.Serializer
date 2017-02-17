using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace FreecraftCore.Serializer.KnownTypes
{
	/// <summary>
	/// Strategy for producing the proper key for the subtype child lookup
	/// </summary>
	public interface IChildKeyStrategy : IChildKeyStrategyAsync
	{
		/// <summary>
		/// Attempts to the read the child key from the source.
		/// </summary>
		/// <param name="source"></param>
		/// <exception cref="ArgumentNullException">Throws if the <see cref="source"/> provided was null.</exception>
		/// <returns>Integer casted child key.</returns>
		int Read([NotNull] IWireStreamReaderStrategy source);

		/// <summary>
		/// Writes the key to the stream using the size implemented by the strategy.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="dest"></param>
		/// <exception cref="ArgumentNullException">Throws if the <see cref="dest"/> provided was null.</exception>
		void Write(int value, [NotNull] IWireStreamWriterStrategy dest);
	}
}
