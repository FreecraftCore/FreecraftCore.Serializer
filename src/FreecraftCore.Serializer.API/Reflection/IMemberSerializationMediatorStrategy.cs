using JetBrains.Annotations;

namespace FreecraftCore.Serializer
{
	public interface IMemberSerializationMediatorStrategy<TContainingType> : IMemberSerializationMediatorStrategy
	{
		void ReadMember([NotNull] TContainingType obj, [NotNull] IWireMemberWriterStrategy dest);

		void SetMember(TContainingType obj, [NotNull] IWireMemberReaderStrategy source);
	}

	public interface IMemberSerializationMediatorStrategy
	{
		void ReadMember(object obj, [NotNull] IWireMemberWriterStrategy dest);

		void SetMember(object obj, [NotNull] IWireMemberReaderStrategy source);
	}
}