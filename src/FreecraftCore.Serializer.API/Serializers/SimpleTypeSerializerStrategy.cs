using System;
using System.Collections.Generic;
using System.Linq;

namespace FreecraftCore.Serializer
{
	public abstract class SimpleTypeSerializerStrategy<TType> : ITypeSerializerStrategy<TType>
	{
		public virtual Type SerializerType { get; } = typeof(TType);

		public abstract SerializationContextRequirement ContextRequirement { get; }

		public abstract TType Read(IWireMemberReaderStrategy source);

		public abstract void Write(TType value, IWireMemberWriterStrategy dest);

		/// <inheritdoc />
		void ITypeSerializerStrategy.Write(object value, IWireMemberWriterStrategy dest)
		{
			Write((TType)value, dest);
		}

		/// <inheritdoc />
		object ITypeSerializerStrategy.Read(IWireMemberReaderStrategy source)
		{
			return Read(source);
		}

		public TType Read(ref TType obj, IWireMemberReaderStrategy source)
		{
			obj = Read(source);

			return obj;
		}

		public object Read(ref object obj, IWireMemberReaderStrategy source)
		{
			TType castedObj = (TType)obj;

			return Read(ref castedObj, source);
		}
	}
}
