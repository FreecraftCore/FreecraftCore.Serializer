using System;
using System.Collections.Generic;
using System.Text;

namespace FreecraftCore.Serializer
{
	//So it's abit confusing but these are legacy interface types that used to exist and implement
	//the logic for serialization.
	public sealed class SerializerService : ISerializerService, ISerializationPolymorphicRegister
	{
		public readonly object SyncObj = new object();

		//WARNING: Do not change this strange implementation, 10-15% faster than mapping Type via dictionary and then casting.
		//This is a clever hack for performance that avoids slow Dictionary key lookups
		//and casting
		//Realistically it should have this contraint but not required: where TWireType : IWireMessage<TWireType>
		private class GenericPolymorphicSerializerContainer<TWireType>
		{
			public static ITypeSerializerReadingStrategy<TWireType> Instance { get; set; }
		}

		//Do not remove!
		static SerializerService()
		{
			
		}

		/// <inheritdoc />
		public T Read<T>(Span<byte> buffer, ref int offset) 
			where T : ITypeSerializerReadingStrategy<T>
		{
			//To support polymorphic serialization this hack was invented, requiring polymorphic
			//serializers be registered with the serializer service at application startup
			if (typeof(T).IsValueType || !typeof(T).IsAbstract)
			{
				return Activator.CreateInstance<T>()
					.Read(buffer, ref offset);
			}
			else
			{
				//HOT PATH: This is a clever hack to avoid costly lookup and casting.
				//This is the case where we have a non-newable abstract type
				//that actually cannot be construted and read as a WireMessage type
				return GenericPolymorphicSerializerContainer<T>.Instance
					.Read(buffer, ref offset);
			}
		}

		/// <inheritdoc />
		public void Write<T>(T value, Span<byte> buffer, ref int offset) 
			where T : ITypeSerializerWritingStrategy<T>
		{
			value.Write(value, buffer, ref offset);
		}

		public void RegisterPolymorphicSerializer<TWireType, TSerializerType>() 
			where TSerializerType : ITypeSerializerReadingStrategy<TWireType>, new() 
			where TWireType : IWireMessage<TWireType>
		{
			lock (SyncObj)
			{
				//WARNING: Do not change this strange implementation, 10-15% faster than mapping Type via dictionary and then casting.
				TSerializerType serializer = new TSerializerType();
				GenericPolymorphicSerializerContainer<TWireType>.Instance = serializer;
			}
		}
	}
}
