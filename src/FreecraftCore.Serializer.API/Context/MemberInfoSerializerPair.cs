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
	/// <summary>
	/// Tuple-like pair of the <see cref="MemberInfo"/> context and the corresponding
	/// <see cref="ITypeSerializerStrategy"/> for serializing the member.
	/// </summary>
	public class MemberAndSerializerPair<TContainingType> : MemberAndSerializerPair
	{
		/// <summary>
		/// Delegate that can grab the <see cref="MemberInformation"/> member value.
		/// </summary>
		[NotNull]
		public Func<TContainingType, object> MemberGetter { get; }

		public MemberAndSerializerPair([NotNull] MemberInfo memberInfo, [NotNull] ITypeSerializerStrategy serializer)
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
				throw new InvalidOperationException($"Failed to build {nameof(MemberAndSerializerPair)} for Member: {memberInfo.Name} for Type: {typeof(TContainingType).FullName}.");;
		}
	}

	/// <summary>
	/// Tuple-like pair of the <see cref="MemberInfo"/> context and the corresponding
	/// <see cref="ITypeSerializerStrategy"/> for serializing the member.
	/// </summary>
	public abstract class MemberAndSerializerPair
	{
		/// <summary>
		/// Cached <see cref="MemberInfo"/>.
		/// </summary>
		[NotNull]
		public MemberInfo MemberInformation { get; }

		/// <summary>
		/// Serializer to serialize for the <see cref="MemberInformation"/>.
		/// </summary>
		[NotNull]
		public ITypeSerializerStrategy TypeSerializer { get; }

		protected MemberAndSerializerPair([NotNull] MemberInfo memberInfo, [NotNull] ITypeSerializerStrategy serializer)
		{
			if (memberInfo == null)
				throw new ArgumentNullException(nameof(memberInfo), $"Provided argument {nameof(memberInfo)} is null.");

			if (serializer == null)
				throw new ArgumentNullException(nameof(serializer), $"Provided argument {nameof(serializer)} is null.");

			MemberInformation = memberInfo;
			TypeSerializer = serializer;

			
		}
	}
}
