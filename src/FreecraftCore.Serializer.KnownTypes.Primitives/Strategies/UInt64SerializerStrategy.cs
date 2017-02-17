using System;
using JetBrains.Annotations;

namespace FreecraftCore.Serializer.KnownTypes
{
	//This is the same as C++/Trinitycore's UInt6432
	/// <summary>
	/// UInt64 serializer.
	/// </summary>
	[KnownTypeSerializer]
	public class UInt64SerializerStrategy : SharedBufferTypeSerializer<UInt64>
	{
		//All primitive serializer stragies are contextless
		/// <inheritdoc />
		public override SerializationContextRequirement ContextRequirement { get; } = SerializationContextRequirement.Contextless;

		/// <inheritdoc />
		protected override unsafe bool PopulateSharedBufferWith(ulong value)
		{
			//Must lock to prevent issues with shared buffer.
			lock (syncObj)
			{
				//Must fix the position to get a byte*
				//See example explaining this memory hack: http://stackoverflow.com/questions/2036718/fastest-way-of-reading-and-writing-binary
				fixed (byte* bytePtr = &this.SharedByteBuffer[0])
					*((ulong*)bytePtr) = value;

				return true;
			}
		}

		/// <inheritdoc />
		protected override unsafe ulong DeserializeFromBuffer([NotNull] byte[] bytes)
		{
			if (bytes == null) throw new ArgumentNullException(nameof(bytes));

			//fix address; See this link for information on this memory hack: http://stackoverflow.com/questions/2036718/fastest-way-of-reading-and-writing-binary
			fixed (byte* bytePtr = &bytes[0])
				return *((ulong*)bytePtr);
		}
	}
}
