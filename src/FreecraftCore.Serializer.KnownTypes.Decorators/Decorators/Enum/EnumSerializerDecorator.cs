using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreecraftCore.Serializer.KnownTypes
{
	/// <summary>
	/// Decorator for Enum serialization.
	/// </summary>
	/// <typeparam name="TEnumType">The type of the Enum.</typeparam>
	/// <typeparam name="TBaseType">The basetype of the Enum.</typeparam>
	public class EnumSerializerDecorator<TEnumType, TBaseType> : ITypeSerializerStrategy<TEnumType>
		where TEnumType : struct
	{
		public Type SerializerType { get { return typeof(TEnumType); } }

		/// <summary>
		/// Decorated serializer. For example. An enum Type : byte would have a ITypeSerializerStrategy{byte}.
		/// </summary>
		private ITypeSerializerStrategy<TBaseType> serializerStrategy { get; }

		//Enums don't require context (Though closer look at TC/WoW payloads may indicate packing in the future)
		public SerializationContextRequirement ContextRequirement { get; } = SerializationContextRequirement.Contextless;

		public EnumSerializerDecorator(IGeneralSerializerProvider serializerProvider)
		{
			if (!typeof(TEnumType).IsEnum)
				throw new InvalidOperationException($"Cannot create an enum decorator for type {typeof(TEnumType).FullName} because it is not an enum.");

			if (typeof(TEnumType).GetEnumUnderlyingType() != typeof(TBaseType))
				throw new InvalidOperationException($"Defining an Enum decorator requires {nameof(TEnumType)}'s base enum type to match {nameof(TBaseType)}.");

			if (serializerProvider == null)
				throw new ArgumentNullException(nameof(serializerProvider), $"Provided service {nameof(IGeneralSerializerProvider)} was null.");

			serializerStrategy = serializerProvider.Get<TBaseType>();
		}


		/// <summary>
		/// Perform the steps necessary to deserialize this data.
		/// </summary>
		/// <param name="source">The reader providing the input data.</param>
		/// <returns>The updated / replacement value.</returns>
		public TEnumType Read(IWireMemberReaderStrategy source)
		{
			return (TEnumType)Enum.ToObject(typeof(TEnumType), (TBaseType)serializerStrategy.Read(source));
		}

		/// <summary>
		/// Perform the steps necessary to serialize this data.
		/// </summary>
		/// <param name="value">The value to be serialized.</param>
		/// <param name="dest">The writer entity that is accumulating the output data.</param>
		public void Write(TEnumType value, IWireMemberWriterStrategy dest)
		{
			//Not great. It's slow conversion and box. Then casting the box to call.
			object boxedBaseEnumValue = Convert.ChangeType(value, typeof(TBaseType));

			if (boxedBaseEnumValue == null)
				throw new Exception();

			serializerStrategy.Write((TBaseType)boxedBaseEnumValue, dest);
		}

		void ITypeSerializerStrategy.Write(object value, IWireMemberWriterStrategy dest)
		{
			Write((TEnumType)value, dest);
		}

		object ITypeSerializerStrategy.Read(IWireMemberReaderStrategy source)
		{
			return Read(source);
		}

	}
}
