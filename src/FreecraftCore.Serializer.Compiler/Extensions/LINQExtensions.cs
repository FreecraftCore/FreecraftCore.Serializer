using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FreecraftCore.Serializer.Internal;
using JetBrains.Annotations;

namespace FreecraftCore.Serializer
{
	internal static class LINQExtensions
	{
		//See: https://stackoverflow.com/questions/1952153/what-is-the-best-way-to-find-all-combinations-of-items-in-an-array/1952336
		/// <summary>
		/// Computes all possible permutations and produces an array of arrays that contain
		/// all permutations with repetition.
		/// </summary>
		/// <typeparam name="T">The element type.</typeparam>
		/// <param name="collection">The collection to permutate.</param>
		/// <param name="length">The expected string length.</param>
		/// <returns>Array of array of size <see cref="length"/> of permutations.</returns>
		public static IEnumerable<IEnumerable<T>> Permutations<T>([NotNull] this IEnumerable<T> collection, int length)
		{
			if(collection == null) throw new ArgumentNullException(nameof(collection));

			// @HelloKitty: This array type check should help avoid pointless allocation.
			//LINQ warns of multiple enumeration
			T[] enumerable = collection is T[] ? (T[])collection : collection.ToArray();

			if (length < 0) throw new ArgumentOutOfRangeException(nameof(length));

			if (length == 0)
				return Array.Empty<IEnumerable<T>>();

			if (length == 1)
				return enumerable.Select(t => new T[] {t})
					.ToArray();

			return Permutations(enumerable, length - 1)
				.SelectMany(t => enumerable,
					(t1, t2) => t1.Concat(new T[] { t2 }))
				.ToArray(); // Probably good to avoid multiple enumerable
		}
	}
}
