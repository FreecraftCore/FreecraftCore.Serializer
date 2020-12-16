using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace FreecraftCore.Serializer
{
	public sealed class SerializationCallbackInvokationExpressionEmitter : IInvokationExpressionEmittable
	{
		private SerializationMode Mode { get; }

		public SerializationCallbackInvokationExpressionEmitter(SerializationMode mode)
		{
			if (!Enum.IsDefined(typeof(SerializationMode), mode)) throw new InvalidEnumArgumentException(nameof(mode), (int) mode, typeof(SerializationMode));
			Mode = mode;
		}

		public InvocationExpressionSyntax Create()
		{
			return InvocationExpression
			(
				MemberAccessExpression
				(
					SyntaxKind.SimpleMemberAccessExpression,
					IdentifierName(CompilerConstants.SERIALZIABLE_OBJECT_REFERENCE_NAME),
					IdentifierName(Mode == SerializationMode.Write ? nameof(ISerializationEventListener.OnBeforeSerialization) : nameof(ISerializationEventListener.OnAfterDeserialization))
				)
			);
		}
	}
}
