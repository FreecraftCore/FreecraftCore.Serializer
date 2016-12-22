using Fasterflect;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;


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
		private IEnumerable<DecoratorHandler> decoratorHandlers { get; }

		/// <summary>
		/// General serializer provider service.
		/// </summary>
		private IGeneralSerializerProvider generalSerializerProviderService { get; }

		/// <summary>
		/// Fallback factory (used to be an event broadcast)
		/// </summary>
		private ISerializerStrategyFactory fallbackFactoryService { get; }

		/// <summary>
		/// Lookup key factory service.
		/// </summary>
		private IContextualSerializerLookupKeyFactory lookupKeyFactoryService { get; }

		public DefaultSerializerStrategyFactory(IEnumerable<DecoratorHandler> handlers, IGeneralSerializerProvider generalSerializerProvider, ISerializerStrategyFactory fallbackFactory, IContextualSerializerLookupKeyFactory lookupKeyFactory)
		{
			if (generalSerializerProvider == null)
				throw new ArgumentNullException(nameof(generalSerializerProvider), $"Provided {nameof(IGeneralSerializerProvider)} service was null.");

			if (handlers == null)
				throw new ArgumentNullException(nameof(handlers), $"Provided {nameof(DecoratorHandler)}s were null. Must be a non-null collection.");

			if (fallbackFactory == null)
				throw new ArgumentNullException(nameof(fallbackFactory), $"Provided {nameof(ISerializerStrategyFactory)}s were null. Must be a non-null collection.");

			//TODO: null check
			lookupKeyFactoryService = lookupKeyFactory;

			decoratorHandlers = handlers;
			generalSerializerProviderService = generalSerializerProvider;
			fallbackFactoryService = fallbackFactory;
		}

		/// <summary>
		/// Attempts to produce a decorated serializer for the provided <see cref="ISerializableTypeContext"/>.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <returns>A decorated serializer or null if none could be created.</returns>
		public ITypeSerializerStrategy<TType> Create<TType>(ISerializableTypeContext context)
		{
			ISerializerStrategyFactory factory = null;

			//Build the contextual key first. We used to do this as one of the last steps but we need
			//to grab the key to check if it already exists.
			context.BuiltContextKey = lookupKeyFactoryService.Create(context);

			if (generalSerializerProviderService.HasSerializerFor(context.BuiltContextKey.Value))
				throw new InvalidOperationException($"Tried to create multiple serializer for already created serialize with Context: {context.ToString()}.");

			foreach (DecoratorHandler handler in decoratorHandlers)
			{
				if (handler.CanHandle(context))
				{
					//If it can handle then we should register the associated types
					foreach (ISerializableTypeContext subContext in handler.GetAssociatedSerializationContexts(context))
					{
						//populate key first; we need to check if we already know about it
						subContext.BuiltContextKey = lookupKeyFactoryService.Create(subContext);

						//Had to add check for if it was registered; don't want to register a type multiple times; was casuing exceptions too
						if (subContext.ContextRequirement == SerializationContextRequirement.Contextless && generalSerializerProviderService.HasSerializerFor(subContext.TargetType))
							continue;

						if (subContext.BuiltContextKey.HasValue && this.generalSerializerProviderService.HasSerializerFor(subContext.BuiltContextKey.Value))
							continue;

						//TODO: Maybe figure out how to get type context inside here to call generic
						//Maybe visitor?
						//Call the create on the fallback handler
						fallbackFactoryService.CallMethod(new Type[] { subContext.TargetType }, nameof(Create), subContext);
					}

					//TODO: If we ever have mutliple decoration then this factory set will break things
					factory = handler;
					break;
				}
			}

			if (factory == null)
				throw new InvalidOperationException($"Couldn't generate a strategy for Type: {context.TargetType}.");

			return factory.Create<TType>(context);
		}
	}
}
