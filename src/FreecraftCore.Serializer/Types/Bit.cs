using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace FreecraftCore.Serializer
{
	[Serializable, StructLayout(LayoutKind.Sequential), ComVisible(true)]
	public struct Bit : IComparable, IFormattable, IConvertible, IComparable<Bit>, IEquatable<Bit>
	{
		public readonly byte Value;

		public int CompareTo(object obj)
		{
			return Value.CompareTo(obj);
		}

		public string ToString(string format, IFormatProvider formatProvider)
		{
			return Value.ToString(format, formatProvider);
		}

		public TypeCode GetTypeCode()
		{
			return Value.GetTypeCode();
		}

		public bool ToBoolean(IFormatProvider provider)
		{
			return ((IConvertible) Value).ToBoolean(provider);
		}

		public byte ToByte(IFormatProvider provider)
		{
			return ((IConvertible) Value).ToByte(provider);
		}

		public char ToChar(IFormatProvider provider)
		{
			return ((IConvertible) Value).ToChar(provider);
		}

		public DateTime ToDateTime(IFormatProvider provider)
		{
			return ((IConvertible) Value).ToDateTime(provider);
		}

		public decimal ToDecimal(IFormatProvider provider)
		{
			return ((IConvertible) Value).ToDecimal(provider);
		}

		public double ToDouble(IFormatProvider provider)
		{
			return ((IConvertible) Value).ToDouble(provider);
		}

		public short ToInt16(IFormatProvider provider)
		{
			return ((IConvertible) Value).ToInt16(provider);
		}

		public int ToInt32(IFormatProvider provider)
		{
			return ((IConvertible) Value).ToInt32(provider);
		}

		public long ToInt64(IFormatProvider provider)
		{
			return ((IConvertible) Value).ToInt64(provider);
		}

		public sbyte ToSByte(IFormatProvider provider)
		{
			return ((IConvertible) Value).ToSByte(provider);
		}

		public float ToSingle(IFormatProvider provider)
		{
			return ((IConvertible) Value).ToSingle(provider);
		}

		public string ToString(IFormatProvider provider)
		{
			return Value.ToString(provider);
		}

		public object ToType(Type conversionType, IFormatProvider provider)
		{
			return ((IConvertible) Value).ToType(conversionType, provider);
		}

		public ushort ToUInt16(IFormatProvider provider)
		{
			return ((IConvertible) Value).ToUInt16(provider);
		}

		public uint ToUInt32(IFormatProvider provider)
		{
			return ((IConvertible) Value).ToUInt32(provider);
		}

		public ulong ToUInt64(IFormatProvider provider)
		{
			return ((IConvertible) Value).ToUInt64(provider);
		}

		public int CompareTo(Bit other)
		{
			return Value.CompareTo(other.Value);
		}

		public bool Equals(Bit other)
		{
			return Value.Equals(other.Value);
		}
	}
}
