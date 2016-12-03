using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreecraftCore.Payload.Serializer
{
	public interface ISerializationContractRegister
	{
		/// <summary>
		/// Attempts to register a contract for the provided <see cref="Type"/>.
		/// </summary>
		/// <returns>True if the type is successfully registered.</returns>
		bool RegisterType<TTypeToRegister>()
			where TTypeToRegister : new();

		/// <summary>
		/// Indicates if the provided <see cref="Type"/> is registered.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		bool isTypeRegistered(Type type);
	}
}
