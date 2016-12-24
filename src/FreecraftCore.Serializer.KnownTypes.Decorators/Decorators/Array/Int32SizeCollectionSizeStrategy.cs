using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace FreecraftCore.Serializer.KnownTypes
{
	/// <summary>
	/// Size strategy for collections that send a <see cref="int"/> size.
	/// </summary>
	public class Int32SizeCollectionSizeStrategy : ICollectionSizeStrategy
	{
		private ITypeSerializerStrategy<int> intSerializer { get; }

		public Int32SizeCollectionSizeStrategy(ITypeSerializerStrategy<int> serializer)
		{
			//TODO: Null check

			intSerializer = serializer;
		}

		/// <summary>
		/// Determines the size of the collection from the stream.
		/// </summary>
		public int Size(IWireMemberReaderStrategy reader)
		{
			//Reads a int from the stream.
			return intSerializer.Read(reader);
		}

		/// <summary>
		/// The size to consider the collection.
		/// </summary>
		public int Size<TCollectionType, TElementType>(TCollectionType collection, IWireMemberWriterStrategy writer)
			where TCollectionType : IEnumerable, IEnumerable<TElementType>
		{
			//Write an int size to the stream
			intSerializer.Write((ushort)collection.Count(), writer);

			//We don't know the size so just provide the size of the collection
			return collection.Count();
		}
	}
}
