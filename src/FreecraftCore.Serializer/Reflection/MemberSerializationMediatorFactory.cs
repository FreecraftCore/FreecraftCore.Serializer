using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
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
			foreach (var attri in info.DeclaringType.GetCustomAttributes<SeperatedCollectionSizeAttribute>())
			{
				//[NotNull] MemberInfo sizeMemberInfo, [NotNull] MemberInfo collectionMemberInfo, [NotNull] ITypeSerializerStrategy serializer, [NotNull] IMemberSerializationMediator<TContainingType> decoratedMediator) 
				if(attri.SizePropertyName == info.Name)
					mediator = Activator.CreateInstance(typeof(ConnectedCollectionSizeSerializationMediator<,>).MakeGenericType(new Type[] {typeof(TContainingType), info.Type()}),
						info, info.DeclaringType.GetMember(attri.CollectionPropertyName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public), strategy, mediator) as IMemberSerializationMediator<TContainingType>;

				//TODO: Undefined behavior if we have to decorate this multiple times
				if(attri.CollectionPropertyName == info.Name)
					mediator = Activator.CreateInstance(typeof(ConnectedCollectionCollectionSerializationMediator<,>).MakeGenericType(new Type[] { typeof(TContainingType), info.Type() }),
						info, info.DeclaringType.GetMember(attri.SizePropertyName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public), strategy, mediator) as IMemberSerializationMediator<TContainingType>;
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

	/// <summary>
	/// Optional read/write serialization mediator. Will
	/// prevent reading/writing based on optional attribute.
	/// </summary>
	/// <typeparam name="TContainingType"></typeparam>
	public sealed class OptionalReadWriteMemberSerializationMediatorDecorator<TContainingType> : IMemberSerializationMediator<TContainingType>
	{
		/// <summary>
		/// The serialization mediator to decorate.
		/// </summary>
		public IMemberSerializationMediator<TContainingType> DecoratedMediator { get; }

		/// <summary>
		/// The mediator that gets the bool value indicating if we should read/write
		/// with the decorated mediator.
		/// </summary>
		private MemberGetterMediator<TContainingType> isReadWriteEnabledGetter { get; }

		public OptionalReadWriteMemberSerializationMediatorDecorator([NotNull] IMemberSerializationMediator<TContainingType> mediator, [NotNull] string memberName)
		{
			if(mediator == null) throw new ArgumentNullException(nameof(mediator));
			if(string.IsNullOrWhiteSpace(memberName)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(memberName));

			//Get member and check no ambious matches
			MemberInfo[] info = typeof(TContainingType)
				.GetTypeInfo()
				.GetMember(memberName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);

			if(info == null || info.Length == 0)
				throw new InvalidOperationException($"Didn't find Member: {memberName} for Optional functionality on Type: {typeof(TContainingType).Name}. Make sure the member is implemented on the Type. Use nameof for compile time safety.");
			if(info.Length > 1)
				throw new InvalidOperationException($"Found ambigious Member: {memberName} for Optional functionality on Type: {typeof(TContainingType).Name}.");

			isReadWriteEnabledGetter = new MemberGetterMediator<TContainingType>(info.First());

			DecoratedMediator = mediator;
		}

		/// <inheritdoc />
		public async Task WriteMemberAsync(object obj, IWireStreamWriterStrategyAsync dest)
		{
			//Check if we should read
			if(!(bool)isReadWriteEnabledGetter.Getter((TContainingType)obj))
				return;

			await DecoratedMediator.WriteMemberAsync(obj, dest);
		}

		/// <inheritdoc />
		public async Task SetMemberAsync(object obj, IWireStreamReaderStrategyAsync source)
		{
			//Check if we should read
			if(!(bool)isReadWriteEnabledGetter.Getter((TContainingType)obj))
				return;

			await DecoratedMediator.SetMemberAsync(obj, source);
		}

		/// <inheritdoc />
		public async Task WriteMemberAsync(TContainingType obj, IWireStreamWriterStrategyAsync dest)
		{
			//Check if we should read
			if(!(bool)isReadWriteEnabledGetter.Getter(obj))
				return;

			await DecoratedMediator.WriteMemberAsync(obj, dest);
		}

		/// <inheritdoc />
		public async Task SetMemberAsync(TContainingType obj, IWireStreamReaderStrategyAsync source)
		{
			//Check if we should read
			if(!(bool)isReadWriteEnabledGetter.Getter(obj))
				return;

			await DecoratedMediator.SetMemberAsync(obj, source);
		}

		/// <inheritdoc />
		public void WriteMember(object obj, IWireStreamWriterStrategy dest)
		{
			//Check if we should read
			if(!(bool)isReadWriteEnabledGetter.Getter((TContainingType)obj))
				return;

			DecoratedMediator.WriteMember(obj, dest);
		}

		/// <inheritdoc />
		public void SetMember(object obj, IWireStreamReaderStrategy source)
		{
			//Check if we should read
			if(!(bool)isReadWriteEnabledGetter.Getter((TContainingType)obj))
				return;

			DecoratedMediator.SetMember(obj, source);
		}

		/// <inheritdoc />
		public void WriteMember(TContainingType obj, IWireStreamWriterStrategy dest)
		{
			//Check if we should read
			if(!(bool)isReadWriteEnabledGetter.Getter(obj))
				return;

			DecoratedMediator.WriteMember(obj, dest);
		}

		/// <inheritdoc />
		public void SetMember(TContainingType obj, IWireStreamReaderStrategy source)
		{
			//Check if we should read
			if(!(bool)isReadWriteEnabledGetter.Getter(obj))
				return;

			DecoratedMediator.SetMember(obj, source);
		}
	}
}