using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using JetBrains.Annotations;

namespace FreecraftCore.Serializer
{
	public sealed class MemberSetterMediator<TContainingType, TMemberType> : MemberReflectionMediator
	{
		[NotNull]
		public Action<TContainingType, TMemberType> Setter { get; }

		//Do a global by generic Type lock
		private static object SyncObj = new object();

		public MemberSetterMediator([NotNull] MemberInfo memberInfo)
		{
			try
			{
				//The below may seem ridiculous, when we could use reflection or even fasterflect, but it makes the different
				//of almost an order of magnitude.
				//Based on: http://stackoverflow.com/questions/321650/how-do-i-set-a-field-value-in-an-c-sharp-expression-tree
				if(memberInfo is FieldInfo && ((FieldInfo)memberInfo).IsInitOnly)
				{
					//If it's a field and we can't write to it we need to emit
					Setter = CreateSetterForReadonlyField((FieldInfo)memberInfo);
				}
				else if(memberInfo is PropertyInfo && !((PropertyInfo)memberInfo).CanWrite)
				{
					//If it's a property and it's a readonly one we'll try to grab the backing field
					Setter = CreateSetterForReadonlyField(MemberReflectionMediator.GetFieldInfo(memberInfo.DeclaringType, $"<{memberInfo.Name}>k__BackingField"));
				}
				else
				{
					//Now we need to do property setting
					ParameterExpression targetExp = Expression.Parameter(memberInfo.DeclaringType, "target");
					ParameterExpression valueExp = Expression.Parameter(typeof(TMemberType), "value");

					// Expression.Property can be used here as well
					MemberExpression memberExp = MemberReflectionMediator.GetPropertyOrFieldExpression(targetExp, memberInfo.Name, memberInfo); //GetPropertyOrField(targetExp, memberInfo.Name);
					BinaryExpression assignExp = Expression.Assign(memberExp, valueExp);

					Setter = Expression.Lambda<Action<TContainingType, TMemberType>>(assignExp, targetExp, valueExp)
						.Compile();
				}
			}
			catch(Exception e)
			{
				throw new InvalidOperationException($"Failed to prepare getter and setter for {typeof(TContainingType).FullName}'s {typeof(TMemberType).FullName} with member Name: {memberInfo.Name}.", e);
			}
		}

		//TODO: Modified source based on Marc Gravell's readonly Protoubf-net set explaination
		//See: http://stackoverflow.com/a/17117548
		static Action<TContainingType, TMemberType> CreateSetterForReadonlyField(FieldInfo field)
		{
			//We lock because it's possible this type already has a setter
			lock(SyncObj)
			{
				//Check first if the method exists
				string methodName = $"set_readonly_{field.Name}";

				MethodInfo setterMethod = typeof(TContainingType).GetTypeInfo().GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic);
				if(setterMethod != null)
					return (Action<TContainingType, TMemberType>)setterMethod.CreateDelegate(typeof(Action<TContainingType, TMemberType>));

				//Must provide the the type to attach this method to and indicate that JIT should skip accessibility checks.
				var method = new DynamicMethod(methodName, null,
					new[] { typeof(TContainingType), typeof(TMemberType) }, field.DeclaringType, true);

				var il = method.GetILGenerator();
				il.Emit(OpCodes.Ldarg_0);
				il.Emit(OpCodes.Castclass, typeof(TContainingType));
				il.Emit(OpCodes.Ldarg_1);
				il.Emit(OpCodes.Stfld, field);
				il.Emit(OpCodes.Ret);
				return (Action<TContainingType, TMemberType>)method.CreateDelegate(typeof(Action<TContainingType, TMemberType>));
			}
		}
	}
}