using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;


namespace FreecraftCore.Serializer.KnownTypes
{
	/// <summary>
	/// Size strategy for collections that send a <see cref="ushort"/> size.
	/// </summary>
	public class UInt16SizeCollectionSizeStrategy : ICollectionSizeStrategy
	{
		[NotNull]
		private ITypeSerializerStrategy<ushort> shortSerializer { get; }

		public UInt16SizeCollectionSizeStrategy([NotNull] ITypeSerializerStrategy<ushort> serializer)
		{
			if (serializer == null) throw new ArgumentNullException(nameof(serializer));

			shortSerializer = serializer;
		}

		/// <summary>
		/// Determines the size of the collection from the stream.
		/// </summary>
		public int Size(IWireStreamReaderStrategy reader)
		{
			if (reader == null) throw new ArgumentNullException(nameof(reader));

			//Reads a short from the stream.
			return shortSerializer.Read(reader);
		}

		/// <summary>
		/// The size to consider the collection.
		/// </summary>
		public int Size<TCollectionType, TElementType>(TCollectionType collection, IWireStreamWriterStrategy writer)
			where TCollectionType : IEnumerable, IEnumerable<TElementType>
		{
			if (collection == null) throw new ArgumentNullException(nameof(collection));
			if (writer == null) throw new ArgumentNullException(nameof(writer));

			//Write a short size to the stream
			shortSerializer.Write((ushort)collection.Count(), writer);

			//We don't know the size so just provide the size of the collection
			return collection.Count();
		}
	}
}
