using Fasterflect.Probing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreecraftCore.Payload.Serializer
{
	/// <summary>
	/// Decorator for Enum serialization.
	/// </summary>
	/// <typeparam name="TEnumType">The type of the Enum.</typeparam>
	/// <typeparam name="TBaseType">The basetype of the Enum.</typeparam>
	public class EnumSerializerDecorator<TEnumType, TBaseType> : ITypeSerializerStrategy<TEnumType>
		where TEnumType : struct, IConvertible
	{
		public Type SerializerType { get { return typeof(TEnumType); } }

		/// <summary>
		/// Decorated serializer. For example. An enum Type : byte would have a ITypeSerializerStrategy{byte}.
		/// </summary>
		private ITypeSerializerStrategy<TBaseType> serializerStrategy { get; }

		public EnumSerializerDecorator(ITypeSerializerStrategy<TBaseType> serializer)
		{
			if (!typeof(TEnumType).IsEnum)
				throw new InvalidOperationException($"Cannot create an enum decorator for type {typeof(TEnumType).FullName} because it is not an enum.");

			if (typeof(TEnumType).GetEnumUnderlyingType() != typeof(TBaseType))
				throw new InvalidOperationException($"Defining an Enum decorator requires {nameof(TEnumType)}'s base enum type to match {nameof(TBaseType)}.");

			serializerStrategy = serializer;
		}

		public TEnumType Read(IWireMemberReaderStrategy source)
		{
			return (TEnumType)Enum.ToObject(typeof(TEnumType), serializerStrategy.Read(source));
		}

		public void Write(TEnumType value, IWireMemberWriterStrategy dest)
		{
			//Not great. It's slow conversion and box. Then casting the box to call.
			object boxedBaseEnumValue = Convert.ChangeType(value, typeof(TBaseType));

			if (boxedBaseEnumValue == null)
				throw new Exception();

			serializerStrategy.Write((TBaseType)boxedBaseEnumValue, dest);
		}
	}
}
