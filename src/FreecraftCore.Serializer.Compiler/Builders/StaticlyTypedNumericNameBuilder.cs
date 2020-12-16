using System;
using System.Collections.Generic;
using System.Text;

namespace FreecraftCore.Serializer
{
	public sealed class StaticlyTypedNumericNameBuilder<TValueType> : INameBuildable
		where TValueType : unmanaged
	{
		public TValueType LiteralValue { get; }

		public StaticlyTypedNumericNameBuilder(TValueType literalValue)
		{
			LiteralValue = literalValue;
		}

		public override string ToString()
		{
			return $"{"StaticTypedNumeric"}_{typeof(TValueType).Name}_{LiteralValue}";
		}

		public string BuildName()
		{
			return ToString();
		}
	}
}
