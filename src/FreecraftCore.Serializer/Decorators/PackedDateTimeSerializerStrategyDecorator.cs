using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;


namespace FreecraftCore.Serializer.KnownTypes
{
	/// <summary>
	/// Serializer for the packed <see cref="DateTime"/> that is based on the packing implemented on Trinitycore.
	/// </summary>
	public class PackedDateTimeSerializerStrategyDecorator : SimpleTypeSerializerStrategy<DateTime>
	{
		//A DateTime is contextless since it only serializes a type not special semantics on a member.
		/// <inheritdoc />
		public override SerializationContextRequirement ContextRequirement { get; } = SerializationContextRequirement.Contextless;

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
			//reads the packed int value from the stream
			return ConvertIntegerToDateTimeRepresentation(decoratedSerializer.Read(source));
		}

		/// <inheritdoc />
		public override void Write(DateTime value, IWireStreamWriterStrategy dest)
		{
			if (dest == null) throw new ArgumentNullException(nameof(dest));

			//Based on ByteBuffer.h from the Trinitycore Project as well as Jackpoz's 3.3.5 packet bot.
			//Trinitycore: append<uint32>((lt.tm_year - 100) << 24 | lt.tm_mon << 20 | (lt.tm_mday - 1) << 14 | lt.tm_wday << 11 | lt.tm_hour << 6 | lt.tm_min);

			//pass to decorated serializer
			decoratedSerializer.Write(ConvertDateTimeToIntegerRepresentation(ref value), dest);
		}

		/// <summary>
		/// Converts a <see cref="DateTime"/> to the WoW expected representation.
		/// (Based on Jackpoz's bot's implementation)
		/// </summary>
		/// <param name="dateTime"></param>
		/// <returns></returns>
		private int ConvertDateTimeToIntegerRepresentation(ref DateTime dateTime)
		{
			return ((dateTime.Year - 2000) << 24 | (dateTime.Month - 1) << 20 | (dateTime.Day - 1) << 14 | (int)dateTime.DayOfWeek << 11 | dateTime.Hour << 6 | dateTime.Minute);
		}

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

		/// <inheritdoc />
		public override async Task WriteAsync(DateTime value, IWireStreamWriterStrategyAsync dest)
		{
			//pass to decorated serializer
			await decoratedSerializer.WriteAsync(ConvertDateTimeToIntegerRepresentation(ref value), dest);
		}

		/// <inheritdoc />
		public override async Task<DateTime> ReadAsync(IWireStreamReaderStrategyAsync source)
		{
			//reads the packed int value from the stream
			return ConvertIntegerToDateTimeRepresentation(await decoratedSerializer.ReadAsync(source));
		}
	}
}
