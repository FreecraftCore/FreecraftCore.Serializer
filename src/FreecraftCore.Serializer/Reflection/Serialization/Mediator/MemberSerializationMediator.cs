using System;
using System.Reflection;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Tuple-like pair of the <see cref="MemberInfo"/> context and the corresponding
	/// <see cref="ITypeSerializerStrategy"/> for serializing the member.
	/// </summary>
	public abstract class MemberSerializationMediator<TContainingType, TMemberType> : MemberSerializationMediator, IMemberSerializationMediator<TContainingType>
	{
		/// <summary>
		/// Delegate that can grab the <see cref="MemberInformation"/> member value.
		/// </summary>
		[NotNull]
		protected MemberGetterMediator<TContainingType> MemberGetter { get; }

		[NotNull]
		protected MemberSetterMediator<TContainingType, TMemberType> MemberSetter { get; }

		protected MemberSerializationMediator([NotNull] MemberInfo memberInfo, [NotNull] ITypeSerializerStrategy serializer)
			: base(memberInfo, serializer)
		{
			if (memberInfo == null) throw new ArgumentNullException(nameof(memberInfo));
			if (serializer == null) throw new ArgumentNullException(nameof(serializer));

			//Due to perf problems fasterflect setting wasn't fast enough.
			//Introducing a compiled lambda to delegate for get/set should provide the much needed preformance.
			MemberSetter = new MemberSetterMediator<TContainingType, TMemberType>(memberInfo);
			MemberGetter = new MemberGetterMediator<TContainingType>(memberInfo);
			//TODO: Handle for net35. Profile fasterflect vs reflection emit
		}

		public abstract void WriteMember(TContainingType obj, IWireStreamWriterStrategy dest);

		public abstract void SetMember(TContainingType obj, IWireStreamReaderStrategy source);

		/// <inheritdoc />
		public abstract Task WriteMemberAsync(TContainingType obj, IWireStreamWriterStrategyAsync dest);

		/// <inheritdoc />
		public abstract Task SetMemberAsync(TContainingType obj, IWireStreamReaderStrategyAsync source);

		public override void WriteMember(object obj, [NotNull] IWireStreamWriterStrategy dest)
		{
			if (dest == null) throw new ArgumentNullException(nameof(dest));

			WriteMember((TContainingType)obj, dest);
		}

		public override void SetMember(object obj, IWireStreamReaderStrategy source)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));

			SetMember((TContainingType)obj, source);
		}

		/// <inheritdoc />
		public override async Task WriteMemberAsync(object obj, IWireStreamWriterStrategyAsync dest)
		{
			if (dest == null) throw new ArgumentNullException(nameof(dest));

			await WriteMemberAsync((TContainingType)obj, dest)
				.ConfigureAwait(false);
		}

		/// <inheritdoc />
		public override async Task SetMemberAsync(object obj, IWireStreamReaderStrategyAsync source)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));

			await SetMemberAsync((TContainingType)obj, source)
				.ConfigureAwait(false);
		}
	}

	/// <summary>
	/// Tuple-like pair of the <see cref="MemberInfo"/> context and the corresponding
	/// <see cref="ITypeSerializerStrategy"/> for serializing the member.
	/// </summary>
	public abstract class MemberSerializationMediator : IMemberSerializationMediator
	{
		/// <summary>
		/// Cached <see cref="MemberInfo"/>.
		/// </summary>
		[NotNull]
		protected MemberInfo MemberInformation { get; }

		/// <summary>
		/// Serializer to serialize for the <see cref="MemberInformation"/>.
		/// </summary>
		[NotNull]
		protected ITypeSerializerStrategy TypeSerializer { get; }

		protected MemberSerializationMediator([NotNull] MemberInfo memberInfo, [NotNull] ITypeSerializerStrategy serializer)
		{
			if (memberInfo == null)
				throw new ArgumentNullException(nameof(memberInfo), $"Provided argument {nameof(memberInfo)} is null.");

			if (serializer == null)
				throw new ArgumentNullException(nameof(serializer), $"Provided argument {nameof(serializer)} is null.");

			MemberInformation = memberInfo;
			TypeSerializer = serializer;
		}

		public abstract void SetMember(object obj, [NotNull] IWireStreamReaderStrategy source);

		public abstract void WriteMember(object obj, [NotNull] IWireStreamWriterStrategy dest);

		/// <inheritdoc />
		public abstract Task WriteMemberAsync(object obj, IWireStreamWriterStrategyAsync dest);

		/// <inheritdoc />
		public abstract Task SetMemberAsync(object obj, IWireStreamReaderStrategyAsync source);
	}
}