using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using JetBrains.Annotations;

namespace FreecraftCore.Serializer
{
	public sealed class EncoderCharacterSizeStrategy
	{
		public int Compute([NotNull] Encoding encoding)
		{
			if (encoding == null) throw new ArgumentNullException(nameof(encoding));

			//Due to how coreclr/core works we need to support potential child Types of the encoding types
			//See: https://github.com/dotnet/coreclr/blob/f31097f14560b193e76a7b2e1e61af9870b5356b/src/System.Private.CoreLib/shared/System/Text/ASCIIEncoding.cs#L24
			//We cannot trust .NET to give us correct sizes
			if(CheckEncodingIsOfType<ASCIIEncoding>(encoding.GetType()))
				return 1;
			else if(CheckEncodingIsOfType<UnicodeEncoding>(encoding.GetType()))
				return 2;
			else if(CheckEncodingIsOfType<UTF32Encoding>(encoding.GetType()))
				return 4;
			else if(CheckEncodingIsOfType<UTF8Encoding>(encoding.GetType()))
				return 1; //In WoW DBC UTF8 strings are null terminated with a single 0 byte.
			else
				throw new InvalidOperationException($"Encounter unknown Encoding: {encoding.GetType().Name}. Due to .NET behavior we cannot trust anything but manual char size.");
		}

		/// <summary>
		/// Indicates if an encoding type is the same as a provided encoding type.
		/// </summary>
		/// <typeparam name="TEncodingType">The encoding type to check.</typeparam>
		/// <param name="encodingTypeToCheck">The type to compare to.</param>
		/// <returns>True if the types match.</returns>
		private static bool CheckEncodingIsOfType<TEncodingType>([NotNull] Type encodingTypeToCheck)
			where TEncodingType : Encoding
		{
			if(encodingTypeToCheck == null) throw new ArgumentNullException(nameof(encodingTypeToCheck));

			return encodingTypeToCheck == typeof(TEncodingType) || typeof(TEncodingType).GetTypeInfo().IsAssignableFrom(encodingTypeToCheck);
		}
	}
}
