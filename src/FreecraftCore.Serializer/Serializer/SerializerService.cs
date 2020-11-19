using System;
using System.Collections.Generic;
using System.Text;

namespace FreecraftCore.Serializer
{
	//So it's abit confusing but these are legacy interface types that used to exist and implement
	//the logic for serialization.
	public sealed class SerializerService : ISerializerService, ISerializationPolymorphicRegister
	{
		public static Dictionary<Type, ITypeSerializerStrategy> PolymorphicSerializers { get; } = new Dictionary<Type, ITypeSerializerStrategy>();

		public static readonly object SyncObj = new object();

		//Do not remove!
		static SerializerService()
		{
			
		}

		/// <inheritdoc />
		public T Read<T>(Span<byte> source, ref int offset) 
			where T : IWireMessage<T>
		{
			//To support polymorphic serialization this hack was invented, requiring polymorphic
			//serializers be registered with the serializer service at application startup
			if (typeof(T).IsValueType || !typeof(T).IsAbstract)
			{
				return Activator.CreateInstance<T>()
					.Read(source, ref offset);
			}
			else
			{
				//This is the case where we have a non-newable abstract type
				//that actually cannot be construted and read as a WireMessage type
				return ((ITypeSerializerStrategy<T>) PolymorphicSerializers[typeof(T)])
					.Read(source, ref offset);
			}
		}

		/// <inheritdoc />
		public void Write<T>(T value, Span<byte> destination, ref int offset) 
			where T : IWireMessage<T>
		{
			value.Write(value, destination, ref offset);
		}

		public void RegisterPolymorphicSerializer<TSerializerType>() 
			where TSerializerType : ITypeSerializerStrategy, new()
		{
			lock (SyncObj)
			{
				TSerializerType serializer = new TSerializerType();
				PolymorphicSerializers[serializer.SerializableType] = serializer;
			}
		}
	}
}
