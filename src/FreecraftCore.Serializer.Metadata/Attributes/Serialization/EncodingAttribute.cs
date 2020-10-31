using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Metadata marker that indicates a <see cref="string"/> should be seril
	/// </summary>
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public sealed class EncodingAttribute : Attribute
	{
		/// <summary>
		/// Indicates the type of encoding that should be used.
		/// </summary>
		public EncodingType DesiredEncodingType { get; }

		public EncodingAttribute(EncodingType desiredEncodingType)
		{
			if(!Enum.IsDefined(typeof(EncodingType), desiredEncodingType)) throw new ArgumentOutOfRangeException(nameof(desiredEncodingType), "Value should be defined in the EncodingType enum.");

			DesiredEncodingType = desiredEncodingType;
		}
	}
}
