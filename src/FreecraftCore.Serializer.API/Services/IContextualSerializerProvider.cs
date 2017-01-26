using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;


namespace FreecraftCore.Serializer
{
	public interface IContextualSerializerProvider : IGeneralSerializerProvider
	{
		/// <summary>
		/// Provides a <see cref="ITypeSerializerStrategy"/> for the <see cref="Type"/> if there is a conextual serializer for the given <paramref name="key"/>.
		/// </summary>
		/// <param name="key">Context key.</param>
		/// <exception cref="KeyNotFoundException">Throws if the key is not contained in the provider service.</exception>
		/// <returns>A valid <see cref="ITypeSerializerStrategy"/> for the given context or null if none were available.</returns>
		[Pure]
		[NotNull]
		ITypeSerializerStrategy Get(ContextualSerializerLookupKey key);

		/// <summary>
		/// Indicates if the <see cref="IGeneralSerializerProvider"/> has a <see cref="ITypeSerializerStrategy"/> for
		/// the provided key.
		/// </summary>
		/// <param name="lookupKey">The lookup key to use for searching.</param>
		/// <returns>True if a serializer is found for the key.</returns>
		[Pure]
		bool HasSerializerFor(ContextualSerializerLookupKey lookupKey);
	}
}
