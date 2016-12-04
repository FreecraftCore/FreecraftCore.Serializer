using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreecraftCore.Serializer
{
	public class MemberInfoBasedSerializationContext : ISerializableTypeContext
	{
		public SerializationContextRequirement ContextRequirement { get; }

		public IEnumerable<Attribute> Metadata
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public Type TargetType
		{
			get
			{
				throw new NotImplementedException();
			}
		}
	}
}
