using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FreecraftCore.Serializer
{
	public abstract class DecoratorHandler : ISerializerDecoraterHandler, ISerializerStrategyFactory
	{
		/// <summary>
		/// Serializer provider service.
		/// </summary>
		protected IContextualSerializerProvider serializerProviderService { get; }

		/// <summary>
		/// Conextual key factory.
		/// </summary>
		protected IContextualSerializerLookupKeyFactory contextualKeyLookupFactoryService { get; }

		public DecoratorHandler(IContextualSerializerProvider serializerProvider, IContextualSerializerLookupKeyFactory keyFactory)
		{
			if (serializerProvider == null)
				throw new ArgumentNullException(nameof(serializerProvider), $"Provided argument {nameof(serializerProvider)} is null.");

			if(keyFactory == null)
				throw new ArgumentNullException(nameof(keyFactory), $"Provided argument {nameof(keyFactory)} is null.");

			serializerProviderService = serializerProvider;
			contextualKeyLookupFactoryService = keyFactory;
		}

		/// <summary>
		/// Indicates if the <see cref="ISerializerDecoraterHandler"/> is able to handle the specified <see cref="ISerializableTypeContext"/>.
		/// </summary>
		/// <param name="context">The member context.</param>
		/// <returns>True if the handler can decorate for the serialization of the specified <see cref="ISerializableTypeContext"/>.</returns>
		public abstract bool CanHandle(ISerializableTypeContext context);

		public ITypeSerializerStrategy<TType> Create<TType>(ISerializableTypeContext context)
		{
			if (!CanHandle(context))
				throw new InvalidOperationException($"Cannot handle Type: {context.TargetType.Name} with a {this.GetType().FullName}.");

			//Build the key first
			context.BuiltContextKey = this.contextualKeyLookupFactoryService.Create(context);

			if (!context.BuiltContextKey.HasValue)
				throw new InvalidOperationException($"Failed to build a {nameof(ContextualSerializerLookupKey)} for the Type: {context.TargetType} with Context: {context.ToString()}.");

			ITypeSerializerStrategy<TType> serializer = TryCreateSerializer<TType>(context);

			if (serializer == null)
				throw new InvalidOperationException($"Failed to generate a serializer for Type: {context.TargetType.Name} with decorator factory {this.GetType().FullName}.");

			return serializer;
		}

		/// <summary>
		/// Gets a collection of <see cref="ISerializableTypeContext"/>s that represent the total collection
		/// of sub-contexts needed to be handled for the given provided <paramref name="context"/>.
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public IEnumerable<ISerializableTypeContext> GetAssociatedSerializationContexts(ISerializableTypeContext context)
		{
			if (!CanHandle(context))
				throw new InvalidOperationException($"Cannot handle Type: {context.TargetType.Name} with a {this.GetType().FullName}.");

			IEnumerable<ISerializableTypeContext> types = TryGetAssociatedSerializableContexts(context);

			if (types == null)
				throw new InvalidOperationException($"Decorator handler {GetType().FullName} produced a null collection of associated types. The collection must never be null.");

			return types;
		}

		//TODO: Doc
		protected abstract IEnumerable<ISerializableTypeContext> TryGetAssociatedSerializableContexts(ISerializableTypeContext context);

		/// <summary>
		/// Creates a <see cref="ITypeSerializerStrategy"/> for the provided <paramref name="forType"/>.
		/// </summary>
		/// <param name="forType">Type the serializer is for.</param>
		/// <returns>A valid <see cref="ITypeSerializerStrategy"/> or null if one could not be constructed.</returns>
		protected abstract ITypeSerializerStrategy<TType> TryCreateSerializer<TType>(ISerializableTypeContext context);
	}
}
