using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

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

		public TType ReadIntoObject(ref TType obj, IWireMemberReaderStrategy source)
		{
			obj = Read(source);

			return obj;
		}

		public object ReadIntoObject(ref object obj, IWireMemberReaderStrategy source)
		{
			TType castedObj = (TType)obj;

			return ReadIntoObject(ref castedObj, source);
		}

		public void ObjectIntoWriter(object obj, IWireMemberWriterStrategy dest)
		{
			if (obj == null) throw new ArgumentNullException(nameof(obj));

			ObjectIntoWriter((TType)obj, dest);
		}

		public void ObjectIntoWriter(TType obj, IWireMemberWriterStrategy dest)
		{
			if (obj == null) throw new ArgumentNullException(nameof(obj));
			if (dest == null) throw new ArgumentNullException(nameof(dest));

			//This is a simple type so the only way to write it is to just write the value
			this.Write(obj, dest);
		}
	}
}
