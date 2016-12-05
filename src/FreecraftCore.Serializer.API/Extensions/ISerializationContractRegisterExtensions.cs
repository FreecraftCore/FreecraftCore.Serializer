using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreecraftCore.Serializer
{
	public static class ISerializationContractRegisterExtensions
	{
		/// <summary>
		/// Indicates if the provided <see cref="Type"/> is registered.
		/// </summary>
		/// <returns>True if a serializer is registered for the <see cref="Type"/>.</returns>
		public static bool isTypeRegistered<TTypeToCheck>(this ISerializationContractRegister register)
		{
			return register.isTypeRegistered(typeof(TTypeToCheck));
		}
	}
}
