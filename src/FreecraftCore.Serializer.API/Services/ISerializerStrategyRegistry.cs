using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;


namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Contract for a serializer registry.
	/// </summary>
	public interface ISerializerStrategyRegistry
	{
		//TODO: Add generic extension methods for RegisterType

		/// <summary>
		/// Registers a contextless <see cref="ITypeSerializerStrategy"/> for the provided type.
		/// </summary>
		/// <param name="type">The key type to register the <see cref="strategy"/> under.</param>
		/// <param name="strategy">The strategy to register.</param>
		/// <exception cref="ArgumentNullException">Throws if either parameter provided is null.</exception>
		/// <returns>True if successfully registered.</returns>
		bool RegisterType([NotNull] Type type, [NotNull] ITypeSerializerStrategy strategy);

		/// <summary>
		/// Registers a serializer with the provided context.
		/// </summary>
		/// <param name="contextFlags">The context type.</param>
		/// <param name="key">The context key.</param>
		/// <param name="type">The type the <see cref="ITypeSerializerStrategy"/> serializes.</param>
		/// <param name="strategy">The serializer strategy.</param>
		/// <exception cref="ArgumentNullException">Throws if either parameter provided is null.</exception>
		/// <returns>True if successfully registered.</returns>
		bool RegisterType(ContextTypeFlags contextFlags, [NotNull] IContextKey key, [NotNull] Type type, [NotNull] ITypeSerializerStrategy strategy);
	}
}
