using System;
using System.Collections.Generic;
using System.Text;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Serializer type for fixed-sized strings that are not null terminated.
	/// Unlike the legacy fixed-size string serializer, reads consume and decode the exact fixed byte range.
	/// </summary>
	/// <typeparam name="TStringSerializerType">The serializer type.</typeparam>
	/// <typeparam name="TStaticTypedSizeValueType">The size-type.</typeparam>
	[KnownTypeSerializer]
	public sealed class DontTerminateFixedSizeStringTypeSerializerStrategy<TStringSerializerType, TStaticTypedSizeValueType>
		: BaseStringTypeSerializerStrategy<DontTerminateFixedSizeStringTypeSerializerStrategy<TStringSerializerType, TStaticTypedSizeValueType>>
		where TStringSerializerType : BaseStringTypeSerializerStrategy<TStringSerializerType>, new()
		where TStaticTypedSizeValueType : StaticTypedNumeric<int>, new()
	{
		private static TStringSerializerType StringSerializer { get; } = new TStringSerializerType();

		/// <summary>
		/// The fixed size value provider.
		/// </summary>
		public static TStaticTypedSizeValueType FixedSize { get; } = new TStaticTypedSizeValueType();

		//Don't remove
		static DontTerminateFixedSizeStringTypeSerializerStrategy()
		{

		}

		public DontTerminateFixedSizeStringTypeSerializerStrategy()
			: base(StringSerializer.EncodingStrategy)
		{

		}

		/// <inheritdoc />
		public sealed override unsafe string Read(Span<byte> buffer, ref int offset)
		{
			int fixedSizeLength = SizeInfo.MinimumCharacterSize * FixedSize.Value;

			if(fixedSizeLength == 0)
				return String.Empty;

			Span<byte> fixedSizeBuffer = buffer.Slice(offset, fixedSizeLength);
			offset += fixedSizeLength;

			fixed (byte* bytes = &fixedSizeBuffer.GetPinnableReference())
				return ReadEncodedString(bytes, fixedSizeLength);//.TrimEnd('\0');
		}

		/// <inheritdoc />
		public sealed override void Write(string value, Span<byte> buffer, ref int offset)
		{
			int fixedSizeLength = SizeInfo.MinimumCharacterSize * FixedSize.Value;

			int lastOffset = offset;
			base.Write(value, buffer.Slice(0, fixedSizeLength + offset), ref offset);

			int missingByteCount = ComputeMissingByteCount(offset, lastOffset, fixedSizeLength);

			if(missingByteCount == 0)
				return;

			for(int i = 0; i < missingByteCount; i++)
				GenericTypePrimitiveSerializerStrategy<byte>.Instance.Write(0, buffer, ref offset);
		}

		private static int ComputeMissingByteCount(int offset, int lastOffset, int fixedSizeLength)
		{
			int bytesWritten = offset - lastOffset;
			int missingByteCount = Math.Max(0, fixedSizeLength - bytesWritten);
			return missingByteCount;
		}
	}
}
