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
			TType castedObj = (TType)obj;

			return ReadIntoObject(castedObj, source);
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
		public virtual Task<TType> ReadIntoObjectAsync(TType obj, IWireStreamReaderStrategyAsync source)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));
			
			//Default implementation is to just read the object from the source.
			return ReadAsync(source);
		}

		/// <inheritdoc />
		public virtual Task ObjectIntoWriterAsync(TType obj, IWireStreamWriterStrategyAsync dest)
		{
			throw new NotImplementedException();
		}

		/// <inheritdoc />
		public Task WriteAsync(object value, IWireStreamWriterStrategyAsync dest)
		{
			return WriteAsync((TType)value, dest);
		}

		/// <inheritdoc />
		Task<object> ITypeSerializerStrategyAsync.ReadAsync(IWireStreamReaderStrategyAsync source)
		{	
			Task<TType> resultTask = ReadAsync(source);

			//Wrap around the task to cast it.
			return new Task<object>(() => resultTask.Result);
		}

		/// <inheritdoc />
		public Task<object> ReadIntoObjectAsync(object obj, IWireStreamReaderStrategyAsync source)
		{
			Task<TType> strongTask = ReadIntoObjectAsync((TType)obj, source);

			//wrap around the task to cast it.
			return new Task<object>(() => strongTask.Result);
		}

		/// <inheritdoc />
		public Task ObjectIntoWriterAsync(object obj, IWireStreamWriterStrategyAsync dest)
		{
			return ObjectIntoWriterAsync((TType)obj, dest);
		}
	}
}
