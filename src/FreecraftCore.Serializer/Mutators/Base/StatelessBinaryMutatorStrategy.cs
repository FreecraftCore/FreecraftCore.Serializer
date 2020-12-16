using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Contract for stateless type serializers that implement <see cref="IBinaryMutatorStrategy"/>
	/// for binary or buffer mutation. Additionally it provides a stateless/shared singleton instance
	/// of the serializer.
	/// </summary>
	/// <typeparam name="TChildType">The child type to construct a singleton instance for.</typeparam>
	public abstract class StatelessBinaryMutatorStrategy<TChildType> : IBinaryMutatorStrategy, ISingletonInstanceProvidable<TChildType>
		where TChildType : StatelessBinaryMutatorStrategy<TChildType>, new()
	{
		/// <summary>
		/// The <see cref="StatelessBinaryMutatorStrategy{TChildType}"/>'s static/singleton instance reference object.
		/// </summary>
		public static TChildType Instance { get; } = new TChildType();

		// See: https://csharpindepth.com/Articles/Singleton
		// Explicit static constructor to tell C# compiler
		// not to mark type as beforefieldinit
		static StatelessBinaryMutatorStrategy()
		{
			//DO NOT REMOVE!!
		}

		/// <inheritdoc />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public abstract void Mutate(Span<byte> source, ref int sourceOffset, Span<byte> destination, ref int destinationOffset);

		/// <inheritdoc />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public abstract void UnMutate(Span<byte> source, ref int sourceOffset, Span<byte> destination, ref int destinationOffset);

		/// <inheritdoc />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public TChildType GetInstance()
		{
			return Instance;
		}
	}
}
