using System;
using System.Linq.Expressions;
using System.Reflection;
using JetBrains.Annotations;
using Reflect.Extent;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Mediator for getting values from a member.
	/// </summary>
	/// <typeparam name="TContainingType">The type that contains the member.</typeparam>
	public sealed class MemberGetterMediator<TContainingType> : MemberReflectionMediator
	{
		/// <summary>
		/// Delegate that can grab the <see cref="MemberInformation"/> member value.
		/// </summary>
		[NotNull]
		public Func<TContainingType, object> Getter { get; }

		public MemberGetterMediator(MemberInfo memberInfo)
		{
			try
			{
				ParameterExpression instanceOfTypeToReadMemberOn = Expression.Parameter(memberInfo.DeclaringType, "instance");
				MemberExpression member = GetPropertyOrFieldExpression(instanceOfTypeToReadMemberOn, memberInfo.Name, memberInfo);
				UnaryExpression castExpression = Expression.TypeAs(member, typeof(object)); //use object to box

				//Build the getter lambda
				Getter = Expression.Lambda(castExpression, instanceOfTypeToReadMemberOn).Compile()
					as Func<TContainingType, object>;

				if(Getter == null)
					throw new InvalidOperationException($"Failed to build {nameof(MemberSerializationMediator)} for Member: {memberInfo.Name} for Type: {typeof(TContainingType).FullName}.");
			}
			catch(Exception e)
			{
				throw new InvalidOperationException($"Failed to prepare getter and setter for {typeof(TContainingType).FullName}'s {memberInfo.Type().Name} with member Name: {memberInfo.Name}.", e);
			}
		}
	}

	//TODO: Redo doc. It's no longer valid at all.
}
