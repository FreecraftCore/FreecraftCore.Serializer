using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;


namespace FreecraftCore.Serializer.KnownTypes
{
	/// <summary>
	/// Serializer for the packed <see cref="DateTime"/> that is based on the packing implemented on Trinitycore.
	/// </summary>
	public class PackedDateTimeSerializerStrategyDecorator : SimpleTypeSerializerStrategy<DateTime>
	{
		/// <inheritdoc />
		public override SerializationContextRequirement ContextRequirement { get; } = SerializationContextRequirement.RequiresContext;

		[NotNull]
		private ITypeSerializerStrategy<int> decoratedSerializer { get; }

		public PackedDateTimeSerializerStrategyDecorator([NotNull] ITypeSerializerStrategy<int> intSerializer)
		{
			if (intSerializer == null)
				throw new ArgumentNullException(nameof(intSerializer), $"Provided arg {intSerializer} is null.");

			decoratedSerializer = intSerializer;
		}

		/// <inheritdoc />
		public override DateTime Read(IWireStreamReaderStrategy source)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));

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

			return new DateTime(2000 + year, 1 + month, 1 + day, hour, minute, 0); //fluent building of the immutable DateTime was pretty but inefficient
		}

		/// <inheritdoc />
		public override void Write(DateTime value, IWireStreamWriterStrategy dest)
		{
			if (dest == null) throw new ArgumentNullException(nameof(dest));

			//Based on ByteBuffer.h from the Trinitycore Project as well as Jackpoz's 3.3.5 packet bot.
			//Trinitycore: append<uint32>((lt.tm_year - 100) << 24 | lt.tm_mon << 20 | (lt.tm_mday - 1) << 14 | lt.tm_wday << 11 | lt.tm_hour << 6 | lt.tm_min);

			int packedTime = ((value.Year - 2000) << 24 | (value.Month - 1) << 20 | (value.Day - 1) << 14 | (int)value.DayOfWeek << 11 | value.Hour << 6 | value.Minute);

			//pass to decorated serializer
			decoratedSerializer.Write(packedTime, dest);
		}
	}
}
