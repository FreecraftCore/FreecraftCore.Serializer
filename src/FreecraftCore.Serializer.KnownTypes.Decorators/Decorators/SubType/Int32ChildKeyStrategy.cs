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

		public Int32ChildKeyStrategy(ITypeSerializerStrategy<int> intSerializer)
		{
			if (intSerializer == null)
				throw new ArgumentNullException(nameof(intSerializer), $"Provided {nameof(ITypeSerializerStrategy<int>)} was null.");

			//We need an int serializer to know how to write the int sized key.
			managedIntegerSerializer = intSerializer;
		}

		public int Read(IWireMemberReaderStrategy source)
		{
			//Read an int from the stream. It should be the child type key
			return managedIntegerSerializer.Read(source);
		}

		public void Write(int value, IWireMemberWriterStrategy dest)
		{
			//Write the int sized key to the stream.
			managedIntegerSerializer.Write(value, dest);
		}
	}
}
