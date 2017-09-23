using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Generic.Math;
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
			if (!typeof(TEnumType).GetTypeInfo().IsEnum)
				throw new InvalidOperationException($"Cannot create an enum decorator for type {typeof(TEnumType).FullName} because it is not an enum.");

			if (typeof(TEnumType).GetTypeInfo().GetEnumUnderlyingType() != typeof(TBaseType))
				throw new InvalidOperationException($"Defining an Enum decorator requires {nameof(TEnumType)}'s base enum type to match {nameof(TBaseType)}.");

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
		public override TEnumType Read(IWireStreamReaderStrategy source)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));

			//TODO: Should be handle exceptions?
			return GenericMath.Convert<TBaseType, TEnumType>(serializerStrategy.Read(source));
		}

		/// <inheritdoc />
		public override void Write(TEnumType value, IWireStreamWriterStrategy dest)
		{
			if (dest == null) throw new ArgumentNullException(nameof(dest));

			serializerStrategy.Write(GenericMath.Convert<TEnumType, TBaseType>(value), dest);
		}

		/// <inheritdoc />
		public override async Task WriteAsync(TEnumType value, IWireStreamWriterStrategyAsync dest)
		{
			if (dest == null) throw new ArgumentNullException(nameof(dest));

			await serializerStrategy.WriteAsync(GenericMath.Convert<TEnumType, TBaseType>(value), dest);
		}

		/// <inheritdoc />
		public override async Task<TEnumType> ReadAsync(IWireStreamReaderStrategyAsync source)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));

			//TODO: Should be handle exceptions?
			return GenericMath.Convert<TBaseType, TEnumType>(await serializerStrategy.ReadAsync(source));
		}
	}
}
