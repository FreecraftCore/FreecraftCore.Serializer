using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreecraftCore.Payload.Serializer
{
	public class SerializerDecorationContext
	{
		public IEnumerable<Attribute> MemberAttributes { get; }

		public IReadOnlyDictionary<Type, ITypeSerializerStrategy> KnownSerializers { get; }

		public SerializerDecorationContext(IEnumerable<Attribute> attributes, IReadOnlyDictionary<Type, ITypeSerializerStrategy> known)
		{
			if (attributes == null)
				throw new ArgumentNullException(nameof(attributes), $"Provided collection of meta-data attributes '{nameof(attributes)}' is null.");

			if (known == null)
				throw new ArgumentNullException(nameof(known), $"Provided {nameof(IReadOnlyDictionary<Type, ITypeSerializerStrategy>)} is null.");

			MemberAttributes = attributes;
			KnownSerializers = known;
		}
	}
}
