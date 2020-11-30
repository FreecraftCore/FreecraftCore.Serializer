using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Metadata marker that indicates a <see cref="string"/> should be seril
	/// </summary>
	[SerializationAttribute]
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

		/// <summary>
		/// Internal compiler use.
		/// </summary>
		/// <param name="attributeParameterData"></param>
		/// <returns></returns>
		internal static EncodingType Parse([NotNull] string attributeParameterData)
		{
			if (attributeParameterData == null) throw new ArgumentNullException(nameof(attributeParameterData));

			return (EncodingType)Enum.Parse(typeof(EncodingType), attributeParameterData, true);
		}
	}
}
