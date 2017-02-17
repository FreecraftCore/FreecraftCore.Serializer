using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace FreecraftCore.Serializer.API.Reflection
{
	public class DisableReadMemberSerializationMediatorDecorator<TContainingType> : IMemberSerializationMediator<TContainingType>
	{
		/// <summary>
		/// The serialization mediator we're decorating.
		/// </summary>
		[NotNull]
		private IMemberSerializationMediator<TContainingType> DecoratedMediator { get; }

		public DisableReadMemberSerializationMediatorDecorator([NotNull] IMemberSerializationMediator<TContainingType> decoratedMediator)
		{
			if (decoratedMediator == null) throw new ArgumentNullException(nameof(decoratedMediator));

			this.DecoratedMediator = decoratedMediator;
		}

		public void WriteMember(object obj, IWireStreamWriterStrategy dest)
		{
			DecoratedMediator.WriteMember(obj, dest);
		}

		public void WriteMember(TContainingType obj, IWireStreamWriterStrategy dest)
		{
			DecoratedMediator.WriteMember(obj, dest);
		}

		public void SetMember(object obj, IWireStreamReaderStrategy source)
		{
			//Just return to prevent reading the member
			return;
		}

		public void SetMember(TContainingType obj, IWireStreamReaderStrategy source)
		{
			//Just return to prevent reading the member
			return;
		}

		/// <inheritdoc />
		public async Task WriteMemberAsync(object obj, IWireStreamWriterStrategyAsync dest)
		{
			await DecoratedMediator.WriteMemberAsync(obj, dest);
		}

		/// <inheritdoc />
		public async Task WriteMemberAsync(TContainingType obj, IWireStreamWriterStrategyAsync dest)
		{
			await DecoratedMediator.WriteMemberAsync(obj, dest);
		}

		/// <inheritdoc />
		public Task SetMemberAsync(object obj, IWireStreamReaderStrategyAsync source)
		{
			//We can run sync and just complete immediately
			return Task.CompletedTask;
		}

		/// <inheritdoc />
		public Task SetMemberAsync(TContainingType obj, IWireStreamReaderStrategyAsync source)
		{
			//We can run sync and just complete immediately
			return Task.CompletedTask;
		}
	}
}
