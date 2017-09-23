using System;
using System.Reflection;
using FreecraftCore.Serializer.API.Reflection;
using JetBrains.Annotations;
using Reflect.Extent;

namespace FreecraftCore.Serializer
{
	//TODO: Document
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
			//We now have a generic arg for the member type which we don't technically have a compile time ref to
			//therefore we must use activator to create
			IMemberSerializationMediator<TContainingType> mediator = 
				Activator.CreateInstance(typeof(DefaultMemberSerializationMediator<,>).MakeGenericType(new Type[] { typeof(TContainingType), info.Type()}),
					info, typeSerializerProvider.Get(lookupKeyFactory.Create(info))) as IMemberSerializationMediator<TContainingType>;

			//TODO: Do checking and exceptions for mediator failure

			if (info.HasAttribute<DontWriteAttribute>())
				mediator = new DisableWriteMemberSerializationMediatorDecorator<TContainingType>(mediator);

			if(info.HasAttribute<DontReadAttribute>())
				mediator = new DisableReadMemberSerializationMediatorDecorator<TContainingType>(mediator);

			return mediator;
		}
	}
}