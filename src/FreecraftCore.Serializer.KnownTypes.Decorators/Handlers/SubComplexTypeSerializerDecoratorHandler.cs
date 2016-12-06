using Fasterflect;
using FreecraftCore.Serializer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FreecraftCore.Serializer.KnownTypes
{
	/// <summary>
	/// Decorator handler and factory for complex types that have children.
	/// </summary>
	[DecoratorHandler]
	public class SubComplexTypeSerializerDecoratorHandler : DecoratorHandler
	{
		public SubComplexTypeSerializerDecoratorHandler(IContextualSerializerProvider serializerProvider, IContextualSerializerLookupKeyFactory contextualKeyLookupFactory)
			: base(serializerProvider, contextualKeyLookupFactory)
		{

		}
		/// <summary>
		/// Indicates if the <see cref="ISerializerDecoraterHandler"/> is able to handle the specified <see cref="ISerializableTypeContext"/>.
		/// </summary>
		/// <param name="context">The member context.</param>
		/// <returns>True if the handler can decorate for the serialization of the specified <see cref="ISerializableTypeContext"/>.</returns>
		public override bool CanHandle(ISerializableTypeContext context)
		{
			if (context == null)
				throw new ArgumentNullException(nameof(context), $"Provided argument {nameof(context)} was null.");

			//Check if the type has wirebase type attributes.
			//If it does then the type is complex and can have subtypes coming across the wire
			return context.TargetType.GetCustomAttributes<WireMessageBaseTypeAttribute>(false).Count() != 0;
		}

		protected override ITypeSerializerStrategy<TType> TryCreateSerializer<TType>(ISerializableTypeContext context)
		{
			//error handling in base

			return new SubComplexTypeSerializerDecorator<TType>(serializerProviderService);
		}

		protected override IEnumerable<ISerializableTypeContext> TryGetAssociatedSerializableContexts(ISerializableTypeContext context)
		{
			//error handling and checking is done in base

			//Grab the children from the metadata; return type contexts so the types can be handled (no context is required because the children are their own registerable type
			return GetAssociatedChildren(context.TargetType).Select(t => new TypeBasedSerializationContext(t));
		}

		private IEnumerable<Type> GetAssociatedChildren(Type type)
		{
			return type.Attributes<WireMessageBaseTypeAttribute>().Select(x => x.ChildType);
		}
	}
}
