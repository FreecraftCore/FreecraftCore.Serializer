using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Meta-data that indicates that the size of a collection is not located near the
	/// collection itself in the stream and that it requires higher level handling.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
	public sealed class SeperatedCollectionSizeAttribute : Attribute
	{
		/// <summary>
		/// The name of the collection property.
		/// </summary>
		public string CollectionPropertyName { get; }

		/// <summary>
		/// The name of the size property.
		/// </summary>
		public string SizePropertyName { get; }

		/// <inheritdoc />
		public SeperatedCollectionSizeAttribute([NotNull] string collectionPropertyName, [NotNull] string sizePropertyName)
		{
			if(string.IsNullOrWhiteSpace(collectionPropertyName)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(collectionPropertyName));
			if(string.IsNullOrWhiteSpace(sizePropertyName)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(sizePropertyName));

			CollectionPropertyName = collectionPropertyName;
			SizePropertyName = sizePropertyName;
		}
	}
}
