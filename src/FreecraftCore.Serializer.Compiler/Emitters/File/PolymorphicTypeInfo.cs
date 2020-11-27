using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Encapsulates the information of a polymorphic child type linking.
	/// </summary>
	public sealed class PolymorphicTypeInfo : IPolymorphicTypeKeyable
	{
		/// <inheritdoc />
		public int Index { get; }

		/// <summary>
		/// The child type.
		/// </summary>
		public Type ChildType { get; }

		public PolymorphicTypeInfo(int index, [NotNull] Type childType)
		{
			Index = index;
			ChildType = childType ?? throw new ArgumentNullException(nameof(childType));
		}
	}
}
