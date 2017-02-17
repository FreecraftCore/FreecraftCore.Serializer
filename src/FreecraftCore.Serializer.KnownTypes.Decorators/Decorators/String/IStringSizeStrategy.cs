using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;


namespace FreecraftCore.Serializer.KnownTypes
{
	/// <summary>
	/// Strategy for determining the size of a string.
	/// </summary>
	public interface IStringSizeStrategy : IStringSizeStrategyAsync
	{
		/// <summary>
		/// Determines the size of the string
		/// <exception cref="ArgumentNullException">Throws if any parameter provided is null.</exception>
		/// </summary>
		int Size([NotNull] string stringValue, [NotNull] IWireStreamWriterStrategy writer);

		/// <summary>
		/// Determines the size of the string fromt he stream.
		/// </summary>
		int Size([NotNull] IWireStreamReaderStrategy reader);
	}
}
