using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading.Tasks;
using JetBrains.Annotations;

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

			try
			{
				ParameterExpression instanceOfTypeToReadMemberOn = Expression.Parameter(memberInfo.DeclaringType, "instance");
				MemberExpression member = GetPropertyOrFieldExpression(instanceOfTypeToReadMemberOn, memberInfo.Name, memberInfo);
				UnaryExpression castExpression = Expression.TypeAs(member, typeof(object)); //use object to box

				//Build the getter lambda
				MemberGetter = Expression.Lambda(castExpression, instanceOfTypeToReadMemberOn).Compile()
					as Func<TContainingType, object>;

				if (MemberGetter == null)
					throw new InvalidOperationException($"Failed to build {nameof(MemberSerializationMediator)} for Member: {memberInfo.Name} for Type: {typeof(TContainingType).FullName}."); ;

				//The below may seem ridiculous, when we could use reflection or even fasterflect, but it makes the different
				//of almost an order of magnitude.
				//Based on: http://stackoverflow.com/questions/321650/how-do-i-set-a-field-value-in-an-c-sharp-expression-tree
				if (memberInfo is FieldInfo && ((FieldInfo)memberInfo).IsInitOnly)
				{
					//If it's a field and we can't write to it we need to emit
					MemberAccessor = CreateSetterForReadonlyField((FieldInfo) memberInfo);
				}
				else if (memberInfo is PropertyInfo && !((PropertyInfo) memberInfo).CanWrite)
				{
					//If it's a property and it's a readonly one we'll try to grab the backing field
					MemberAccessor = CreateSetterForReadonlyField(GetFieldInfo(memberInfo.DeclaringType, $"<{memberInfo.Name}>k__BackingField"));
				}
				else
				{
					//Now we need to do property setting
					ParameterExpression targetExp = Expression.Parameter(memberInfo.DeclaringType, "target");
					ParameterExpression valueExp = Expression.Parameter(typeof(TMemberType), "value");

					// Expression.Property can be used here as well
					MemberExpression memberExp = GetPropertyOrFieldExpression(targetExp, memberInfo.Name, memberInfo);//GetPropertyOrField(targetExp, memberInfo.Name);
					BinaryExpression assignExp = Expression.Assign(memberExp, valueExp);

					MemberAccessor = Expression.Lambda<Action<TContainingType, TMemberType>>(assignExp, targetExp, valueExp)
						.Compile();
				}
			}
			catch (Exception e)
			{
				throw new InvalidOperationException($"Failed to prepare getter and setter for {typeof(TContainingType).FullName}'s {typeof(TMemberType).FullName} with member Name: {memberInfo.Name}.", e);
			}
			
			//TODO: Handle for net35. Profile fasterflect vs reflection emit
		}

		//TODO: Modified source based on Marc Gravell's readonly Protoubf-net set explaination
		//See: http://stackoverflow.com/a/17117548
		static Action<TContainingType, TMemberType> CreateSetterForReadonlyField(FieldInfo field)
		{
			//Must provide the the type to attach this method to and indicate that JIT should skip accessibility checks.
			var method = new DynamicMethod($"set_readonly_{field.Name}", null,
				new[] { typeof(TContainingType), typeof(TMemberType) }, field.DeclaringType, true);

			var il = method.GetILGenerator();
			il.Emit(OpCodes.Ldarg_0);
			il.Emit(OpCodes.Castclass, typeof(TContainingType));
			il.Emit(OpCodes.Ldarg_1);
			il.Emit(OpCodes.Stfld, field);
			il.Emit(OpCodes.Ret);
			return (Action<TContainingType, TMemberType>)method.CreateDelegate(typeof(Action<TContainingType, TMemberType>));
		}

		private static FieldInfo GetFieldInfo(Type type, string name)
		{
			FieldInfo[] fields = type.GetRuntimeFields()
				.Where(p => p.Name.Equals(name))
				.ToArray();

			if (fields.Length == 1)
			{
				//Core of the fix: if the type is not the same as the type who declared the property we should look at the declaring type
				return fields[0].DeclaringType == type ? fields[0]
					:
					fields[0].DeclaringType.GetRuntimeFields()
						.FirstOrDefault(p => p.Name.Equals(name));
			}
			else
			{
				throw new NotSupportedException(name);
			}
		}

		private static PropertyInfo GetPropertyInfo(Type type, string name)
		{
			PropertyInfo[] properties = type.GetRuntimeProperties()
				.Where(p => p.Name.Equals(name))
				.ToArray();

			if (properties.Length == 1)
			{
				//Core of the fix: if the type is not the same as the type who declared the property we should look at the declaring type
				return properties[0].DeclaringType == type ? properties[0]
					: properties[0].DeclaringType.GetRuntimeProperties()
						.FirstOrDefault(p => p.Name.Equals(name));
			}
			else
			{
				throw new NotSupportedException(name);
			}
		}

		//TODO: Figure out why we have to do this in later versions of .NET/netstandard
		//We have to use this hack to handle properties from inherited classes
		//See: http://stackoverflow.com/a/8042602
		private static MemberExpression GetPropertyOrFieldExpression(Expression baseExpr, string name, MemberInfo info)
		{
			if (baseExpr == null) throw new ArgumentNullException(nameof(baseExpr));
			if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException(nameof(name));

			Type type = baseExpr.Type;

			//TODO: Refactor
			if (info is PropertyInfo)
			{
				return Expression.Property(baseExpr, GetPropertyInfo(type, name));
			}
			else if (info is FieldInfo)
			{
				return Expression.Field(baseExpr, GetFieldInfo(type, name));
			}

			throw new NotSupportedException($"Provided member Name: {name} is neither a field nor a property.");
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

			await WriteMemberAsync((TContainingType)obj, dest);
		}

		/// <inheritdoc />
		public override async Task SetMemberAsync(object obj, IWireStreamReaderStrategyAsync source)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));

			await SetMemberAsync((TContainingType)obj, source);
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
