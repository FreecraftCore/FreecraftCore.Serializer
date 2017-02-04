using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Fasterflect;
using System.Reflection;
using System.Linq.Expressions;
using JetBrains.Annotations;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Represents a complex type definition that combines multiple knowntypes or other complex types.
	/// </summary>
	/// <typeparam name="TComplexType"></typeparam>
	public class ComplexTypeSerializerDecorator<TComplexType> : ComplexTypeSerializer<TComplexType> //TComplex type should be a class. Can't constraint it though.
	{
		//New constaint is gone; this provided an efficient way to create new instances over Activator.CreateInstance.
		//Search compiled lambda and new constaint operator on google to see dicussions about it
		private static Func<TComplexType> instanceGeneratorDelegate { get; } = Expression.Lambda<Func<TComplexType>>(Expression.New(typeof(TComplexType))).Compile();

		public ComplexTypeSerializerDecorator([NotNull] IEnumerable<IMemberSerializationMediator<TComplexType>> serializationDirections) //todo: create a better way to provide serialization instructions
			: base(serializationDirections)
		{
			if(!typeof(TComplexType).IsClass)
				throw new ArgumentException($"Provided generic Type: {typeof(TComplexType).FullName} must be a reference type.", nameof(TComplexType));
		}

		//TODO: Error handling
		/// <inheritdoc />
		public override TComplexType Read(IWireMemberReaderStrategy source)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));

			return Read(instanceGeneratorDelegate(), source);
		}

		private TComplexType Read(TComplexType obj, IWireMemberReaderStrategy source)
		{
			SetMembersFromReaderData(obj, source);

			return obj;
		}

		/// <inheritdoc />
		public override TComplexType Read(ref TComplexType obj, IWireMemberReaderStrategy source)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));

			return obj == null ? Read(source) : Read(obj = instanceGeneratorDelegate(), source);
		}

		//TODO: Error handling
		/// <inheritdoc />
		public override void Write(TComplexType value, IWireMemberWriterStrategy dest)
		{
			if (dest == null) throw new ArgumentNullException(nameof(dest));

			WriteMemberData(value, dest);
		}
	}
}
