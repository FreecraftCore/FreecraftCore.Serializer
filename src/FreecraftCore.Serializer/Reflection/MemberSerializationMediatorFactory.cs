using System;
using System.Linq;
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
			if(info == null) throw new ArgumentNullException(nameof(info));

			ITypeSerializerStrategy strategy = typeSerializerProvider.Get(lookupKeyFactory.Create(info));
			//Construct a default and then we can decorate as needed
			//We now have a generic arg for the member type which we don't technically have a compile time ref to
			//therefore we must use activator to create
			IMemberSerializationMediator<TContainingType> mediator = 
				Activator.CreateInstance(typeof(DefaultMemberSerializationMediator<,>).MakeGenericType(new Type[] { typeof(TContainingType), info.Type()}),
					info, strategy) as IMemberSerializationMediator<TContainingType>;

			//TODO: Do checking and exceptions for mediator failure

			//Check for seperated collection size attributes. We need to decorate for each one this is linked to
			foreach (var attri in info.DeclaringType.GetTypeInfo().GetCustomAttributes<SeperatedCollectionSizeAttribute>(false))
			{
				//[NotNull] MemberInfo sizeMemberInfo, [NotNull] MemberInfo collectionMemberInfo, [NotNull] ITypeSerializerStrategy serializer, [NotNull] IMemberSerializationMediator<TContainingType> decoratedMediator) 
				if(attri.SizePropertyName == info.Name)
				{
					MemberInfo collectionMember = info.DeclaringType.GetTypeInfo().GetMember(attri.CollectionPropertyName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).First();

					mediator = Activator.CreateInstance(typeof(ConnectedCollectionSizeSerializationMediator<,>).MakeGenericType(new Type[] { typeof(TContainingType), info.Type() }),
						info, collectionMember, strategy, mediator) as IMemberSerializationMediator<TContainingType>;
				}

				//TODO: Undefined behavior if we have to decorate this multiple times
				if(attri.CollectionPropertyName == info.Name)
				{
					MemberInfo sizeMember = info.DeclaringType.GetTypeInfo().GetMember(attri.SizePropertyName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).First();

					mediator = Activator.CreateInstance(typeof(ConnectedCollectionCollectionSerializationMediator<,,>).MakeGenericType(new Type[] { typeof(TContainingType), sizeMember.Type(), info.Type() }),
						info, sizeMember, strategy, mediator) as IMemberSerializationMediator<TContainingType>;
				}
			}

			if(info.HasAttribute<DontWriteAttribute>())
				mediator = new DisableWriteMemberSerializationMediatorDecorator<TContainingType>(mediator);

			if(info.HasAttribute<DontReadAttribute>())
				mediator = new DisableReadMemberSerializationMediatorDecorator<TContainingType>(mediator);

			if(info.HasAttribute<OptionalAttribute>())
				mediator = new OptionalReadWriteMemberSerializationMediatorDecorator<TContainingType>(mediator, info.GetCustomAttribute<OptionalAttribute>().MemberName);

			return mediator;
		}
	}
}