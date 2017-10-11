using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace FreecraftCore.Serializer
{
	public class DefaultMemberSerializationMediator<TContainingType, TMemberType> : MemberSerializationMediator<TContainingType, TMemberType>
	{
		public DefaultMemberSerializationMediator([NotNull] MemberInfo memberInfo, [NotNull] ITypeSerializerStrategy serializer) 
			: base(memberInfo, serializer)
		{

		}

		public override void WriteMember([NotNull] TContainingType obj, [NotNull] IWireStreamWriterStrategy dest)
		{
			if (obj == null) throw new ArgumentNullException(nameof(obj));
			if (dest == null) throw new ArgumentNullException(nameof(dest));

			//TODO: Check how TC handles optionals or nulls.
			//Do we write nothing? Do we write 0?
			//object memberValue = value.TryGetValue(serializerInfo.MemberInformation.Name);
			object memberValue = MemberGetter.Getter(obj); //instead of fasterflect we use delegate to getter

			//We used to do a null check here. It was kind of pointless to do and it was slightly expensive since they weren't all
			//reference types. No reason to check nullness; it's just a perf issue

			TypeSerializer.Write(memberValue, dest);
		}

		public override void SetMember(TContainingType obj, [NotNull] IWireStreamReaderStrategy source)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));

			try
			{
				MemberSetter.Setter(obj, (TMemberType) TypeSerializer.Read(source));
			}
			catch (Exception e)
			{
				throw new InvalidOperationException($"Failed to set member {MemberInformation.Name} on Type: {MemberInformation.DeclaringType} for Type: {obj.GetType().Name}.", e);
			}
		}

		/// <inheritdoc />
		public override async Task WriteMemberAsync(TContainingType obj, IWireStreamWriterStrategyAsync dest)
		{
			if (obj == null) throw new ArgumentNullException(nameof(obj));
			if (dest == null) throw new ArgumentNullException(nameof(dest));

			//TODO: Check how TC handles optionals or nulls.
			//Do we write nothing? Do we write 0?
			//object memberValue = value.TryGetValue(serializerInfo.MemberInformation.Name);
			object memberValue = MemberGetter.Getter(obj); //instead of fasterflect we use delegate to getter

			//We used to do a null check here. It was kind of pointless to do and it was slightly expensive since they weren't all
			//reference types. No reason to check nullness; it's just a perf issue

			await TypeSerializer.WriteAsync(memberValue, dest);
		}

		/// <inheritdoc />
		public override async Task SetMemberAsync(TContainingType obj, IWireStreamReaderStrategyAsync source)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));

#if NET35
			throw new NotImplementedException();
#else
			MemberSetter.Setter(obj, (TMemberType)await TypeSerializer.ReadAsync(source));
#endif
		}
	}
}
