using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
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

			//We cannot trust .NET to give us correct sizes
			if(encodingStrategy.GetType() == typeof(ASCIIEncoding))
				CharacterSize = 1;
			else if(encodingStrategy.GetType() == typeof(UnicodeEncoding))
				CharacterSize = 2;
			else if(encodingStrategy.GetType() == typeof(UTF32Encoding))
				CharacterSize = 4;
			else
				throw new InvalidOperationException($"Encounted unknown Encoding: {encodingStrategy.GetType().Name}. Due to .NET behavior we cannot trust anything but manual char size.");
		}
	}
}
