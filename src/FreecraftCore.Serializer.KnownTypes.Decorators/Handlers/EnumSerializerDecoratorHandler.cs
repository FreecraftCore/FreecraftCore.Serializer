using Fasterflect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FreecraftCore.Serializer.KnownTypes
{
	/// <summary>
	/// Decorator handler and factory for <see cref="Enum"/> serializers.
	/// </summary>
	[DecoratorHandler]
	public class EnumSerializerDecoratorHandler : DecoratorHandler
	{
		public EnumSerializerDecoratorHandler(IContextualSerializerProvider serializerProvider, IContextualSerializerLookupKeyFactory contextualKeyLookupFactory)
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

			return context.TargetType.IsEnum;
		}

		protected override ITypeSerializerStrategy<TType> TryCreateSerializer<TType>(ISerializableTypeContext context)
		{
			//error handling in base
			return typeof(EnumSerializerDecorator<,>).MakeGenericType(context.TargetType, context.TargetType.GetEnumUnderlyingType())
						.CreateInstance(serializerProviderService) as ITypeSerializerStrategy<TType>;
		}

		protected override IEnumerable<ISerializableTypeContext> TryGetAssociatedSerializableContexts(ISerializableTypeContext context)
		{
			//error handling and checking is done in base

			//An enum only requires its base underlying type to be registered therefore no context is required
			return new ISerializableTypeContext[] { new TypeBasedSerializationContext(context.TargetType.GetEnumUnderlyingType()) };
		}
	}
}
