using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FreecraftCore.Serializer
{
	public abstract class DectoratorHandler : ISerializerDecoraterHandler, ISerializerStrategyFactory
	{
		/// <summary>
		/// Indicates if the <see cref="ISerializerDecoraterHandler"/> is able to handle the specified <see cref="ISerializableTypeContext"/>.
		/// </summary>
		/// <param name="context">The member context.</param>
		/// <returns>True if the handler can decorate for the serialization of the specified <see cref="ISerializableTypeContext"/>.</returns>
		public abstract bool CanHandle(ISerializableTypeContext context);

		public ITypeSerializerStrategy Create(ISerializableTypeContext context)
		{
			if (!CanHandle(context))
				throw new InvalidOperationException($"Cannot handle Type: {context.TargetType.Name} with a {this.GetType().FullName}.");

			ITypeSerializerStrategy serializer = TryCreateSerializer(context);

			if (serializer == null)
				throw new InvalidOperationException($"Failed to generate a serializer for Type: {context.TargetType.Name} with decorator factory {this.GetType().FullName}.");

			return serializer;
		}

		/// <summary>
		/// Gets a collection of <see cref="Type"/> objects that represent all of the required types that must be
		/// registered for this decorator to work. These should be registered before attempting to use the decorator.
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public IEnumerable<Type> GetAssociatedRegisterableTypes(ISerializableTypeContext context)
		{
			if (!CanHandle(context))
				throw new InvalidOperationException($"Cannot handle Type: {context.TargetType.Name} with a {this.GetType().FullName}.");

			IEnumerable<Type> types = TryGetAssociatedTypes(context);

			if (types == null)
				throw new InvalidOperationException($"Decorator handler {GetType().FullName} produced a null collection of associated types. The collection must never be null.");

			return types;
		}

		protected abstract IEnumerable<Type> TryGetAssociatedTypes(ISerializableTypeContext context);

		/// <summary>
		/// Creates a <see cref="ITypeSerializerStrategy"/> for the provided <paramref name="forType"/>.
		/// </summary>
		/// <param name="forType">Type the serializer is for.</param>
		/// <returns>A valid <see cref="ITypeSerializerStrategy"/> or null if one could not be constructed.</returns>
		protected abstract ITypeSerializerStrategy TryCreateSerializer(ISerializableTypeContext context);
	}
}
