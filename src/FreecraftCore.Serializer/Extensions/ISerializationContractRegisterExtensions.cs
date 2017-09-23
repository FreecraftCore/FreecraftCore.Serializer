using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;


namespace FreecraftCore.Serializer
{
	public static class ISerializationContractRegisterExtensions
	{
		/// <summary>
		/// Indicates if the provided <see cref="Type"/> is registered.
		/// </summary>
		/// <returns>True if a serializer is registered for the <see cref="Type"/>.</returns>
		[Pure]
		public static bool isTypeRegistered<TTypeToCheck>([NotNull] this ISerializationContractRegister register)
		{
			if (register == null) throw new ArgumentNullException(nameof(register));

			return register.isTypeRegistered(typeof(TTypeToCheck));
		}
	}
}
