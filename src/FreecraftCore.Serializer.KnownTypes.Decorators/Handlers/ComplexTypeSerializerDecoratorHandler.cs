using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Fasterflect;
using System.Reflection;

namespace FreecraftCore.Serializer
{
	//TODO: Doc
	[DecoratorHandler]
	public class ComplexTypeSerializerDecoratorHandler : DecoratorHandler
	{
		private IContextualSerializerLookupKeyFactory contextualKeyLookupFactoryService { get; }

		public ComplexTypeSerializerDecoratorHandler(IContextualSerializerProvider serializerProvider, IContextualSerializerLookupKeyFactory contextualKeyLookupFactory)
			: base(serializerProvider)
		{
			//TODO: null check

			contextualKeyLookupFactoryService = contextualKeyLookupFactory;
		}

		public override bool CanHandle(ISerializableTypeContext context)
		{
			return !context.HasContextualMemberMetadata() && context.ContextRequirement == SerializationContextRequirement.Contextless;
		}

		protected override ITypeSerializerStrategy<TType> TryCreateSerializer<TType>(ISerializableTypeContext context)
		{
			//TODO: Cleaner/better way to provide instuctions
			//Build the instructions for serializaiton
			IEnumerable<MemberAndSerializerPair<TType>> orderedMemberInfos = context.TargetType.MembersWith<WireMemberAttribute>(System.Reflection.MemberTypes.Field | System.Reflection.MemberTypes.Property, Flags.InstanceAnyDeclaredOnly)
				.OrderBy(x => x.Attribute<WireMemberAttribute>().MemberOrder)
				.Select(x => new MemberAndSerializerPair<TType>(x, serializerProviderService.Get(contextualKeyLookupFactoryService.Create(x))))
				.ToArray();

			return new ComplexTypeSerializerDecorator<TType>(orderedMemberInfos);
		}

		protected override IEnumerable<ISerializableTypeContext> TryGetAssociatedSerializableContexts(ISerializableTypeContext context)
		{
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
