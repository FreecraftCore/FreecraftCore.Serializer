using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FreecraftCore.Serializer
{
	//Used to override the key value
	public class ContextKeyOverrideDecorator : ISerializableTypeContext
	{
		private ISerializableTypeContext managedContext { get; }

		public SerializationContextRequirement ContextRequirement
		{
			get
			{
				return managedContext.ContextRequirement;
			}
		}

		public IEnumerable<Attribute> MemberMetadata
		{
			get
			{
				return managedContext.MemberMetadata;
			}
		}

		public IEnumerable<Attribute> TypeMetadata
		{
			get
			{
				return managedContext.TypeMetadata;
			}
		}

		public Type TargetType
		{
			get
			{
				return managedContext.TargetType;
			}
		}

		//Should we block set for this?
		public ContextualSerializerLookupKey? BuiltContextKey { get; set; }

		public ContextKeyOverrideDecorator(ISerializableTypeContext context, ContextualSerializerLookupKey key)
		{
			managedContext = context;

			BuiltContextKey = key;
		}
	}
}
