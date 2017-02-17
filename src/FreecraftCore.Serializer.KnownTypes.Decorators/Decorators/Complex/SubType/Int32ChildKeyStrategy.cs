using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using JetBrains.Annotations;

namespace FreecraftCore.Serializer.KnownTypes
{
	/// <summary>
	/// Strategy for reading and writing int sized child keys from the stream.
	/// </summary>
	public class Int32ChildKeyStrategy : IChildKeyStrategy
	{
		[NotNull]
		private ITypeSerializerStrategy<int> managedIntegerSerializer { get; }

		private InformationHandlingFlags typeHandlingFlags { get; }

		public Int32ChildKeyStrategy([NotNull] ITypeSerializerStrategy<int> intSerializer, InformationHandlingFlags typeHandling)
		{
			if (intSerializer == null)
				throw new ArgumentNullException(nameof(intSerializer), $"Provided {nameof(ITypeSerializerStrategy<int>)} was null.");

			if (!Enum.IsDefined(typeof(InformationHandlingFlags), typeHandling))
				throw new ArgumentOutOfRangeException(nameof(typeHandling), "Value should be defined in the InformationHandlingFlags enum.");

			/*int i;

			if (!Enum.IsDefined(typeof(InformationHandlingFlags), typeHandling) && Int32.TryParse(typeHandling.ToString(), out i))
				throw new InvalidEnumArgumentException(nameof(typeHandling), (int)typeHandling,
					typeof(InformationHandlingFlags));*/

			//We need an int serializer to know how to write the int sized key.
			managedIntegerSerializer = intSerializer;
			typeHandlingFlags = typeHandling;
		}

		public int Read(IWireStreamReaderStrategy source)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));

			//Read an int from the stream. It should be the child type key
			return typeHandlingFlags.HasFlag(InformationHandlingFlags.DontConsumeRead) ? ConvertToInt(source.PeakBytes(4)) 
				: managedIntegerSerializer.Read(source);
		}

		public void Write(int value, IWireStreamWriterStrategy dest)
		{
			if (dest == null) throw new ArgumentNullException(nameof(dest));

			//If the key should be consumed then we should write one, to be consumed.
			//Otherwise if it's not then something in the stream will be read and then left in
			//meaning we need to write nothing
			if (!typeHandlingFlags.HasFlag(InformationHandlingFlags.DontWrite))
				managedIntegerSerializer.Write(value, dest); //Write the int sized key to the stream.
		}

		private unsafe int ConvertToInt([NotNull] byte[] bytes)
		{
			if (bytes == null) throw new ArgumentNullException(nameof(bytes));

			//fix address; See this link for information on this memory hack: http://stackoverflow.com/questions/2036718/fastest-way-of-reading-and-writing-binary
			fixed (byte* bytePtr = &bytes[0])
				return *((int*)bytePtr);
		}
	}
}
