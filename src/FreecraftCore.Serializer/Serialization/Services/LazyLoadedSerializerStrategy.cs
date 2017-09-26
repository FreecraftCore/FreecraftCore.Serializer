using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace FreecraftCore.Serializer
{
	internal sealed class LazyLoadedSerializerStrategy<TType> : ITypeSerializerStrategy<TType>, ICompilable
	{
		public Lazy<ITypeSerializerStrategy<TType>> CastedProvidedSerializer { get; }

		/// <summary>
		/// The lazily loaded decorated serializer.
		/// </summary>
		public ITypeSerializerStrategy<TType> DecoratedSerializer => CastedProvidedSerializer.Value;

		public Type SerializerType { get; }

		[NotNull]
		private ILazySerializerProvider Provider { get; }

		public SerializationContextRequirement ContextRequirement { get; }

		public LazyLoadedSerializerStrategy(SerializationContextRequirement contextRequirement, [NotNull] Type serializerType, [NotNull] ILazySerializerProvider provider)
		{
			if(serializerType == null) throw new ArgumentNullException(nameof(serializerType));
			if(provider == null) throw new ArgumentNullException(nameof(provider));
			if(!Enum.IsDefined(typeof(SerializationContextRequirement), contextRequirement)) throw new ArgumentOutOfRangeException(nameof(contextRequirement), "Value should be defined in the SerializationContextRequirement enum.");

			ContextRequirement = contextRequirement;
			SerializerType = serializerType;
			Provider = provider;

			CastedProvidedSerializer = new Lazy<ITypeSerializerStrategy<TType>>(() => provider.SerializerStrategy.Value as ITypeSerializerStrategy<TType>, true);
		}

		public TType FromBytes([NotNull] byte[] bytes)
		{
			return DecoratedSerializer.FromBytes(bytes);
		}

		public byte[] GetBytes([NotNull] TType obj)
		{
			return DecoratedSerializer.GetBytes(obj);
		}

		public byte[] GetBytes([NotNull] object obj)
		{
			return DecoratedSerializer.GetBytes(obj);
		}

		public void ObjectIntoWriter([NotNull] TType obj, [NotNull] IWireStreamWriterStrategy dest)
		{
			DecoratedSerializer.ObjectIntoWriter(obj, dest);
		}

		public void ObjectIntoWriter([CanBeNull] object obj, [NotNull] IWireStreamWriterStrategy dest)
		{
			DecoratedSerializer.ObjectIntoWriter(obj, dest);
		}

		public async Task ObjectIntoWriterAsync([NotNull] TType obj, [NotNull] IWireStreamWriterStrategyAsync dest)
		{
			await DecoratedSerializer.ObjectIntoWriterAsync(obj, dest);
		}

		public async Task ObjectIntoWriterAsync([CanBeNull] object obj, [NotNull] IWireStreamWriterStrategyAsync dest)
		{
			await DecoratedSerializer.ObjectIntoWriterAsync(obj, dest);
		}

		public TType Read([NotNull] IWireStreamReaderStrategy source)
		{
			return DecoratedSerializer.Read(source);
		}

		public async Task<TType> ReadAsync([NotNull] IWireStreamReaderStrategyAsync source)
		{
			return await DecoratedSerializer.ReadAsync(source);
		}

		public TType ReadIntoObject([CanBeNull] TType obj, [NotNull] IWireStreamReaderStrategy source)
		{
			return DecoratedSerializer.ReadIntoObject(obj, source);
		}

		public object ReadIntoObject([CanBeNull] object obj, [NotNull] IWireStreamReaderStrategy source)
		{
			return DecoratedSerializer.ReadIntoObject(obj, source);
		}

		public async Task<TType> ReadIntoObjectAsync([CanBeNull] TType obj, [NotNull] IWireStreamReaderStrategyAsync source)
		{
			return await DecoratedSerializer.ReadIntoObjectAsync(obj, source);
		}

		public async Task<object> ReadIntoObjectAsync([CanBeNull] object obj, [NotNull] IWireStreamReaderStrategyAsync source)
		{
			return await DecoratedSerializer.ReadIntoObjectAsync(obj, source);
		}

		public void Write(TType value, [NotNull] IWireStreamWriterStrategy dest)
		{
			DecoratedSerializer.Write(value, dest);
		}

		public void Write(object value, [NotNull] IWireStreamWriterStrategy dest)
		{
			DecoratedSerializer.Write(value, dest);
		}

		public async Task WriteAsync(TType value, [NotNull] IWireStreamWriterStrategyAsync dest)
		{
			await DecoratedSerializer.WriteAsync(value, dest);
		}

		public async Task WriteAsync(object value, [NotNull] IWireStreamWriterStrategyAsync dest)
		{
			await DecoratedSerializer.WriteAsync(value, dest);
		}

		object IObjectByteReader.FromBytes(byte[] bytes)
		{
			return DecoratedSerializer.FromBytes(bytes);
		}

		object ITypeSerializerStrategy.Read(IWireStreamReaderStrategy source)
		{
			return DecoratedSerializer.Read(source);
		}

		async Task<object> ITypeSerializerStrategyAsync.ReadAsync(IWireStreamReaderStrategyAsync source)
		{
			return await DecoratedSerializer.ReadAsync(source);
		}

		[MethodImpl(MethodImplOptions.NoOptimization)]
		public void Compile()
		{
			object o = DecoratedSerializer.SerializerType;
		}
	}
}
