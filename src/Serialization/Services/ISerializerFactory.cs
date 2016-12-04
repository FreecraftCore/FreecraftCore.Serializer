using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreecraftCore.Payload.Serializer
{
	/// <summary>
	/// Service that created <see cref="ITypeSerializerStrategy"/>s.
	/// </summary>
	public interface ISerializerFactory
	{
		/// <summary>
		/// Creates a <see cref="ITypeSerializerStrategy"/> for the provided <paramref name="forType"/>.
		/// </summary>
		/// <param name="forType">Type the serializer is for.</param>
		/// <returns>A valid <see cref="ITypeSerializerStrategy"/> or null if one could not be constructed.</returns>
		ITypeSerializerStrategy Create(Type forType);
	}
}
