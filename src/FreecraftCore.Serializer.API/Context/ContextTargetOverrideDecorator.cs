using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FreecraftCore.Serializer
{
	//Use to override target type
	public class ContextTargetOverrideDecorator : ISerializableTypeContext
	{
		public ContextualSerializerLookupKey? BuiltContextKey
		{
			get
			{
				return ((ISerializableTypeContext)managedContext).BuiltContextKey;
			}

			set
			{
				((ISerializableTypeContext)managedContext).BuiltContextKey = value;
			}
		}

		public SerializationContextRequirement ContextRequirement
		{
			get
			{
				return ((ISerializableTypeContext)managedContext).ContextRequirement;
			}
		}

		public IEnumerable<Attribute> MemberMetadata
		{
			get
			{
				return ((ISerializableTypeContext)managedContext).MemberMetadata;
			}
		}

		public Type TargetType { get; }

		public IEnumerable<Attribute> TypeMetadata
		{
			get
			{
				return ((ISerializableTypeContext)managedContext).TypeMetadata;
			}
		}

		private ISerializableTypeContext managedContext { get; }

		public ContextTargetOverrideDecorator(ISerializableTypeContext context, Type targetType)
		{
			managedContext = context;

			TargetType = targetType;
		}
	}
}
