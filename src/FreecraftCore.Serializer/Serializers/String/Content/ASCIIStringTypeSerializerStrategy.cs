﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// ASCII encoding implementation of string serialization.
	/// </summary>
	[KnownTypeSerializer]
	public sealed class ASCIIStringTypeSerializerStrategy : BaseStringTypeSerializerStrategy<ASCIIStringTypeSerializerStrategy>, IFixedLengthCharacterSerializerStrategy
	{
		/// <inheritdoc />
		public int CharacterSize => SizeInfo.MaximumCharacterSize;

		public ASCIIStringTypeSerializerStrategy()
			: base(Encoding.ASCII)
		{
			//This is the default that WoW uses
			//however now that the serializer is being used for other projects
			//we need to have the option to use different ones.
		}
	}
}
