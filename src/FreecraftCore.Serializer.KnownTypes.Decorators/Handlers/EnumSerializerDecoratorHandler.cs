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
	public class EnumSerializerDecoratorHandler : DectoratorHandler
	{
		/// <summary>
		/// Serializer factory service.
		/// </summary>
		private IGeneralSerializerProvider serializerProviderService { get; }

		public EnumSerializerDecoratorHandler(IGeneralSerializerProvider serializerProvider)
		{
			if (serializerProvider == null)
				throw new ArgumentNullException(nameof(serializerProvider), $"Provided service {nameof(IGeneralSerializerProvider)} was null.");

			serializerProviderService = serializerProvider;
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

		protected override ITypeSerializerStrategy TryCreateSerializer(ISerializableTypeContext context)
		{
			//error handling in base
			return typeof(EnumSerializerDecorator<,>).MakeGenericType(context.TargetType, context.TargetType.GetEnumUnderlyingType())
						.CreateInstance(serializerProviderService) as ITypeSerializerStrategy;
		}

		protected override IEnumerable<Type> TryGetAssociatedTypes(ISerializableTypeContext context)
		{
			//error handling and checking is done in base

			//An enum only requires its base underlying type to be registered
			return new Type[] { context.TargetType.GetEnumUnderlyingType() };
		}
	}
}
