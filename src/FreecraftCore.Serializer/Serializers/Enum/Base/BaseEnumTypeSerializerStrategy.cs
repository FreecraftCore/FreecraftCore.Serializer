using System;
using System.Collections.Generic;
using System.Text;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Contract for <see cref="Enum"/> serializer strategy.
	/// </summary>
	/// <typeparam name="TChildType">The child serializer type to use for <see cref="StatelessTypeSerializerStrategy{TChildType,T}"/> input.</typeparam>
	/// <typeparam name="T">The enum type.</typeparam>
	public abstract class BaseEnumTypeSerializerStrategy<TChildType, T> : StatelessTypeSerializerStrategy<TChildType, T> 
		where TChildType : BaseEnumTypeSerializerStrategy<TChildType, T>, new()
		where T : Enum
	{

	}
}
