using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using FreecraftCore.Serializer.Internal;
using Glader.Essentials;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FreecraftCore.Serializer
{
	public sealed class ArrayTypeSerializationStatementsBlockEmitter : BaseInvokationExpressionEmitter<IArrayTypeSymbol>
	{
		public ArrayTypeSerializationStatementsBlockEmitter([NotNull] IArrayTypeSymbol actualType, [NotNull] ISymbol member, SerializationMode mode) 
			: base(actualType, member, mode)
		{

		}

		public override InvocationExpressionSyntax Create()
		{
			//Overriding default behavior.
			bool hasSaneDefaults = ActualType.HasAttributeExact<WireSaneDefaultsAttribute>(true);

			//TODO: Support seperated collection sizes
			//Case where the array will be send with length-prefixed size

			//Prefer length-prefixing for performance, that is why it's the sane default.
			if (Member.HasAttributeExact<SendSizeAttribute>() || (hasSaneDefaults && !Member.HasAttributeExact<KnownSizeAttribute>())) //SANE DEFAULT
			{
				IInvokationExpressionEmittable invokable;

				//Only other case is sane defaults are enabled.
				if (Member.HasAttributeExact<SendSizeAttribute>())
					invokable = CreateSendSizeExpressionEmitter(Member.GetAttributeExact<SendSizeAttribute>());
				else
					invokable = CreateSendSizeExpressionEmitter(PrimitiveSizeType.Int32); //SANE DEFAULT size

				var generator = new ArraySerializerGenerator(ActualType, Member, Mode, invokable);
				return generator.Create();
			}
			else if (Member.HasAttributeExact<KnownSizeAttribute>())
			{
				var generator = new ArraySerializerGenerator(ActualType, Member, Mode, CreateFixedSizeExpressionEmitter(Member.GetAttributeExact<KnownSizeAttribute>()));
				return generator.Create();
			}
			else
			{
				//TODO: If this isn't the LAST member then there is issues!!
				//Assume it's write ALL and then other side will need to ReadToEnd (but the attribute isn't needed)
				var defaultGenerator = new ArraySerializerGenerator(ActualType, Member, Mode, CreateDefaultExpressionEmitter());
				return defaultGenerator.Create();
			}
		}

		private IInvokationExpressionEmittable CreateDefaultExpressionEmitter()
		{
			if (ActualType.ElementType.IsPrimitive())
				return new DefaultPrimitiveArrayInvokationExpressionEmitter(ActualType, Member, Mode);
			else
				return new DefaultComplexArrayInvokationExpressionEmitter(ActualType, Member, Mode);
		}

		private IInvokationExpressionEmittable CreateFixedSizeExpressionEmitter([NotNull] AttributeData knownSizeAttri)
		{
			if (knownSizeAttri == null) throw new ArgumentNullException(nameof(knownSizeAttri));

			//TODO: If this ever changes we're fucked.
			int size = int.Parse(knownSizeAttri.ConstructorArguments.First().ToCSharpString());

			if (ActualType.ElementType.IsPrimitive())
				return new FixedSizePrimitiveArrayInvokationExpressionEmitter(ActualType, Member, size, Mode);
			else
				return new FixedSizeComplexArrayInvokationExpressionEmitter(ActualType, Member, size, Mode);
		}

		private IInvokationExpressionEmittable CreateSendSizeExpressionEmitter([NotNull] AttributeData sendSizeAttri)
		{
			if (sendSizeAttri == null) throw new ArgumentNullException(nameof(sendSizeAttri));

			//TODO: If this ever changes we're fucked.
			PrimitiveSizeType sizeType = InternalEnumExtensions.ParseFull<PrimitiveSizeType>(sendSizeAttri.ConstructorArguments.First().ToCSharpString(), true);

			return CreateSendSizeExpressionEmitter(sizeType);
		}

		private IInvokationExpressionEmittable CreateSendSizeExpressionEmitter(PrimitiveSizeType sizeType)
		{
			if (ActualType.ElementType.IsPrimitive())
				return new SendSizePrimitiveArrayInvokationExpressionEmitter(ActualType, Member, sizeType, Mode);
			else
				return new SendSizeComplexArrayInvokationExpressionEmitter(ActualType, Member, sizeType, Mode);
		}
	}
}
