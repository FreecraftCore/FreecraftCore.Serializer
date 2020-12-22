using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using FreecraftCore.Serializer.Internal;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace FreecraftCore.Serializer
{
	internal sealed class FixedSizeComplexArrayInvokationExpressionEmitter
		: BaseComplexArrayInvokationExpressionEmitter<FixedSizeComplexArrayTypeSerializerStrategy>
	{
		public int KnownSize { get; }

		public FixedSizeComplexArrayInvokationExpressionEmitter([NotNull] IArrayTypeSymbol arraySymbol, ISymbol member, int knownSize, SerializationMode mode)
			: base(arraySymbol, member, mode)
		{
			KnownSize = knownSize;
		}

		protected override IEnumerable<SyntaxNodeOrToken> CalculateGenericTypeParameters()
		{
			foreach (var baseYield in base.CalculateGenericTypeParameters())
				yield return baseYield;

			yield return IdentifierName(new StaticlyTypedNumericNameBuilder<Int32>(KnownSize).BuildName());
		}
	}
}
