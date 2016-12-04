using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreecraftCore.Payload.Serializer
{
	public class SubComplexTypeSerializerDecorator<TBaseType> : ITypeSerializerStrategy<TBaseType>
		where TBaseType : class, new()
	{
		public Type SerializerType { get { return typeof(TBaseType); } }

		private Dictionary<int, ITypeSerializerStrategy> serializationSubtypeMap { get; }

		private ISerializerFactory serializerFactoryService { get; }

		public SubComplexTypeSerializerDecorator(ISerializerFactory serializerFactory)
		{
			if (serializerFactory == null)
				throw new ArgumentNullException(nameof(serializerFactory), $"Provided {nameof(ISerializerFactory)} service was null.");

			serializerFactoryService = serializerFactory;
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
	}
}
