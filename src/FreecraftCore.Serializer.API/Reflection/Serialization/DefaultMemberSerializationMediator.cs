using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Fasterflect;
using JetBrains.Annotations;

namespace FreecraftCore.Serializer
{
	public class DefaultMemberSerializationMediator<TContainingType> : MemberSerializationMediator<TContainingType>
	{
		public DefaultMemberSerializationMediator([NotNull] MemberInfo memberInfo, [NotNull] ITypeSerializerStrategy serializer) 
			: base(memberInfo, serializer)
		{

		}

		public override void WriteMember([NotNull] TContainingType obj, [NotNull] IWireMemberWriterStrategy dest)
		{
			if (obj == null) throw new ArgumentNullException(nameof(obj));
			if (dest == null) throw new ArgumentNullException(nameof(dest));

			//TODO: Check how TC handles optionals or nulls.
			//Do we write nothing? Do we write 0?
			//object memberValue = value.TryGetValue(serializerInfo.MemberInformation.Name);
			object memberValue = MemberGetter(obj); //instead of fasterflect we use delegate to getter

			//We used to do a null check here. It was kind of pointless to do and it was slightly expensive since they weren't all
			//reference types. No reason to check nullness; it's just a perf issue

			try
			{
				TypeSerializer.Write(memberValue, dest);
			}
			catch (NullReferenceException e)
			{
				throw new InvalidOperationException($"Serializer failed to find serializer for member name: {MemberInformation.Name} and type: {MemberInformation.Type()}.", e);
			}
		}

		public override void WriteMember(object obj, [NotNull] IWireMemberWriterStrategy dest)
		{
			WriteMember((TContainingType)obj, dest);
		}

		public override void SetMember(object obj, IWireMemberReaderStrategy source)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));

			SetMember((TContainingType)obj, source);
		}

		public override void SetMember(TContainingType obj, [NotNull] IWireMemberReaderStrategy source)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));

			//TODO: Can we do this more efficiently without reading string name?
			obj.TrySetValue(MemberInformation.Name, TypeSerializer.Read(source));
		}
	}
}
