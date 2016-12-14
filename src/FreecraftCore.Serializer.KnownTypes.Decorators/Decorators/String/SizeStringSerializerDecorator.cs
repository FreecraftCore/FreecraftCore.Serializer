using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreecraftCore.Serializer.KnownTypes
{
	public class SizeStringSerializerDecorator : ITypeSerializerStrategy<string>
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
		/// The string serializer that is being decorated.
		/// </summary>
		private ITypeSerializerStrategy<string> decoratedSerializer { get; }

		public SizeStringSerializerDecorator(IStringSizeStrategy size, ITypeSerializerStrategy<string> stringSerializer)
		{
			sizeProvider = size;
			decoratedSerializer = stringSerializer;
		}

		public string Read(IWireMemberReaderStrategy source)
		{
			//The size must come from the strategy provided
			int size = sizeProvider.Size(source);

			byte[] bytes = source.ReadBytes(size);

			return Encoding.ASCII.GetString(bytes).TrimEnd('\0'); //TODO: Come up with better way to avoid/remove null terminator for sent size strings
		}

		public void Write(string value, IWireMemberWriterStrategy dest)
		{
			int size = sizeProvider.Size(value, dest);

			//Now that we know the size, and the header will be written if it was needed, we can write it
			//Don't write the size. Leave it up to the strategy above
			decoratedSerializer.Write(value, dest);

			//If the size isn't the same as the provided size we need to pad it
			if (value.Length < size)
				dest.Write(new byte[(size - value.Length) + 1]);
			else
				dest.Write((byte)0); //always add null terminator
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
