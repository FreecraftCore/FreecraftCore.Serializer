using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Contract for a serializer registry.
	/// </summary>
	public interface ISerializerStrategyRegistry
	{
		/// <summary>
		/// Registers a contextless <see cref="ITypeSerializerStrategy"/> for the provided type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns>True if successfully registered.</returns>
		bool RegisterType(Type type, ITypeSerializerStrategy strategy);

		/// <summary>
		/// Registers a serializer with the provided context.
		/// </summary>
		/// <param name="contextFlags">The context type.</param>
		/// <param name="key">The context key.</param>
		/// <param name="type">The type the <see cref="ITypeSerializerStrategy"/> serializes.</param>
		/// <param name="strategy">The serializer strategy.</param>
		/// <returns>True if successfully registered.</returns>
		bool RegisterType(ContextTypeFlags contextFlags, IContextKey key, Type type, ITypeSerializerStrategy strategy);
	}
}
