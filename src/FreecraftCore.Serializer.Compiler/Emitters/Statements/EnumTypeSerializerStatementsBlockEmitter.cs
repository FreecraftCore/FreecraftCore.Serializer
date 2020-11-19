using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FreecraftCore.Serializer
{
	public sealed class EnumTypeSerializerStatementsBlockEmitter : BaseInvokationExpressionEmitter
	{
		public EnumTypeSerializerStatementsBlockEmitter([NotNull] Type actualType, [NotNull] MemberInfo member, SerializationMode mode) 
			: base(actualType, member, mode)
		{

		}

		public override InvocationExpressionSyntax Create()
		{
			EnumStringAttribute enumStringAttri = Member.GetCustomAttribute<EnumStringAttribute>();
			EnumSizeAttribute enumSizeAttri = Member.GetCustomAttribute<EnumSizeAttribute>();

			//TODO: Support custom primitivie specifier.
			//Primitive serialization YAY
			if (enumStringAttri == null)
			{
				//What type to serialize the enum as
				Type serializeAsType = enumSizeAttri == null ? ActualType.GetEnumUnderlyingType() : Type.GetType($"System.{enumSizeAttri.SizeType.ToString()}", true);

				var generator = new RawEnumPrimitiveSerializationGenerator(ActualType, Member, Mode, serializeAsType);
				return generator.Create();
			}
			else
			{
				//They want STRING serialization for the enum, so defer it to the string type serializer.
				InvocationExpressionSyntax invocation = new StringTypeSerializationStatementsBlockEmitter(ActualType, Member, Mode)
					.Create();

				if (Mode == SerializationMode.Write)
					return invocation;

				//We need special handling when READ mode, because we need to parse back to enum
				//which by the way is QUITE slow.
				//Enum.Parse<TType>
				EnumParseInvocationExpressionEmitter emitter = new EnumParseInvocationExpressionEmitter(ActualType, Member, invocation);
				return emitter.Create();
			}
		}
	}
}
