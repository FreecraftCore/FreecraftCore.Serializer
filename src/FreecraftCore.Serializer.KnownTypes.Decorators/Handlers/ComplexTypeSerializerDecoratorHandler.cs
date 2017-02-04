using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Fasterflect;
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
		public override bool CanHandle([NotNull] ISerializableTypeContext context)
		{
			if (context == null) throw new ArgumentNullException(nameof(context));

			return !context.HasContextualMemberMetadata() && context.ContextRequirement == SerializationContextRequirement.Contextless;
		}

		/// <inheritdoc />
		protected override ITypeSerializerStrategy<TType> TryCreateSerializer<TType>(ISerializableTypeContext context)
		{
			if (context == null) throw new ArgumentNullException(nameof(context));

			return new ComplexTypeSerializerDecorator<TType>(new MemberSerializationMediatorCollection<TType>(SerializationMediatorFactory));
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
