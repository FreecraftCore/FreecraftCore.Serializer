using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Generic.Math;
using JetBrains.Annotations;

namespace FreecraftCore.Serializer.KnownTypes
{
	/// <summary>
	/// Generic key reading and writing strategy.
	/// </summary>
	/// <typeparam name="TKeyType">The type of key.</typeparam>
	public class GenericChildKeyStrategy<TKeyType> : IChildKeyStrategy
		where TKeyType : struct
	{
		/// <summary>
		/// Indicates if the key should be consumed from the stream.
		/// </summary>
		private InformationHandlingFlags typeHandlingFlags { get; }

		[NotNull]
		private ITypeSerializerStrategy<TKeyType> KeyTypeSerializerStrategy { get; }

		public GenericChildKeyStrategy(InformationHandlingFlags typeHandling, [NotNull] ITypeSerializerStrategy<TKeyType> keyTypeSerializerStrategy)
		{
			if (keyTypeSerializerStrategy == null) throw new ArgumentNullException(nameof(keyTypeSerializerStrategy));

			int i;

			if (!Enum.IsDefined(typeof(InformationHandlingFlags), typeHandling) && Int32.TryParse(typeHandling.ToString(), out i))
				throw new ArgumentOutOfRangeException(nameof(typeHandling), "Value should be defined in the InformationHandlingFlags enum.");
			/*int i;

			if (!Enum.IsDefined(typeof(InformationHandlingFlags), typeHandling) && Int32.TryParse(typeHandling.ToString(), out i))
				throw new InvalidEnumArgumentException(nameof(typeHandling), (int)typeHandling,
					typeof(InformationHandlingFlags));*/

			typeHandlingFlags = typeHandling;
			KeyTypeSerializerStrategy = keyTypeSerializerStrategy;
		}

		/// <inheritdoc />
		public int Read(IWireStreamReaderStrategy source)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));

			//Read the key from the stream.
			TKeyType key = KeyTypeSerializerStrategy
				.Read(typeHandlingFlags.HasFlag(InformationHandlingFlags.DontConsumeRead) ? source.WithOnlyPeeking() : source);

			return GenericMath.Convert<TKeyType, int>(key);
		}

		/// <inheritdoc />
		public void Write(int value, IWireStreamWriterStrategy dest)
		{
			if (dest == null) throw new ArgumentNullException(nameof(dest));

			//If the key shouldn't be written then we avoid writing it
			//It may be that the data is needed to be left in the stream to indicate
			//something about the type later down the line.
			if (!typeHandlingFlags.HasFlag(InformationHandlingFlags.DontWrite))
				KeyTypeSerializerStrategy.Write(GenericMath.Convert<int, TKeyType>(value), dest);
		}

		/// <inheritdoc />
		public async Task<int> ReadAsync(IWireStreamReaderStrategyAsync source)
		{
			//Read the key from the stream.
			TKeyType key = await KeyTypeSerializerStrategy
				.ReadAsync(typeHandlingFlags.HasFlag(InformationHandlingFlags.DontConsumeRead) ? source.WithOnlyPeekingAsync() : source);

			return GenericMath.Convert<TKeyType, int>(key);
		}

		/// <inheritdoc />
		public async Task WriteAsync(int value, IWireStreamWriterStrategyAsync dest)
		{
			if (dest == null) throw new ArgumentNullException(nameof(dest));

			//If the key shouldn't be written then we avoid writing it
			//It may be that the data is needed to be left in the stream to indicate
			//something about the type later down the line.
			if (!typeHandlingFlags.HasFlag(InformationHandlingFlags.DontWrite))
				await KeyTypeSerializerStrategy.WriteAsync(GenericMath.Convert<int, TKeyType>(value), dest);
		}
	}
}
