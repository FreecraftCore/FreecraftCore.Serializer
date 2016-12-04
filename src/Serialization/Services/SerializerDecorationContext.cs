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

		public IEnumerable<ITypeSerializerStrategy> KnownSerializers { get; }

		public SerializerDecorationContext(IEnumerable<Attribute> attributes, IEnumerable<ITypeSerializerStrategy> known)
		{
			if (attributes == null)
				throw new ArgumentNullException(nameof(attributes), $"Provided collection of meta-data attributes '{nameof(attributes)}' is null.");

			if (known == null)
				throw new ArgumentNullException(nameof(known), $"Provided collection of known {nameof(ITypeSerializerStrategy)} is null.");

			MemberAttributes = attributes;
			KnownSerializers = known;
		}
	}
}
