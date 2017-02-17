using System;
using JetBrains.Annotations;

namespace FreecraftCore.Serializer.KnownTypes
{
	//This is the same as C++/Trinitycore's int32
	/// <summary>
	/// UInt32 serializer.
	/// </summary>
	[KnownTypeSerializer]
	public class UInt32SerializerStrategy : SharedBufferTypeSerializer<uint>
	{
		//All primitive serializer stragies are contextless
		/// <inheritdoc />
		public override SerializationContextRequirement ContextRequirement { get; } = SerializationContextRequirement.Contextless;

		/// <inheritdoc />
		protected override unsafe bool PopulateSharedBufferWith(uint value)
		{
			//Must lock to prevent issues with shared buffer.
			lock (syncObj)
			{
				//Must fix the position to get a byte*
				//See example explaining this memory hack: http://stackoverflow.com/questions/2036718/fastest-way-of-reading-and-writing-binary
				fixed (byte* bytePtr = &this.SharedByteBuffer[0])
					*((uint*)bytePtr) = value;

				return true;
			}
		}

		/// <inheritdoc />
		protected override unsafe uint DeserializeFromBuffer([NotNull] byte[] bytes)
		{
			if (bytes == null) throw new ArgumentNullException(nameof(bytes));

			//fix address; See this link for information on this memory hack: http://stackoverflow.com/questions/2036718/fastest-way-of-reading-and-writing-binary
			fixed (byte* bytePtr = &bytes[0])
				return *((uint*)bytePtr);
		}
	}
}
