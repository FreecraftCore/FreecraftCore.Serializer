using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using JetBrains.Annotations;


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
		/// <exception cref="InvalidOperationException">If a <see cref="ITypeSerializerStrategy"/> could not be created from the provided <see cref="context"/>.</exception>
		/// <returns>A valid <see cref="ITypeSerializerStrategy"/>.</returns>
		[NotNull] //Should always be not null. Throw instead of null.
		ITypeSerializerStrategy<TType> Create<TType>([NotNull] ISerializableTypeContext context);
	}
}
