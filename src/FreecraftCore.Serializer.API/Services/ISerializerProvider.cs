using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Service that provides <see cref="ITypeSerializerStrategy"/>s.
	/// </summary>
	public interface ISerializerProvider
	{
		/// <summary>
		/// Provides a <see cref="ITypeSerializerStrategy"/> for the <see cref="Type"/>.
		/// </summary>
		/// <param name="type">The type the <see cref="ITypeSerializerStrategy"/> serializes.</param>
		/// <returns>A valid <see cref="ITypeSerializerStrategy"/> or null if none were available.</returns>
		ITypeSerializerStrategy Get(Type type);

		/// <summary>
		/// Indicates if the <see cref="ISerializerProvider"/> has a <see cref="ITypeSerializerStrategy"/> for
		/// the provided <see cref="Type"/>.
		/// </summary>
		/// <param name="type">Type to lookup a <see cref="ITypeSerializerStrategy"/> for.</param>
		/// <returns>True if the provider has and can provide the a <see cref="ITypeSerializerStrategy"/> for the type.</returns>
		bool HasSerializerFor(Type type);
	}
}
