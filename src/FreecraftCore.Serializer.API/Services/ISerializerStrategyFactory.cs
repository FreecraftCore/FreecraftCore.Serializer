using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Service that creates <see cref="ITypeSerializerStrategy"/>s.
	/// </summary>
	public interface ISerializerStrategyFactory
	{
		/// <summary>
		/// Creates a <see cref="ITypeSerializerStrategy"/> for the provided <see cref="ISerializableTypeContext"/>
		/// </summary>
		/// <param name="context">The member context for the <see cref="ITypeSerializerStrategy"/>.</param>
		/// <returns>A valid <see cref="ITypeSerializerStrategy"/> or null if one could not be constructed.</returns>
		ITypeSerializerStrategy Create(ISerializableTypeContext context);
	}
}
