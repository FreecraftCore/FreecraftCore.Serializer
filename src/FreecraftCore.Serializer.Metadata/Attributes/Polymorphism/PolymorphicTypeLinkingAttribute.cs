using System;
using System.Collections.Generic;
using System.Text;

namespace FreecraftCore.Serializer
{
	[AttributeUsage(AttributeTargets.Class)]
	public abstract class PolymorphicTypeLinkingAttribute : Attribute, IPolymorphicTypeKeyable
	{
		/// <inheritdoc />
		public int Index { get; }

		protected PolymorphicTypeLinkingAttribute(int index)
		{
			Index = index;
		}
	}
}
