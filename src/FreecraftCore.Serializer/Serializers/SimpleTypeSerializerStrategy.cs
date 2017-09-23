using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FreecraftCore.Serializer
{
	//TODO: Doc
	public abstract class SimpleTypeSerializerStrategy<TType> : ITypeSerializerStrategy<TType>
	{
		/// <inheritdoc />
		public virtual Type SerializerType { get; } = typeof(TType);

		/// <inheritdoc />
		public abstract SerializationContextRequirement ContextRequirement { get; }

		/// <inheritdoc />
		public abstract TType Read(IWireStreamReaderStrategy source);

		/// <inheritdoc />
		public abstract void Write(TType value, IWireStreamWriterStrategy dest);

		/// <inheritdoc />
		public abstract Task WriteAsync(TType value, IWireStreamWriterStrategyAsync dest);
		
		/// <inheritdoc />
		public abstract Task<TType> ReadAsync(IWireStreamReaderStrategyAsync source);

		//******************************************** Implementers don't need to implement these

		/// <inheritdoc />
		void ITypeSerializerStrategy.Write(object value, IWireStreamWriterStrategy dest)
		{
			Write((TType)value, dest);
		}

		/// <inheritdoc />
		object ITypeSerializerStrategy.Read(IWireStreamReaderStrategy source)
		{
			return Read(source);
		}

		/// <inheritdoc />
		public virtual TType ReadIntoObject(TType obj, IWireStreamReaderStrategy source)
		{
			obj = Read(source);

			return obj;
		}

		/// <inheritdoc />
		public object ReadIntoObject(object obj, IWireStreamReaderStrategy source)
		{
			return ReadIntoObject((TType)obj, source);
		}

		/// <inheritdoc />
		public void ObjectIntoWriter(object obj, IWireStreamWriterStrategy dest)
		{
			if (obj == null) throw new ArgumentNullException(nameof(obj));

			ObjectIntoWriter((TType)obj, dest);
		}

		/// <inheritdoc />
		public virtual void ObjectIntoWriter(TType obj, IWireStreamWriterStrategy dest)
		{
			if (obj == null) throw new ArgumentNullException(nameof(obj));
			if (dest == null) throw new ArgumentNullException(nameof(dest));

			//This is a simple type so the only way to write it is to just write the value
			this.Write(obj, dest);
		}

		/// <inheritdoc />
		public byte[] GetBytes(object obj)
		{
			return GetBytes((TType) obj);
		}

		/// <inheritdoc />
		public virtual byte[] GetBytes(TType obj)
		{
			DefaultStreamWriterStrategy dest = new DefaultStreamWriterStrategy();
			Write(obj, dest);
			return dest.GetBytes();
		}

		/// <inheritdoc />
		object IObjectByteReader.FromBytes(byte[] bytes)
		{
			return FromBytes(bytes);
		}

		/// <inheritdoc />
		public virtual TType FromBytes(byte[] bytes)
		{
			DefaultStreamReaderStrategy source = new DefaultStreamReaderStrategy(bytes);
			return Read(source);
		}

		//Async implementation

		/// <inheritdoc />
		public virtual async Task<TType> ReadIntoObjectAsync(TType obj, IWireStreamReaderStrategyAsync source)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));

			//We can't really write or read into an object on a simple type.
			
			//Default implementation is to just read the object from the source.
			return await ReadAsync(source);
		}

		/// <inheritdoc />
		public virtual async Task ObjectIntoWriterAsync(TType obj, IWireStreamWriterStrategyAsync dest)
		{
			if (dest == null) throw new ArgumentNullException(nameof(dest));

			//We can't really write or read into an object on a simple type.

			await WriteAsync(obj, dest);
		}

		/// <inheritdoc />
		public async Task WriteAsync(object value, IWireStreamWriterStrategyAsync dest)
		{
			await WriteAsync((TType)value, dest);
		}

		/// <inheritdoc />
		async Task<object> ITypeSerializerStrategyAsync.ReadAsync(IWireStreamReaderStrategyAsync source)
		{	
			return await ReadAsync(source);
		}

		/// <inheritdoc />
		public async Task<object> ReadIntoObjectAsync(object obj, IWireStreamReaderStrategyAsync source)
		{
			return await ReadIntoObjectAsync((TType) obj, source);
		}

		/// <inheritdoc />
		public async Task ObjectIntoWriterAsync(object obj, IWireStreamWriterStrategyAsync dest)
		{
			await ObjectIntoWriterAsync((TType)obj, dest);
		}
	}
}
