using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;


namespace FreecraftCore.Serializer.KnownTypes
{
	/// <summary>
	/// Decorator for Enum serialization.
	/// </summary>
	/// <typeparam name="TEnumType">The type of the Enum.</typeparam>
	/// <typeparam name="TBaseType">The basetype of the Enum.</typeparam>
	public class EnumSerializerDecorator<TEnumType, TBaseType> : SimpleTypeSerializerStrategy<TEnumType>
		where TEnumType : struct
	{
		/// <summary>
		/// Decorated serializer. For example. An enum Type : byte would have a ITypeSerializerStrategy{byte}.
		/// </summary>
		[NotNull]
		private ITypeSerializerStrategy<TBaseType> serializerStrategy { get; }

		//Enums don't require context (Though closer look at TC/WoW payloads may indicate packing in the future)
		public override SerializationContextRequirement ContextRequirement => SerializationContextRequirement.Contextless;

		public EnumSerializerDecorator([NotNull] IGeneralSerializerProvider serializerProvider)
		{
			if (!typeof(TEnumType).IsEnum)
				throw new InvalidOperationException($"Cannot create an enum decorator for type {typeof(TEnumType).FullName} because it is not an enum.");
#if !NET35
			if (typeof(TEnumType).GetEnumUnderlyingType() != typeof(TBaseType))
				throw new InvalidOperationException($"Defining an Enum decorator requires {nameof(TEnumType)}'s base enum type to match {nameof(TBaseType)}.");
#else
			if (Enum.GetUnderlyingType(typeof(TEnumType)) != typeof(TBaseType))
				throw new InvalidOperationException($"Defining an Enum decorator requires {nameof(TEnumType)}'s base enum type to match {nameof(TBaseType)}.");
#endif

			if (serializerProvider == null)
				throw new ArgumentNullException(nameof(serializerProvider), $"Provided service {nameof(IGeneralSerializerProvider)} was null.");

			try
			{
				serializerStrategy = serializerProvider.Get<TBaseType>();
			}
			catch (InvalidOperationException e)
			{
				throw new InvalidOperationException($"Failed to create strategy for Type: {typeof(TBaseType).FullName} for enum decorator for enum Type: {typeof(TEnumType).FullName}", e);
			}
		}

		/// <inheritdoc />
		public override TEnumType Read(IWireMemberReaderStrategy source)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));

			//TODO: Should be handle exceptions?
			return (TEnumType)Enum.ToObject(typeof(TEnumType), (TBaseType)serializerStrategy.Read(source));
		}

		/// <inheritdoc />
		public override void Write(TEnumType value, IWireMemberWriterStrategy dest)
		{
			if (dest == null) throw new ArgumentNullException(nameof(dest));

			//TODO: Increase perf
			//Not great. It's slow conversion and box. Then casting the box to call.
			object boxedBaseEnumValue = Convert.ChangeType(value, typeof(TBaseType));

			if (boxedBaseEnumValue == null)
				throw new Exception();

			serializerStrategy.Write((TBaseType)boxedBaseEnumValue, dest);
		}
	}
}
