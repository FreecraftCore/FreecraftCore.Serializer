using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Service that decorates known serialization stratagies for more complex <see cref="Type"/> serialization.
	/// </summary>
	public interface ISerializerDecoratorService : ITypeDiscoveryPublisher
	{
		/// <summary>
		/// Indicates if the provided <see cref="ISerializableTypeContext"/> <paramref name="context"/> requires decoration.
		/// </summary>
		/// <param name="context">The <see cref="ISerializableTypeContext"/> context used to verify for potential serializer decoration.</param>
		/// <returns>True if the <see cref="ISerializableTypeContext"/> requires decoration with the service.</returns>
		bool RequiresDecorating(ISerializableTypeContext context);

		/// <summary>
		/// Generates a decorated serializer for the provided <see cref="ISerializableTypeContext"/>.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <returns>A decorated serializer or throws if none could be created.</returns>
		ITypeSerializerStrategy GenerateDecoratedSerializer(ISerializableTypeContext context);
	}
}
