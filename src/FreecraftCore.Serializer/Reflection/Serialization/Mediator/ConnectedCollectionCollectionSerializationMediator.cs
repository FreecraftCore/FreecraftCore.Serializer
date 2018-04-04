using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Generic.Math;
using JetBrains.Annotations;
using Reinterpret.Net;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Serialization mediator decorator that adds size inspection to reinsert and handle the size of a colleciton from
	/// its field/property.
	/// Access the size of the collection and reinsert into the stream to allow reading.
	/// </summary>
	/// <typeparam name="TContainingType"></typeparam>
	/// <typeparam name="TSizeType"></typeparam>
	public sealed class ConnectedCollectionCollectionSerializationMediator<TContainingType, TSizeType, TMemberType> : MemberSerializationMediator<TContainingType, TMemberType>
		where TSizeType : struct
	{
		/// <summary>
		/// The serialization mediator we're decorating.
		/// </summary>
		[NotNull]
		private IMemberSerializationMediator<TContainingType> DecoratedMediator { get; }

		/// <summary>
		/// Getter that provides access to the collection
		/// </summary>
		private MemberGetterMediator<TContainingType> SizeMemberGetter { get; }

		private int SizeOfCollectionSizeType { get; } = Marshal.SizeOf<TSizeType>();

		/// <inheritdoc />
		public ConnectedCollectionCollectionSerializationMediator([NotNull] MemberInfo collectionMemberInfo, [NotNull] MemberInfo sizeMemberInfo, [NotNull] ITypeSerializerStrategy serializer, [NotNull] IMemberSerializationMediator<TContainingType> decoratedMediator)
			: base(collectionMemberInfo, serializer)
		{
			if(collectionMemberInfo == null) throw new ArgumentNullException(nameof(collectionMemberInfo));
			if(decoratedMediator == null) throw new ArgumentNullException(nameof(decoratedMediator));

			SizeMemberGetter = new MemberGetterMediator<TContainingType>(sizeMemberInfo);
			DecoratedMediator = decoratedMediator;
		}

		/// <inheritdoc />
		public override void WriteMember(TContainingType obj, IWireStreamWriterStrategy dest)
		{
			//TODO: Should we spend CPU cycles to reset the original value? This kind of changes the state of the DTO
			DecoratedMediator.WriteMember(obj, new SkipSomeBytesWireStreamWriterStrategyDecorator(dest, SizeOfCollectionSizeType));
		}

		/// <inheritdoc />
		public override void SetMember(TContainingType obj, IWireStreamReaderStrategy source)
		{
			//We need to reconvert the size back to bytes to reinsert into the stream
			//so that the collection can recieve it and then be able to deserialize the elements.
			byte[] bytes = ((TSizeType)SizeMemberGetter.Getter(obj)).Reinterpret();

			//We don't need to modify how we set the size member
			DecoratedMediator.SetMember(obj, source.PreprendWithBytes(bytes));
		}

		/// <inheritdoc />
		public override Task WriteMemberAsync(TContainingType obj, IWireStreamWriterStrategyAsync dest)
		{
			return DecoratedMediator.WriteMemberAsync(obj, new SkipSomeBytesWireStreamWriterStrategyDecoratorAsync(dest, SizeOfCollectionSizeType));
		}

		/// <inheritdoc />
		public override Task SetMemberAsync(TContainingType obj, IWireStreamReaderStrategyAsync source)
		{
			//We need to reconvert the size back to bytes to reinsert into the stream
			//so that the collection can recieve it and then be able to deserialize the elements.
			byte[] bytes = ((TSizeType)SizeMemberGetter.Getter(obj)).Reinterpret();

			//We don't need to modify how we set the size member
			return DecoratedMediator.SetMemberAsync(obj, source.PreprendWithBytesAsync(bytes));
		}
	}
}
