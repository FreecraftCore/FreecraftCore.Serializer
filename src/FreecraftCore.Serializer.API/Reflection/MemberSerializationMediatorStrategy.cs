using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using System.Linq.Expressions;
using Fasterflect;
using JetBrains.Annotations;

namespace FreecraftCore.Serializer
{
	//TODO: Redo doc. It's no longer valid at all.
	/// <summary>
	/// Tuple-like pair of the <see cref="MemberInfo"/> context and the corresponding
	/// <see cref="ITypeSerializerStrategy"/> for serializing the member.
	/// </summary>
	public class MemberSerializationMediatorStrategy<TContainingType> : MemberSerializationMediatorStrategy, IMemberSerializationMediatorStrategy<TContainingType>
	{
		/// <summary>
		/// Delegate that can grab the <see cref="MemberInformation"/> member value.
		/// </summary>
		[NotNull]
		protected Func<TContainingType, object> MemberGetter { get; }

		public MemberSerializationMediatorStrategy([NotNull] MemberInfo memberInfo, [NotNull] ITypeSerializerStrategy serializer)
			: base(memberInfo, serializer)
		{
			if (memberInfo == null) throw new ArgumentNullException(nameof(memberInfo));
			if (serializer == null) throw new ArgumentNullException(nameof(serializer));

			//Due to perf problems fasterflect setting wasn't fast enough.
			//Introducing a compiled lambda to delegate for get/set should provide the much needed preformance.

			ParameterExpression instanceOfTypeToReadMemberOn = Expression.Parameter(memberInfo.DeclaringType, "instance");
			MemberExpression member = Expression.PropertyOrField(instanceOfTypeToReadMemberOn, memberInfo.Name);
			UnaryExpression castExpression = Expression.TypeAs(member, typeof(object)); //use object to box

			//Build the getter lambda
			MemberGetter = Expression.Lambda(castExpression, instanceOfTypeToReadMemberOn).Compile()
				as Func<TContainingType, object>;

			if(MemberGetter == null)
				throw new InvalidOperationException($"Failed to build {nameof(MemberSerializationMediatorStrategy)} for Member: {memberInfo.Name} for Type: {typeof(TContainingType).FullName}.");;
		}

		public void ReadMember([NotNull] TContainingType obj, [NotNull] IWireMemberWriterStrategy dest)
		{
			if (obj == null) throw new ArgumentNullException(nameof(obj));
			if (dest == null) throw new ArgumentNullException(nameof(dest));

			//TODO: Check how TC handles optionals or nulls.
			//Do we write nothing? Do we write 0?
			//object memberValue = value.TryGetValue(serializerInfo.MemberInformation.Name);
			object memberValue = MemberGetter(obj); //instead of fasterflect we use delegate to getter

			if (memberValue == null)
				throw new InvalidOperationException($"Provider FieldName: {MemberInformation.Name} on Type: {MemberInformation.Type()} is null. The serializer doesn't support null.");

			try
			{
				TypeSerializer.Write(memberValue, dest);
			}
			catch (NullReferenceException e)
			{
				throw new InvalidOperationException($"Serializer failed to find serializer for member name: {MemberInformation.Name} and type: {MemberInformation.Type()}.", e);
			}
		}

		public override void ReadMember(object obj, [NotNull] IWireMemberWriterStrategy dest)
		{
			ReadMember((TContainingType)obj, dest);
		}

		public override void SetMember(object obj, IWireMemberReaderStrategy source)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));

			SetMember((TContainingType)obj, source);
		}

		public void SetMember(TContainingType obj, [NotNull] IWireMemberReaderStrategy source)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));

			//TODO: Can we do this more efficiently without reading string name?
			obj.TrySetValue(MemberInformation.Name, TypeSerializer.Read(source));
		}
	}

	/// <summary>
	/// Tuple-like pair of the <see cref="MemberInfo"/> context and the corresponding
	/// <see cref="ITypeSerializerStrategy"/> for serializing the member.
	/// </summary>
	public abstract class MemberSerializationMediatorStrategy : IMemberSerializationMediatorStrategy
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

		protected MemberSerializationMediatorStrategy([NotNull] MemberInfo memberInfo, [NotNull] ITypeSerializerStrategy serializer)
		{
			if (memberInfo == null)
				throw new ArgumentNullException(nameof(memberInfo), $"Provided argument {nameof(memberInfo)} is null.");

			if (serializer == null)
				throw new ArgumentNullException(nameof(serializer), $"Provided argument {nameof(serializer)} is null.");

			MemberInformation = memberInfo;
			TypeSerializer = serializer;
		}

		public abstract void SetMember(object obj, [NotNull] IWireMemberReaderStrategy source);

		public abstract void ReadMember(object obj, [NotNull] IWireMemberWriterStrategy dest);
	}
}
