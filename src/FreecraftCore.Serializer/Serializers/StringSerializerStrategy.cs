using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FreecraftCore.Serializer.KnownTypes
{
	/// <summary>
	/// <see cref="ITypeSerializerStrategy"/> for Type <see cref="string"/>.
	/// </summary>
	[KnownTypeSerializer]
	public class StringSerializerStrategy : SimpleTypeSerializerStrategy<string>
	{
		//All primitive serializer stragies are contextless
		/// <inheritdoc />
		public override SerializationContextRequirement ContextRequirement { get; } = SerializationContextRequirement.Contextless;

		/// <inheritdoc />
		public override void Write(string value, IWireStreamWriterStrategy dest)
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
		public override string Read(IWireStreamReaderStrategy source)
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
			return stringBytes.Count == 0 ? null : Encoding.ASCII.GetString(stringBytes.ToArray());
		}

		/// <inheritdoc />
		public override async Task WriteAsync(string value, IWireStreamWriterStrategyAsync dest)
		{
			if (dest == null) throw new ArgumentNullException(nameof(dest));

			//See sync method for doc
			byte[] stringBytes = Encoding.ASCII.GetBytes(value);

			await dest.WriteAsync(stringBytes);

			//Write the null terminator; Client expects it.
			await dest.WriteAsync(0);
		}

		/// <inheritdoc />
		public override async Task<string> ReadAsync(IWireStreamReaderStrategyAsync source)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));
			//See sync method for doc

			//TODO: Find an average size for WoW strings
			List<byte> stringBytes = new List<byte>();

			byte currentByte = await source.ReadByteAsync();

			//TODO: Security/prevent spoofs causing exceptions
			while (currentByte != 0)
			{
				stringBytes.Add(currentByte);

				currentByte = await source.ReadByteAsync();
			}

			//TODO: Invesitgate expected WoW/TC behavior for strings of length 0. Currently violates contract for return type.
			//Serializer design decision: Return null instead of String.Empty for no strings
			return stringBytes.Count == 0 ? null : Encoding.ASCII.GetString(stringBytes.ToArray());
		}
	}
}
