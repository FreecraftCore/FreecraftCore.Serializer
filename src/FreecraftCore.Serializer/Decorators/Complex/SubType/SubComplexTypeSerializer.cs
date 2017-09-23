using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace FreecraftCore.Serializer.KnownTypes
{
	public abstract class SubComplexTypeSerializer<TBaseType> : ComplexTypeSerializer<TBaseType>
	{
		[NotNull]
		protected IDeserializationPrototypeFactory<TBaseType> prototypeGeneratorService { get; }

		protected SubComplexTypeSerializer([NotNull] IDeserializationPrototypeFactory<TBaseType> prototypeGenerator, [NotNull] IEnumerable<IMemberSerializationMediator<TBaseType>> serializationDirections, 
			[NotNull] IGeneralSerializerProvider serializerProvider) 
			: base(serializationDirections, serializerProvider)
		{
			if (prototypeGenerator == null) throw new ArgumentNullException(nameof(prototypeGenerator));

			this.prototypeGeneratorService = prototypeGenerator;
		}
	}
}
