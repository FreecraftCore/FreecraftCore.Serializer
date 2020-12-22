using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using FreecraftCore.Serializer.Internal;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FreecraftCore.Serializer
{
	public sealed class EnumTypeSerializerStatementsBlockEmitter : BaseInvokationExpressionEmitter
	{
		public EnumTypeSerializerStatementsBlockEmitter([NotNull] ITypeSymbol actualType, [NotNull] ISymbol member, SerializationMode mode) 
			: base(actualType, member, mode)
		{

		}

		public override InvocationExpressionSyntax Create()
		{
			//TODO: Support custom primitivie specifier.
			//Primitive serialization YAY
			if (!Member.HasAttributeExact<EnumStringAttribute>())
			{
				//What type to serialize the enum as
				string serializeAsType = ComputeTypeToSerializeTo();

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

		private string ComputeTypeToSerializeTo()
		{
			if (Member.HasAttributeExact<EnumSizeAttribute>())
			{
				PrimitiveSizeType sizeType = EnumSizeAttribute.Parse(Member.GetAttributeExact<EnumSizeAttribute>().ConstructorArguments.First().ToCSharpString());
				return sizeType.ToString();
			}
			else
			{
				//This is hacky but covers the case that enum array types can use this emitter.
				if (ActualType.IsEnumType())
					return ActualType.GetEnumUnderlyingType().Name;
				else if (ActualType is IArrayTypeSymbol arraySymbol)
					return arraySymbol.ElementType.GetEnumUnderlyingType().Name;
				else
					throw new NotSupportedException($"Type: {ActualType.Name} cannot emit enum serialization.");
			}
		}
	}
}
