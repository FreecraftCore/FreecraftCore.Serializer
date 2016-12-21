using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace FreecraftCore.Serializer
{
	public interface ISerializationContractRegister
	{
		/// <summary>
		/// Attempts to register a contract for the provided <see cref="Type"/>.
		/// </summary>
		/// <returns>The type serializer if successfully registered.</returns>
		ITypeSerializerStrategy<TTypeToRegister> RegisterType<TTypeToRegister>();

		/// <summary>
		/// Indicates if the provided <see cref="Type"/> is registered.
		/// </summary>
		/// <param name="type"></param>
		/// <returns>True if a serializer is registered for the provided <see cref="Type"/>.</returns>
		bool isTypeRegistered(Type type);
	}
}
