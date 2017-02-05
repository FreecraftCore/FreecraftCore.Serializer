using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace FreecraftCore.Serializer
{
	public interface IDeserializationPrototypeFactory
	{
		/// <summary>
		/// Creates an empty instance of the provided generic type argument <typeparamref name="TType"/>.
		/// </summary>
		/// <typeparam name="TType"></typeparam>
		/// <returns></returns>
		[NotNull]
		TType Create<TType>();
	}

	public interface IDeserializationPrototypeFactory<TType> : IDeserializationPrototypeFactory
	{
		/// <summary>
		/// Creates an empty instance of the declared generic type argument of the type <typeparamref name="TType"/>.
		/// </summary>
		/// <returns></returns>
		[NotNull]
		TType Create();
	}
}
