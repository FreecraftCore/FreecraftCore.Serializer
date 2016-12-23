using System;
using System.Collections.Generic;
using System.Linq;

namespace FreecraftCore.Serializer.KnownTypes
{
	/// <summary>
	/// Strategy for reading and writing int sized child keys from the stream.
	/// </summary>
	public class Int32ChildKeyStrategy : IChildKeyStrategy
	{
		private ITypeSerializerStrategy<int> managedIntegerSerializer { get; }

		private bool shouldConsumeKey { get; }

		public Int32ChildKeyStrategy(ITypeSerializerStrategy<int> intSerializer, bool shouldConsume)
		{
			if (intSerializer == null)
				throw new ArgumentNullException(nameof(intSerializer), $"Provided {nameof(ITypeSerializerStrategy<int>)} was null.");

			//We need an int serializer to know how to write the int sized key.
			managedIntegerSerializer = intSerializer;
			shouldConsumeKey = shouldConsume;
		}

		public int Read(IWireMemberReaderStrategy source)
		{
			//Read an int from the stream. It should be the child type key
			return shouldConsumeKey ? managedIntegerSerializer.Read(source) :
				ConvertToInt(source.PeakBytes(4));
		}

		public void Write(int value, IWireMemberWriterStrategy dest)
		{
			//If the key should be consumed then we should write one, to be consumed.
			//Otherwise if it's not then something in the stream will be read and then left in
			//meaning we need to write nothing
			if (shouldConsumeKey)
				managedIntegerSerializer.Write(value, dest); //Write the int sized key to the stream.
		}

		private unsafe int ConvertToInt(byte[] bytes)
		{
			//fix address; See this link for information on this memory hack: http://stackoverflow.com/questions/2036718/fastest-way-of-reading-and-writing-binary
			fixed (byte* bytePtr = &bytes[0])
				return *((int*)bytePtr);
		}
	}
}
