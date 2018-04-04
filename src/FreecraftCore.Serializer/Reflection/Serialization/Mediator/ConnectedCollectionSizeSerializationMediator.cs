using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Generic.Math;
using JetBrains.Annotations;
using Reinterpret.Net;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Serialization mediator decorator that adds collection inspection to determine the the value of the field/property.
	/// Access the collection to check its size and write.
	/// </summary>
	/// <typeparam name="TContainingType"></typeparam>
	/// <typeparam name="TSizeType"></typeparam>
	public sealed class ConnectedCollectionSizeSerializationMediator<TContainingType, TSizeType> : MemberSerializationMediator<TContainingType, TSizeType>
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
		private MemberGetterMediator<TContainingType> CollectionGetter { get; }

		/// <inheritdoc />
		public ConnectedCollectionSizeSerializationMediator([NotNull] MemberInfo sizeMemberInfo, [NotNull] MemberInfo collectionMemberInfo, [NotNull] ITypeSerializerStrategy serializer, [NotNull] IMemberSerializationMediator<TContainingType> decoratedMediator) 
			: base(sizeMemberInfo, serializer)
		{
			if(collectionMemberInfo == null) throw new ArgumentNullException(nameof(collectionMemberInfo));
			if(decoratedMediator == null) throw new ArgumentNullException(nameof(decoratedMediator));

			CollectionGetter = new MemberGetterMediator<TContainingType>(collectionMemberInfo);
			DecoratedMediator = decoratedMediator;
		}

		/// <inheritdoc />
		public override void WriteMember(TContainingType obj, IWireStreamWriterStrategy dest)
		{
			WriteCollectionSizeToField(obj);

			//TODO: Should we spend CPU cycles to reset the original value? This kind of changes the state of the DTO
			DecoratedMediator.WriteMember(obj, dest);
		}

		private void WriteCollectionSizeToField(TContainingType obj)
		{
			//We must first access the collection to get the size and then write it into the field
			//before we try to truly serialize it
			ICollection enumerable = CollectionGetter.Getter(obj) as ICollection;

			if(enumerable == null)
				throw new InvalidOperationException($"Tried to read the size of collection in Type: {typeof(TContainingType).Name} but did not implement {nameof(ICollection)}.");

			MemberSetter.Setter(obj, GenericMath.Convert<int, TSizeType>(enumerable.Count));
		}

		/// <inheritdoc />
		public override void SetMember(TContainingType obj, IWireStreamReaderStrategy source)
		{
			//We don't need to modify how we set the size member
			DecoratedMediator.SetMember(obj, source);
		}

		/// <inheritdoc />
		public override Task WriteMemberAsync(TContainingType obj, IWireStreamWriterStrategyAsync dest)
		{
			WriteCollectionSizeToField(obj);

			return DecoratedMediator.WriteMemberAsync(obj, dest);
		}

		/// <inheritdoc />
		public override Task SetMemberAsync(TContainingType obj, IWireStreamReaderStrategyAsync source)
		{
			//We don't need to modify how we set the size member
			return DecoratedMediator.SetMemberAsync(obj, source);
		}
	}
}
