using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreecraftCore
{
	/// <summary>
	/// Contract for types that implement serialization strategies for
	/// the specified <typeparamref name="T"/>.
	/// </summary>
	/// <typeparam name="T">The Type to serialize.</typeparam>
	public interface ITypeSerializerStrategy<T>
	{
		/// <summary>
		/// Reads a copy of <typeparamref name="T"/> from the buffer <paramref name="source"/>
		/// starting from <paramref name="offset"/>
		/// </summary>
		/// <param name="source">The source buffer to read from.</param>
		/// <param name="offset">The starting offset into the buffer to read from.</param>
		/// <returns></returns>
		T Read(Span<byte> source, int offset);

		void Write(T value, Span<byte> source, int offset);
	}
}
