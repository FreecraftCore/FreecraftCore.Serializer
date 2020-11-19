using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FreecraftCore.Serializer
{
	public sealed class ArrayTypeSerializationStatementsBlockEmitter : BaseSerializationStatementsBlockEmitter
	{
		public ArrayTypeSerializationStatementsBlockEmitter([NotNull] Type actualType, [NotNull] MemberInfo member, SerializationMode mode) 
			: base(actualType, member, mode)
		{

		}

		public override List<StatementSyntax> CreateStatements()
		{
			List<StatementSyntax> statements = new List<StatementSyntax>();

			//Firstly, we must know if there are any attributes
			SendSizeAttribute sendSizeAttri = Member.GetCustomAttribute<SendSizeAttribute>();
			KnownSizeAttribute knownSizeAttri = Member.GetCustomAttribute<KnownSizeAttribute>();

			if (sendSizeAttri != null && knownSizeAttri != null)
				throw new InvalidOperationException($"Emit failed for Member: {ActualType} in Type: {Member.DeclaringType}. Cannot use Attributes: {nameof(SendSizeAttribute)} and {nameof(KnownSizeAttribute)} together.");

			//TODO: Support seperated collection sizes
			//Case where the array will be send with length-prefixed size
			if (sendSizeAttri != null)
			{
				var generator = new ArraySerializerGenerator(Member.Name, CreateSendSizeExpressionEmitter(sendSizeAttri), Mode);
				statements.Add(generator.Create());
			}

			if (knownSizeAttri != null)
			{
				var generator = new ArraySerializerGenerator(Member.Name, CreateFixedSizeExpressionEmitter(knownSizeAttri), Mode);
				statements.Add(generator.Create());
			}

			//TODO: If this isn't the LAST member then there is issues!!
			//Assume it's write ALL and then other side will need to ReadToEnd (but the attribute isn't needed)
			if (knownSizeAttri == null && sendSizeAttri == null)
			{
				var generator = new ArraySerializerGenerator(Member.Name, CreateDefaultExpressionEmitter(), Mode);
				statements.Add(generator.Create());
			}

			return statements;
		}

		private IInvokationExpressionEmittable CreateDefaultExpressionEmitter()
		{
			if (ActualType.GetElementType().IsPrimitive)
				return new DefaultPrimitiveArrayInvokationExpressionEmitter(ActualType.GetElementType(), Mode);
			else
				return new DefaultComplexArrayInvokationExpressionEmitter(ActualType.GetElementType(), Mode);
		}

		private IInvokationExpressionEmittable CreateFixedSizeExpressionEmitter([NotNull] KnownSizeAttribute knownSizeAttri)
		{
			if (knownSizeAttri == null) throw new ArgumentNullException(nameof(knownSizeAttri));

			if (ActualType.GetElementType().IsPrimitive)
				return new FixedSizePrimitiveArrayInvokationExpressionEmitter(ActualType.GetElementType(), knownSizeAttri.KnownSize, Mode);
			else
				return new FixedSizeComplexArrayInvokationExpressionEmitter(ActualType.GetElementType(), knownSizeAttri.KnownSize, Mode);
		}

		private IInvokationExpressionEmittable CreateSendSizeExpressionEmitter([NotNull] SendSizeAttribute sendSizeAttri)
		{
			if (sendSizeAttri == null) throw new ArgumentNullException(nameof(sendSizeAttri));

			if(ActualType.GetElementType().IsPrimitive)
				return new SendSizePrimitiveArrayInvokationExpressionEmitter(ActualType.GetElementType(), sendSizeAttri.TypeOfSize, Mode);
			else
				return new SendSizeComplexArrayInvokationExpressionEmitter(ActualType.GetElementType(), sendSizeAttri.TypeOfSize, Mode);
		}
	}
}
