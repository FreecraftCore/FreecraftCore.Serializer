using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace FreecraftCore
{
	/// <summary>
	/// Mark on Types to indicate to the serializer you want to use a custom type
	/// serializer.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public sealed class IncludeCustomTypeSerializerAttribute : Attribute
	{
		/// <summary>
		/// Indicates the custom type serializer to use for this Type.
		/// </summary>
		public Type TypeSerializerType { get; }

		/// <inheritdoc />
		public IncludeCustomTypeSerializerAttribute([NotNull] Type typeSerializerType)
		{
			if(typeSerializerType == null) throw new ArgumentNullException(nameof(typeSerializerType));
			//We cannot make sure it's a type serializer. Since we don't reference the assembly that is defined in
			//in the Metadata project.
			TypeSerializerType = typeSerializerType;
		}
	}
}
