using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace FreecraftCore.Serializer.KnownTypes
{
	/// <summary>
	/// Size strategy for collections that send a <see cref="ushort"/> size.
	/// </summary>
	public class UInt16SizeCollectionSizeStrategy : ICollectionSizeStrategy
	{
		private ITypeSerializerStrategy<ushort> shortSerializer { get; }

		public UInt16SizeCollectionSizeStrategy(ITypeSerializerStrategy<ushort> serializer)
		{
			//TODO: Null check

			shortSerializer = serializer;
		}

		/// <summary>
		/// Determines the size of the collection from the stream.
		/// </summary>
		public int Size(IWireMemberReaderStrategy reader)
		{
			//Reads a short from the stream.
			return shortSerializer.Read(reader);
		}

		/// <summary>
		/// The size to consider the collection.
		/// </summary>
		public int Size<TCollectionType, TElementType>(TCollectionType collection, IWireMemberWriterStrategy writer)
			where TCollectionType : IEnumerable, IEnumerable<TElementType>
		{

			//Write a short size to the stream
			shortSerializer.Write((ushort)collection.Count(), writer);

			//We don't know the size so just provide the size of the collection
			return collection.Count();
		}
	}
}
