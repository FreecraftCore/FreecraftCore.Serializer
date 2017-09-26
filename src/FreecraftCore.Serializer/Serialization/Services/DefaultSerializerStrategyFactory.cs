using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using JetBrains.Annotations;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Service that builds decorators around serializers for semi-complex and complex types.
	/// </summary>
	public class DefaultSerializerStrategyFactory : ISerializerStrategyFactory
	{
		/// <summary>
		/// Decorator handlers for primitives.
		/// </summary>
		[NotNull]
		private IEnumerable<DecoratorHandler> decoratorHandlers { get; }

		/// <summary>
		/// General serializer provider service.
		/// </summary>
		[NotNull]
		private IContextualSerializerProvider serializerProviderService { get; }

		/// <summary>
		/// Fallback factory (used to be an event broadcast)
		/// </summary>
		[NotNull]
		private ISerializerStrategyFactory fallbackFactoryService { get; }

		/// <summary>
		/// Lookup key factory service.
		/// </summary>
		[NotNull]
		private IContextualSerializerLookupKeyFactory lookupKeyFactoryService { get; }

		public ISerializerStrategyRegistry StrategyRegistry { get; }

		public DefaultSerializerStrategyFactory([NotNull] IEnumerable<DecoratorHandler> handlers, [NotNull] IContextualSerializerProvider serializerProvider, 
			[NotNull] ISerializerStrategyFactory fallbackFactory, [NotNull] IContextualSerializerLookupKeyFactory lookupKeyFactory, ISerializerStrategyRegistry strategyRegistry)
		{
			if (serializerProvider == null)
				throw new ArgumentNullException(nameof(serializerProvider), $"Provided {nameof(IContextualSerializerProvider)} service was null.");

			if (handlers == null)
				throw new ArgumentNullException(nameof(handlers), $"Provided {nameof(DecoratorHandler)}s were null. Must be a non-null collection.");

			if (fallbackFactory == null)
				throw new ArgumentNullException(nameof(fallbackFactory), $"Provided {nameof(ISerializerStrategyFactory)} service was null.");

			if (lookupKeyFactory == null)
				throw new ArgumentNullException(nameof(lookupKeyFactory), $"Provided {nameof(IContextualSerializerLookupKeyFactory)} service was null.");

			lookupKeyFactoryService = lookupKeyFactory;
			StrategyRegistry = strategyRegistry ?? throw new ArgumentNullException(nameof(strategyRegistry));
			decoratorHandlers = handlers;
			serializerProviderService = serializerProvider;
			fallbackFactoryService = fallbackFactory;
		}

		/// <summary>
		/// Attempts to produce a decorated serializer for the provided <see cref="ISerializableTypeContext"/>.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <exception cref="InvalidOperationException">Throws if the <see cref="ITypeSerializerStrategy{TType}"/> could not be created.</exception>
		/// <returns>A decorated serializer.</returns>
		public ITypeSerializerStrategy<TType> Create<TType>([NotNull] ISerializableTypeContext context)
		{
			if(context == null) throw new ArgumentNullException(nameof(context));

			//Build the contextual key first. We used to do this as one of the last steps but we need
			//to grab the key to check if it already exists.
			context.BuiltContextKey = lookupKeyFactoryService.Create(context);

			if(serializerProviderService.HasSerializerFor(context.BuiltContextKey.Value))
				throw new InvalidOperationException($"Tried to create multiple serializer for already created serialize with Context: {context.ToString()}.");

			IEnumerable<ISerializableTypeContext> byLikelyRegisteration = GetAllSubTypeContexts(context, Enumerable.Empty<ISerializableTypeContext>())
					.Where(s => !context.BuiltContextKey.Value.Equals(s.BuiltContextKey.Value))
					.Distinct(new SerializableTypeContextComparer())
					.ToList();

			//We must register all subcontexts first
			foreach(ISerializableTypeContext subContext in byLikelyRegisteration)
			{
				this.GetType().GetTypeInfo().GetMethod(nameof(InternalCreate), BindingFlags.NonPublic | BindingFlags.Instance)
					.MakeGenericMethod(subContext.TargetType)
					.Invoke(this, new object[] {subContext});
			}

			//Now that all dependencies in the object graph are registered we can create the requested type
			return InternalCreate<TType>(context);
		}

		private ITypeSerializerStrategy<TType> InternalCreate<TType>([NotNull] ISerializableTypeContext context)
		{
			if(context == null) throw new ArgumentNullException(nameof(context));

			DecoratorHandler handler = decoratorHandlers.First(h => h.CanHandle(context));

			ITypeSerializerStrategy<TType> strategy = handler.Create<TType>(context);

			if(strategy == null)
				throw new InvalidOperationException($"Couldn't generate a strategy for Type: {context.TargetType} with Context: {context.BuiltContextKey?.ToString()}.");

			//If the serializer is contextless we can register it with the general provider
			RegisterNewSerializerStrategy(context, strategy);

			return strategy;
		}

		private void RegisterNewSerializerStrategy(ISerializableTypeContext context, ITypeSerializerStrategy strategy)
		{
			if(strategy.ContextRequirement == SerializationContextRequirement.Contextless)
				StrategyRegistry.RegisterType(context.TargetType, strategy);
			else
			{
				//TODO: Clean this up
				if(context.HasContextualKey())
				{
					//Register the serializer with the context key that was built into the serialization context.
					StrategyRegistry.RegisterType(context.BuiltContextKey.Value.ContextFlags, context.BuiltContextKey.Value.ContextSpecificKey, context.TargetType, strategy);
				}
				else
					throw new InvalidOperationException($"Serializer was created but Type: {context.TargetType} came with no contextual key in the end for a contextful serialization context.");
			}
		}

		private IEnumerable<ISerializableTypeContext> GetAllSubTypeContexts([NotNull] ISerializableTypeContext context, IEnumerable<ISerializableTypeContext> knownContexts)
		{
			List<ISerializableTypeContext> contexts = GetUnknownUnregisteredSubTypeContexts(context, knownContexts)
				.ToList();

			foreach(ISerializableTypeContext s in contexts)
				s.BuiltContextKey = lookupKeyFactoryService.Create(s);

			//Get only the new contexts
			contexts = contexts.Except(knownContexts, new SerializableTypeContextComparer())
				.ToList();

			//After we've made context unique ones we haven't registered we NEED to make sure that the ones we know in this stack
			//are included in the knowns we pass to ones below this recurring stack
			knownContexts = knownContexts.Concat(contexts);

			//If there are any new we need to recurr to get their dependencies
			foreach(var s in contexts)
			{
				knownContexts = knownContexts.Concat(GetAllSubTypeContexts(s, knownContexts));
			}
				

			//Return the entire tree of contexts
			return knownContexts;
		}

		private IEnumerable<ISerializableTypeContext> GetUnknownUnregisteredSubTypeContexts([NotNull] ISerializableTypeContext context, IEnumerable<ISerializableTypeContext> knownContexts)
		{
			return decoratorHandlers.First(h => h.CanHandle(context))
				.GetAssociatedSerializationContexts(context)
				.Select(s =>
				{
					s.BuiltContextKey = lookupKeyFactoryService.Create(s);
					return s;
				})
				.Where(s => !serializerProviderService.HasSerializerFor(s.BuiltContextKey.Value)) //make sure we ignore known registered types
				.Except(knownContexts, new SerializableTypeContextComparer())
				.ToList();
		}

		public class SerializableTypeContextComparer : IEqualityComparer<ISerializableTypeContext>
		{
			public bool Equals(ISerializableTypeContext x, ISerializableTypeContext y)
			{
				if(x.ContextRequirement == SerializationContextRequirement.Contextless)
					if(y.ContextRequirement == SerializationContextRequirement.Contextless)
						return x.TargetType == y.TargetType;

				if(!x.BuiltContextKey.HasValue || !y.BuiltContextKey.HasValue)
					throw new ArgumentException($"Type context T1: {x.TargetType} T2: {y.TargetType} don't have keys.");

				return x.BuiltContextKey.Value.Equals(y.BuiltContextKey.Value);
			}

			public int GetHashCode(ISerializableTypeContext obj)
			{
				return obj.BuiltContextKey.Value.GetHashCode();
			}
		}
	}
}
