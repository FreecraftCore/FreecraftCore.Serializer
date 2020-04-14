using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FreecraftCore.Serializer
{
	public abstract class BaseStringSerializerStrategy : SimpleTypeSerializerStrategy<string>
	{
		/// <summary>
		/// The encoding strategy to use for the string serialization.
		/// </summary>
		protected Encoding EncodingStrategy { get; }

		/// <summary>
		/// Size of the individual chars.
		/// </summary>
		protected int CharacterSize { get; }

		public BaseStringSerializerStrategy([NotNull] Encoding encodingStrategy)
		{
			if(encodingStrategy == null) throw new ArgumentNullException(nameof(encodingStrategy));

			EncodingStrategy = encodingStrategy;
			//Due to how coreclr/core works we need to support potential child Types of the encoding types
			//See: https://github.com/dotnet/coreclr/blob/f31097f14560b193e76a7b2e1e61af9870b5356b/src/System.Private.CoreLib/shared/System/Text/ASCIIEncoding.cs#L24
			//We cannot trust .NET to give us correct sizes
			if(CheckEncodingIsOfType<ASCIIEncoding>(encodingStrategy.GetType()))
				CharacterSize = 1;
			else if(CheckEncodingIsOfType<UnicodeEncoding>(encodingStrategy.GetType()))
				CharacterSize = 2;
			else if(CheckEncodingIsOfType<UTF32Encoding>(encodingStrategy.GetType()))
				CharacterSize = 4;
			else if(CheckEncodingIsOfType<UTF8Encoding>(encodingStrategy.GetType()))
				CharacterSize = 1; //In WoW DBC UTF8 strings are null terminated with a single 0 byte.
			else
				throw new InvalidOperationException($"Encounted unknown Encoding: {encodingStrategy.GetType().Name}. Due to .NET behavior we cannot trust anything but manual char size.");
		}

		//TODO: Doc
		protected static bool CheckEncodingIsOfType<TEncodingType>([NotNull] Type encodingTypeToCheck)
			where TEncodingType : Encoding
		{
			if(encodingTypeToCheck == null) throw new ArgumentNullException(nameof(encodingTypeToCheck));

			return encodingTypeToCheck == typeof(TEncodingType) || typeof(TEncodingType).GetTypeInfo().IsAssignableFrom(encodingTypeToCheck);
		}
	}
}
