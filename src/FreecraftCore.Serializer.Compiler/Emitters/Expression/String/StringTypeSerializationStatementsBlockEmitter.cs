using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// <see cref="IStatementsBlockEmittable"/> implementation for emitting string type serialization.
	/// </summary>
	public sealed class StringTypeSerializationStatementsBlockEmitter : BaseInvokationExpressionEmitter
	{
		public StringTypeSerializationStatementsBlockEmitter([NotNull] ITypeSymbol actualType, [NotNull] ISymbol member, SerializationMode mode)
			: base(actualType, member, mode)
		{

		}

		public override InvocationExpressionSyntax Create()
		{
			//Default to ASCII if no encoding specified
			EncodingType encodingType = EncodingType.ASCII;

			if (Member.HasAttributeExact<EncodingAttribute>())
				encodingType = EncodingAttribute.Parse(Member.GetAttributeExact<EncodingAttribute>().ConstructorArguments.First().ToCSharpString());

			if (Member.HasAttributeExact<KnownSizeAttribute>() && Member.HasAttributeExact<SendSizeAttribute>())
				throw new InvalidOperationException($"Emit failed for Member: {ActualType} in Type: {Member.ContainingType.Name}. Cannot use Attributes: {nameof(SendSizeAttribute)} and {nameof(KnownSizeAttribute)} together.");
			
			if (Member.HasAttributeExact<SendSizeAttribute>() && !Member.HasAttributeExact<DontTerminateAttribute>())
			{
				PrimitiveSizeType sendSize = SendSizeAttribute.Parse(Member.GetAttributeExact<SendSizeAttribute>().ConstructorArguments.First().ToCSharpString());
				var generator = new RawLengthPrefixedStringTypeSerializationGenerator(ActualType, Member, Mode, encodingType, sendSize);
				return generator.Create();
			}
			else if (Member.HasAttributeExact<SendSizeAttribute>())
			{
				//Dont Terminate attribute FOUND!
				PrimitiveSizeType sendSize = SendSizeAttribute.Parse(Member.GetAttributeExact<SendSizeAttribute>().ConstructorArguments.First().ToCSharpString());

				var generator = new RawDontTerminateLengthPrefixedStringTypeSerializationGenerator(ActualType, Member, Mode, encodingType, sendSize);
				return generator.Create();
			}
			else if (Member.HasAttributeExact<KnownSizeAttribute>())
			{
				int size = KnownSizeAttribute.Parse(Member.GetAttributeExact<KnownSizeAttribute>().ConstructorArguments.First().ToCSharpString());

				var generator = new RawKnownSizeStringTypeSerializerGenerator(ActualType, Member, Mode, encodingType, size, !Member.HasAttributeExact<DontTerminateAttribute>());
				return generator.Create();
			}
			else
			{
				//If it's not knownsize or sendsize then let's emit the default!
				var generator = new RawDefaultStringTypeSerializerGenerator(ActualType, Member, Mode, encodingType, !Member.HasAttributeExact<DontTerminateAttribute>());
				return generator.Create();
			}
		}
	}
}
