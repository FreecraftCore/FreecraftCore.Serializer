using System;
using System.Reflection;
using Fasterflect;
using FreecraftCore.Serializer.API.Reflection;
using JetBrains.Annotations;

namespace FreecraftCore.Serializer
{
	public class MemberSerializationMediatorFactory : IMemberSerializationMediatorFactory
	{
		[NotNull]
		private IContextualSerializerProvider typeSerializerProvider { get; }

		[NotNull]
		private IContextualSerializerLookupKeyFactory lookupKeyFactory { get; }

		public MemberSerializationMediatorFactory([NotNull] IContextualSerializerProvider typeSerializerProvider, [NotNull] IContextualSerializerLookupKeyFactory lookupKeyFactory)
		{
			if (typeSerializerProvider == null) throw new ArgumentNullException(nameof(typeSerializerProvider));
			if (lookupKeyFactory == null) throw new ArgumentNullException(nameof(lookupKeyFactory));

			this.typeSerializerProvider = typeSerializerProvider;
			this.lookupKeyFactory = lookupKeyFactory;
		}

		public IMemberSerializationMediator<TContainingType> Create<TContainingType>(MemberInfo info)
		{
			if (info == null) throw new ArgumentNullException(nameof(info));

			//Construct a default and then we can decorate as needed
			IMemberSerializationMediator<TContainingType> mediator = 
				new DefaultMemberSerializationMediator<TContainingType>(info, typeSerializerProvider.Get(lookupKeyFactory.Create(info)));

			if(info.HasAttribute<DontWriteAttribute>())
				mediator = new DisableWriteMemberSerializationMediatorDecorator<TContainingType>(mediator);

			if(info.HasAttribute<DontReadAttribute>())
				mediator = new DisableReadMemberSerializationMediatorDecorator<TContainingType>(mediator);

			return mediator;
		}
	}
}