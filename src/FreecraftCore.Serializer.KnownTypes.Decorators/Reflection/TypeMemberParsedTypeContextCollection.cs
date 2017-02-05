using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Fasterflect;
using JetBrains.Annotations;

namespace FreecraftCore.Serializer
{
	public class TypeMemberParsedTypeContextCollection : IEnumerable<ISerializableTypeContext>
	{
		[NotNull]
		public Type TypeToInspect { get; }

		[NotNull]
		private Lazy<IEnumerable<ISerializableTypeContext>> contextCollection { get; }

		public TypeMemberParsedTypeContextCollection([NotNull] Type typeToInspect)
		{
			if (typeToInspect == null) throw new ArgumentNullException(nameof(typeToInspect));

			TypeToInspect = typeToInspect;
			contextCollection = new Lazy<IEnumerable<ISerializableTypeContext>>(CreateContextCollection, true);
		}

		protected IEnumerable<ISerializableTypeContext> CreateContextCollection()
		{
#if !NET35
			return TypeToInspect.Members(MemberTypes.Field | MemberTypes.Property, Flags.InstanceAnyVisibility)
				.Where(mi => mi.HasAttribute<WireMemberAttribute>())
				.Select(mi => new MemberInfoBasedSerializationContext(mi)) //provide memberinfo context; context is important for complex type members
				.ToArray();

#else
			//net35 doesn't have co/contra-variance. Unity3D does though because it's psuedo-net35. Just cast on net35
			return TypeToInspect.Members(MemberTypes.Field | MemberTypes.Property, Flags.InstanceAnyVisibility)
				.Where(mi => mi.HasAttribute<WireMemberAttribute>())
				.Select(mi => new MemberInfoBasedSerializationContext(mi) as ISerializableTypeContext) //provide memberinfo context; context is important for complex type members
				.ToArray();
#endif
		}

		public IEnumerator<ISerializableTypeContext> GetEnumerator()
		{
			return contextCollection.Value.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
