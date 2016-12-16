using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreecraftCore.Serializer.KnownTypes
{
	public class EnumStringSerializerDecorator<TEnumType> : ITypeSerializerStrategy<TEnumType>
			where TEnumType : struct
	{
		public Type SerializerType { get { return typeof(TEnumType); } }

		public SerializationContextRequirement ContextRequirement { get; } = SerializationContextRequirement.RequiresContext;

		public ITypeSerializerStrategy<string> decoratedSerializer { get; }

		public EnumStringSerializerDecorator(ITypeSerializerStrategy<string> stringSerializer)
		{
			if (stringSerializer == null)
				throw new ArgumentNullException(nameof(stringSerializer), $"Provided argument {stringSerializer} is null.");

			decoratedSerializer = stringSerializer;
		}

		/// <summary>
		/// Perform the steps necessary to deserialize this data.
		/// </summary>
		/// <param name="source">The reader providing the input data.</param>
		/// <returns>The updated / replacement value.</returns>
		public TEnumType Read(IWireMemberReaderStrategy source)
		{
			//Read the string serialized version of the enum value
			string readString = decoratedSerializer.Read(source);

			//TODO: What should we do if it's empty or null?
			return (TEnumType)Enum.Parse(typeof(TEnumType), readString);
		}

		/// <summary>
		/// Perform the steps necessary to serialize this data.
		/// </summary>
		/// <param name="value">The value to be serialized.</param>
		/// <param name="dest">The writer entity that is accumulating the output data.</param>
		public void Write(TEnumType value, IWireMemberWriterStrategy dest)
		{
			//Just write the string to the stream
			decoratedSerializer.Write(value.ToString(), dest);
		}

		void ITypeSerializerStrategy.Write(object value, IWireMemberWriterStrategy dest)
		{
			Write((TEnumType)value, dest);
		}

		object ITypeSerializerStrategy.Read(IWireMemberReaderStrategy source)
		{
			return Read(source);
		}
	}
}
