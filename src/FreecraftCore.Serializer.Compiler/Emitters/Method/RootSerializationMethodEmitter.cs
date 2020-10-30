using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FreecraftCore.Serializer
{
	public class RootSerializationMethodBlockEmitter<TSerializableType> : IMethodBlockEmittable
		where TSerializableType : new()
	{
		public BlockSyntax CreateBlock()
		{
			//TODO: Figure out how we should decide which strategy to use, right now only simple and flat are supported.
			//TSerializableType
			return new FlatComplexTypeSerializationMethodBlockEmitter<TSerializableType>()
				.CreateBlock();
		}
	}
}
