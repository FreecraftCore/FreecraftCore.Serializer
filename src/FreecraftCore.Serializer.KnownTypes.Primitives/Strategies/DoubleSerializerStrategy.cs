using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;


namespace FreecraftCore.Serializer.KnownTypes
{
	//Trinitycore just casts doubles to bytes (append((uint8 *)&value, sizeof(value));)
	/// <summary>
	/// A known-type serializer for <see cref="double"/>.
	/// </summary>
	[KnownTypeSerializer]
	public class DoubleSerializerStrategy : SharedBufferTypeSerializer<double>
	{
		//All primitive serializer stragies are contextless
		/// <inheritdoc />
		public override SerializationContextRequirement ContextRequirement { get; } = SerializationContextRequirement.Contextless;

		/// <inheritdoc />
		public unsafe override void Write(double value, [NotNull] IWireMemberWriterStrategy dest)
		{
			if (dest == null) throw new ArgumentNullException(nameof(dest));

			//Must lock to prevent issues with shared buffer.
			lock (syncObj)
			{
				//Must fix the position to get a byte*
				//See example explaining this memory hack: http://stackoverflow.com/questions/2036718/fastest-way-of-reading-and-writing-binary
				fixed (byte* bytePtr = &this.sharedByteBuffer[0])
					*((double*)bytePtr) = value;

				//Stay locked when you write the byte[] to the stream
				dest.Write(sharedByteBuffer);
			}
		}

		/// <inheritdoc />
		public unsafe override double Read([NotNull] IWireMemberReaderStrategy source)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));

			//Read 4 bytes (double size)
			byte[] bytes = source.ReadBytes(sizeof(double));

			//fix address; See this link for information on this memory hack: http://stackoverflow.com/questions/2036718/fastest-way-of-reading-and-writing-binary
			fixed (byte* bytePtr = &bytes[0])
				return *((double*)bytePtr);
		}
	}
}
