﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace FreecraftCore.Serializer
{
	//This messages with the type signature if it's uncommented.
	/*public partial class TypeStub : IWireMessage<TypeStub>
	{
		public Type SerializableType => typeof(TypeStub);

		public TypeStub Read(Span<byte> buffer, ref int offset)
		{
			TypeStub_Serializer.Instance.InternalRead(this, source, ref offset);
			return this;
		}
		public void Write(TypeStub value, Span<byte> buffer, ref int offset)
		{
			TypeStub_Serializer.Instance.InternalWrite(this, buffer, ref offset);
		}
	}*/
}

namespace FreecraftCore.Serializer
{
	//THIS CODE IS FOR AUTO-GENERATED SERIALIZERS! DO NOT MODIFY UNLESS YOU KNOW WELL!
	/// <summary>
	/// FreecraftCore.Serializer's AUTO-GENERATED (do not edit) serialization
	/// code for the Type: <see cref="TypeStub"/>
	/// </summary>
	/*internal sealed class AutoGeneratedTemplateSerializerStrategy 
		: BaseAutoGeneratedSerializerStrategy<AutoGeneratedTemplateSerializerStrategy, TypeStub>
	{
		protected internal override void InternalRead(TypeStub value, Span<byte> buffer, ref int offset)
		{
			throw new NotImplementedException();
		}

		protected internal override void InternalWrite(TypeStub value, Span<byte> buffer, ref int offset)
		{
			throw new NotImplementedException();
		}
	}*/
}
