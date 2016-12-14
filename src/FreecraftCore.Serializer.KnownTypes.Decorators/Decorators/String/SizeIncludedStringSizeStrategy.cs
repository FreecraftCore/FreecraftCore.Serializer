using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreecraftCore.Serializer.KnownTypes
{
	public class SizeIncludedStringSizeStrategy<TSizeType> : IStringSizeStrategy
		where TSizeType : struct
	{
		private ITypeSerializerStrategy<TSizeType> sizeSerializer { get; }

		public SizeIncludedStringSizeStrategy(ITypeSerializerStrategy<TSizeType> serializer)
		{
			if (serializer == null)
				throw new ArgumentNullException(nameof(serializer), $"Provided argument {serializer} is null.");

			sizeSerializer = serializer;
		}

		public int Size(IWireMemberReaderStrategy reader)
		{
			TSizeType size = sizeSerializer.Read(reader);

			//Using JonSkeets MiscUtils we can convert objects efficently
			return MiscUtil.Operator<TSizeType, int>.Convert(size);
		}

		public int Size(string stringValue, IWireMemberWriterStrategy writer)
		{
			int size = stringValue.Length;

			//add one for null terminator
			//Using JonSkeets MiscUtils we can convert objects efficently
			sizeSerializer.Write(MiscUtil.Operator<int, TSizeType>.Convert(size + 1), writer);

			return size;
		}
	}
}
