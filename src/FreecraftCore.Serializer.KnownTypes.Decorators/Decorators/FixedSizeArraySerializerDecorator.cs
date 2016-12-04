using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreecraftCore.Serializer.KnownTypes
{
	/// <summary>
	/// Decorator for a fixed size array.
	/// </summary>
	/// <typeparam name="TObjectType">The type of the object the array contains.</typeparam>
	public class FixedSizeArraySerializerDecorator<TObjectType> : ArraySerializerDecorator<TObjectType>
	{
		/// <summary>
		/// Indicates the size of the fixed array.
		/// </summary>
		public byte ArraySize { get; }

		//TODO: Right now context doesn't do much. Make it so we can share serializers.
		public override SerializationContextRequirement ContextRequirement { get { return SerializationContextRequirement.RequiresContext; } }

		public FixedSizeArraySerializerDecorator(ISerializerProvider serializerProvider, byte arraySize) 
			: base(serializerProvider)
		{
			ArraySize = arraySize;
		}

		//override the reading and just provide the fixed size.
		protected override byte GetCollectionSize(IWireMemberReaderStrategy source)
		{
			return ArraySize;
		}

		//override the reading and just provide the fixed size.
		protected override byte SetCollectionSize(IWireMemberWriterStrategy dest, byte size)
		{
			return ArraySize;
		}
	}
}
