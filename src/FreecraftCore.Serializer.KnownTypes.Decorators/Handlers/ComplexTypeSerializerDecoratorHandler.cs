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

			//TODO: Cleaner/better way to provide instuctions
			//Build the instructions for serializaiton
			IEnumerable<IMemberSerializationMediator<TType>> orderedMemberInfos = context.TargetType.MembersWith<WireMemberAttribute>(MemberTypes.Field | MemberTypes.Property, Flags.InstanceAnyDeclaredOnly)
				.OrderBy(x => x.Attribute<WireMemberAttribute>().MemberOrder)
				.Select(x => SerializationMediatorFactory.Create<TType>(x))
				.ToArray();

			return new ComplexTypeSerializerDecorator<TType>(orderedMemberInfos);
		}

		/// <inheritdoc />
		protected override IEnumerable<ISerializableTypeContext> TryGetAssociatedSerializableContexts(ISerializableTypeContext context)
		{
			if (context == null) throw new ArgumentNullException(nameof(context));

			//We need context when we refer to the members of a Type. They could be marked with metadata that could cause a serializer to be context based

#if !NET35
			return context.TargetType.Members(MemberTypes.Field | MemberTypes.Property, Flags.InstanceAnyVisibility)
				.Where(mi => mi.HasAttribute<WireMemberAttribute>())
				.Select(mi => new MemberInfoBasedSerializationContext(mi)); //provide memberinfo context; context is important for complex type members

#else
			//net35 doesn't have co/contra-variance. Unity3D does though because it's psuedo-net35. Just cast on net35
			return context.TargetType.Members(MemberTypes.Field | MemberTypes.Property, Flags.InstanceAnyVisibility)
				.Where(mi => mi.HasAttribute<WireMemberAttribute>())
				.Select(mi => new MemberInfoBasedSerializationContext(mi) as ISerializableTypeContext); //provide memberinfo context; context is important for complex type members
#endif
		}
	}
}
