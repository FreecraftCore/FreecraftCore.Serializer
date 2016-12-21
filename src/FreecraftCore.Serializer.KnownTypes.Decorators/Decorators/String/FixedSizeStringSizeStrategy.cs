using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


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
			if (size < 0)
				throw new ArgumentException($"Provided size {size} is less than 0.", nameof(size));

			FixedSize = size;
		}

		public int Size(IWireMemberReaderStrategy reader)
		{
			return FixedSize;
		}

		public int Size(string stringValue, IWireMemberWriterStrategy writer)
		{
			if (stringValue.Length > FixedSize)
				throw new InvalidOperationException($"Enountered string size larged than {FixedSize} declared.");

			return FixedSize;
		}
	}
}
