using System;
using System.Collections.Generic;
using System.Text;

namespace FreecraftCore.Serializer.KnownTypes
{
	/// <summary>
	/// <see cref="ITypeSerializerStrategy"/> for Type <see cref="string"/>.
	/// </summary>
	[KnownTypeSerializer]
	public class StringSerializerStrategy : ITypeSerializerStrategy<string>
	{
		//All primitive serializer stragies are contextless
		/// <inheritdoc />
		public SerializationContextRequirement ContextRequirement { get; } = SerializationContextRequirement.Contextless;

		/// <inheritdoc />
		public Type SerializerType { get; } = typeof(string);

		/// <inheritdoc />
		public void Write(string value, IWireMemberWriterStrategy dest)
		{
			if (dest == null) throw new ArgumentNullException(nameof(dest));

			//Review the source for Trinitycore's string reading for their ByteBuffer (payload/packet) Type.
			//(ctr+f << for std::string): http://www.trinitycore.net/d1/d17/ByteBuffer_8h_source.html
			//They use 0 byte to terminate the string in the stream
			
			//TODO: Pointer hack for speed
			//Convert the string to bytes
			//Not sure about encoding yet
			byte[] stringBytes = Encoding.ASCII.GetBytes(value);
			
			dest.Write(stringBytes);
			
			//Write the null terminator; Client expects it.
			dest.Write(0);
		}
		
		/// <summary>
		/// Perform the steps necessary to deserialize a string.
		/// </summary>
		/// <param name="source">The reader providing the input data.</param>
		/// <returns>A string value from the reader.</returns>
		public string Read(IWireMemberReaderStrategy source)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));

			//Review the source for Trinitycore's string reading for their ByteBuffer (payload/packet) Type.
			//(ctr+f >> for std::string): http://www.trinitycore.net/d1/d17/ByteBuffer_8h_source.html
			//They use 0 byte to terminate the string in the stream
			
			//TODO: We can likely do some fancy pointer stuff to make this much cheaper.
			//Read a byte from the stream; Stop when we find a 0
			List<byte> stringBytes = new List<byte>();
			
			byte currentByte = source.ReadByte();
			
			//TODO: Security/prevent spoofs causing exceptions
			while(currentByte != 0)
			{
				stringBytes.Add(currentByte);

				currentByte = source.ReadByte();
			}
			
			//TODO: Invesitgate expected WoW/TC behavior for strings of length 0. Currently violates contract for return type.
			//Serializer design decision: Return null instead of String.Empty for no strings
			return stringBytes.Count == 0 ? null : System.Text.Encoding.ASCII.GetString(stringBytes.ToArray());
		}

		void ITypeSerializerStrategy.Write(object value, IWireMemberWriterStrategy dest)
		{
			Write((string)value, dest);
		}

		object ITypeSerializerStrategy.Read(IWireMemberReaderStrategy source)
		{
			return Read(source);
		}

		public string Read(ref string obj, IWireMemberReaderStrategy source)
		{
			obj = Read(source);

			return obj;
		}
	}
}
