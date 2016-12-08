using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreecraftCore.Serializer.KnownTypes
{
	public class FixedSizeStringSerializerDecorator : ITypeSerializerStrategy<string>
	{
		/// <summary>
		/// Indicates the context requirement for this serializer strategy.
		/// (Ex. If it requires context then a new one must be made or context must be provided to it for it to serializer for multiple members)
		/// </summary>
		public SerializationContextRequirement ContextRequirement { get; } = SerializationContextRequirement.RequiresContext;

		/// <summary>
		/// Indicates the <see cref="TType"/> of the serializer.
		/// </summary>
		public Type SerializerType { get; } = typeof(string);

		/// <summary>
		/// Provides the size of the fixed string.
		/// </summary>
		public IStringSizeStrategy sizeProvider { get; }

		/// <summary>
		/// 
		/// </summary>
		private ITypeSerializerStrategy<string> decoratedSerializer { get; }

		public FixedSizeStringSerializerDecorator(IStringSizeStrategy size, ITypeSerializerStrategy<string> stringSerializer)
		{
			sizeProvider = size;
			decoratedSerializer = stringSerializer;
		}

		public string Read(IWireMemberReaderStrategy source)
		{
			//The size is known so it is required that we override the handling of the default read
			int size = sizeProvider.Size(source);

			byte[] bytes = source.ReadBytes(size);

			return Encoding.ASCII.GetString(bytes);
		}

		public void Write(string value, IWireMemberWriterStrategy dest)
		{
			int size = sizeProvider.Size(value, dest);

			//Now that it is properly padded the string we can write it
			//Don't write the size. The size is known
			decoratedSerializer.Write(value, dest);

			//If the size isn't the same as the provided size we need to pad it
			if (value.Length < size)
				dest.Write(new byte[(size - value.Length) + 1]);
			else
				dest.Write((byte)0);
		}

		public void Write(object value, IWireMemberWriterStrategy dest)
		{
			Write((string)value, dest);
		}

		object ITypeSerializerStrategy.Read(IWireMemberReaderStrategy source)
		{
			return Read(source);
		}
	}
}
