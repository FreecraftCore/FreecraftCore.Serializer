using System;
using System.Collections.Generic;
using System.Text;

namespace FreecraftCore.Serializer
{
	public interface ISerializationPolymorphicRegister
	{
		/// <summary>
		/// Registered a polymorphic serializer that can handle deserialization
		/// of abstract or polymorphic supported types.
		/// 
		/// This method should only be called at startup. It is not safe to call this method in the middle of using
		/// the serializer.
		/// </summary>
		/// <typeparam name="TSerializerType">The serializer type.</typeparam>
		/// <typeparam name="TWireType"></typeparam>
		void RegisterPolymorphicSerializer<TWireType, TSerializerType>()
			where TSerializerType : ITypeSerializerReadingStrategy<TWireType>, new()
			where TWireType : IWireMessage<TWireType>;
	}
}
