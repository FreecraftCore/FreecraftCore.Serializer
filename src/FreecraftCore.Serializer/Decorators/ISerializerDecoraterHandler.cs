using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using JetBrains.Annotations;


namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Contract for a decorater handler.
	/// </summary>
	public interface ISerializerDecoraterHandler
	{
		/// <summary>
		/// Indicates if the <see cref="ISerializerDecoraterHandler"/> is able to handle the specified <see cref="ISerializableTypeContext"/>.
		/// </summary>
		/// <param name="context">The member context.</param>
		/// <exception cref="ArgumentNullException">Throws null if the provided <see cref="context"/> is null.</exception>
		/// <returns>True if the handler can decorate for the serialization of the specified <see cref="ISerializableTypeContext"/>.</returns>
		[Pure]
		bool CanHandle([NotNull] ISerializableTypeContext context);

		/// <summary>
		/// Gets a collection of <see cref="Type"/> objects that represent all of the required types that must be
		/// registered for this decorator to work. These should be registered before attempting to use the decorator.
		/// </summary>
		/// <param name="context"></param>
		/// <exception cref="ArgumentNullException">Throws null if the provided <see cref="context"/> is null.</exception>
		/// <returns>Returns a collection of related <see cref="ISerializableTypeContext"/>s</returns>
		[Pure]
		[NotNull]
		IEnumerable<ISerializableTypeContext> GetAssociatedSerializationContexts([NotNull] ISerializableTypeContext context);
	}
}
