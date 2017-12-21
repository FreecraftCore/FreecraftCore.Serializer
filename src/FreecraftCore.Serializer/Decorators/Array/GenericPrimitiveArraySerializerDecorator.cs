using FreecraftCore.Serializer.KnownTypes;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Reinterpret.Net;

namespace FreecraftCore.Serializer
{
	public class GenericPrimitiveArraySerializerDecorator<TType> : SimpleTypeSerializerStrategy<TType[]>
		where TType : struct
	{
		/// <inheritdoc />
		public override Type SerializerType { get; } = typeof(TType[]);

		public override SerializationContextRequirement ContextRequirement { get; }

		/// <summary>
		/// The decorated underlying element serializer.
		/// </summary>
		[NotNull]
		protected ITypeSerializerStrategy<TType> DecoratedSerializer { get; }

		/// <summary>
		/// The strategy that determines sizing.
		/// </summary>
		[NotNull]
		protected ICollectionSizeStrategy SizeStrategyService { get; }

		private int SizeOfType { get; } = Marshal.SizeOf(typeof(TType));

		public GenericPrimitiveArraySerializerDecorator([NotNull] IGeneralSerializerProvider serializerProvider, [NotNull] ICollectionSizeStrategy sizeStrategy, SerializationContextRequirement contextReq)
		{
			if(serializerProvider == null)
				throw new ArgumentNullException(nameof(serializerProvider), $"Provided {nameof(IGeneralSerializerProvider)} to needed to decorate was null.");

			if(sizeStrategy == null)
				throw new ArgumentNullException(nameof(sizeStrategy), $"Provided {nameof(ICollectionSizeStrategy)} to needed to decorate was null.");

			if(!Enum.IsDefined(typeof(SerializationContextRequirement), contextReq)) throw new ArgumentOutOfRangeException(nameof(contextReq), "Value should be defined in the SerializationContextRequirement enum.");

			ContextRequirement = contextReq;
			SizeStrategyService = sizeStrategy;
			DecoratedSerializer = serializerProvider.Get<TType>();
		}

		/// <inheritdoc />
		public override TType[] Read(IWireStreamReaderStrategy source)
		{
			int byteCount = SizeOfType * SizeStrategyService.Size(source);

			byte[] bytes = source.ReadBytes(byteCount);

			return bytes.ReinterpretToArray<TType>();
		}

		/// <inheritdoc />
		public override void Write(TType[] value, IWireStreamWriterStrategy dest)
		{
			if(dest == null) throw new ArgumentNullException(nameof(dest));

			int size = SizeStrategyService.Size<TType[], TType>(value, dest);

			//We no longer verify size thanks to PHANTASY STAR ONLINE. Thanks Sega. Sometimes we have to fake the size

			dest.Write(value.Reinterpret());
		}

		/// <inheritdoc />
		public override async Task WriteAsync(TType[] value, IWireStreamWriterStrategyAsync dest)
		{
			if(dest == null) throw new ArgumentNullException(nameof(dest));

			int size = await SizeStrategyService.SizeAsync<TType[], TType>(value, dest)
				.ConfigureAwait(false);

			//We no longer verify size thanks to PHANTASY STAR ONLINE. Thanks Sega. Sometimes we have to fake the size

			await dest.WriteAsync(value.Reinterpret())
				.ConfigureAwait(false);
		}

		/// <inheritdoc />
		public override async Task<TType[]> ReadAsync(IWireStreamReaderStrategyAsync source)
		{
			int size = await SizeStrategyService.SizeAsync(source)
				.ConfigureAwait(false);

			int byteCount = SizeOfType * size;

			byte[] bytes = await source.ReadBytesAsync(byteCount)
				.ConfigureAwait(false);

			return bytes.ReinterpretToArray<TType>();
		}
	}
}
