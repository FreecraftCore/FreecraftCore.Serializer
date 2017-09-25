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

			ITypeSerializerStrategy<TType> strategy = null;

			//Build the contextual key first. We used to do this as one of the last steps but we need
			//to grab the key to check if it already exists.
			context.BuiltContextKey = lookupKeyFactoryService.Create(context);

			if(serializerProviderService.HasSerializerFor(context.BuiltContextKey.Value))
				throw new InvalidOperationException($"Tried to create multiple serializer for already created serialize with Context: {context.ToString()}.");

			DecoratorHandler handler = decoratorHandlers.First(h => h.CanHandle(context));

			//We must register all subcontexts first
			CreateaAndRegisterDistinctObjectGraphSerializableTypeContexts(context, Enumerable.Empty<ISerializableTypeContext>());

			strategy = handler.Create<TType>(context);

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

		public IEnumerable<ISerializableTypeContext> CreateaAndRegisterDistinctObjectGraphSerializableTypeContexts([NotNull] ISerializableTypeContext context, IEnumerable<ISerializableTypeContext> currentContexts)
		{
			if(context == null) throw new ArgumentNullException(nameof(context));

			int currentCount = currentContexts.Count();

			List<ISerializableTypeContext> newContexts = new List<ISerializableTypeContext>();

			foreach(DecoratorHandler handler in decoratorHandlers)
			{
				if(!handler.CanHandle(context))
					continue;

				IEnumerable<ISerializableTypeContext> serializationContexts = handler.GetAssociatedSerializationContexts(context);

				foreach(ISerializableTypeContext subContext in serializationContexts)
				{
					//populate key first; we need to check if we already know about it
					subContext.BuiltContextKey = lookupKeyFactoryService.Create(subContext);
				}

				newContexts = newContexts.Concat(serializationContexts).ToList();
				break;
			}

			//We want to recurr if we found more contexts this recurrision
			if(currentCount != currentContexts.Concat(newContexts).Distinct(new SerializableTypeContextComparer()).Count())
			{
				IEnumerable<ISerializableTypeContext> contextsToProvideRecursively = currentContexts
					.Concat(newContexts)
					.Distinct(new SerializableTypeContextComparer());

				foreach(ISerializableTypeContext s in newContexts)
					contextsToProvideRecursively = contextsToProvideRecursively
						.Concat(CreateaAndRegisterDistinctObjectGraphSerializableTypeContexts(s, contextsToProvideRecursively))
						.Distinct(new SerializableTypeContextComparer());
			}
				

			//This point we have fully unique creatable without circular dependency/reference types
			//If it can handle then we should register the associated types
			foreach(ISerializableTypeContext subContext in newContexts)
			{
				//populate key first; we need to check if we already know about it
				subContext.BuiltContextKey = lookupKeyFactoryService.Create(subContext);

				if(!subContext.BuiltContextKey.HasValue)
					throw new InvalidOperationException($"Type: {subContext.TargetType}");

				//Had to add check for if it was registered; don't want to register a type multiple times; was casuing exceptions too
				if(subContext.ContextRequirement == SerializationContextRequirement.Contextless && serializerProviderService.HasSerializerFor(subContext.TargetType))
					continue;

				if(subContext.BuiltContextKey.HasValue && this.serializerProviderService.HasSerializerFor(subContext.BuiltContextKey.Value))
					continue;

				DecoratorHandler handler = decoratorHandlers.First(h => h.CanHandle(subContext));

				ITypeSerializerStrategy strat = handler.GetType().GetTypeInfo()
						.GetMethod(nameof(handler.Create))
						.MakeGenericMethod(subContext.TargetType)
						.Invoke(handler, new object[1] {subContext}) as ITypeSerializerStrategy;

				RegisterNewSerializerStrategy(subContext, strat);
			}

			//basecase is we've call register on all subcontexts and registered all the ones we know.
			return newContexts.Distinct(new SerializableTypeContextComparer());
		}

		public class SerializableTypeContextComparer : IEqualityComparer<ISerializableTypeContext>
		{
			public bool Equals(ISerializableTypeContext x, ISerializableTypeContext y)
			{
				if(x == null)
					return y == null;

				if(y == null)
					return x == null;

				if(x.TargetType == y.TargetType)
					if(x.ContextRequirement == SerializationContextRequirement.Contextless && y.ContextRequirement == SerializationContextRequirement.Contextless)
						return true;

				if(x.HasContextualKey() && y.HasContextualKey())
					return x.BuiltContextKey.Equals(y);

				return false;
			}

			public int GetHashCode(ISerializableTypeContext obj)
			{
				string contextString = obj.HasContextualKey() ? obj.BuiltContextKey.HasValue.ToString() : "null";
				return $"{contextString}{obj.TargetType}".GetHashCode();
			}
		}
	}
}
