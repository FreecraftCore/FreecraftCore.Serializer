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
				//There is some DUMB but very special cases where the sent size needs an offset
				//This offset is done in the READ direction. If 2 is read, we ADD 1. So there ends up seeming to be 3.
				//But when we send, this means we SUBTRACT one.
				//When writing N items we want to write N - X. The otherside will X so it will still understand
				//This was added to suppose very rare scenarios where the encoded size was off from the actual size.
				//See old: https://github.com/FreecraftCore/FreecraftCore.Serializer/blob/ca3355122cdfb3d0555e56a5f8c60104b35e16b5/src/FreecraftCore.Serializer/Decorators/Array/GenericCollectionSizeStrategy.cs
				int addedSize = sendSizeAttri.AddedSize;
				string baseSizeText = $"{CompilerConstants.SERIALZIABLE_OBJECT_REFERENCE_NAME}.{Member.Name}.{nameof(Array.Length)}";
				string sizeText = addedSize == 0 ? baseSizeText : $"{baseSizeText} - {addedSize}";

				//TODO: Replace size member access if needed for seperated size.
				var generator = new PrimitiveArraySerializerGenerator(Member.Name, sendSizeAttri.TypeOfSize, sizeText, true);
				statements.Add(generator.Create());
			}

			if (knownSizeAttri != null)
			{
				var generator = new PrimitiveArraySerializerGenerator(Member.Name, PrimitiveSizeType.Int32, $"{knownSizeAttri.KnownSize}", false);
				statements.Add(generator.Create());
			}

			//TODO: If this isn't the LAST member then there is issues!!
			//Assume it's write ALL and then other side will need to ReadToEnd (but the attribute isn't needed)
			if (knownSizeAttri == null && sendSizeAttri == null)
			{
				var generator = new PrimitiveArraySerializerGenerator(Member.Name, PrimitiveSizeType.Int32, "0", false);
				statements.Add(generator.Create());
			}

			return statements;
		}
	}
}
