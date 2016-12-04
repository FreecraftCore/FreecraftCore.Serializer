using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace FreecraftCore.Serializer.KnownTypes
{
	/*public class SubComplexTypeSerializerDecorator<TBaseType> : ITypeSerializerStrategy<TBaseType>, ISerializerDecoraterHandler
		where TBaseType : class, new()
	{
		public Type SerializerType { get { return typeof(TBaseType); } }

		private ISerializerFactory serializerFactoryService { get; }

		public SubComplexTypeSerializerDecorator(ISerializerFactory serializerFactory)
		{
			if (serializerFactory == null)
				throw new ArgumentNullException(nameof(serializerFactory), $"Provided {nameof(ISerializerFactory)} service was null.");

			serializerFactoryService = serializerFactory;
		}

		public bool CanHandle(Type type)
		{
			if (type == null)
				throw new ArgumentNullException(nameof(type), $"Provided argument {nameof(type)} was null.");

			//Check if the type has wirebase type attributes.
			//If it does then the type is complex and can have subtypes coming across the wire
			return type.GetCustomAttributes<WireMessageBaseTypeAttribute>(false).Count() != 0;
		}

		public TBaseType Read(IWireMemberReaderStrategy source)
		{
			//throw new NotImplementedException();
			return new TBaseType();
		}

		public void Write(TBaseType value, IWireMemberWriterStrategy dest)
		{
			//throw new NotImplementedException();
		}
	}*/

/*//First all children must be registered. Their object graphs may contain members that are complex
				IEnumerable<WireMessageBaseTypeAttribute> subtypeAttributes = typeBeingSerialized.GetCustomAttributes<WireMessageBaseTypeAttribute>(false);

				foreach (Type childType in subtypeAttributes.Select(x => x.ChildType))
				{
					//Call generic registry
					contractRegistry.CallMethod(new Type[] { childType }, "RegisterType"); //todo: replace reflection
				}

				//At this point all children are registered. A SubComplexTypeDecorator would be able to gather and map the serializers
				ITypeSerializerStrategy generatedSerializer = typeof(SubComplexTypeSerializerDecorator<>).MakeGenericType(typeBeingSerialized).CreateInstance(serializerFactoryService) as ITypeSerializerStrategy;

				if (generatedSerializer == null)
					throw new InvalidOperationException($"Failed to generate decorated serializer for Type: {typeBeingSerialized}.");

				return generatedSerializer;*/
}
