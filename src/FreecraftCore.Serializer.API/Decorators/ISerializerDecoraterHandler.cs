using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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
		/// <returns>True if the handler can decorate for the serialization of the specified <see cref="ISerializableTypeContext"/>.</returns>
		bool CanHandle(ISerializableTypeContext context);

		/// <summary>
		/// Gets a collection of <see cref="Type"/> objects that represent all of the required types that must be
		/// registered for this decorator to work. These should be registered before attempting to use the decorator.
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		IEnumerable<ISerializableTypeContext> GetAssociatedSerializationContexts(ISerializableTypeContext context);
	}
}
