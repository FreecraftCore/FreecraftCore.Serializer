using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;


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
				//Build the relevant flags
				ContextTypeFlags flags = ContextTypeFlags.None;

				if (context.HasMemberAttribute<ReverseDataAttribute>())
					flags |= ContextTypeFlags.Reverse;

				if (context.HasMemberAttribute<EnumStringAttribute>())
					flags |= ContextTypeFlags.EnumString;

				if (context.HasMemberAttribute<SendSizeAttribute>())
					return new ContextualSerializerLookupKey(flags | ContextTypeFlags.SendSize, new SendSizeContextKey(context.GetMemberAttribute<SendSizeAttribute>().TypeOfSize), context.TargetType);

				if (context.HasMemberAttribute<KnownSizeAttribute>())
					return new ContextualSerializerLookupKey(flags | ContextTypeFlags.FixedSize, new SizeContextKey(context.GetMemberAttribute<KnownSizeAttribute>().KnownSize), context.TargetType);

				//If we're here then we have flags that weren't mutually exclusive
				return new ContextualSerializerLookupKey(flags, new NoContextKey(), context.TargetType);
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
