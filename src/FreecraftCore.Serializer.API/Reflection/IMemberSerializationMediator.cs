using JetBrains.Annotations;

namespace FreecraftCore.Serializer
{
	public interface IMemberSerializationMediator<TContainingType> : IMemberSerializationMediator
	{
		void WriteMember([NotNull] TContainingType obj, [NotNull] IWireMemberWriterStrategy dest);

		void SetMember(TContainingType obj, [NotNull] IWireMemberReaderStrategy source);
	}

	public interface IMemberSerializationMediator
	{
		void WriteMember(object obj, [NotNull] IWireMemberWriterStrategy dest);

		void SetMember(object obj, [NotNull] IWireMemberReaderStrategy source);
	}
}