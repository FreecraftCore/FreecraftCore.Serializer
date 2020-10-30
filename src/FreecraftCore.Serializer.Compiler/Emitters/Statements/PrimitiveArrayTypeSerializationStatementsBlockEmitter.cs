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
		public PrimitiveArrayTypeSerializationStatementsBlockEmitter([NotNull] Type primitiveType, [NotNull] MemberInfo member) 
			: base(primitiveType, member)
		{

		}

		public override List<StatementSyntax> CreateStatements()
		{
			List<StatementSyntax> statements = new List<StatementSyntax>();

			//Firstly, we must know if there are any attributes
			SendSizeAttribute sendSizeAttri = Member.GetCustomAttribute<SendSizeAttribute>();
			KnownSizeAttribute knownSizeAttri = Member.GetCustomAttribute<KnownSizeAttribute>();

			if (sendSizeAttri != null && knownSizeAttri != null)
				throw new InvalidOperationException($"Emit failed for Member: {PrimitiveType} in Type: {Member.DeclaringType}. Cannot use Attributes: {nameof(SendSizeAttribute)} and {nameof(KnownSizeAttribute)} together.");

			//TODO: Support seperated collection sizes
			//Case where the array will be send with length-prefixed size
			if (sendSizeAttri != null)
			{
				//TODO: Replace size member access if needed for seperated size.
				var generator = new PrimitiveArraySerializerGenerator(Member.Name, sendSizeAttri.TypeOfSize, $"{CompilerConstants.SERIALZIABLE_OBJECT_REFERENCE_NAME}.{Member.Name}.{nameof(Array.Length)}", true);
				statements.Add(generator.Create());
			}

			if (knownSizeAttri != null)
			{
				var generator = new PrimitiveArraySerializerGenerator(Member.Name, SendSizeAttribute.SizeType.Int32, $"{knownSizeAttri.KnownSize}", false);
				statements.Add(generator.Create());
			}

			//TODO: If this isn't the LAST member then there is issues!!
			//Assume it's write ALL and then other side will need to ReadToEnd (but the attribute isn't needed)
			if (knownSizeAttri == null && sendSizeAttri == null)
			{
				var generator = new PrimitiveArraySerializerGenerator(Member.Name, SendSizeAttribute.SizeType.Int32, "0", false);
				statements.Add(generator.Create());
			}

			return statements;
		}
	}
}
