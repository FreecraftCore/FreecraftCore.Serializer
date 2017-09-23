using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reinterpret.Net;

namespace FreecraftCore.Serializer
{
	public class GenericTypePrimitiveSharedBufferSerializerStrategy<TType> : SharedBufferTypeSerializer<TType>
		where TType : struct
	{
		public GenericTypePrimitiveSharedBufferSerializerStrategy()
		{

		}

		//All generic primitive serializers are contextless
		public override SerializationContextRequirement ContextRequirement { get; } = SerializationContextRequirement.Contextless;

		protected override TType DeserializeFromBuffer(byte[] bytes)
		{
			return bytes.Reinterpret<TType>();
		}

		protected override bool PopulateSharedBufferWith(TType value)
		{
			//This pushes the value into the buffer.
			value.Reinterpret(SharedByteBuffer, 0);

			return true;
		}
	}
}
