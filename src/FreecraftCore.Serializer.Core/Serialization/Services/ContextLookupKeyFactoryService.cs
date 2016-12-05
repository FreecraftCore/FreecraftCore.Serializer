using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Service that generators context keys based on contextual information provided to it.
	/// </summary>
	public class ContextLookupKeyFactoryService : IContextualSerializerLookupKeyFactory
	{
		//Context is talked about a lot in this project. Context doesn't matter here. Any instance of this class yields the same results
		public ContextualSerializerLookupKey Create(ISerializableTypeContext context)
		{
			//We must inspect the metadata to build the key
			if(context.HasContextualMemberMetadata())
			{
				//Check member data for fixed size attribute
				//It's the only one that causes context to matter
				if (context.HasMemberAttribute<KnownSizeAttribute>())
					return new ContextualSerializerLookupKey(ContextTypeFlags.FixedSize, new FixedSizeContextKey(context.GetMemberAttribute<KnownSizeAttribute>().KnownSize), context.TargetType);
			}

			return new ContextualSerializerLookupKey(ContextTypeFlags.None, NoContextKey.Value, context.TargetType);
		}

		public ContextualSerializerLookupKey Create(MemberInfo memberInfo)
		{
			//Clever and lazy
			return Create(new MemberInfoBasedSerializationContext(memberInfo));
		}
	}
}
