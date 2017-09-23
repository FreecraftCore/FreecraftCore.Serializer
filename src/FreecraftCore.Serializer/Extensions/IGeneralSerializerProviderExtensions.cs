using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;


namespace FreecraftCore.Serializer
{
	public static class IGeneralSerializerProviderExtensions
	{
		/// <summary>
		/// Provides a <see cref="ITypeSerializerStrategy{T}"/> for the type <typeparamref name="TRequestedType"/> from the provided
		/// <see cref="IGeneralSerializerProvider"/> service.
		/// </summary>
		/// <param name="provider">The extended object.</param>
		/// <exception cref="InvalidOperationException"></exception>
		/// <returns>A valid <see cref="ITypeSerializerStrategy{T}"/> from the given provided or null if none were available.</returns>
		[Pure]
		[NotNull]
		public static ITypeSerializerStrategy<TRequestedType> Get<TRequestedType>([NotNull] this IGeneralSerializerProvider provider)
		{
			if (provider == null) throw new ArgumentNullException(nameof(provider));

			ITypeSerializerStrategy<TRequestedType> strat = provider.Get(typeof(TRequestedType)) as ITypeSerializerStrategy<TRequestedType>;

			if (strat == null)
				throw new InvalidOperationException($"Unabled to locate registered Type: {typeof(TRequestedType).FullName}. Make sure to properly register the type beforing requesting a serializer.");

			return strat;
		}

		/// <summary>
		/// Indicates if the <see cref="IGeneralSerializerProvider"/> has a <see cref="ITypeSerializerStrategy"/> for
		/// the type <typeparamref name="TTypeToCheck"/>.
		/// </summary>
		/// <param name="provider">The extended object.</param>
		/// <returns>True if the provider has and can provide the a <see cref="ITypeSerializerStrategy"/> for the type.</returns>
		[Pure]
		public static bool HasSerializerFor<TTypeToCheck>([NotNull] this IGeneralSerializerProvider provider)
		{
			if (provider == null) throw new ArgumentNullException(nameof(provider));

			return provider.HasSerializerFor(typeof(TTypeToCheck));
		}
	}
}
