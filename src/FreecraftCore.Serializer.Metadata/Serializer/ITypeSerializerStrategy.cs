﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Metadata marker interface.
	/// </summary>
	public interface ITypeSerializerStrategy
	{
		/// <summary>
		/// Represents the type that the serializer can/does serialize.
		/// </summary>
		Type SerializableType { get; }
	}

	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="T">The Type to serialize.</typeparam>
	public interface ITypeSerializerWritingStrategy<in T> : ITypeSerializerStrategy
	{
		/// <summary>
		/// Writes a copy of <typeparamref name="T"/> <paramref name="value"/> starting at <paramref name="offset"/>
		/// into the buffer <paramref name="buffer"/>.
		/// </summary>
		/// <param name="value">The value to write.</param>
		/// <param name="buffer">The destination buffer to write the value into.</param>
		/// <param name="offset">The offset to start writing the value into.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		void Write(T value, Span<byte> buffer, ref int offset);
	}

	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="T">The Type to serialize.</typeparam>
	public interface ITypeSerializerReadingStrategy<out T> : ITypeSerializerStrategy
	{
		/// <summary>
		/// Reads a copy of <typeparamref name="T"/> from the buffer <paramref name="buffer"/>
		/// starting from <paramref name="offset"/>
		/// </summary>
		/// <param name="buffer">The source buffer to read from.</param>
		/// <param name="offset">The starting offset into the buffer to read from.</param>
		/// <returns></returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		T Read(Span<byte> buffer, ref int offset);
	}

	/// <summary>
	/// Contract for types that implement serialization strategies for
	/// the specified <typeparamref name="T"/>.
	/// </summary>
	/// <typeparam name="T">The Type to serialize.</typeparam>
	public interface ITypeSerializerStrategy<T> : ITypeSerializerWritingStrategy<T>, ITypeSerializerReadingStrategy<T>
	{

	}
}
