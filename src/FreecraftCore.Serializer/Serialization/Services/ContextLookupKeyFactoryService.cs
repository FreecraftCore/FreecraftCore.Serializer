using System;
using System.Collections.Concurrent;
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
		private object syncObj = new object();

		private Dictionary<Type, Dictionary<ContextTypeFlags, ContextualSerializerLookupKey>> CachedKeyStore { get; }
			= new Dictionary<Type, Dictionary<ContextTypeFlags, ContextualSerializerLookupKey>>();

		/// <inheritdoc />
		public ContextualSerializerLookupKey Create([NotNull] ISerializableTypeContext context)
		{
			if (context == null) throw new ArgumentNullException(nameof(context));

			//We must inspect the metadata to build the key
			if(!context.HasContextualMemberMetadata())
				GetKeyFromStorage(ContextTypeFlags.None, context.TargetType);

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
			return GetKeyFromStorage(flags, context.TargetType);
		}

		private ContextualSerializerLookupKey GetKeyFromStorage(ContextTypeFlags flags, Type t)
		{
			lock(syncObj)
			{
				if(CachedKeyStore.ContainsKey(t))
				{
					if(CachedKeyStore[t].ContainsKey(flags))
						return CachedKeyStore[t][flags];
					else
						return CachedKeyStore[t][flags] = new ContextualSerializerLookupKey(flags, NoContextKey.Value, t);
				}
				else
				{
					CachedKeyStore.Add(t, new Dictionary<ContextTypeFlags, ContextualSerializerLookupKey>());
					return GetKeyFromStorage(flags, t);
				}
			}
		}

		/// <inheritdoc />
		public ContextualSerializerLookupKey Create(MemberInfo memberInfo)
		{
			//Clever and lazy
			return Create(new MemberInfoBasedSerializationContext(memberInfo));
		}
	}
}
