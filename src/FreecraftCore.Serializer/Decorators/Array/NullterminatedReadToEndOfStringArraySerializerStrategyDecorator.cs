using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using JetBrains.Annotations;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace FreecraftCore.Serializer.KnownTypes
{
	/// <summary>
	/// Decorator that will read all nullterminated strings till the end of the buffer is reached.
	/// </summary>
	public class NullterminatedReadToEndOfStringArraySerializerStrategyDecorator : SimpleTypeSerializerStrategy<string[]>
	{
		/// <inheritdoc />
		public override SerializationContextRequirement ContextRequirement { get; } = SerializationContextRequirement.RequiresContext;

		private ITypeSerializerStrategy<string> StringSerialzier { get; }

		public NullterminatedReadToEndOfStringArraySerializerStrategyDecorator([NotNull] ITypeSerializerStrategy<string> stringSerialzier)
		{
			if(stringSerialzier == null) throw new ArgumentNullException(nameof(stringSerialzier));

			StringSerialzier = stringSerialzier;
		}

		/// <inheritdoc />
		public override string[] Read(IWireStreamReaderStrategy source)
		{
			byte[] bytes = source.ReadAllBytes();
			FixedBufferWireReaderStrategy fixedStrategy = new FixedBufferWireReaderStrategy(bytes, 0, bytes.Length);

			QuickList<string> strings = new QuickList<string>(4);

			//Read until the fixed buffer is empty. This iwll give us all strings without read exceptions when end is reached.
			while(!fixedStrategy.isFinished)
				strings.Add(StringSerialzier.Read(fixedStrategy));

			//We can avoid some copies like this
			return strings.Count == strings._items.Length ? strings._items : strings.ToArray();
		}

		/// <inheritdoc />
		public override void Write(string[] value, IWireStreamWriterStrategy dest)
		{
			for(int i = 0; i < value.Length; i++)
				StringSerialzier.Write(value[i], dest);
		}

		/// <inheritdoc />
		public override async Task WriteAsync(string[] value, IWireStreamWriterStrategyAsync dest)
		{
			for(int i = 0; i < value.Length; i++)
				await StringSerialzier.WriteAsync(value[i], dest);
		}

		/// <inheritdoc />
		public override async Task<string[]> ReadAsync(IWireStreamReaderStrategyAsync source)
		{
			byte[] bytes = await source.ReadAllBytesAsync();
			FixedBufferWireReaderStrategy fixedStrategy = new FixedBufferWireReaderStrategy(bytes, 0, bytes.Length);

			QuickList<string> strings = new QuickList<string>(4);

			//Read until the fixed buffer is empty. This iwll give us all strings without read exceptions when end is reached.
			while(!fixedStrategy.isFinished)
				strings.Add(StringSerialzier.Read(fixedStrategy));

			//We can avoid some copies like this
			return strings.Count == strings._items.Length ? strings._items : strings.ToArray();
		}

		//TODO: Refactor this into a stream
		private class FixedBufferWireReaderStrategy : DefaultStreamManipulationStrategy<Stream>, IWireStreamReaderStrategy
		{
			int Count { get; }

			int TargetEndPosition { get; }

			/// <summary>
			/// Indicates if the entire buffer has been read
			/// </summary>
			public bool isFinished => TargetEndPosition <= ManagedStream.Position;

			//TODO: Overloads that take the byte buffer instead
			public FixedBufferWireReaderStrategy([NotNull] byte[] bytes, int start, int count)
				: base((Stream)new MemoryStream(bytes), (bool)true)
			{
				if(bytes == null) throw new ArgumentNullException(nameof(bytes), $"Provided argument {nameof(bytes)} must not be null.");
				if(count < 0) throw new ArgumentOutOfRangeException(nameof(count), $"Requested negative Count: {count}.");
				if(start < 0 || bytes.Length < start + count) throw new ArgumentOutOfRangeException(nameof(start));

				//Set the position of the stream to the start.
				ManagedStream.Position = start;
				Count = count;
				TargetEndPosition = start + count;
			}

			public FixedBufferWireReaderStrategy([NotNull] Stream stream, int count)
				: base(stream, (bool)false)
			{
				if(stream == null) throw new ArgumentNullException(nameof(stream), $"Provided argument {nameof(stream)} must not be null.");
				if(count < 0) throw new ArgumentOutOfRangeException(nameof(count));

				Count = count;
			}

			public byte[] ReadAllBytes()
			{
				return ReadBytes(Math.Max(0, (int)(Count - ManagedStream.Position)));
			}

			public byte ReadByte()
			{
				if(Count == ManagedStream.Position)
					throw new InvalidOperationException("Failed to read a desired byte from the stream.");

				//would be -1 if it's invalid
				int b = ManagedStream.ReadByte();

				//TODO: Contract interface doesn't mention throwing in this case. Should we throw?
				if(b == -1)
					throw new InvalidOperationException("Failed to read a desired byte from the stream.");

				return (byte)b;
			}

			public byte[] ReadBytes(int count)
			{
				if(Count < ManagedStream.Position + count)
					throw new InvalidOperationException($"Failed to read a desired bytes from the stream. Count: {Count} Position: {ManagedStream.Position} Requested: {count}");

				byte[] bytes = new byte[count];

				ManagedStream.Read(bytes, 0, count);

				return bytes;
			}

			public byte PeekByte()
			{
				if(Count == ManagedStream.Position)
					throw new InvalidOperationException("Failed to read a desired bytes from the stream.");

				byte b = ReadByte();

				//Move it back one
				ManagedStream.Position = ManagedStream.Position - 1;

				return b;
			}

			public byte[] PeekBytes(int count)
			{
				if(Count < ManagedStream.Position + count)
					throw new InvalidOperationException("Failed to read a desired bytes from the stream.");

				byte[] bytes = ReadBytes(count);

				//Now move the stream back
				ManagedStream.Position = ManagedStream.Position - count;

				return bytes;
			}
		}
	}

	// Licensed to the .NET Foundation under one or more agreements.
	// The .NET Foundation licenses this file to you under the MIT license.
	// See the LICENSE file in the project root for more information.

	// Implements a variable-size List that uses an array of objects to store the
	// elements. A List has a capacity, which is the allocated length
	// of the internal array. As elements are added to a List, the capacity
	// of the List is automatically increased as required by reallocating the
	// internal array.
	[DebuggerDisplay("Count = {Count}")]
	internal class QuickList<T>
	{
		private const int DefaultCapacity = 4;

		public T[] _items; // Do not rename (binary serialization)
		private int _size; // Do not rename (binary serialization)
		private int _version; // Do not rename (binary serialization)
		private object _syncRoot;

		private static readonly T[] s_emptyArray = new T[0];

		// Constructs a List. The list is initially empty and has a capacity
		// of zero. Upon adding the first element to the list the capacity is
		// increased to DefaultCapacity, and then increased in multiples of two
		// as required.
		public QuickList()
		{
			_items = s_emptyArray;
		}

		// Constructs a List with a given initial capacity. The list is
		// initially empty, but will have room for the given number of elements
		// before any reallocations are required.
		// 
		public QuickList(int capacity)
		{
			if(capacity == 0)
				_items = s_emptyArray;
			else
				_items = new T[capacity];
		}

		// Gets and sets the capacity of this list.  The capacity is the size of
		// the internal array used to hold items.  When set, the internal 
		// array of the list is reallocated to the given capacity.
		// 
		public int Capacity
		{
			get
			{
				return _items.Length;
			}
			set
			{
				if(value != _items.Length)
				{
					if(value > 0)
					{
						T[] newItems = new T[value];
						if(_size > 0)
						{
							Array.Copy(_items, 0, newItems, 0, _size);
						}
						_items = newItems;
					}
					else
					{
						_items = s_emptyArray;
					}
				}
			}
		}

		// Read-only property describing how many elements are in the List.
		public int Count
		{
			get
			{
				return _size;
			}
		}

		// Sets or Gets the element at the given index.
		public T this[int index]
		{
			get
			{
				return _items[index];
			}

			set
			{
				_items[index] = value;
				_version++;
			}
		}

		private static bool IsCompatibleObject(object value)
		{
			// Non-null values are fine.  Only accept nulls if T is a class or Nullable<U>.
			// Note that default(T) is not equal to null for value types except when T is Nullable<U>. 
			return ((value is T) || (value == null && default(T) == null));
		}

		// Adds the given object to the end of this list. The size of the list is
		// increased by one. If required, the capacity of the list is doubled
		// before adding the new element.
		//
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Add(T item)
		{
			var array = _items;
			var size = _size;
			_version++;
			if((uint)size < (uint)array.Length)
			{
				_size = size + 1;
				array[size] = item;
			}
			else
			{
				AddWithResize(item);
			}
		}

		// Non-inline from List.Add to improve its code quality as uncommon path
		[MethodImpl(MethodImplOptions.NoInlining)]
		private void AddWithResize(T item)
		{
			var size = _size;
			EnsureCapacity(size + 1);
			_size = size + 1;
			_items[size] = item;
		}

		// Ensures that the capacity of this list is at least the given minimum
		// value. If the current capacity of the list is less than min, the
		// capacity is increased to twice the current capacity or to min,
		// whichever is larger.
		//
		private void EnsureCapacity(int min)
		{
			if(_items.Length < min)
			{
				//We only increase by one.
				int newCapacity = _items.Length == 0 ? DefaultCapacity : _items.Length * 2;
				// Allow the list to grow to maximum possible capacity (~2G elements) before encountering overflow.
				// Note that this check works even when _items.Length overflowed thanks to the (uint) cast
				if(newCapacity < min) newCapacity = min;
				Capacity = newCapacity;
			}
		}


		// Returns the index of the first occurrence of a given value in a range of
		// this list. The list is searched forwards, starting at index
		// index and ending at count number of elements. The
		// elements of the list are compared to the given value using the
		// Object.Equals method.
		// 
		// This method uses the Array.IndexOf method to perform the
		// search.
		// 
		public int IndexOf(T item, int index)
		{
			return Array.IndexOf(_items, item, index, _size - index);
		}

		// Returns the index of the first occurrence of a given value in a range of
		// this list. The list is searched forwards, starting at index
		// index and upto count number of elements. The
		// elements of the list are compared to the given value using the
		// Object.Equals method.
		// 
		// This method uses the Array.IndexOf method to perform the
		// search.
		// 
		public int IndexOf(T item, int index, int count)
		{
			return Array.IndexOf(_items, item, index, count);
		}

		// Inserts an element into this list at a given index. The size of the list
		// is increased by one. If required, the capacity of the list is doubled
		// before inserting the new element.
		// 
		public void Insert(int index, T item)
		{
			// Note that insertions at the end are legal.

			if(_size == _items.Length) EnsureCapacity(_size + 1);
			if(index < _size)
			{
				Array.Copy(_items, index, _items, index + 1, _size - index);
			}
			_items[index] = item;
			_size++;
			_version++;
		}

		// ToArray returns an array containing the contents of the List.
		// This requires copying the List, which is an O(n) operation.
		public T[] ToArray()
		{
			if(_size == 0)
			{
				return s_emptyArray;
			}

			T[] array = new T[_size];
			Array.Copy(_items, 0, array, 0, _size);
			return array;
		}
	}
}