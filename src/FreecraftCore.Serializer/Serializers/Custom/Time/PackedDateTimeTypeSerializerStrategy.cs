using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Serializer strategy for <see cref="DateTime"/> in the "Packed" form.
	/// </summary>
	public sealed class PackedDateTimeTypeSerializerStrategy : StatelessTypeSerializerStrategy<PackedDateTimeTypeSerializerStrategy, DateTime>
	{
		public sealed override DateTime Read(Span<byte> source, ref int offset)
		{
			//Based on ByteBuffer.h from the Trinitycore Project, Jackpoz's 3.3.5 packet bot and WoWPacketParser
			//reads the packed int value from the stream
			int packedTime = GenericTypePrimitiveSerializerStrategy<int>.Instance.Read(source, ref offset);
			return ConvertIntegerToDateTimeRepresentation(packedTime);
		}

		public sealed override void Write(DateTime value, Span<byte> destination, ref int offset)
		{
			//Based on ByteBuffer.h from the Trinitycore Project as well as Jackpoz's 3.3.5 packet bot.
			//Trinitycore: append<uint32>((lt.tm_year - 100) << 24 | lt.tm_mon << 20 | (lt.tm_mday - 1) << 14 | lt.tm_wday << 11 | lt.tm_hour << 6 | lt.tm_min);
			int packedTime = ConvertDateTimeToIntegerRepresentation(ref value);
			GenericTypePrimitiveSerializerStrategy<int>.Instance.Write(packedTime, destination, ref offset);
		}

		/// <summary>
		/// Converts a <see cref="DateTime"/> to the WoW expected representation.
		/// (Based on Jackpoz's bot's implementation)
		/// </summary>
		/// <param name="dateTime"></param>
		/// <returns></returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private int ConvertDateTimeToIntegerRepresentation(ref DateTime dateTime)
		{
			return ((dateTime.Year - 2000) << 24 | (dateTime.Month - 1) << 20 | (dateTime.Day - 1) << 14 | (int)dateTime.DayOfWeek << 11 | dateTime.Hour << 6 | dateTime.Minute);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private DateTime ConvertIntegerToDateTimeRepresentation(int packedDateTime)
		{
			//Don't ask me about this; this is based on Jackpoz's bitmasking and shifting and WPP
			int minute = packedDateTime & 0x3F;
			int hour = (packedDateTime >> 6) & 0x1F;
			int day = (packedDateTime >> 14) & 0x3F;
			int month = (packedDateTime >> 20) & 0xF;
			int year = (packedDateTime >> 24) & 0x1F;

			//int weekDay = (packedDate >> 11) & 7;
			//int something2 = (packedDate >> 29) & 3; always 0

			return new DateTime(2000 + year, 1 + month, 1 + day, hour, minute, 0); //fluent building of the immutable DateTime was pretty but inefficient
		}
	}
}
