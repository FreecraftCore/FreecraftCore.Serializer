using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace FreecraftCore.Serializer
{
	public abstract class MemberReflectionMediator
	{
		protected static FieldInfo GetFieldInfo(Type type, string name)
		{
			FieldInfo[] fields = type.GetRuntimeFields()
				.Where(p => p.Name.Equals(name))
				.ToArray();

			if(fields.Length == 1)
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

		//TODO: Figure out why we have to do this in later versions of .NET/netstandard
		//We have to use this hack to handle properties from inherited classes
		//See: http://stackoverflow.com/a/8042602
		protected static MemberExpression GetPropertyOrFieldExpression(Expression baseExpr, string name, MemberInfo info)
		{
			if(baseExpr == null) throw new ArgumentNullException(nameof(baseExpr));
			if(string.IsNullOrWhiteSpace(name)) throw new ArgumentException(nameof(name));

			Type type = baseExpr.Type;

			//TODO: Refactor
			if(info is PropertyInfo)
			{
				return Expression.Property(baseExpr, (PropertyInfo)GetPropertyInfo(type, name));
			}
			else if(info is FieldInfo)
			{
				return Expression.Field(baseExpr, (FieldInfo)GetFieldInfo(type, name));
			}

			throw new NotSupportedException($"Provided member Name: {name} is neither a field nor a property.");
		}

		protected static PropertyInfo GetPropertyInfo(Type type, string name)
		{
			PropertyInfo[] properties = type.GetRuntimeProperties()
				.Where(p => p.Name.Equals(name))
				.ToArray();

			if(properties.Length == 1)
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
	}
}