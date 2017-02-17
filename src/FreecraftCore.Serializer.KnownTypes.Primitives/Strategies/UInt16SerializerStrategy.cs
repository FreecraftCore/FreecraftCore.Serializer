using System;
using JetBrains.Annotations;

namespace FreecraftCore.Serializer.KnownTypes
{
	//This is the same as C++/Trinitycore's Int16 (or uint16 which we're going to use Int16s for)
	/// <summary>
	/// Description of Int16TypeSerializerStrategy.
	/// </summary>
	[KnownTypeSerializer]
	public class UInt16SerializerStrategy : SharedBufferTypeSerializer<UInt16>
	{
		//All primitive serializer stragies are contextless
		/// <inheritdoc />
		public override SerializationContextRequirement ContextRequirement { get; } = SerializationContextRequirement.Contextless;

		/// <inheritdoc />
		protected override unsafe bool PopulateSharedBufferWith(ushort value)
		{
			//Must lock to prevent issues with shared buffer.
			lock (syncObj)
			{
				//Must fix the position to get a byte*
				//See example explaining this memory hack: http://stackoverflow.com/questions/2036718/fastest-way-of-reading-and-writing-binary
				fixed (byte* bytePtr = &this.SharedByteBuffer[0])
					*((ushort*)bytePtr) = value;

				return true;
			}
		}

		/// <inheritdoc />
		protected override unsafe ushort DeserializeFromBuffer([NotNull] byte[] bytes)
		{
			if (bytes == null) throw new ArgumentNullException(nameof(bytes));

			//fix address; See this link for information on this memory hack: http://stackoverflow.com/questions/2036718/fastest-way-of-reading-and-writing-binary
			fixed (byte* bytePtr = &bytes[0])
				return *((ushort*)bytePtr);
		}
	}
}
