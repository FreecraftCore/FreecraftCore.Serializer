using System;
using System.Collections.Generic;
using System.Text;

namespace FreecraftCore.Serializer
{
	public abstract class BaseArrayTypeSerializerStrategy<TChildType, TElementType> : StatelessTypeSerializerStrategy<TChildType, TElementType[]> 
		where TChildType : BaseArrayTypeSerializerStrategy<TChildType, TElementType>, new()
	{

	}
}
