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
		[NotNull]
		protected IDeserializationPrototypeFactory<TComplexType> prototypeGeneratorService { get; }

		public ComplexTypeSerializerDecorator([NotNull] IEnumerable<IMemberSerializationMediator<TComplexType>> serializationDirections, [NotNull] IDeserializationPrototypeFactory<TComplexType> prototypeGenerator, [NotNull] IGeneralSerializerProvider serializerProvider) //todo: create a better way to provide serialization instructions
			: base(serializationDirections, serializerProvider)
		{
			if (prototypeGenerator == null) throw new ArgumentNullException(nameof(prototypeGenerator));

			if(!typeof(TComplexType).IsClass)
				throw new ArgumentException($"Provided generic Type: {typeof(TComplexType).FullName} must be a reference type.", nameof(TComplexType));

			prototypeGeneratorService = prototypeGenerator;
		}

		//TODO: Error handling
		/// <inheritdoc />
		public override TComplexType Read(IWireMemberReaderStrategy source)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));

			return Read(prototypeGeneratorService.Create(), source);
		}

		private TComplexType Read(TComplexType obj, IWireMemberReaderStrategy source)
		{
			SetMembersFromReaderData(obj, source);

			return obj;
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
