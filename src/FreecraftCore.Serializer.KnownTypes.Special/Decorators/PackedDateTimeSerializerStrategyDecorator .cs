using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreecraftCore.Serializer.KnownTypes
{
	/// <summary>
	/// Serializer for the packed <see cref="DateTime"/> that is based on the packing implemented on Trinitycore.
	/// </summary>
	public class PackedDateTimeSerializerStrategyDecorator : ITypeSerializerStrategy<DateTime>
	{
		/// <summary>
		/// Indicates the context requirement for this serializer strategy.
		/// (Ex. If it requires context then a new one must be made or context must be provided to it for it to serialize for multiple members)
		/// </summary>
		public SerializationContextRequirement ContextRequirement { get; } = SerializationContextRequirement.RequiresContext;

		/// <summary>
		/// Indicates the <see cref="TType"/> of the serializer.
		/// </summary>
		public Type SerializerType { get; } = typeof(DateTime);

		/// <summary>
		/// Serializer this special type decorator decorates around.
		/// </summary>
		private ITypeSerializerStrategy<int> decoratedSerializer { get; }

		public PackedDateTimeSerializerStrategyDecorator(ITypeSerializerStrategy<int> intSerializer)
		{
			if (intSerializer == null)
				throw new ArgumentNullException(nameof(intSerializer), $"Provided arg {intSerializer} is null.");

			decoratedSerializer = intSerializer;
		}

		public DateTime Read(IWireMemberReaderStrategy source)
		{
			//Based on ByteBuffer.h from the Trinitycore Project, Jackpoz's 3.3.5 packet bot and WoWPacketParser
			int packedDateTime = decoratedSerializer.Read(source); //reads the packed int value from the stream

			//Don't ask me about this; this is based on Jackpoz's bitmasking and shifting and WPP
			int minute = packedDateTime & 0x3F;
			int hour = (packedDateTime >> 6) & 0x1F;
			int day = (packedDateTime >> 14) & 0x3F;
			int month = (packedDateTime >> 20) & 0xF;
			int year = (packedDateTime >> 24) & 0x1F;

			//int weekDay = (packedDate >> 11) & 7;
			//int something2 = (packedDate >> 29) & 3; always 0

			return new DateTime(2000, 1, 1).AddYears(year).AddMonths(month).AddDays(day).AddHours(hour).AddMinutes(minute);
		}

		public void Write(object value, IWireMemberWriterStrategy dest)
		{
			Write((DateTime)value, dest);
		}

		public void Write(DateTime value, IWireMemberWriterStrategy dest)
		{
			//Based on ByteBuffer.h from the Trinitycore Project as well as Jackpoz's 3.3.5 packet bot.
			//Trinitycore: append<uint32>((lt.tm_year - 100) << 24 | lt.tm_mon << 20 | (lt.tm_mday - 1) << 14 | lt.tm_wday << 11 | lt.tm_hour << 6 | lt.tm_min);

			int packedTime = ((value.Year - 2000) << 24 | (value.Month - 1) << 20 | (value.Day - 1) << 14 | (int)value.DayOfWeek << 11 | value.Hour << 6 | value.Minute);

			//pass to decorated serializer
			decoratedSerializer.Write(packedTime, dest);
		}

		object ITypeSerializerStrategy.Read(IWireMemberReaderStrategy source)
		{
			return Read(source);
		}
	}
}
