﻿using Fasterflect;
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
	/// Service that builds decorators around serializers for semi-complex types.
	/// </summary>
	public class DefaultSerializerDecoratorService : ISerializerDecoratorService
	{
		/// <summary>
		/// Decorator handlers.
		/// </summary>
		private IEnumerable<DectoratorHandler> decoratorHandlers { get; }

		/// <summary>
		/// Event that can be subscribed to for alerts on found associated types.
		/// </summary>
		public event FoundUnknownAssociatedType OnFoundUnknownAssociatedType;

		public DefaultSerializerDecoratorService(IEnumerable<DectoratorHandler> handlers)
		{
			if (handlers == null)
				throw new ArgumentNullException(nameof(handlers), $"Provided {nameof(DectoratorHandler)}s were null. Must be a non-null collection.");

			decoratorHandlers = handlers;
		}

		/// <summary>
		/// Indicates if the provided <see cref="ISerializableTypeContext"/> <paramref name="context"/> requires decoration.
		/// </summary>
		/// <param name="context">The <see cref="ISerializableTypeContext"/> context used to verify for potential serializer decoration.</param>
		/// <returns>True if the <see cref="ISerializableTypeContext"/> requires decoration with the service.</returns>
		public bool RequiresDecorating(ISerializableTypeContext context)
		{
			//Check all the handlers to see if this type requires a decorated serializer
			foreach (ISerializerDecoraterHandler handler in decoratorHandlers)
				if (handler.CanHandle(context))
					return true;

			//This is fine; means the type is probably primitive or just complex
			return false;
		}

		/// <summary>
		/// Attempts to produce a decorated serializer for the provided <see cref="ISerializableTypeContext"/>.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <returns>A decorated serializer or null if none could be created.</returns>
		public ITypeSerializerStrategy GenerateDecoratedSerializer(ISerializableTypeContext context)
		{
			ISerializerStrategyFactory factory = null;

			foreach(DectoratorHandler handler in decoratorHandlers)
			{
				if(handler.CanHandle(context))
				{
					//If it can handle then we should register the associated types
					foreach(Type t in handler.GetAssociatedRegisterableTypes(context))
					{
						//Broadcast that we found an associated type
						OnFoundUnknownAssociatedType?.Invoke(t);
					}
				}
			}

			if (factory == null)
				throw new InvalidOperationException($"Couldn't generate a strategy for Type: {context.TargetType}.");

			return factory.Create(context);
		}
	}
}