using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace FreecraftCore.Serializer.KnownTypes
{
	public abstract class SubComplexTypeSerializer<TBaseType> : ComplexTypeSerializer<TBaseType>
	{
		protected SubComplexTypeSerializer([NotNull] IEnumerable<IMemberSerializationMediator<TBaseType>> serializationDirections) 
			: base(serializationDirections)
		{

		}
	}
}
