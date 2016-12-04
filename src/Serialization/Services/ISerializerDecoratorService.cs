using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreecraftCore.Payload.Serializer
{
	/// <summary>
	/// Service that decorates known serialization stratagies for more complex <see cref="Type"/> serialization.
	/// </summary>
	public interface ISerializerDecoratorService
	{
		/// <summary>
		/// Indicates if the provided <see cref="Type"/> <paramref name="t"/> requires decoration.
		/// </summary>
		/// <param name="t">The <see cref="Type"/> to verify for potential serializer decoration.</param>
		/// <returns>True if the <see cref="Type"/> requires decoration with the service.</returns>
		bool RequiresDecorating(Type t);

		/// <summary>
		/// Parses the provided <see cref="Type"/> and if it's a decoratable type it provides the proper Type that
		/// would require registering.
		/// </summary>
		/// <param name="t"></param>
		/// <returns></returns>
		IEnumerable<Type> GrabTypesThatRequiresRegister(Type t);


		/// <summary>
		/// Attempts to produce a decorated serializer for the provided <see cref="Type"/>.
		/// </summary>
		/// <param name="typeBeingSerialized">Type that is being serialized.</param>
		/// <param name="context">The context.</param>
		/// <returns>A decorated serializer or null if none could be created.</returns>
		ITypeSerializerStrategy TryProduceDecoratedSerializer(Type typeBeingSerialized, SerializerDecorationContext context);
	}
}
