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

		public DefaultSerializerStrategyFactory([NotNull] IEnumerable<DecoratorHandler> handlers, [NotNull] IContextualSerializerProvider serializerProvider, 
			[NotNull] ISerializerStrategyFactory fallbackFactory, [NotNull] IContextualSerializerLookupKeyFactory lookupKeyFactory)
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
			if (context == null) throw new ArgumentNullException(nameof(context));

			ISerializerStrategyFactory factory = null;

			//Build the contextual key first. We used to do this as one of the last steps but we need
			//to grab the key to check if it already exists.
			context.BuiltContextKey = lookupKeyFactoryService.Create(context);

			if (serializerProviderService.HasSerializerFor(context.BuiltContextKey.Value))
				throw new InvalidOperationException($"Tried to create multiple serializer for already created serialize with Context: {context.ToString()}.");

			foreach (DecoratorHandler handler in decoratorHandlers)
			{
				if (!handler.CanHandle(context))
					continue;

				//If it can handle then we should register the associated types
				foreach (ISerializableTypeContext subContext in handler.GetAssociatedSerializationContexts(context))
				{
					//populate key first; we need to check if we already know about it
					subContext.BuiltContextKey = lookupKeyFactoryService.Create(subContext);

					//Had to add check for if it was registered; don't want to register a type multiple times; was casuing exceptions too
					if (subContext.ContextRequirement == SerializationContextRequirement.Contextless && serializerProviderService.HasSerializerFor(subContext.TargetType))
						continue;

					if (subContext.BuiltContextKey.HasValue && this.serializerProviderService.HasSerializerFor(subContext.BuiltContextKey.Value))
						continue;

						//This is ugly because we dropped Fasterflect AND .NETStandard1.3 is kinda ugly with reflection
						//TODO: Maybe figure out how to get type context inside here to call generic
						//Maybe visitor?
						//Call the create on the fallback handler
						fallbackFactoryService.GetType().GetTypeInfo().GetMethod(nameof(Create))
							.MakeGenericMethod(subContext.TargetType)
							.Invoke(fallbackFactoryService, new object[] { subContext });		
				}

				//TODO: If we ever have mutliple decoration then this factory set will break things
				factory = handler;
				break;
			}

			if (factory == null)
				throw new InvalidOperationException($"Couldn't generate a strategy for Type: {context.TargetType} with Context: {context.BuiltContextKey?.ToString()}.");

			return factory.Create<TType>(context);
		}
	}
}
