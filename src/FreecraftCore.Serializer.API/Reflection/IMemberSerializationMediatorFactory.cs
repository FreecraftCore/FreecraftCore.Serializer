using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Factory service that creates <see cref="IMemberSerializationMediator{TContainingType}"/> objects.
	/// </summary>
	public interface IMemberSerializationMediatorFactory
	{
		/// <summary>
		/// Creates a new <see cref="IMemberSerializationMediator{TContainingType}"/> based on the provided <see cref="MemberInfo"/>.
		/// </summary>
		/// <typeparam name="TContainingType">The type the mediator provides access to.</typeparam>
		/// <param name="info"></param>
		/// <returns></returns>
		[NotNull]
		IMemberSerializationMediator<TContainingType> Create<TContainingType>([NotNull] MemberInfo info);
	}
}
