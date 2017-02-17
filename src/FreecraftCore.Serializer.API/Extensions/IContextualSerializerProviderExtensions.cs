using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using JetBrains.Annotations;


namespace FreecraftCore.Serializer.API.Extensions
{
	public static class IContextualSerializerProviderExtensions
	{
		/// <summary>
		/// Provides a <see cref="ITypeSerializerStrategy"/> for the <see cref="Type"/> if there is a conextual serializer for the given <paramref name="key"/>.
		/// </summary>
		/// <param name="provider">The extended object.</param>
		/// <param name="contextFlags">The context flags that indicate the type of context.</param>
		/// <param name="type">The type the <see cref="ITypeSerializerStrategy"/> serializes.</param>
		/// <param name="key">Context key.</param>
		/// <returns>A valid <see cref="ITypeSerializerStrategy"/> for the given context or null if none were available.</returns>
		[Pure]
		[CanBeNull]
		public static ITypeSerializerStrategy Get([NotNull] this IContextualSerializerProvider provider, ContextTypeFlags contextFlags, [NotNull] IContextKey key, [NotNull] Type type)
		{
			if (provider == null)
				throw new ArgumentNullException(nameof(provider), $"Cannot call extension method on null {nameof(IContextualSerializerProvider)}");

			if (key == null) throw new ArgumentNullException(nameof(key));
			if (type == null) throw new ArgumentNullException(nameof(type));
			if (!Enum.IsDefined(typeof(ContextTypeFlags), contextFlags))
				throw new ArgumentOutOfRangeException(nameof(contextFlags), "Value should be defined in the ContextTypeFlags enum.");
			/*if (!Enum.IsDefined(typeof(ContextTypeFlags), contextFlags))
				throw new InvalidEnumArgumentException(nameof(contextFlags), (int) contextFlags, typeof(ContextTypeFlags));*/

			return provider.Get(new ContextualSerializerLookupKey(contextFlags, key, type));
		}

		/// <summary>
		/// Indicates if the <see cref="IContextualSerializerProvider"/> has a <see cref="ITypeSerializerStrategy"/> for
		/// the provided <see cref="Type"/> and context.
		/// </summary>
		/// <param name="provider">The extended object.</param>
		/// <param name="contextFlags">The context flags that indicate the type of context.</param>
		/// <param name="type">Type to lookup a <see cref="ITypeSerializerStrategy"/> for.</param>
		/// <param name="key">Context key.</param>
		/// <returns>True if the provider has and can provide the a <see cref="ITypeSerializerStrategy"/> for the type and context key.</returns>
		[Pure]
		public static bool HasSerializerFor([NotNull] this IContextualSerializerProvider provider, ContextTypeFlags contextFlags, [NotNull] IContextKey key, [NotNull] Type type)
		{
			if (provider == null)
				throw new ArgumentNullException(nameof(provider), $"Cannot call extension method on null {nameof(IContextualSerializerProvider)}");

			if (key == null) throw new ArgumentNullException(nameof(key));
			if (type == null) throw new ArgumentNullException(nameof(type));
			if (!Enum.IsDefined(typeof(ContextTypeFlags), contextFlags))
				throw new ArgumentOutOfRangeException(nameof(contextFlags), "Value should be defined in the ContextTypeFlags enum.");
			/*if (!Enum.IsDefined(typeof(ContextTypeFlags), contextFlags))
				throw new InvalidEnumArgumentException(nameof(contextFlags), (int) contextFlags, typeof(ContextTypeFlags));*/

			return provider.HasSerializerFor(new ContextualSerializerLookupKey(contextFlags, key, type));
		}

	}
}
