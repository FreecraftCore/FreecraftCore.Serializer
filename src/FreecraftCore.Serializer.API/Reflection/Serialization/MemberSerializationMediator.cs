using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Linq.Expressions;
using Fasterflect;
using JetBrains.Annotations;

#if NET35
using System.Reflection.Emit;
#endif

namespace FreecraftCore.Serializer
{
	//TODO: Redo doc. It's no longer valid at all.
	/// <summary>
	/// Tuple-like pair of the <see cref="MemberInfo"/> context and the corresponding
	/// <see cref="ITypeSerializerStrategy"/> for serializing the member.
	/// </summary>
	public  abstract class MemberSerializationMediator<TContainingType, TMemberType> : MemberSerializationMediator, IMemberSerializationMediator<TContainingType>
	{
		/// <summary>
		/// Delegate that can grab the <see cref="MemberInformation"/> member value.
		/// </summary>
		[NotNull]
		protected Func<TContainingType, object> MemberGetter { get; }

		[NotNull]
		protected Action<TContainingType, TMemberType> MemberAccessor { get; }

		protected MemberSerializationMediator([NotNull] MemberInfo memberInfo, [NotNull] ITypeSerializerStrategy serializer)
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
				throw new InvalidOperationException($"Failed to build {nameof(MemberSerializationMediator)} for Member: {memberInfo.Name} for Type: {typeof(TContainingType).FullName}.");;

			//The below may seem ridiculous, when we could use reflection or even fasterflect, but it makes the different
			//of almost an order of magnitude.
			//Based on: http://stackoverflow.com/questions/321650/how-do-i-set-a-field-value-in-an-c-sharp-expression-tree
#if !NET35
			//Now we need to do property setting
			ParameterExpression targetExp = Expression.Parameter(memberInfo.DeclaringType, "target");
			ParameterExpression valueExp = Expression.Parameter(typeof(TMemberType), "value");

			// Expression.Property can be used here as well
			MemberExpression memberExp = GetPropertyOrField(targetExp, memberInfo.Name);
			BinaryExpression assignExp = Expression.Assign(memberExp, valueExp);

			MemberAccessor = Expression.Lambda<Action<TContainingType, TMemberType>>(assignExp, targetExp, valueExp)
				.Compile();
#endif
			//TODO: Handle for net35. Profile fasterflect vs reflection emit
		}

		//TODO: Find out why netcore/netstandard requires this hack to get the proper property/field expression
		//Solution for failed property location here: http://stackoverflow.com/a/8042602
		private static MemberExpression GetPropertyOrField(Expression baseExpr, string name)
		{
			if (baseExpr == null)
			{
				throw new ArgumentNullException("baseExpr");
			}
			if (string.IsNullOrWhiteSpace(name))
			{
				throw new ArgumentException("name");
			}
			var type = baseExpr.Type;
			var properties = type
				.GetTypeInfo().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
				.Where(p => p.Name.Equals(name))
				.ToArray();
			if (properties.Length == 1)
			{
				var res = properties[0];
				if (res.DeclaringType != type)
				{
					// Here is the core of the fix:
					var tmp = res
						.DeclaringType
						.GetTypeInfo().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
						.Where(p => p.Name.Equals(name))
						.ToArray();
					if (tmp.Length == 1)
					{
						return Expression.Property(baseExpr, tmp[0]);
					}
				}
				return Expression.Property(baseExpr, res);
			}
			if (properties.Length != 0)
			{
				throw new NotSupportedException(name);
			}
			var fields = type
				.GetTypeInfo().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
				.Where(p => p.Name.Equals(name))
				.ToArray();
			if (fields.Length == 1)
			{
				return Expression.Field(baseExpr, fields[0]);
			}
			if (fields.Length != 0)
			{
				throw new NotSupportedException(name);
			}
			throw new ArgumentException(
				string.Format(
					"Type [{0}] does not define property/field called [{1}]"
				, type
				, name
				)
			);
		}

		public abstract void WriteMember(TContainingType obj, IWireStreamWriterStrategy dest);

		public abstract void SetMember(TContainingType obj, IWireStreamReaderStrategy source);
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
	}
}
