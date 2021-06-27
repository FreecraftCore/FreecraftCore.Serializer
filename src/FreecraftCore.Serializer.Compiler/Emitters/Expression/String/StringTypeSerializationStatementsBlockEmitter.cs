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

			//Overriding default behavior.
			bool hasSaneDefaults = ActualType.HasAttributeExact<WireSaneDefaultsAttribute>(true);

			if (Member.HasAttributeExact<EncodingAttribute>())
				encodingType = EncodingAttribute.Parse(Member.GetAttributeExact<EncodingAttribute>().ConstructorArguments.First().ToCSharpString());

			if (Member.HasAttributeExact<KnownSizeAttribute>() && Member.HasAttributeExact<SendSizeAttribute>() && !hasSaneDefaults)
				throw new InvalidOperationException($"Emit failed for Member: {ActualType} in Type: {Member.ContainingType.Name}. Cannot use Attributes: {nameof(SendSizeAttribute)} and {nameof(KnownSizeAttribute)} together.");
			
			if (IsSendSizeString() && !Member.HasAttributeExact<DontTerminateAttribute>())
			{
				PrimitiveSizeType sendSize = SendSizeAttribute.Parse(Member.GetAttributeExact<SendSizeAttribute>().ConstructorArguments.First().ToCSharpString());
				var generator = new RawLengthPrefixedStringTypeSerializationGenerator(ActualType, Member, Mode, encodingType, sendSize);
				return generator.Create();
			}
			else if (IsSendSizeString() || (hasSaneDefaults && !IsKnownSizeString())) //SANE DEFAULT
			{
				//Dont Terminate attribute FOUND!
				PrimitiveSizeType sendSize = PrimitiveSizeType.Int32; //SANE DEFAULT size 65,000

				if (Member.HasAttributeExact<SendSizeAttribute>())
					sendSize = SendSizeAttribute.Parse(Member.GetAttributeExact<SendSizeAttribute>().ConstructorArguments.First().ToCSharpString());

				var generator = new RawDontTerminateLengthPrefixedStringTypeSerializationGenerator(ActualType, Member, Mode, encodingType, sendSize);
				return generator.Create();
			}
			else if (IsKnownSizeString())
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

		private bool IsKnownSizeString()
		{
			//TODO: We currently have no way to support KnownSize metadata when using string arrays.
			if (ActualType is IArrayTypeSymbol)
				return false;
			else
				return Member.HasAttributeExact<KnownSizeAttribute>();
		}

		private bool IsSendSizeString()
		{
			//TODO: We currently have no way to support KnownSize metadata when using string arrays.
			if(ActualType is IArrayTypeSymbol)
				return false;
			else
				return Member.HasAttributeExact<SendSizeAttribute>();
		}
	}
}
