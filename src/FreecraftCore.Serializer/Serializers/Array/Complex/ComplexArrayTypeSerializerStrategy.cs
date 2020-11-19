using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Non-generic nameof metadata.
	/// </summary>
	internal class ComplexArrayTypeSerializerStrategy : BaseArraySerializerNonGenericMarker
	{

	}

	/// <summary>
	/// Contract for generic complex/custom array element type serializer.
	/// </summary>
	/// <typeparam name="T">The element type.</typeparam>
	/// <typeparam name="TElementSerializerType">The element type serializer.</typeparam>
	public sealed class ComplexArrayTypeSerializerStrategy<TElementSerializerType, T> : BaseArrayTypeSerializerStrategy<ComplexArrayTypeSerializerStrategy<TElementSerializerType, T>, T>
		where T : class //closest to primitive constraints we can get
		where TElementSerializerType : StatelessTypeSerializerStrategy<TElementSerializerType, T>, new()
	{
		/// <summary>
		/// The decorated element type serializer.
		/// </summary>
		private static TElementSerializerType DecoratedSerializer { get; } = new TElementSerializerType();

		static ComplexArrayTypeSerializerStrategy()
		{
			
		}

		/// <inheritdoc />
		public sealed override unsafe T[] Read(Span<byte> buffer, ref int offset)
		{
			//This is SUPER inefficient, causing MANY allocations and a BIG allocation and copy at the end
			//This should ONLY ever be used if we absolutely do not and cannot know how many elements there are.
			List<T> tempList = new List<T>();

			while(offset < buffer.Length)
				tempList.Add(DecoratedSerializer.Read(buffer, ref offset));

			return tempList.ToArray();
		}

		/// <inheritdoc />
		public sealed override unsafe void Write(T[] value, Span<byte> buffer, ref int offset)
		{
			if (value.Length == 0)
				return;

			for (int i = 0; i < value.Length; i++)
				DecoratedSerializer.Write(value[i], buffer, ref offset);
		}
	}
}
