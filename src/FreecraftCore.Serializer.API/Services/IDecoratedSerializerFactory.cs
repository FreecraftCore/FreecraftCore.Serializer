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
	public interface IDecoratedSerializerFactory : ITypeDiscoveryPublisher, ISerializerStrategyFactory
	{
		/// <summary>
		/// Indicates if the provided <see cref="ISerializableTypeContext"/> <paramref name="context"/> requires decoration.
		/// </summary>
		/// <param name="context">The <see cref="ISerializableTypeContext"/> context used to verify for potential serializer decoration.</param>
		/// <returns>True if the <see cref="ISerializableTypeContext"/> requires decoration with the service.</returns>
		bool RequiresDecorating(ISerializableTypeContext context);
	}
}
