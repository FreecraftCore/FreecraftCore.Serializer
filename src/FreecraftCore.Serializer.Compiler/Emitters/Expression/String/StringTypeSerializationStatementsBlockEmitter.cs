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
	public sealed class StringTypeSerializationStatementsBlockEmitter : BaseInvokationExpressionEmitter
	{
		public StringTypeSerializationStatementsBlockEmitter([NotNull] Type actualType, [NotNull] MemberInfo member, SerializationMode mode)
			: base(actualType, member, mode)
		{

		}

		public override InvocationExpressionSyntax Create()
		{
			//Firstly, we must know if the string is sending its size
			SendSizeAttribute sendSizeAttri = Member.GetCustomAttribute<SendSizeAttribute>();
			KnownSizeAttribute knownSizeAttri = Member.GetCustomAttribute<KnownSizeAttribute>();
			EncodingAttribute encodingAttri = Member.GetCustomAttribute<EncodingAttribute>();
			DontTerminateAttribute dontTerminateAttribute = Member.GetCustomAttribute<DontTerminateAttribute>();

			//Default to ASCII if no encoding specified
			if (encodingAttri == null)
				encodingAttri = new EncodingAttribute(EncodingType.ASCII);

			if (knownSizeAttri != null && sendSizeAttri != null)
				throw new InvalidOperationException($"Emit failed for Member: {ActualType} in Type: {Member.DeclaringType}. Cannot use Attributes: {nameof(SendSizeAttribute)} and {nameof(KnownSizeAttribute)} together.");

			if (sendSizeAttri != null && dontTerminateAttribute == null)
			{
				var generator = new RawLengthPrefixedStringTypeSerializationGenerator(ActualType, Member, Mode, encodingAttri.DesiredEncodingType, sendSizeAttri.TypeOfSize);
				return generator.Create();
			}
			else if (sendSizeAttri != null)
			{
				//Dont Terminate attribute FOUND!
				var generator = new RawDontTerminateLengthPrefixedStringTypeSerializationGenerator(ActualType, Member, Mode, encodingAttri.DesiredEncodingType, sendSizeAttri.TypeOfSize);
				return generator.Create();
			}

			if (knownSizeAttri != null)
			{
				var generator = new RawKnownSizeStringTypeSerializerGenerator(ActualType, Member, Mode, encodingAttri.DesiredEncodingType, knownSizeAttri.KnownSize, dontTerminateAttribute == null);
				return generator.Create();
			}

			//If it's not knownsize or sendsize then let's emit the default!
			if (sendSizeAttri == null && knownSizeAttri == null)
			{
				var generator = new RawDefaultStringTypeSerializerGenerator(ActualType, Member, Mode, encodingAttri.DesiredEncodingType, dontTerminateAttribute == null);
				return generator.Create();
			}

			throw new InvalidOperationException($"Cannot handle Member: {Member.Name} on Type: {Member.DeclaringType.Name}.");
		}
	}
}
