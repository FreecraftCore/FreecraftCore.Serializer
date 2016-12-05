using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fasterflect;
using System.Reflection;

namespace FreecraftCore.Serializer
{
	//TODO: Doc
	[DecoratorHandler]
	public class ComplexTypeSerializerDecoratorHandler : DecoratorHandler
	{
		public ComplexTypeSerializerDecoratorHandler(IContextualSerializerProvider serializerProvider, IContextualSerializerLookupKeyFactory contextualKeyLookupFactory)
			: base(serializerProvider, contextualKeyLookupFactory)
		{

		}

		public override bool CanHandle(ISerializableTypeContext context)
		{
			return !context.HasContextualMemberMetadata() && context.ContextRequirement == SerializationContextRequirement.Contextless;
		}

		protected override ITypeSerializerStrategy TryCreateSerializer(ISerializableTypeContext context)
		{
			//TODO: Cleaner/better way to provide instuctions
			//Build the instructions for serializaiton
			IEnumerable<MemberAndSerializerPair> orderedMemberInfos = context.TargetType.MembersWith<WireMemberAttribute>(System.Reflection.MemberTypes.Field | System.Reflection.MemberTypes.Property, Flags.InstanceAnyDeclaredOnly)
				.OrderBy(x => x.Attribute<WireMemberAttribute>().MemberOrder)
				.Select(x => new MemberAndSerializerPair(x, serializerProviderService.Get(contextualKeyLookupFactoryService.Create(x))))
				.ToArray();

			return ComplexTypeSerializerDecorator.Create(context.TargetType, orderedMemberInfos);
		}

		protected override IEnumerable<ISerializableTypeContext> TryGetAssociatedSerializableContexts(ISerializableTypeContext context)
		{
			//We need context when we refer to the members of a Type. They could be marked with metadata that could cause a serializer to be context based

			return context.TargetType.Members(MemberTypes.Field | MemberTypes.Property, Flags.InstanceAnyVisibility)
				.Where(mi => mi.HasAttribute<WireMemberAttribute>())
				.Select(mi => new MemberInfoBasedSerializationContext(mi)); //provide memberinfo context; context is important for complex type members
		}
	}
}
