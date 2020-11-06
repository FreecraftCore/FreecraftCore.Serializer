using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FreecraftCore.Serializer
{
	public sealed class PrimitiveArrayTypeSerializationStatementsBlockEmitter : BaseSerializationStatementsBlockEmitter
	{
		public PrimitiveArrayTypeSerializationStatementsBlockEmitter([NotNull] Type actualType, [NotNull] MemberInfo member) 
			: base(actualType, member)
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
				var generator = new PrimitiveArraySerializerGenerator(Member.Name, new SendSizePrimitiveArrayInvokationExpressionEmitter(ActualType.GetElementType(), sendSizeAttri.TypeOfSize));
				statements.Add(generator.Create());
			}

			if (knownSizeAttri != null)
			{
				var generator = new PrimitiveArraySerializerGenerator(Member.Name, new FixedSizePrimitiveArrayInvokationExpressionEmitter(ActualType.GetElementType(), knownSizeAttri.KnownSize));
				statements.Add(generator.Create());
			}

			//TODO: If this isn't the LAST member then there is issues!!
			//Assume it's write ALL and then other side will need to ReadToEnd (but the attribute isn't needed)
			if (knownSizeAttri == null && sendSizeAttri == null)
			{
				var generator = new PrimitiveArraySerializerGenerator(Member.Name, new DefaultPrimitiveArrayInvokationExpressionEmitter(ActualType.GetElementType()));
				statements.Add(generator.Create());
			}

			return statements;
		}
	}
}
