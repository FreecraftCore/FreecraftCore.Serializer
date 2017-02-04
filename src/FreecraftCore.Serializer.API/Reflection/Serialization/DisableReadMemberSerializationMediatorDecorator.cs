using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace FreecraftCore.Serializer.API.Reflection
{
	public class DisableReadMemberSerializationMediatorDecorator<TContainingType> : IMemberSerializationMediator<TContainingType>
	{
		[NotNull]
		private IMemberSerializationMediator<TContainingType> decoratedMediator { get; }

		public DisableReadMemberSerializationMediatorDecorator([NotNull] IMemberSerializationMediator<TContainingType> decoratedMediator)
		{
			if (decoratedMediator == null) throw new ArgumentNullException(nameof(decoratedMediator));

			this.decoratedMediator = decoratedMediator;
		}

		public void WriteMember(object obj, IWireMemberWriterStrategy dest)
		{
			decoratedMediator.WriteMember(obj, dest);
		}

		public void WriteMember(TContainingType obj, IWireMemberWriterStrategy dest)
		{
			decoratedMediator.WriteMember(obj, dest);
		}

		public void SetMember(object obj, IWireMemberReaderStrategy source)
		{
			//Just return to prevent reading the member
			return;
		}

		public void SetMember(TContainingType obj, IWireMemberReaderStrategy source)
		{
			//Just return to prevent reading the member
			return;
		}
	}
}
