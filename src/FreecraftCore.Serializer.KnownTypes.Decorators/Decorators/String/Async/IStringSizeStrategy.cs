using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;


namespace FreecraftCore.Serializer.KnownTypes
{
	/// <summary>
	/// Strategy for determining the size of a string using async.
	/// </summary>
	public interface IStringSizeStrategyAsync
	{
		/// <summary>
		/// Determines the size of the string
		/// <exception cref="ArgumentNullException">Throws if any parameter provided is null.</exception>
		/// </summary>
		Task<int> SizeAsync([NotNull] string stringValue, [NotNull] IWireStreamWriterStrategyAsync writer);

		/// <summary>
		/// Determines the size of the string fromt he stream.
		/// </summary>
		Task<int> SizeAsync([NotNull] IWireStreamReaderStrategyAsync reader);
	}
}
