using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Contract for stateless type serializers that implement <see cref="ITypeSerializerStrategy{T}"/>
	/// for read/write serialization. Additionally it provides a stateless/shared singleton instance
	/// of the serializer.
	/// </summary>
	/// <typeparam name="TChildType">The child type to construct a singleton instance for.</typeparam>
	/// <typeparam name="T">The serialized type.</typeparam>
	public abstract class StatelessTypeSerializerStrategy<TChildType, T> : ITypeSerializerStrategy<T>
		where TChildType : StatelessTypeSerializerStrategy<TChildType, T>, new()
	{
		/// <summary>
		/// The <see cref="StatelessTypeSerializerStrategy{TChildType,T}"/>'s static/singleton instance reference object.
		/// </summary>
		public static TChildType Instance { get; } = new TChildType();

		/// <inheritdoc />
		public Type SerializableType { get; } = typeof(T);

		// See: https://csharpindepth.com/Articles/Singleton
		// Explicit static constructor to tell C# compiler
		// not to mark type as beforefieldinit
		static StatelessTypeSerializerStrategy()
		{
			//DO NOT REMOVE!!
		}

		/// <inheritdoc />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public abstract T Read(Span<byte> source, ref int offset);

		/// <inheritdoc />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public abstract void Write(T value, Span<byte> destination, ref int offset);
	}
}
