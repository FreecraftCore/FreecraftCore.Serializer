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
	/// Decorator handler and factory for <see cref="Array"/> serializers.
	/// </summary>
	public class ArraySerializerDecoratorHandler : DectoratorHandler
	{
		/// <summary>
		/// Serializer factory service.
		/// </summary>
		private IGeneralSerializerProvider serializerProviderService { get; }

		public ArraySerializerDecoratorHandler(IGeneralSerializerProvider serializerProvider)
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

			return context.TargetType.IsArray;
		}

		protected override ITypeSerializerStrategy TryCreateSerializer(ISerializableTypeContext context)
		{
			//error handling in base

			//TODO: Handle contextless requests. The future may require a single array serializer for all unknown sizes.
			if(context.HasMemberAttribute<KnownSizeAttribute>())
			{
				//If we know about the size then we should create a knownsize array decorator
				return typeof(FixedSizeArraySerializerDecorator<>).MakeGenericType(context.TargetType)
					.CreateInstance(serializerProviderService, context.GetMemberAttribute<KnownSizeAttribute>().KnownSize) as ITypeSerializerStrategy;
			}
			else
			{
				//If we know about the size then we should create a knownsize array decorator
				return typeof(ArraySerializerDecorator<>).MakeGenericType(context.TargetType)
					.CreateInstance(serializerProviderService) as ITypeSerializerStrategy;
			}
		}

		protected override IEnumerable<Type> TryGetAssociatedTypes(ISerializableTypeContext context)
		{
			//error handling and checking is done in base

			//An enum only requires its base underlying type to be registered
			return new Type[] { context.TargetType.GetElementType() };
		}
	}
}
