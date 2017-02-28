using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using JetBrains.Annotations;

namespace FreecraftCore.Serializer
{
	//TODO: Doc
	[DecoratorHandler]
	public class ComplexTypeSerializerDecoratorHandler : DecoratorHandler
	{
		[NotNull]
		private IMemberSerializationMediatorFactory SerializationMediatorFactory { get; }

		public ComplexTypeSerializerDecoratorHandler([NotNull] IContextualSerializerProvider serializerProvider, [NotNull] IMemberSerializationMediatorFactory serializationMediatorFactory)
			: base(serializerProvider)
		{
			if (serializationMediatorFactory == null) throw new ArgumentNullException(nameof(serializationMediatorFactory));

			SerializationMediatorFactory = serializationMediatorFactory;
		}

		/// <inheritdoc />
		public override bool CanHandle(ISerializableTypeContext context)
		{
			if (context == null) throw new ArgumentNullException(nameof(context));

			//We can handle any type technically
			return true;
		}

		/// <inheritdoc />
		protected override ITypeSerializerStrategy<TType> TryCreateSerializer<TType>(ISerializableTypeContext context)
		{
			if (context == null) throw new ArgumentNullException(nameof(context));

			ITypeSerializerStrategy<TType> complexTypeSerializerDecorator = new ComplexTypeSerializerDecorator<TType>(new MemberSerializationMediatorCollection<TType>(SerializationMediatorFactory), new LambdabasedDeserializationPrototyeFactory<TType>(), serializerProviderService);

			//Check for compression flags
			if(context.BuiltContextKey.Value.ContextFlags.HasFlag(ContextTypeFlags.Compressed))
				complexTypeSerializerDecorator = new CompressionTypeSerializerStrategyDecorator<TType>(complexTypeSerializerDecorator, serializerProviderService.Get<uint>());

			return complexTypeSerializerDecorator;
		}

		/// <inheritdoc />
		protected override IEnumerable<ISerializableTypeContext> TryGetAssociatedSerializableContexts(ISerializableTypeContext context)
		{
			if (context == null) throw new ArgumentNullException(nameof(context));

			//We need context when we refer to the members of a Type. They could be marked with metadata that could cause a serializer to be context based

			return new TypeMemberParsedTypeContextCollection(context.TargetType);
		}
	}
}
