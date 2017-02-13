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
		[NotNull]
		bool RegisterType<TTypeToRegister>();

		/// <summary>
		/// Indicates if the provided <see cref="Type"/> is registered.
		/// </summary>
		/// <param name="type"></param>
		/// <returns>True if a serializer is registered for the provided <see cref="Type"/>.</returns>
		[Pure]
		bool isTypeRegistered([NotNull] Type type);

		/// <summary>
		/// Links the child type with the base type.
		/// This is similar to <see cref="WireDataContractBaseTypeAttribute"/> but at runtime.
		/// This is helpful when library boundaries exsist and the desire for minimializing dependencies
		/// across type libraries is a goal.
		/// </summary>
		/// <typeparam name="TChildType">The child type.</typeparam>
		/// <typeparam name="TBaseType">The base type.</typeparam>
		/// <returns></returns>
		bool Link<TChildType, TBaseType>()
			where TChildType : TBaseType;
	}
}
