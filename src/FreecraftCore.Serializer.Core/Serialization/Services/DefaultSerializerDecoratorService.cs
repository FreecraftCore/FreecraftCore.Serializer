using Fasterflect;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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
		/// Event that can be subscribed to for alerts on found associated types.
		/// </summary>
		public event FoundUnknownAssociatedType OnFoundUnknownAssociatedType;

		public DefaultSerializerStrategyFactory(IEnumerable<DecoratorHandler> handlers, IGeneralSerializerProvider generalSerializerProvider)
		{
			if (generalSerializerProvider == null)
				throw new ArgumentNullException(nameof(generalSerializerProvider), $"Provided {nameof(IGeneralSerializerProvider)} service was null.");

			if (handlers == null)
				throw new ArgumentNullException(nameof(handlers), $"Provided {nameof(DecoratorHandler)}s were null. Must be a non-null collection.");

			decoratorHandlers = handlers;
			generalSerializerProviderService = generalSerializerProvider;
		}

		/// <summary>
		/// Attempts to produce a decorated serializer for the provided <see cref="ISerializableTypeContext"/>.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <returns>A decorated serializer or null if none could be created.</returns>
		public ITypeSerializerStrategy Create(ISerializableTypeContext context)
		{
			ISerializerStrategyFactory factory = null;

			foreach (DecoratorHandler handler in decoratorHandlers)
			{
				if (handler.CanHandle(context))
				{
					//If it can handle then we should register the associated types
					foreach (ISerializableTypeContext subContext in handler.GetAssociatedSerializationContexts(context))
					{
						//Broadcast that we found an associated type if it's not known
						//Because we only know the Type there will be no context and we need a TypeContext
						OnFoundUnknownAssociatedType?.Invoke(subContext);
					}

					//TODO: If we ever have mutliple decoration then this factory set will break things
					factory = handler;
					break;
				}
			}

			if (factory == null)
				throw new InvalidOperationException($"Couldn't generate a strategy for Type: {context.TargetType}.");

			return factory.Create(context);
		}
	}
}
