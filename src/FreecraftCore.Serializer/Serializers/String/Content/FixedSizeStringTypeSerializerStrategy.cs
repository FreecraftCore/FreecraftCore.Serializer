using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Serializer type for terminated fixed-sized strings.
	/// (Note: FixedSize 6 will be 7 long when including nullterminator.
	/// This changed from old implementation so watch out!!)
	/// </summary>
	/// <typeparam name="TStringSerializerType">The serializer type.</typeparam>
	/// <typeparam name="TStringTerminatorSerializerType">The null terminator type</typeparam>
	/// <typeparam name="TStaticTypedSizeValueType">The size-type.</typeparam>
	[KnownTypeSerializer]
	public sealed class FixedSizeStringTypeSerializerStrategy<TStringSerializerType, TStaticTypedSizeValueType, TStringTerminatorSerializerType> 
		: BaseStringTypeSerializerStrategy<FixedSizeStringTypeSerializerStrategy<TStringSerializerType, TStaticTypedSizeValueType, TStringTerminatorSerializerType>>
		where TStringSerializerType : BaseStringTypeSerializerStrategy<TStringSerializerType>, new()
		where TStringTerminatorSerializerType : BaseStringTerminatorSerializerStrategy<TStringTerminatorSerializerType>, new()
		where TStaticTypedSizeValueType : StaticTypedNumeric<int>, new()
	{
		private static TStringTerminatorSerializerType TerminatorSerializer { get; } = new TStringTerminatorSerializerType();

		//Don't remove
		static FixedSizeStringTypeSerializerStrategy()
		{
			
		}

		public FixedSizeStringTypeSerializerStrategy()
			: base(FixedSizeStringTypeSerializerStrategy<TStringSerializerType, TStaticTypedSizeValueType>.Instance.EncodingStrategy)
		{

		}

		/// <inheritdoc />
		public sealed override unsafe string Read(Span<byte> buffer, ref int offset)
		{
			string value = FixedSizeStringTypeSerializerStrategy<TStringSerializerType, TStaticTypedSizeValueType>.Instance.Read(buffer, ref offset);

			//Type specified a terminator would be sent.
			TerminatorSerializer.Read(buffer, ref offset);
			return value;
		}

		/// <inheritdoc />
		public sealed override unsafe void Write(string value, Span<byte> buffer, ref int offset)
		{
			FixedSizeStringTypeSerializerStrategy<TStringSerializerType, TStaticTypedSizeValueType>.Instance.Write(value, buffer, ref offset);

			//Type requested terminator
			TerminatorSerializer.Write(value, buffer, ref offset);
		}
	}

	/// <summary>
	/// Serializer type for terminated fixed-sized strings.
	/// (Note: FixedSize 6 will be 7 long when including nullterminator.
	/// This changed from old implementation so watch out!!)
	/// </summary>
	/// <typeparam name="TStringSerializerType">The serializer type.</typeparam>
	/// <typeparam name="TStaticTypedSizeValueType">The size-type.</typeparam>
	[KnownTypeSerializer]
	public sealed class FixedSizeStringTypeSerializerStrategy<TStringSerializerType, TStaticTypedSizeValueType> : BaseStringTypeSerializerStrategy<FixedSizeStringTypeSerializerStrategy<TStringSerializerType, TStaticTypedSizeValueType>>
		where TStringSerializerType : BaseStringTypeSerializerStrategy<TStringSerializerType>, new()
		where TStaticTypedSizeValueType : StaticTypedNumeric<int>, new()
	{
		private static TStringSerializerType StringSerializer { get; } = new TStringSerializerType();

		/// <summary>
		/// The fixed size value provider.
		/// </summary>
		public static TStaticTypedSizeValueType FixedSize { get; } = new TStaticTypedSizeValueType();

		//Don't remove
		static FixedSizeStringTypeSerializerStrategy()
		{

		}

		public FixedSizeStringTypeSerializerStrategy()
			: base(StringSerializer.EncodingStrategy)
		{

		}

		/// <inheritdoc />
		public sealed override string Read(Span<byte> buffer, ref int offset)
		{
			int lastOffset = offset;
			int fixedSizeLength = this.CharacterSize * FixedSize.Value;

			string value = base.Read(buffer.Slice(0, offset + fixedSizeLength), ref offset);

			//Important that we determine if the fixed length isn't complete.
			//WARNING: THIS ONLY WORKS WITH FIXED-WIDTH CHARACTER ENCODING!!
			int missingByteCount = ComputeMissingByteCount(offset, lastOffset, fixedSizeLength);

			//We don't have to clear the buffer or zero it out. We just skip reading.
			if(missingByteCount != 0)
				offset += missingByteCount;

			return value;
		}

		/// <inheritdoc />
		public sealed override void Write(string value, Span<byte> buffer, ref int offset)
		{
			int fixedSizeLength = this.CharacterSize * FixedSize.Value;

			int lastOffset = offset;
			base.Write(value, buffer.Slice(0, fixedSizeLength + offset), ref offset);

			//Important that we determine if the fixed length isn't complete.
			//WARNING: THIS ONLY WORKS WITH FIXED-WIDTH CHARACTER ENCODING!!
			int missingByteCount = ComputeMissingByteCount(offset, lastOffset, fixedSizeLength);

			if (missingByteCount == 0)
				return;

			//Important to zero out the buffer, it may not be 0. Never assume it's 0.
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
