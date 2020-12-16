using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Metadata marker interface.
	/// </summary>
	public interface ITargetedTypeSerializerStrategy : ITypeSerializerStrategy
	{

	}

	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="T">The Type to serialize.</typeparam>
	public interface ITargetedTypeSerializerWritingStrategy<in T> : ITargetedTypeSerializerStrategy
	{
		//Cannot do internal protected because we must reference it in generated WireMessages
		//Partial externally implemented serialization.
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		void InternalWrite(T value, Span<byte> buffer, ref int offset);
	}

	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="T">The Type to serialize.</typeparam>
	public interface ITargetedTypeSerializerReadingStrategy<in T> : ITargetedTypeSerializerStrategy
	{
		//Cannot do internal protected because we must reference it in generated WireMessages
		//Partial externally implemented deserialization.
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		void InternalRead(T value, Span<byte> buffer, ref int offset);
	}

	/// <summary>
	/// Contract for types that implement serialization strategies for
	/// the specified <typeparamref name="T"/>.
	/// </summary>
	/// <typeparam name="T">The Type to serialize.</typeparam>
	public interface ITargetedTypeSerializerStrategy<in T> : ITargetedTypeSerializerReadingStrategy<T>, ITargetedTypeSerializerWritingStrategy<T>
	{

	}
}
