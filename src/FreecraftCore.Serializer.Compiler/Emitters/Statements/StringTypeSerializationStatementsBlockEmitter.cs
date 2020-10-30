using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// <see cref="IStatementsBlockEmittable"/> implementation for emitting string type serialization.
	/// </summary>
	public sealed class StringTypeSerializationStatementsBlockEmitter : BaseSerializationStatementsBlockEmitter
	{
		public StringTypeSerializationStatementsBlockEmitter([NotNull] Type memberType, [NotNull] MemberInfo member)
			: base(memberType, member)
		{

		}

		public override List<StatementSyntax> CreateStatements()
		{
			List<StatementSyntax> statements = new List<StatementSyntax>();

			//Firstly, we must know if the string is sending its size
			SendSizeAttribute sendSizeAttri = Member.GetCustomAttribute<SendSizeAttribute>();
			KnownSizeAttribute knownSizeAttri = Member.GetCustomAttribute<KnownSizeAttribute>();
			EncodingAttribute encodingAttri = Member.GetCustomAttribute<EncodingAttribute>();
			DontTerminateAttribute dontTerminateAttribute = Member.GetCustomAttribute<DontTerminateAttribute>();

			//Default to ASCII if no encoding specified
			if (encodingAttri == null)
				encodingAttri = new EncodingAttribute(EncodingType.ASCII);

			if (knownSizeAttri != null && sendSizeAttri != null)
				throw new InvalidOperationException($"Emit failed for Member: {PrimitiveType} in Type: {Member.DeclaringType}. Cannot use Attributes: {nameof(SendSizeAttribute)} and {nameof(KnownSizeAttribute)} together.");

			if (sendSizeAttri != null && dontTerminateAttribute == null)
			{
				var generator = new RawLengthPrefixedStringTypeSerializationGenerator(encodingAttri.DesiredEncodingType, Member.Name, sendSizeAttri.TypeOfSize);
				statements.Add(generator.Create());
			}
			else if (sendSizeAttri != null)
			{
				//Dont Terminate attribute FOUND!
				var generator = new RawDontTerminateLengthPrefixedStringTypeSerializationGenerator(encodingAttri.DesiredEncodingType, Member.Name, sendSizeAttri.TypeOfSize);
				statements.Add(generator.Create());
			}

			if (knownSizeAttri != null)
			{
				var generator = new RawKnownSizeStringTypeSerializerGenerator(Member.Name, encodingAttri.DesiredEncodingType, knownSizeAttri.KnownSize, dontTerminateAttribute == null);
				statements.Add(generator.Create());
			}

			//If it's not knownsize or sendsize then let's emit the default!
			if (sendSizeAttri == null && knownSizeAttri == null)
			{
				var generator = new RawDefaultStringTypeSerializerGenerator(Member.Name, encodingAttri.DesiredEncodingType, dontTerminateAttribute == null);
				statements.Add(generator.Create());
			}

			return statements;
		}
	}
}
