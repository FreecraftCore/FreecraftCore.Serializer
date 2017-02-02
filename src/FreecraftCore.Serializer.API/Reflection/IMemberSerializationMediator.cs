using JetBrains.Annotations;

namespace FreecraftCore.Serializer
{
	public interface IMemberSerializationMediator<TContainingType> : IMemberSerializationMediator
	{
		void ReadMember([NotNull] TContainingType obj, [NotNull] IWireMemberWriterStrategy dest);

		void SetMember(TContainingType obj, [NotNull] IWireMemberReaderStrategy source);
	}

	public interface IMemberSerializationMediator
	{
		void ReadMember(object obj, [NotNull] IWireMemberWriterStrategy dest);

		void SetMember(object obj, [NotNull] IWireMemberReaderStrategy source);
	}
}