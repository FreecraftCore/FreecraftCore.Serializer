using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreecraftCore.Serializer
{
	public class DefaultContextualSerializerProvider : IContexualSerializerProvider, IGeneralSerializerProvider
	{
		/// <summary>
		/// General <see cref="ITypeSerializerStrategy"/> provider service.
		/// </summary>
		private IGeneralSerializerProvider generalSerializerProviderService { get; }

		/// <summary>
		/// Map of contextual serializers.
		/// </summary>
		private IReadOnlyDictionary<int, ITypeSerializerStrategy> contextualSerializerCollection { get; }

		public DefaultContextualSerializerProvider(IGeneralSerializerProvider generalSerializerProvider, IReadOnlyDictionary<int, ITypeSerializerStrategy> contextualSerializers)
		{
			if(generalSerializerProvider == null)
				throw new ArgumentNullException(nameof(generalSerializerProvider), $"Provided argument {nameof(generalSerializerProvider)} was null.");

			if (contextualSerializers == null)
				throw new ArgumentNullException(nameof(contextualSerializers), $"Provided argument {nameof(contextualSerializers)} was null.");

			contextualSerializerCollection = contextualSerializers;
			generalSerializerProviderService = generalSerializerProvider;
		}

		/// <summary>
		/// Provides a <see cref="ITypeSerializerStrategy"/> for the <see cref="Type"/> if there is a conextual serializer for the given <paramref name="key"/>.
		/// </summary>
		/// <param name="type">The type the <see cref="ITypeSerializerStrategy"/> serializes.</param>
		/// <param name="key">Context key.</param>
		/// <returns>A valid <see cref="ITypeSerializerStrategy"/> for the given context or null if none were available.</returns>
		public ITypeSerializerStrategy Get(int key, Type type)
		{
			if (contextualSerializerCollection.ContainsKey(key))
				return contextualSerializerCollection[key];
			else
			{
				//We should try to provide a general serializer
				//WARNING: Just because we didn't find a contextual serializer doesn't mean there shouldn't be one. We can't know that here.
				return generalSerializerProviderService.Get(type);
			}
		}


		/// <summary>
		/// Indicates if the <see cref="IGeneralSerializerProvider"/> has a <see cref="ITypeSerializerStrategy"/> for
		/// the provided <see cref="Type"/>.
		/// </summary>
		/// <param name="type">Type to lookup a <see cref="ITypeSerializerStrategy"/> for.</param>
		/// <param name="key">Context key.</param>
		/// <returns>True if the provider has and can provide the a <see cref="ITypeSerializerStrategy"/> for the type and context key.</returns>
		public bool HasSerializerFor(int key, Type type)
		{
			return contextualSerializerCollection.ContainsKey(key) || generalSerializerProviderService.HasSerializerFor(type);
		}

		ITypeSerializerStrategy IGeneralSerializerProvider.Get(Type type)
		{
			//default to general provider
			return generalSerializerProviderService.Get(type);
		}

		bool IGeneralSerializerProvider.HasSerializerFor(Type type)
		{
			return generalSerializerProviderService.HasSerializerFor(type);
		}
	}
}
