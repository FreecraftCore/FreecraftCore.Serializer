﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Reinterpret.Net;

namespace FreecraftCore.Serializer
{
	//T does not need to be newable because it will be abstract
	/// <summary>
	/// Base implementation for a serialization strategy able to the handle complex process
	/// of polymorphic type serialization.
	/// </summary>
	/// <typeparam name="TChildType">The child type serializer.</typeparam>
	/// <typeparam name="T">The base polymorphic type.</typeparam>
	/// <typeparam name="TPolymorphicKeySizeType">The primitive type for the polymorphic key/data.</typeparam>
	/// <typeparam name="TPolymorphicKeySerializerType">The serializer type for the polymorphic key.</typeparam>
	public abstract class BasePolymorphicAutoGeneratedRecordSerializerStrategy<TChildType, T, TPolymorphicKeySizeType, TPolymorphicKeySerializerType>
		: StatelessTypeSerializerStrategy<TChildType, T>, ITargetedTypeSerializerStrategy<T>
		where TChildType : StatelessTypeSerializerStrategy<TChildType, T>, new()
		where TPolymorphicKeySizeType : unmanaged
		where T : IWireMessage<T>
		where TPolymorphicKeySerializerType : StatelessTypeSerializerStrategy<TPolymorphicKeySerializerType, TPolymorphicKeySizeType>, new()
	{
		/// <summary>
		/// The serializer strategy for the generic key type.
		/// </summary>
		protected static TPolymorphicKeySerializerType KeySerializer { get; } = new TPolymorphicKeySerializerType();

		static BasePolymorphicAutoGeneratedRecordSerializerStrategy()
		{
			if(!typeof(T).IsAbstract)
				throw new InvalidOperationException($"Type: {typeof(T).Name} must be abstract to be used in polymorphic serialization.");
		}

		/// <summary>
		/// Auto-generated deserialization/read code.
		/// </summary>
		/// <param name="buffer">The buffer to read from.</param>
		/// <param name="offset">Initial offset into the buffer.</param>
		/// <returns>Deserialized instance of the requested Type.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override T Read(Span<byte> buffer, ref int offset)
		{
			//This reads the key, but doesn't move the offset because consuming types don't allow their type information
			//to be consumed by the serializer hidden away from their control.
			TPolymorphicKeySizeType key = KeySerializer.Read(buffer, offset); //do not use ref here

			//The reason we cast the key is it's just plain easier.
			return CreateType(key.Reinterpret<TPolymorphicKeySizeType, int>(), buffer, ref offset);
		}

		/// <summary>
		/// Auto-generated serialization/write code.
		/// </summary>
		/// <param name="value">The value to write.</param>
		/// <param name="buffer">The destination buffer to write to.</param>
		/// <param name="offset">Initial offset into the buffer.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public sealed override void Write(T value, Span<byte> buffer, ref int offset)
		{
			value.Write(value, buffer, ref offset);
		}

		/// <summary>
		/// Creates a derived type that inherits from the base type T.
		/// Using the input <see cref="key"/> to determine which child type.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="buffer"></param>
		/// <param name="offset"></param>
		/// <returns></returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected abstract T CreateType(int key, Span<byte> buffer, ref int offset);

		//These exist and are implemented ONLY because some source code generation expected them to exist
		//as a legacy reason.
		//Though pretty sure this won't work AT ALL for polymorphic types since IWireMessage calls this and we call Read
		//This will absolutely break.
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void InternalRead(T baseTypeStub, Span<byte> buffer, ref int offset)
		{
			baseTypeStub.Read(buffer, ref offset);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void InternalWrite(T baseTypeStub, Span<byte> buffer, ref int offset)
		{
			baseTypeStub.Write(baseTypeStub, buffer, ref offset);
		}
	}

	//T does not need to be newable because it will be abstract
	/// <summary>
	/// Base implementation for a serialization strategy able to the handle complex process
	/// of polymorphic type serialization.
	/// </summary>
	/// <typeparam name="TChildType">The child type serializer.</typeparam>
	/// <typeparam name="T">The base polymorphic type.</typeparam>
	/// <typeparam name="TPolymorphicKeySizeType">The primitive type for the polymorphic key/data.</typeparam>
	public abstract class BasePolymorphicAutoGeneratedRecordSerializerStrategy<TChildType, T, TPolymorphicKeySizeType> 
		: BasePolymorphicAutoGeneratedRecordSerializerStrategy<TChildType, T, TPolymorphicKeySizeType, GenericTypePrimitiveSerializerStrategy<TPolymorphicKeySizeType>>
		where TChildType : StatelessTypeSerializerStrategy<TChildType, T>, new()
		where TPolymorphicKeySizeType : unmanaged
		where T : IWireMessage<T>
	{
		
	}
}