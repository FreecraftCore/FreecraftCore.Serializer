using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Contract for types that can convert <see cref="object"/> to and from bytes.
	/// </summary>
	public interface IObjectByteConverter
	{
		/// <summary>
		/// Coverts the <see cref="object"/> to byte representation.
		/// </summary>
		/// <param name="obj">Object to convert to bytes.</param>
		/// <returns>A non-null byte array representation of <see cref="obj"/>.</returns>
		[Pure]
		[NotNull]
		byte[] GetBytes([NotNull]object obj);

		/// <summary>
		/// Coverts the <see cref="byte[]"/> preresentation of an object to the object.
		/// </summary>
		/// <param name="bytes">Bytes to use for conversion.</param>
		/// <returns>A non-null object from the byte representation.</returns>
		[Pure]
		[NotNull]
		object FromBytes([NotNull]byte[] bytes);
	}

	/// <summary>
	/// Contract for types that can convert <see cref="TObjectType"/> to and from bytes.
	/// </summary>
	public interface IObjectByteConverter<TObjectType> : IObjectByteConverter
	{
		/// <summary>
		/// Coverts the <see cref="TObjectType"/> to byte representation.
		/// </summary>
		/// <param name="obj">Object to convert to bytes.</param>
		/// <returns>A non-null byte array representation of <see cref="obj"/>.</returns>
		[Pure]
		[NotNull]
		byte[] GetBytes([NotNull]TObjectType obj);

		/// <summary>
		/// Coverts the <see cref="byte[]"/> preresentation of an object to the object.
		/// </summary>
		/// <param name="bytes">Bytes to use for conversion.</param>
		/// <returns>A non-null object from the byte representation.</returns>
		[Pure]
		[NotNull]
		new TObjectType FromBytes([NotNull]byte[] bytes);
	}
}
