using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace FreecraftCore.Serializer.API.Extensions
{
	public static class IContextualSerializerProviderExtensions
	{
		/// <summary>
		/// Provides a <see cref="ITypeSerializerStrategy"/> for the <see cref="Type"/> if there is a conextual serializer for the given <paramref name="key"/>.
		/// </summary>
		/// <param name="contextFlags">The context flags that indicate the type of context.</param>
		/// <param name="type">The type the <see cref="ITypeSerializerStrategy"/> serializes.</param>
		/// <param name="key">Context key.</param>
		/// <returns>A valid <see cref="ITypeSerializerStrategy"/> for the given context or null if none were available.</returns>
		public static ITypeSerializerStrategy Get(this IContextualSerializerProvider provider, ContextTypeFlags contextFlags, IContextKey key, Type type)
		{
			return provider.Get(new ContextualSerializerLookupKey(contextFlags, key, type));
		}

		/// <summary>
		/// Indicates if the <see cref="IGeneralSerializerProvider"/> has a <see cref="ITypeSerializerStrategy"/> for
		/// the provided <see cref="Type"/>.
		/// </summary>
		/// <param name="contextFlags">The context flags that indicate the type of context.</param>
		/// <param name="type">Type to lookup a <see cref="ITypeSerializerStrategy"/> for.</param>
		/// <param name="key">Context key.</param>
		/// <returns>True if the provider has and can provide the a <see cref="ITypeSerializerStrategy"/> for the type and context key.</returns>
		public static bool HasSerializerFor(this IContextualSerializerProvider provider, ContextTypeFlags contextFlags, IContextKey key, Type type)
		{
			return provider.HasSerializerFor(new ContextualSerializerLookupKey(contextFlags, key, type));
		}

	}
}
