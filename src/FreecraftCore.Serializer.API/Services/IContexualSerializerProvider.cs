using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreecraftCore.Serializer
{
	public interface IContexualSerializerProvider : IGeneralSerializerProvider
	{
		/// <summary>
		/// Provides a <see cref="ITypeSerializerStrategy"/> for the <see cref="Type"/> if there is a conextual serializer for the given <paramref name="key"/>.
		/// </summary>
		/// <param name="type">The type the <see cref="ITypeSerializerStrategy"/> serializes.</param>
		/// <param name="key">Context key.</param>
		/// <returns>A valid <see cref="ITypeSerializerStrategy"/> for the given context or null if none were available.</returns>
		ITypeSerializerStrategy Get(int key, Type type);

		/// <summary>
		/// Indicates if the <see cref="IGeneralSerializerProvider"/> has a <see cref="ITypeSerializerStrategy"/> for
		/// the provided <see cref="Type"/>.
		/// </summary>
		/// <param name="type">Type to lookup a <see cref="ITypeSerializerStrategy"/> for.</param>
		/// <param name="key">Context key.</param>
		/// <returns>True if the provider has and can provide the a <see cref="ITypeSerializerStrategy"/> for the type and context key.</returns>
		bool HasSerializerFor(int key, Type type);
	}
}
