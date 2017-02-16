using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using JetBrains.Annotations;

#if !NET35
using System.Threading.Tasks;
#endif


namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Default implementation of the <see cref="IWireStreamWriterStrategy"/> that writes bytes into
	/// an internally managed stream.
	/// </summary>
	public class DefaultStreamWriterStrategy : DefaultStreamManipulationStrategy<MemoryStream>, IWireStreamWriterStrategy //we use MemoryStream for efficient ToArray
	{
		public DefaultStreamWriterStrategy([NotNull] MemoryStream stream)
			: base(stream, false)
		{

		}

		public DefaultStreamWriterStrategy()
			: base(new MemoryStream(), true)
		{

		}

		/// <inheritdoc />
		public byte[] GetBytes()
		{
			return ManagedStream.ToArray();
		}

		/// <inheritdoc />
		public void Write(byte data)
		{
			ManagedStream.WriteByte(data);
		}

		/// <inheritdoc />
		public void Write(byte[] data)
		{
			//Stream handles throwing. Don't need to validate or check.
			ManagedStream.Write(data, 0, data.Length);
		}

		/// <inheritdoc />
		public void Write(byte[] data, int offset, int count)
		{
			//Stream handles throwing. Don't need to validate or check.
			ManagedStream.Write(data, offset, count);
		}
	}
}
