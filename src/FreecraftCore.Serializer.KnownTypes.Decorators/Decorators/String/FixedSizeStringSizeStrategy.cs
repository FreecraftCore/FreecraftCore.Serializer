using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;


namespace FreecraftCore.Serializer.KnownTypes
{
	public class FixedSizeStringSizeStrategy : IStringSizeStrategy
	{
		/// <summary>
		/// The fixed size length of the string.
		/// </summary>
		public int FixedSize { get; }

		public FixedSizeStringSizeStrategy(int size)
		{
			if (size <= 0)
				throw new ArgumentOutOfRangeException(nameof(size), size, $"Provided size {size} is less than or equal to 0.");

			FixedSize = size;
		}

		/// <inheritdoc />
		public int Size(IWireStreamReaderStrategy reader)
		{
			if (reader == null) throw new ArgumentNullException(nameof(reader));

			return FixedSize;
		}

		/// <inheritdoc />
		public int Size(string stringValue, IWireStreamWriterStrategy writer)
		{
			//Strings can't be null as fixed size. Force empty.
			if (stringValue == null) throw new ArgumentNullException(nameof(stringValue));
			if (writer == null) throw new ArgumentNullException(nameof(writer));

			if (stringValue.Length > FixedSize)
				throw new InvalidOperationException($"Enountered string size larged than {FixedSize} declared.");

			return FixedSize;
		}
	}
}
