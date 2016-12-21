using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace FreecraftCore.Serializer
{
	public class SerializerStrategyProvider : IContextualSerializerProvider, IGeneralSerializerProvider, ISerializerStrategyRegistry
	{
		private IDictionary<Type, ITypeSerializerStrategy> contextlessSerializerLookupTable { get; }

		/// <summary>
		/// Lookup table that takes a context as a key and produces a serialization strategy.
		/// </summary>
		private IDictionary<ContextualSerializerLookupKey, ITypeSerializerStrategy> strategyLookupTable { get; }

		public SerializerStrategyProvider()
		{
			contextlessSerializerLookupTable = new Dictionary<Type, ITypeSerializerStrategy>();
			strategyLookupTable = new Dictionary<ContextualSerializerLookupKey, ITypeSerializerStrategy>();
		}

		/// <summary>
		/// Provides a <see cref="ITypeSerializerStrategy"/> for the <see cref="Type"/>.
		/// </summary>
		/// <param name="type">The type the <see cref="ITypeSerializerStrategy"/> serializes.</param>
		/// <returns>A valid <see cref="ITypeSerializerStrategy"/> or null if none were available.</returns>
		public ITypeSerializerStrategy Get(Type type)
		{
			return contextlessSerializerLookupTable[type];
		}

		/// <summary>
		/// Provides a <see cref="ITypeSerializerStrategy"/> for the <see cref="Type"/> if there is a conextual serializer for the given <paramref name="key"/>.
		/// </summary>
		/// <param name="contextFlags">The context flags that indicate the type of context.</param>
		/// <param name="type">The type the <see cref="ITypeSerializerStrategy"/> serializes.</param>
		/// <param name="key">Context key.</param>
		/// <returns>A valid <see cref="ITypeSerializerStrategy"/> for the given context or null if none were available.</returns>
		public ITypeSerializerStrategy Get(ContextualSerializerLookupKey key)
		{
			//If they ask for none it's best to just call general
			if (key.ContextFlags == ContextTypeFlags.None)
				return Get(key.ContextType);

			try
			{
				return strategyLookupTable[key];
			}
			catch(KeyNotFoundException e)
			{
				throw new InvalidOperationException($"Key was not found in the service {key.ToString()}.", e);
			}
		}

		/// <summary>
		/// Indicates if the <see cref="IGeneralSerializerProvider"/> has a <see cref="ITypeSerializerStrategy"/> for
		/// the provided <see cref="Type"/>.
		/// </summary>
		/// <param name="type">Type to lookup a <see cref="ITypeSerializerStrategy"/> for.</param>
		/// <returns>True if the provider has and can provide the a <see cref="ITypeSerializerStrategy"/> for the type.</returns>
		public bool HasSerializerFor(Type type)
		{
			return contextlessSerializerLookupTable.ContainsKey(type);
		}

		/// <summary>
		/// Indicates if the <see cref="IGeneralSerializerProvider"/> has a <see cref="ITypeSerializerStrategy"/> for
		/// the provided <see cref="Type"/>.
		/// </summary>
		/// <param name="contextFlags">The context flags that indicate the type of context.</param>
		/// <param name="type">Type to lookup a <see cref="ITypeSerializerStrategy"/> for.</param>
		/// <param name="key">Context key.</param>
		/// <returns>True if the provider has and can provide the a <see cref="ITypeSerializerStrategy"/> for the type and context key.</returns>
		public bool HasSerializerFor(ContextualSerializerLookupKey key)
		{
			return strategyLookupTable.ContainsKey(key);
		}

		/// <summary>
		/// Registers a contextless <see cref="ITypeSerializerStrategy"/> for the provided type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns>True if successfully registered.</returns>
		public bool RegisterType(Type type, ITypeSerializerStrategy strategy)
		{
			if (type == null)
				throw new ArgumentNullException(nameof(type), $"Provided argument {nameof(type)} is null.");

			if (strategy == null)
				throw new ArgumentNullException(nameof(strategy), $"Provided argument {nameof(strategy)} is null.");

			//Already have one
			if (this.HasSerializerFor(type))
				return true;

			//This is a contextless serializer. We should register it as contextless to save perf
			//Though we should check if that is ture.
			if (strategy.ContextRequirement != SerializationContextRequirement.Contextless)
				throw new InvalidOperationException($"Provided serializer Type: {strategy.GetType().FullName} is not a contextless serializer. Cannot register without context.");

			//Register a new contextless serializer
			contextlessSerializerLookupTable.Add(type, strategy);

			return true;
		}

		/// <summary>
		/// Registers a serializer with the provided context.
		/// </summary>
		/// <param name="contextFlags">The context type.</param>
		/// <param name="key">The context key.</param>
		/// <param name="type">The type the <see cref="ITypeSerializerStrategy"/> serializes.</param>
		/// <param name="strategy">The serializer strategy.</param>
		/// <returns>True if successfully registered.</returns>
		public bool RegisterType(ContextTypeFlags contextFlags, IContextKey key, Type type, ITypeSerializerStrategy strategy)
		{
			if (type == null)
				throw new ArgumentNullException(nameof(type), $"Provided argument {nameof(type)} is null.");

			//Check if contextflags are none. We can skip everything and delegate to contextless
			if(contextFlags == ContextTypeFlags.None)
				return RegisterType(type, strategy);

			if (strategy == null)
				throw new ArgumentNullException(nameof(strategy), $"Provided argument {nameof(strategy)} is null.");

			if (key == null)
				throw new ArgumentNullException(nameof(key), $"Provided argument {nameof(key)} is null.");

			//Register a new contextless serializer
			strategyLookupTable.Add(new ContextualSerializerLookupKey(contextFlags, key, type), strategy);

			return true;
		}
	}
}
