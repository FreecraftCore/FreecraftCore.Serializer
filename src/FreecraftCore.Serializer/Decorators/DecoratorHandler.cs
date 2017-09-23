using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using JetBrains.Annotations;


namespace FreecraftCore.Serializer
{
	public abstract class DecoratorHandler : ISerializerDecoraterHandler, ISerializerStrategyFactory
	{
		/// <summary>
		/// Serializer provider service.
		/// </summary>
		[NotNull]
		protected IContextualSerializerProvider serializerProviderService { get; }

		protected DecoratorHandler([NotNull] IContextualSerializerProvider serializerProvider)
		{
			if (serializerProvider == null)
				throw new ArgumentNullException(nameof(serializerProvider), $"Provided argument {nameof(serializerProvider)} is null.");

			serializerProviderService = serializerProvider;
		}

		/// <inheritdoc />
		[Pure]
		public abstract bool CanHandle(ISerializableTypeContext context);

		/// <inheritdoc />
		[Pure]
		[NotNull]
		public ITypeSerializerStrategy<TType> Create<TType>([NotNull] ISerializableTypeContext context)
		{
			if (context == null) throw new ArgumentNullException(nameof(context));

			if (!CanHandle(context))
				throw new InvalidOperationException($"Cannot handle Type: {context.TargetType.Name} with a {this.GetType().FullName}.");

			if (!context.BuiltContextKey.HasValue)
				throw new InvalidOperationException($"Failed to build a {nameof(ContextualSerializerLookupKey)} for the Type: {context.TargetType} with Context: {context.ToString()}.");

			ITypeSerializerStrategy<TType> serializer = TryCreateSerializer<TType>(context);

			if (serializer == null)
				throw new InvalidOperationException($"Failed to generate a serializer for Type: {context.TargetType.Name} with decorator factory {this.GetType().FullName}.");

			return serializer;
		}

		/// <inheritdoc />
		[Pure]
		public IEnumerable<ISerializableTypeContext> GetAssociatedSerializationContexts([NotNull] ISerializableTypeContext context)
		{
			if (context == null) throw new ArgumentNullException(nameof(context));

			if (!CanHandle(context))
				throw new InvalidOperationException($"Cannot handle Type: {context.TargetType.Name} with a {this.GetType().FullName}.");

			IEnumerable<ISerializableTypeContext> types = TryGetAssociatedSerializableContexts(context);

			if (types == null)
				throw new InvalidOperationException($"Decorator handler {GetType().FullName} produced a null collection of associated types. The collection must never be null.");

			return types;
		}

		//TODO: Doc
		[Pure]
		[NotNull]
		protected abstract IEnumerable<ISerializableTypeContext> TryGetAssociatedSerializableContexts([NotNull] ISerializableTypeContext context);

		/// <summary>
		/// Creates a <see cref="ITypeSerializerStrategy"/> for the provided <paramref name="context"/>.
		/// </summary>
		/// <param name="context">Type the serializer is for.</param>
		/// <returns>A valid <see cref="ITypeSerializerStrategy"/> or null if one could not be constructed.</returns>
		[Pure]
		[NotNull]
		protected abstract ITypeSerializerStrategy<TType> TryCreateSerializer<TType>([NotNull] ISerializableTypeContext context);
	}
}
