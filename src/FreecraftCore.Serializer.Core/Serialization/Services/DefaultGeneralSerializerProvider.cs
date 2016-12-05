using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fasterflect;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Provider service that provides contextless serializers.
	/// </summary>
	public class DefaultGeneralSerializerProvider : IGeneralSerializerProvider
	{
		/// <summary>
		/// Known serializers that the factory can produce.
		/// </summary>
		IReadOnlyDictionary<Type, ITypeSerializerStrategy> knownSerializableTypeSerializers { get; }

		public DefaultGeneralSerializerProvider(IReadOnlyDictionary<Type, ITypeSerializerStrategy> knownTypes)
		{
			if (knownTypes == null)
				throw new ArgumentNullException(nameof(knownTypes), $"Provided collection of known types {nameof(knownTypes)} must be non-null.");

			knownSerializableTypeSerializers = knownTypes;
		}

		/// <summary>
		/// Provides a <see cref="ITypeSerializerStrategy"/> for the <see cref="Type"/>.
		/// </summary>
		/// <param name="type">The type the <see cref="ITypeSerializerStrategy"/> serializes.</param>
		/// <returns>A valid <see cref="ITypeSerializerStrategy"/> or null if none were available.</returns>
		public ITypeSerializerStrategy Get(Type type)
		{
			return knownSerializableTypeSerializers[type];
		}

		/// <summary>
		/// Indicates if the <see cref="IGeneralSerializerProvider"/> has a <see cref="ITypeSerializerStrategy"/> for
		/// the provided <see cref="Type"/>.
		/// </summary>
		/// <param name="type">Type to lookup a <see cref="ITypeSerializerStrategy"/> for.</param>
		/// <returns>True if the provider has and can provide the a <see cref="ITypeSerializerStrategy"/> for the type.</returns>
		public bool HasSerializerFor(Type type)
		{
			return knownSerializableTypeSerializers.ContainsKey(type);
		}
	}
}
