using System;
using System.Collections.Generic;
using System.Text;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Contract for array type serializers.
	/// </summary>
	/// <typeparam name="TChildType">The child serializer.</typeparam>
	/// <typeparam name="TElementType">The element type.</typeparam>
	public abstract class BaseArrayTypeSerializerStrategy<TChildType, TElementType> : StatelessTypeSerializerStrategy<TChildType, TElementType[]>
		where TChildType : BaseArrayTypeSerializerStrategy<TChildType, TElementType>, new()
	{

	}
}
