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

		public sealed override unsafe string Read(Span<byte> buffer, ref int offset)
		{
			string value = FixedSizeStringTypeSerializerStrategy<TStringSerializerType, TStaticTypedSizeValueType>.Instance.Read(buffer, ref offset);

			//Type specified a terminator would be sent.
			TerminatorSerializer.Read(buffer, ref offset);
			return value;
		}

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

		public sealed override unsafe string Read(Span<byte> buffer, ref int offset)
		{
			int fixedSizeLength = this.CharacterSize * FixedSize.Value;
			return base.Read(buffer.Slice(0, offset + fixedSizeLength + offset), ref offset);
		}

		public sealed override unsafe void Write(string value, Span<byte> buffer, ref int offset)
		{
			int fixedSizeLength = this.CharacterSize * FixedSize.Value;

			int lastOffset = offset;
			base.Write(value, buffer.Slice(0, fixedSizeLength + offset), ref offset);

			//Force the fixed length buffer write, and null the buffer out (could be data in it otherwise)
			while(offset < lastOffset)
				GenericTypePrimitiveSerializerStrategy<byte>.Instance.Write(0, buffer, ref offset);
		}
	}
}
