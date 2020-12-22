using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FreecraftCore.Serializer.Internal;
using JetBrains.Annotations;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Mark on Types to indicate to the serializer you want to use a custom type
	/// serializer.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public sealed class CustomTypeSerializerAttribute : Attribute
	{
		/// <summary>
		/// Indicates the custom type serializer to use for this Type.
		/// </summary>
		public Type TypeSerializerType { get; }

		/// <inheritdoc />
		public CustomTypeSerializerAttribute([NotNull] Type typeSerializerType)
		{
			if(typeSerializerType == null) throw new ArgumentNullException(nameof(typeSerializerType));
			//We cannot make sure it's a type serializer. Since we don't reference the assembly that is defined in
			//in the Metadata project.
			TypeSerializerType = typeSerializerType;
		}
	}
}
