using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;


namespace FreecraftCore.Serializer
{
	public interface ISerializationContractRegister
	{
		/// <summary>
		/// Attempts to register a contract for the provided <typeparam name="TTypeToRegister"></typeparam>.
		/// </summary>
		/// <returns>The <see cref="ITypeSerializerStrategy{TType}"/> for the requested <typeparamref name="TTypeToRegister"/>.</returns>
		[NotNull]
		ITypeSerializerStrategy<TTypeToRegister> RegisterType<TTypeToRegister>();

		/// <summary>
		/// Indicates if the provided <see cref="Type"/> is registered.
		/// </summary>
		/// <param name="type"></param>
		/// <returns>True if a serializer is registered for the provided <see cref="Type"/>.</returns>
		[Pure]
		bool isTypeRegistered([NotNull] Type type);
	}
}
