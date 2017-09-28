using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using JetBrains.Annotations;


namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Service that generators context keys based on contextual information provided to it.
	/// </summary>
	public class ContextLookupKeyFactoryService : IContextualSerializerLookupKeyFactory
	{
		/// <inheritdoc />
		public ContextualSerializerLookupKey Create([NotNull] ISerializableTypeContext context)
		{
			if (context == null) throw new ArgumentNullException(nameof(context));

			//We must inspect the metadata to build the key
			if (!context.HasContextualMemberMetadata())
				return new ContextualSerializerLookupKey(ContextTypeFlags.None, NoContextKey.Value, context.TargetType);

			//Build the relevant flags
			ContextTypeFlags flags = ContextTypeFlags.None;

			if(context.HasMemberAttribute<ReadToEndAttribute>())
				flags |= ContextTypeFlags.ReadToEnd;

			if(context.HasMemberAttribute<ReverseDataAttribute>())
				flags |= ContextTypeFlags.Reverse;

			if(context.HasMemberAttribute<EnumStringAttribute>())
				flags |= ContextTypeFlags.EnumString;

			//Check for no terminate too
			if(context.HasMemberAttribute<DontTerminateAttribute>())
				flags |= ContextTypeFlags.DontTerminate;

			if(context.HasMemberAttribute<CompressAttribute>())
				flags |= ContextTypeFlags.Compressed;

			//Encoding requires multiple flags to be set.
			//We can't rely on the context key since it may be used for size
			if(context.HasMemberAttribute<EncodingAttribute>())
			{
				flags |= ContextTypeFlags.Encoding;
				EncodingAttribute attri = context.GetMemberAttribute<EncodingAttribute>();
				switch(attri.DesiredEncodingType)
				{
					case EncodingType.ASCII:
						flags |= ContextTypeFlags.ASCII;
						break;

					case EncodingType.UTF16:
						flags |= ContextTypeFlags.UTF16;
						break;

					case EncodingType.UTF32:
						flags |= ContextTypeFlags.UTF32;
						break;
				}
			}

			if(context.HasMemberAttribute<SendSizeAttribute>())
				return new ContextualSerializerLookupKey(flags | ContextTypeFlags.SendSize, new SendSizeContextKey(context.GetMemberAttribute<SendSizeAttribute>().TypeOfSize), context.TargetType);

			if(context.HasMemberAttribute<KnownSizeAttribute>())
				return new ContextualSerializerLookupKey(flags | ContextTypeFlags.FixedSize, new SizeContextKey(context.GetMemberAttribute<KnownSizeAttribute>().KnownSize), context.TargetType);

			//If we're here then we have flags that weren't mutually exclusive
			return new ContextualSerializerLookupKey(flags, new NoContextKey(), context.TargetType);
		}

		/// <inheritdoc />
		public ContextualSerializerLookupKey Create(MemberInfo memberInfo)
		{
			//Clever and lazy
			return Create(new MemberInfoBasedSerializationContext(memberInfo));
		}
	}
}
