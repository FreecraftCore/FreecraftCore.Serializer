using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FreecraftCore.Serializer.Internal;
using JetBrains.Annotations;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Attribute that marks a generic serializable type
	/// as having a specific known/forward declared closed generic Type.
	/// Specifies that serialization code should be emitted for every closed generic primitive type combination.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public sealed class PrimitiveGenericAttribute : BaseGenericListAttribute
	{
		/// <summary>
		/// Roslyn compiler exposed instance of the property.
		/// </summary>
		internal static PrimitiveGenericAttribute Instance { get; } = new PrimitiveGenericAttribute();

		private static ConcurrentDictionary<int, Type[][]> CachedPerms { get; } = new();

		internal static List<Type> CachedTypes { get; } = new()
		{
			typeof(sbyte),
			typeof(byte),
			typeof(ushort),
			typeof(short),
			typeof(uint),
			typeof(int),
			typeof(ulong),
			typeof(long),
			typeof(float),
			typeof(bool),
			typeof(double),
		};

		//Do not remove.
		static PrimitiveGenericAttribute()
		{
			
		}

		public PrimitiveGenericAttribute()
		{
		}

		// Basically hacked into here for perf (lots of perf issues with this source generator)
		/// <summary>
		/// Computes or retrieves from the cache the permutations.
		/// </summary>
		/// <param name="arity"></param>
		/// <returns></returns>
		public Type[][] GetPermutations(int arity)
		{
			if (CachedPerms.ContainsKey(arity))
				return CachedPerms[arity];

			var perms = Permutations(this, arity)
				.Select(em => em.ToArray())
				.ToArray();

			return CachedPerms[arity] = perms;
		}

		/// <inheritdoc />
		public override IEnumerator<Type> GetEnumerator()
		{
			return CachedTypes.GetEnumerator();
		}

		//See: https://stackoverflow.com/questions/1952153/what-is-the-best-way-to-find-all-combinations-of-items-in-an-array/1952336
		/// <summary>
		/// Computes all possible permutations and produces an array of arrays that contain
		/// all permutations with repetition.
		/// </summary>
		/// <typeparam name="T">The element type.</typeparam>
		/// <param name="collection">The collection to permutate.</param>
		/// <param name="length">The expected string length.</param>
		/// <returns>Array of array of size <see cref="length"/> of permutations.</returns>
		private static IEnumerable<IEnumerable<T>> Permutations<T>([NotNull] IEnumerable<T> collection, int length)
		{
			if(collection == null) throw new ArgumentNullException(nameof(collection));

			// @HelloKitty: This array type check should help avoid pointless allocation.
			//LINQ warns of multiple enumeration
			T[] enumerable = collection is T[]? (T[])collection : collection.ToArray();

			if(length < 0) throw new ArgumentOutOfRangeException(nameof(length));

			if(length == 0)
				return Array.Empty<IEnumerable<T>>();

			if(length == 1)
				return enumerable.Select(t => new T[] { t })
					.ToArray();

			return Permutations(enumerable, length - 1)
				.SelectMany(t => enumerable,
					(t1, t2) => t1.Concat(new T[] { t2 }))
				.ToArray(); // Probably good to avoid multiple enumerable
		}
	}
}
