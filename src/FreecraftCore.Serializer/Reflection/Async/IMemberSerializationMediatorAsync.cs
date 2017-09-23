using System.Threading.Tasks;
using JetBrains.Annotations;

namespace FreecraftCore.Serializer
{
	//TODO: Document
	public interface IMemberSerializationMediatorAsync<TContainingType> : IMemberSerializationMediatorAsync
	{
		Task WriteMemberAsync([NotNull] TContainingType obj, [NotNull] IWireStreamWriterStrategyAsync dest);

		Task SetMemberAsync(TContainingType obj, [NotNull] IWireStreamReaderStrategyAsync source);
	}

	public interface IMemberSerializationMediatorAsync
	{
		Task WriteMemberAsync(object obj, [NotNull] IWireStreamWriterStrategyAsync dest);

		Task SetMemberAsync(object obj, [NotNull] IWireStreamReaderStrategyAsync source);
	}
}