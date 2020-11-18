using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using FreecraftCore.Serializer;

namespace FreecraftCore
{
	/// <summary>
	/// Contract for types that are self-serializable.
	/// Means they can read/write their binary representation.
	/// Basically it means they are their own <see cref="ITypeSerializerStrategy{T}"/>
	/// implementation.
	/// </summary>
	/// <typeparam name="TMessageType"></typeparam>
	public interface ISelfSerializable<TMessageType> : ITypeSerializerStrategy<TMessageType>
		where TMessageType : ISelfSerializable<TMessageType>
	{
		
	}
}
