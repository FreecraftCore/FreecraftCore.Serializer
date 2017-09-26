using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FreecraftCore.Serializer.API.Extensions;
using JetBrains.Annotations;


namespace FreecraftCore.Serializer
{
	internal interface ILazySerializerProvider
	{
		Lazy<ITypeSerializerStrategy> SerializerStrategy { get; }
	}

	internal sealed class ContextlessLazyStrategyProvider : ILazySerializerProvider
	{
		public Lazy<ITypeSerializerStrategy> SerializerStrategy { get; }

		public ContextlessLazyStrategyProvider(IContextualSerializerProvider provider, Type t)
		{
			SerializerStrategy = new Lazy<ITypeSerializerStrategy>(() => provider.Get(t), true);
		}
	}

	internal sealed class ContextualLazyStrategyProvider : ILazySerializerProvider
	{
		public Lazy<ITypeSerializerStrategy> SerializerStrategy { get; }

		public ContextualLazyStrategyProvider(IContextualSerializerProvider provider, ContextualSerializerLookupKey key)
		{
			SerializerStrategy = new Lazy<ITypeSerializerStrategy>(() => provider.Get(key), true);
		}
	}

	public class SerializerStrategyProvider : IContextualSerializerProvider, ISerializerStrategyRegistry, IEnumerable<Type>, ICompilable
	{
		[NotNull]
		private IDictionary<Type, ITypeSerializerStrategy> contextlessSerializerLookupTable { get; }

		/// <summary>
		/// Lookup table that takes a context as a key and produces a serialization strategy.
		/// </summary>
		[NotNull]
		private IDictionary<ContextualSerializerLookupKey, ITypeSerializerStrategy> strategyLookupTable { get; }

		public SerializerStrategyProvider()
		{
			contextlessSerializerLookupTable = new Dictionary<Type, ITypeSerializerStrategy>();
			strategyLookupTable = new Dictionary<ContextualSerializerLookupKey, ITypeSerializerStrategy>();
		}

		/// <inheritdoc />
		public ITypeSerializerStrategy Get([NotNull]Type type)
		{
			//TODO: If we just grab the serializer and throw instead we can get a speed up
			if(!contextlessSerializerLookupTable.ContainsKey(type))
			{
				//Return a lazy loaded version
				ContextlessLazyStrategyProvider lazyProvider = new ContextlessLazyStrategyProvider(this, type);

				return Activator.CreateInstance(typeof(LazyLoadedSerializerStrategy<>).MakeGenericType(type), SerializationContextRequirement.Contextless, type, lazyProvider)
					as ITypeSerializerStrategy;
			}

			return contextlessSerializerLookupTable[type];
		}

		/// <inheritdoc />
		public ITypeSerializerStrategy Get(ContextualSerializerLookupKey key)
		{
			//If they ask for none it's best to just call general
			if (key.ContextFlags == ContextTypeFlags.None)
				return Get(key.ContextType);

			if(strategyLookupTable.ContainsKey(key))
				return strategyLookupTable[key];

			//Return a lazy loaded version if it doesn't exist
			ContextualLazyStrategyProvider lazyProvider = new ContextualLazyStrategyProvider(this, key);

			return Activator.CreateInstance(typeof(LazyLoadedSerializerStrategy<>).MakeGenericType(key.ContextType), SerializationContextRequirement.Contextless, key.ContextType, lazyProvider)
				as ITypeSerializerStrategy;
		}

		/// <inheritdoc />
		public bool HasSerializerFor(Type type)
		{
			return contextlessSerializerLookupTable.ContainsKey(type);
		}

		/// <inheritdoc />
		public bool HasSerializerFor(ContextualSerializerLookupKey key)
		{
			return strategyLookupTable.ContainsKey(key);
		}


		/// <inheritdoc />
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

		/// <inheritdoc />
		public bool RegisterType(ContextTypeFlags contextFlags, IContextKey key, Type type, ITypeSerializerStrategy strategy)
		{
			if (type == null)
				throw new ArgumentNullException(nameof(type), $"Provided argument {nameof(type)} is null.");

			if (strategy == null)
				throw new ArgumentNullException(nameof(strategy), $"Provided argument {nameof(strategy)} is null.");

			if (key == null)
				throw new ArgumentNullException(nameof(key), $"Provided argument {nameof(key)} is null.");

			//Check if contextflags are none. We can skip everything and delegate to contextless
			if (contextFlags == ContextTypeFlags.None)
				return RegisterType(type, strategy);

			//Register a new contextless serializer
			strategyLookupTable.Add(new ContextualSerializerLookupKey(contextFlags, key, type), strategy);

			return true;
		}

		/// <inheritdoc />
		public IEnumerator<Type> GetEnumerator()
		{
			return contextlessSerializerLookupTable.Keys
				.Concat(strategyLookupTable.Values.Select(c => c.SerializerType))
				.GetEnumerator();
		}

		/// <inheritdoc />
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public void Compile()
		{
			foreach(ITypeSerializerStrategy s in contextlessSerializerLookupTable.Values)
				if(s is ICompilable com)
					com.Compile();

			foreach(ITypeSerializerStrategy s in strategyLookupTable.Values)
				if(s is ICompilable com)
					com.Compile();
		}
	}
}
