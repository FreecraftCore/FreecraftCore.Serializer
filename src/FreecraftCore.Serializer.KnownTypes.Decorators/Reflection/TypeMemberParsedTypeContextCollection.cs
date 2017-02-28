using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Reflect.Extent;

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
			return TypeToInspect.GetTypeInfo().DeclaredMembers
				.Where(mi => mi is FieldInfo || mi is PropertyInfo)
				.Where(mi => mi.HasAttribute<WireMemberAttribute>())
				.Select(mi => new MemberInfoBasedSerializationContext(mi)) //provide memberinfo context; context is important for complex type members
				.ToArray();
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
